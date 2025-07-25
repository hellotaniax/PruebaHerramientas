using Dapper;
using GestionTareas.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestionTareas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialTareasController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public HistorialTareasController(IDbConnection connection)
        {
            _connection = connection;
        }
        // GET: api/<HistorialTareasController>
        [HttpGet]
        public IActionResult GetAll()
        {
            var query = @"
                SELECT ht.id, ht.tarea_id AS TareaId, ht.estado_anterior AS EstadoAnterior, 
                       ht.estado_actual AS EstadoActual, ht.fecha_cambio AS FechaCambio, 
                       ht.usuario_id AS UsuarioId, u.nombre AS UsuarioNombre
                FROM historial_tareas ht
                JOIN usuarios u ON ht.usuario_id = u.id";
            var historiales = _connection.Query(query).ToList();
            return Ok(historiales);
        }

        // GET api/<HistorialTareasController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var query = @"
                SELECT ht.id, ht.tarea_id AS TareaId, ht.estado_anterior AS EstadoAnterior, 
                       ht.estado_actual AS EstadoActual, ht.fecha_cambio AS FechaCambio, 
                       ht.usuario_id AS UsuarioId, u.nombre AS UsuarioNombre
                FROM historial_tareas ht
                JOIN usuarios u ON ht.usuario_id = u.id
                WHERE ht.id = @id";


            var historial = _connection.QuerySingleOrDefault(query, new { id });
            if (historial == null)
                return NotFound();
            return Ok(historial);
        }

        // POST api/<HistorialTareasController>
        [HttpPost]
        public IActionResult Post([FromBody] HistorialTarea historialTarea)
        {
            var query = @"
                INSERT INTO historial_tareas (tarea_id, estado_anterior, estado_actual, fecha_cambio, usuario_id)
                VALUES (@TareaId, @EstadoAnterior, @EstadoActual, @FechaCambio, @UsuarioId);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = _connection.ExecuteScalar<int>(query, historialTarea);

            return Get(id);
        }

        // PUT api/<HistorialTareasController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] HistorialTarea historialTarea)
        {
            var query = @"
                UPDATE historial_tareas 
                SET tarea_id = @TareaId, estado_anterior = @EstadoAnterior, 
                    estado_actual = @EstadoActual, fecha_cambio = @FechaCambio, usuario_id = @UsuarioId
                WHERE id = @Id";
            var filasAfectadas = _connection.Execute(query, new 
            { 
                historialTarea.TareaId, 
                historialTarea.EstadoAnterior, 
                historialTarea.EstadoActual, 
                historialTarea.FechaCambio, 
                historialTarea.UsuarioId, 
                Id = id 
            });
            if (filasAfectadas == 0)
                return NotFound();
            return NoContent();
        }

        // DELETE api/<HistorialTareasController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var query = "DELETE FROM historial_tareas WHERE id = @id";
            var filasAfectadas = _connection.Execute(query, new { id });
            if (filasAfectadas == 0)
                return NotFound();
            return NoContent();
        }
    }
}
