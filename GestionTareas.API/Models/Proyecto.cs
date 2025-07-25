namespace GestionTareas.API.Models
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } // Ejemplo: "Activo", "Completado", "Cancelado"
    }
}
