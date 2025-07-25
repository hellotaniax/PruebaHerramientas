namespace GestionTareas.API.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; } // Ejemplo: "Admin", "Usuario"
    }
}
