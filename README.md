
# CRUD de Clientes con Fotos - POS Tienda de Tecnología

## Descripción del Proyecto

Este proyecto es una aplicación de escritorio en **C# Windows Forms** para gestionar clientes de una tienda de tecnología, utilizando **MySQL** como base de datos.  
Permite realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) incluyendo la **gestión de fotos** de los clientes.  

Cada cliente tiene los siguientes campos:

- `cedula` (VARCHAR) - Clave primaria
- `nombre` (VARCHAR)
- `apellido` (VARCHAR)
- `telefono` (VARCHAR)
- `correo` (VARCHAR)
- `direccion` (VARCHAR)
- `foto` (VARCHAR) - Nombre del archivo de la foto
- `creado_en` (TIMESTAMP) - Fecha de creación

<img width="1919" height="1027" alt="Captura de pantalla 2025-10-03 104107" src="https://github.com/user-attachments/assets/e1b10ed9-527a-4455-900c-156bb93d2ac3" />
<img width="1919" height="1026" alt="Captura de pantalla 2025-10-03 104053" src="https://github.com/user-attachments/assets/ace6d775-06c6-4527-8b06-96cfc45cdfdb" />
<img width="1919" height="1025" alt="Captura de pantalla 2025-10-03 104044" src="https://github.com/user-attachments/assets/5d5456e5-bf3c-4b3c-b4a2-76f1beb42660" />


---

## Estructura del Proyecto

```
POS_TiendaTecnologia/
│
├─ POS_TiendaTecnologia.sln
├─ POS_TiendaTecnologia/
│   ├─ Form1.cs
│   ├─ Form1.Designer.cs
│   ├─ Program.cs
│   ├─ Models/
│   │   └─ Cliente.cs
│   ├─ Business/
│   │   └─ ClienteService.cs
│   └─ Fotos/          <-- Carpeta donde se guardan las imágenes
│
└─ README.md
```

---

## Base de Datos

**Creación de la base de datos:**

```sql
CREATE DATABASE IF NOT EXISTS pos_tienda_tecnologia
CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE pos_tienda_tecnologia;

CREATE TABLE IF NOT EXISTS clientes (
    cedula VARCHAR(20) NOT NULL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100),
    telefono VARCHAR(20),
    correo VARCHAR(100),
    direccion VARCHAR(255),
    foto VARCHAR(255),
    creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

---

## Funcionalidades CRUD con Fotos

### 1. Guardar Cliente (`btnGuardar`)

- Verifica que la cédula no esté vacía.
- Permite seleccionar una foto con `OpenFileDialog`.
- Copia la foto a la carpeta `Fotos` dentro de la aplicación.
- Inserta los datos en MySQL, incluyendo el nombre de la foto.

```csharp
cmd.Parameters.AddWithValue("@foto", nombreFoto); // Nombre de la foto guardada
```

### 2. Editar Cliente (`btnEditar`)

- Actualiza todos los datos del cliente.
- Si se selecciona una nueva foto, reemplaza la anterior en la carpeta `Fotos`.
- Actualiza el campo `foto` en la base de datos.

```csharp
string query = @"UPDATE clientes 
                 SET nombre=@nombre,
                     apellido=@apellido,
                     telefono=@telefono,
                     correo=@correo,
                     direccion=@direccion,
                     foto=@foto
                 WHERE cedula=@cedula";
```

### 3. Eliminar Cliente (`btnEliminar`)

- Solicita confirmación antes de eliminar.
- Obtiene el nombre de la foto del cliente.
- Borra el registro de la base de datos.
- Elimina la foto del disco si existe.
- Limpia los campos y el `DataGridView`.

```csharp
string rutaFoto = Path.Combine(Application.StartupPath, "Fotos", fotoNombre);
if (File.Exists(rutaFoto))
{
    File.Delete(rutaFoto);
}
```

### 4. Buscar Cliente (`btnBuscar`)

- Busca al cliente por cédula.
- Carga los datos en los `TextBox` y la foto en `PictureBox`.
- Se puede combinar con el `DataGridView` para seleccionar clientes.

### 5. Listar Clientes (`DataGridView`)

- `DataGridView` muestra todos los clientes de la base de datos.
- Al seleccionar una fila se cargan los datos en el formulario, incluida la foto:

```csharp
string rutaImagen = Path.Combine(Application.StartupPath, "Fotos", foto);
if (File.Exists(rutaImagen))
{
    pictureBoxFoto.Image = Image.FromFile(rutaImagen);
}
else
{
    pictureBoxFoto.Image = null;
}
```

### 6. Vista previa de la foto (`pictureBoxFoto_Click`)

- Permite ver la imagen en pantalla completa.

```csharp
if (pictureBoxFoto.Image != null)
{
    using (Form visor = new Form())
    {
        PictureBox pb = new PictureBox();
        pb.Dock = DockStyle.Fill;
        pb.SizeMode = PictureBoxSizeMode.Zoom;
        pb.Image = pictureBoxFoto.Image;
        visor.Controls.Add(pb);
        visor.ShowDialog();
    }
}
```

---

## Buenas Prácticas Implementadas

- Uso de **parametrización en MySQL** para evitar inyección SQL.
- Separación de lógica: `Models` para datos y `Business` para servicios.
- Manejo de excepciones con `try-catch`.
- Validación de datos antes de ejecutar operaciones.
- Gestión de archivos de manera segura (verificación de existencia antes de borrar o copiar).
- Carpeta `Fotos` dentro de la aplicación para centralizar imágenes.

---

## Notas

- Las fotos se guardan únicamente como nombres de archivo en la base de datos, no como blobs.
- Se recomienda tener la carpeta `Fotos` en la misma ubicación que el ejecutable.
- CRUD completo con integración de imágenes funcional en Windows Forms.
