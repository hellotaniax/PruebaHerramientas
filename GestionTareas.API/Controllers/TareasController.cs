using Dapper;
using GestionTareas.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestionTareas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public TareasController(IDbConnection connection)
        {
            _connection = connection;
        }
        // GET: api/<TareasController>
        [HttpGet]
        public IActionResult GetAll()
        {
            var tareas = _connection.Query("SELECT id, titulo, descripcion, fecha_creacion, fecha_vencimiento AS FechaVencimiento, estado, prioridad FROM tareas").ToList();
            return Ok(tareas);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var query = @"
                SELECT 
                    t.id, t.titulo, t.descripcion, t.fecha_creacion, t.fecha_vencimiento AS FechaVencimiento, 
                    t.estado, t.prioridad, t.usuario_asignado_id AS UsuarioAsignadoId, t.proyecto_id AS ProyectoId,
                    u.id, u.nombre, u.email, u.rol,
                    p.id, p.nombre, p.descripcion, p.fecha_inicio, p.fecha_fin, p.estado
                FROM tareas t
                LEFT JOIN usuarios u ON t.usuario_asignado_id = u.id
                LEFT JOIN proyectos p ON t.proyecto_id = p.id
                WHERE t.id = @id";

            var tarea = _connection.Query<Tarea, Usuario, Proyecto, Tarea>(
                query,
                (t, u, p) => {
                    t.UsuarioAsignado = u;
                    t.Proyecto = p;
                    return t;
                },
                new { id },
                splitOn: "id,id"
            ).FirstOrDefault();

            if (tarea == null)
                return NotFound();
            return Ok(tarea);
        }

        // POST api/<TareasController>
        [HttpPost]
        public IActionResult Post([FromBody] Tarea tarea)
        {
            var query = @"INSERT INTO tareas (titulo,descripcion,fecha_creacion,fecha_vencimiento,estado,prioridad,usuario_asignado_id,proyecto_id)
                          VALUES(@Titulo,@Descripcion,@FechaCreacion,@FechaVencimiento,@Estado,@Prioridad,@UsuarioAsignadoId,@ProyectoId);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = _connection.ExecuteScalar<int>(query, tarea);

            // Consultar la cita insertada con JOINs y devolverla como respuesta
            return Get(id);
        }

        // PUT api/<TareasController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Tarea tarea)
        {
            var query = "UPDATE tareas SET titulo = @Titulo, descripcion = @Descripcion, fecha_vencimiento = @FechaVencimiento, estado = @Estado, prioridad = @Prioridad, usuario_asignado_id = @UsuarioAsignadoId, proyecto_id = @ProyectoId WHERE id = @Id";
            var filasAfectadas = _connection.Execute(query, new
            {
                tarea.Titulo,
                tarea.Descripcion,
                tarea.FechaVencimiento,
                tarea.Estado,
                tarea.Prioridad,
                tarea.UsuarioAsignadoId,
                tarea.ProyectoId,
                Id = id
            });
            if (filasAfectadas == 0)
                return NotFound();
            return NoContent();
        }

        // DELETE api/<TareasController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Eliminar las filas relacionadas en historial_tareas
            var queryHistorial = "DELETE FROM historial_tareas WHERE tarea_id = @id";
            _connection.Execute(queryHistorial, new { id });

            // Luego eliminar la tarea
            var query = "DELETE FROM tareas WHERE id = @id";
            var filasAfectadas = _connection.Execute(query, new { id });

            if (filasAfectadas == 0)
                return NotFound();

            return NoContent();
        }
    }
}
