using GestionTareas.API.Models;

namespace GestionTareas.MVC.Models
{
    internal class ProyectoDetallesViewModel
    {
        public Proyecto Proyecto { get; set; }
        public List<Tarea> Tareas { get; set; }
    }
}