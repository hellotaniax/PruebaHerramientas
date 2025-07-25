namespace GestionTareas.API.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } // Ejemplo: "Pendiente", "En Progreso", "Completada", "Cancelada"
        public string Prioridad { get; set; } // Ejemplo: 1 (Alta), 2 (Media), 3 (Baja)
        public int UsuarioId { get; set; } // Relación con Usuario
        public Usuario? Usuario { get; set; } // Navegación a Usuario
        public int ProyectoId { get; set; } // Relación con Proyecto
        public Proyecto? Proyecto { get; set; } // Navegación a Proyecto

    }
}
