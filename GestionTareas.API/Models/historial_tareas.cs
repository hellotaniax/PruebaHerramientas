namespace GestionTareas.API.Models
{
    public class historial_tareas
    {
        public int Id { get; set; }
        public int TareaId { get; set; } // Relación con Tarea
        public Tarea Tarea { get; set; } // Navegación a Tarea
        public string EstadoAnterior { get; set; } // Ejemplo: "Pendiente", "En Progreso", "Completada"
        public string EstadoNuevo { get; set; } // Ejemplo: "Pendiente", "En Progreso", "Completada"
        public DateTime FechaCambio { get; set; } = DateTime.Now;
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

    }
}
