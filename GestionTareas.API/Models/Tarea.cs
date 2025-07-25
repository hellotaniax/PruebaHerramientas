namespace GestionTareas.API.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaVencimiento { get; set; }
        public string Estado { get; set; } // Ejemplo: "Pendiente", "En Progreso", "Completada"
        public string Prioridad { get; set; } 
        public int UsuarioAsignadoId { get; set; } // Relación con Usuario
        public Usuario? UsuarioAsignado { get; set; } // Navegación a Usuario
        public int ProyectoId { get; set; } // Relación con Proyecto
        public Proyecto? Proyecto { get; set; } // Navegación a Proyecto

    }
}
