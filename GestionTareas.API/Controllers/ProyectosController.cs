using Dapper;
using GestionTareas.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestionTareas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public ProyectosController(IDbConnection connection)
        {
            _connection = connection;
        }

        // GET api/Proyectos/{id}/tareas
        [HttpGet("{id}/tareas")]
        public IActionResult GetTareasByProyecto(int id)
        {
            var query = @"
            SELECT t.id, t.titulo, t.descripcion, t.fecha_creacion, t.fecha_vencimiento AS FechaVencimiento, t.estado 
            FROM tareas t
            WHERE t.proyecto_id = @ProyectoId";

            var tareas = _connection.Query<Tarea>(query, new { ProyectoId = id }).ToList();

            // Depuración
            if (tareas.Count == 1)
            {
                Console.WriteLine($"Solo se encontró una tarea con el ProyectoId {id}");
            }

            if (tareas == null || !tareas.Any())
            {
                return NotFound($"No tasks found for project with ID {id}.");
            }

            return Ok(tareas);

        }
        // GET: api/<ProyectosController>
        [HttpGet]
        public IActionResult GetAll()
        {
            var proyectos = _connection.Query<Proyecto>("SELECT id,nombre,descripcion,fecha_inicio,fecha_fin AS FechaFin,estado FROM proyectos").ToList();
            return Ok(proyectos);
        }

        // GET api/<ProyectosController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var proyecto = _connection.QuerySingleOrDefault<Proyecto>("SELECT id,nombre,descripcion,fecha_inicio,fecha_fin AS FechaFin,estado FROM proyectos WHERE id = @id", new { id });
            if (proyecto == null)
                return NotFound();
            return Ok(proyecto);
        }

        // POST api/<ProyectosController>
        [HttpPost]
        public IActionResult Post([FromBody] Proyecto proyecto)
        {
            var query = @"INSERT INTO proyectos (nombre,descripcion,fecha_inicio,fecha_fin,estado)
                          VALUES(@Nombre,@Descripcion,@FechaInicio,@FechaFin,@Estado);
                          SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = _connection.QuerySingle<int>(query, proyecto);

            var pObtenido = _connection.QuerySingle<Proyecto>(
                "SELECT id,nombre,descripcion,fecha_inicio,fecha_fin AS FechaFin,estado FROM proyectos WHERE id = @id", new { id });

            return CreatedAtAction(nameof(Get), new { id = pObtenido.Id }, pObtenido);
        }

        // PUT api/<ProyectosController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Proyecto proyecto)
        {
            var query = "UPDATE proyectos SET nombre = @Nombre, descripcion = @Descripcion, fecha_inicio = @FechaInicio, fecha_fin = @FechaFin, estado = @Estado WHERE id = @Id";
            var filasAfectadas = _connection.Execute(query, new { proyecto.Nombre, proyecto.Descripcion, proyecto.FechaInicio, proyecto.FechaFin, proyecto.Estado, Id = id });
            if (filasAfectadas == 0)
                return NotFound();
            return NoContent();

        }

        // DELETE api/<ProyectosController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var query = "DELETE FROM proyectos WHERE id = @Id";
            var filasAfectadas = _connection.Execute(query, new { Id = id });
            if (filasAfectadas == 0)
                return NotFound();
            return NoContent();

        }
    }
}
