using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using POS_TiendaTecnologia.Models;

namespace POS_TiendaTecnologia.Data
{
    public class ClienteDAL
    {
        public DataTable ObtenerTodos()
        {
            DataTable dt = new DataTable();
            using (var conn = Conexion.GetConnection())
            {
                string sql = "SELECT cedula, nombre, apellido, telefono, correo, direccion, foto FROM clientes";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public Cliente ObtenerPorCedula(string cedula)
        {
            Cliente cliente = null;
            using (var conn = Conexion.GetConnection())
            {
                conn.Open();
                string sql = "SELECT cedula, nombre, apellido, telefono, correo, direccion, foto FROM clientes WHERE cedula = @cedula";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedula);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new Cliente
                            {
                                Cedula = reader["cedula"].ToString(),
                                Nombre = reader["nombre"].ToString(),
                                Apellido = reader["apellido"].ToString(),
                                Telefono = reader["telefono"].ToString(),
                                Correo = reader["correo"].ToString(),
                                Direccion = reader["direccion"].ToString(),
                                Foto = reader["foto"].ToString()
                            };
                        }
                    }
                }
            }
            return cliente;
        }

        public bool Insertar(Cliente c)
        {
            using (var conn = Conexion.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO clientes (cedula, nombre, apellido, telefono, correo, direccion, foto)
                               VALUES (@cedula, @nombre, @apellido, @telefono, @correo, @direccion, @foto)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cedula", c.Cedula);
                    cmd.Parameters.AddWithValue("@nombre", c.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", c.Apellido ?? "");
                    cmd.Parameters.AddWithValue("@telefono", c.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@correo", c.Correo ?? "");
                    cmd.Parameters.AddWithValue("@direccion", c.Direccion ?? "");
                    cmd.Parameters.AddWithValue("@foto", c.Foto ?? "");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Actualizar(Cliente c)
        {
            using (var conn = Conexion.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE clientes
                               SET nombre=@nombre, apellido=@apellido, telefono=@telefono, correo=@correo, direccion=@direccion, foto=@foto
                               WHERE cedula=@cedula";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", c.Nombre);
                    cmd.Parameters.AddWithValue("@apellido", c.Apellido ?? "");
                    cmd.Parameters.AddWithValue("@telefono", c.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@correo", c.Correo ?? "");
                    cmd.Parameters.AddWithValue("@direccion", c.Direccion ?? "");
                    cmd.Parameters.AddWithValue("@foto", c.Foto ?? "");
                    cmd.Parameters.AddWithValue("@cedula", c.Cedula);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Eliminar(string cedula)
        {
            using (var conn = Conexion.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM clientes WHERE cedula = @cedula";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedula);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
