using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using POS_TiendaTecnologia.Data;
using POS_TiendaTecnologia.Models;

namespace POS_TiendaTecnologia.Business
{
    public class ClienteService
    {
        private readonly ClienteDAL dal = new ClienteDAL();

        public DataTable ObtenerTodos()
        {
            return dal.ObtenerTodos();
        }

        public Cliente ObtenerPorCedula(string cedula)
        {
            return dal.ObtenerPorCedula(cedula);
        }

        public bool Insertar(Cliente c)
        {
            // Aquí podrías agregar validaciones de negocio (correo válido, etc.)
            return dal.Insertar(c);
        }

        public bool Actualizar(Cliente c)
        {
            return dal.Actualizar(c);
        }

        public bool Eliminar(string cedula)
        {
            return dal.Eliminar(cedula);
        }
    }
}

