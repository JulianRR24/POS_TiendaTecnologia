using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using POS_TiendaTecnologia.Business;
using POS_TiendaTecnologia.Models;
using System.IO;
using MySql.Data.MySqlClient;



namespace POS_TiendaTecnologia
{
    public partial class Form1 : Form
    {
        private string rutaSeleccionadaFoto = ""; // ruta completa de la foto seleccionada
        string cadenaConexion = "server=localhost;database=pos_tienda_tecnologia;user=root;password=0623;";

        public Form1()
        {
            InitializeComponent();
            CargarClientes();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //Boton nuevo
            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("Debe ingresar la cédula del cliente que desea eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirmar antes de eliminar
            DialogResult confirmacion = MessageBox.Show(
                "¿Está seguro de que desea eliminar este cliente?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmacion == DialogResult.No)
                return;

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    // Obtener el nombre de la foto para borrarla
                    string fotoNombre = "";
                    string queryFoto = "SELECT foto FROM clientes WHERE cedula = @cedula";
                    using (MySqlCommand cmdFoto = new MySqlCommand(queryFoto, conexion))
                    {
                        cmdFoto.Parameters.AddWithValue("@cedula", txtCedula.Text);
                        object resultado = cmdFoto.ExecuteScalar();
                        if (resultado != null && resultado != DBNull.Value)
                        {
                            fotoNombre = resultado.ToString();
                        }
                    }

                    // Borrar el cliente de la base de datos
                    string queryEliminar = "DELETE FROM clientes WHERE cedula = @cedula";
                    using (MySqlCommand cmdEliminar = new MySqlCommand(queryEliminar, conexion))
                    {
                        cmdEliminar.Parameters.AddWithValue("@cedula", txtCedula.Text);
                        int filasAfectadas = cmdEliminar.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            // Borrar la foto del cliente si existe
                            if (!string.IsNullOrEmpty(fotoNombre))
                            {
                                string rutaFoto = Path.Combine(Application.StartupPath, "Fotos", fotoNombre);
                                if (File.Exists(rutaFoto))
                                {
                                    File.Delete(rutaFoto);
                                }
                            }

                            MessageBox.Show("Cliente eliminado correctamente ✅", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiar campos y resetear foto
                            LimpiarCampos();
                            pictureBoxFoto.Image = null;
                            linkLabelFoto.Text = "Seleccionar imagen";
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el cliente para eliminar ❌", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }

                // Recargar DataGridView
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para limpiar los campos después de eliminar
        private void LimpiarCampos()
        {
            txtCedula.Text = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            txtDireccion.Text = "";
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar que se haya ingresado la cédula
                if (string.IsNullOrWhiteSpace(txtCedula.Text))
                {
                    MessageBox.Show("Debe ingresar la cédula del cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Guardar la foto en la carpeta Fotos
                string nombreFoto = "";
                if (!string.IsNullOrEmpty(rutaSeleccionadaFoto) && pictureBoxFoto.Image != null)
                {
                    nombreFoto = Path.GetFileName(rutaSeleccionadaFoto);

                    // Crear carpeta Fotos si no existe
                    string carpetaFotos = Path.Combine(Application.StartupPath, "Fotos");
                    if (!Directory.Exists(carpetaFotos))
                    {
                        Directory.CreateDirectory(carpetaFotos);
                    }

                    string rutaDestino = Path.Combine(carpetaFotos, nombreFoto);
                    if (File.Exists(rutaSeleccionadaFoto))
                    {
                        File.Copy(rutaSeleccionadaFoto, rutaDestino, true); // true = sobrescribir si existe
                    }
                }

                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string query = @"INSERT INTO clientes (cedula, nombre, apellido, telefono, correo, direccion, foto)
                             VALUES (@cedula, @nombre, @apellido, @telefono, @correo, @direccion, @foto)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@cedula", txtCedula.Text);
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@apellido", txtApellido.Text);
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@correo", txtCorreo.Text);
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@foto", nombreFoto); // Guardamos solo el nombre de la imagen

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cliente guardado con éxito ✅", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar campos después de guardar
                txtCedula.Clear();
                txtNombre.Clear();
                txtApellido.Clear();
                txtTelefono.Clear();
                txtCorreo.Clear();
                txtDireccion.Clear();
                pictureBoxFoto.Image = null;
                linkLabelFoto.Text = "Seleccionar imagen";
                rutaSeleccionadaFoto = ""; // Limpiamos la variable global

                // Recargar DataGridView
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnEditar_Click(object sender, EventArgs e)
        {
            // Validar que haya cédula
            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("Debe buscar un cliente primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Guardar la foto en la carpeta Fotos si hay una seleccionada
                string nombreFoto = "";
                if (!string.IsNullOrEmpty(rutaSeleccionadaFoto) && pictureBoxFoto.Image != null)
                {
                    nombreFoto = Path.GetFileName(rutaSeleccionadaFoto);

                    // Crear carpeta Fotos si no existe
                    string carpetaFotos = Path.Combine(Application.StartupPath, "Fotos");
                    if (!Directory.Exists(carpetaFotos))
                    {
                        Directory.CreateDirectory(carpetaFotos);
                    }

                    string rutaDestino = Path.Combine(carpetaFotos, nombreFoto);
                    if (File.Exists(rutaSeleccionadaFoto))
                    {
                        File.Copy(rutaSeleccionadaFoto, rutaDestino, true); // sobrescribir si existe
                    }
                }
                else
                {
                    // Si no se seleccionó nueva foto, mantener la foto actual del DataGridView
                    nombreFoto = linkLabelFoto.Text == "Seleccionar imagen" ? "" : linkLabelFoto.Text;
                }

                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string query = @"UPDATE clientes 
                             SET nombre = @nombre,
                                 apellido = @apellido,
                                 telefono = @telefono,
                                 correo = @correo,
                                 direccion = @direccion,
                                 foto = @foto
                             WHERE cedula = @cedula";

                    using (MySqlCommand comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@cedula", txtCedula.Text);
                        comando.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        comando.Parameters.AddWithValue("@apellido", txtApellido.Text);
                        comando.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        comando.Parameters.AddWithValue("@correo", txtCorreo.Text);
                        comando.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                        comando.Parameters.AddWithValue("@foto", nombreFoto);

                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                            MessageBox.Show("Cliente actualizado correctamente ✅", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("No se encontró el cliente para actualizar ❌", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Limpiar variable global de la foto
                rutaSeleccionadaFoto = "";

                // Recargar DataGridView
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("Por favor ingrese la cédula a buscar", "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string query = "SELECT * FROM clientes WHERE cedula = @cedula";

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@cedula", txtCedula.Text);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Cargamos los valores en los textbox
                                txtNombre.Text = reader["nombre"].ToString();
                                txtApellido.Text = reader["apellido"].ToString();
                                txtTelefono.Text = reader["telefono"].ToString();
                                txtCorreo.Text = reader["correo"].ToString();
                                txtDireccion.Text = reader["direccion"].ToString();

                                // Foto
                                string foto = reader["foto"].ToString();
                                if (!string.IsNullOrEmpty(foto))
                                {
                                    linkLabelFoto.Text = foto;

                                    string rutaImagen = Path.Combine(Application.StartupPath, "Fotos", foto);
                                    if (File.Exists(rutaImagen))
                                    {
                                        pictureBoxFoto.Image = Image.FromFile(rutaImagen);
                                    }
                                    else
                                    {
                                        pictureBoxFoto.Image = null;
                                    }
                                }
                                else
                                {
                                    linkLabelFoto.Text = "Seleccionar imagen";
                                    pictureBoxFoto.Image = null;
                                }

                                MessageBox.Show("Cliente encontrado", "Información",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No se encontró un cliente con esa cédula", "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);

                                // Limpiar los campos si no hay datos
                                txtNombre.Clear();
                                txtApellido.Clear();
                                txtTelefono.Clear();
                                txtCorreo.Clear();
                                txtDireccion.Clear();
                                linkLabelFoto.Text = "Seleccionar imagen";
                                pictureBoxFoto.Image = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void txtCedula_TextChanged(object sender, EventArgs e)
        {
            //Texbox de Cedula
        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            //Texbox de Nombre
        }

        private void txtApellido_TextChanged(object sender, EventArgs e)
        {
            //Texbox de Apellido
        }

        private void txtTelefono_TextChanged(object sender, EventArgs e)
        {
            //Texbox de Telefono
        }

        private void txtCorreo_TextChanged(object sender, EventArgs e)
        {
            //Texbox de Correo
        }

        private void txtDireccion_TextChanged(object sender, EventArgs e)
        {
            //Texbox de Direccion
        }

        private void linkLabelFoto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Crea un nuevo diálogo para abrir archivos
            using (OpenFileDialog dialogoAbrir = new OpenFileDialog())
            {
                // Configura el filtro para mostrar solo archivos de imagen
                dialogoAbrir.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                dialogoAbrir.Title = "Seleccionar imagen";

                // Muestra el diálogo y verifica si se seleccionó un archivo
                if (dialogoAbrir.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Guarda la ruta completa en la variable global
                        rutaSeleccionadaFoto = dialogoAbrir.FileName;

                        // Muestra solo el nombre del archivo en el LinkLabel
                        linkLabelFoto.Text = Path.GetFileName(rutaSeleccionadaFoto);

                        // Carga la imagen en el PictureBox
                        pictureBoxFoto.Image = Image.FromFile(rutaSeleccionadaFoto);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar la imagen: " + ex.Message, "Error",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void pictureBoxFoto_Click(object sender, EventArgs e)
        {
            // Si hay una imagen cargada, la muestra en pantalla completa
            if (pictureBoxFoto.Image != null)
            {
                using (Form visor = new Form())
                {
                    visor.Text = "Vista previa de la imagen";
                    visor.StartPosition = FormStartPosition.CenterScreen;
                    visor.WindowState = FormWindowState.Maximized;
                    visor.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                    
                    PictureBox pb = new PictureBox();
                    pb.Dock = DockStyle.Fill;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.Image = pictureBoxFoto.Image;
                    
                    visor.Controls.Add(pb);
                    visor.ShowDialog();
                }
            }
        }

        private void CargarClientes()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string query = "SELECT cedula, nombre, apellido, telefono, correo, direccion, foto FROM clientes";

                    MySqlCommand cmd = new MySqlCommand(query, conexion);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvClientes.DataSource = dt;

                    // Opcional: ajustar columnas
                    dgvClientes.Columns["cedula"].HeaderText = "Cédula";
                    dgvClientes.Columns["nombre"].HeaderText = "Nombre";
                    dgvClientes.Columns["apellido"].HeaderText = "Apellido";
                    dgvClientes.Columns["telefono"].HeaderText = "Teléfono";
                    dgvClientes.Columns["correo"].HeaderText = "Correo";
                    dgvClientes.Columns["direccion"].HeaderText = "Dirección";
                    dgvClientes.Columns["foto"].Visible = false; // ocultamos la columna de la foto
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message);
            }
        }

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvClientes.Rows[e.RowIndex];

                txtCedula.Text = fila.Cells["cedula"].Value.ToString();
                txtNombre.Text = fila.Cells["nombre"].Value.ToString();
                txtApellido.Text = fila.Cells["apellido"].Value.ToString();
                txtTelefono.Text = fila.Cells["telefono"].Value.ToString();
                txtCorreo.Text = fila.Cells["correo"].Value.ToString();
                txtDireccion.Text = fila.Cells["direccion"].Value.ToString();

                string foto = fila.Cells["foto"].Value.ToString();
                if (!string.IsNullOrEmpty(foto))
                {
                    linkLabelFoto.Text = foto;
                    string rutaImagen = Path.Combine(Application.StartupPath, "Fotos", foto);
                    if (File.Exists(rutaImagen))
                    {
                        pictureBoxFoto.Image = Image.FromFile(rutaImagen);
                    }
                    else
                    {
                        pictureBoxFoto.Image = null;
                    }
                }
                else
                {
                    linkLabelFoto.Text = "Seleccionar imagen";
                    pictureBoxFoto.Image = null;
                }
            }
        }
    }
}
