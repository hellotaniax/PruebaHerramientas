using Dapper;
using GestionTareas.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestionTareas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IDbConnection _connection;
        public UsuariosController(IDbConnection connection)
        {
            _connection = connection;
        }
        // GET: api/<UsuariosController>
        [HttpGet]
        public IActionResult GetAll()
        {
            var usuarios = _connection.Query<Usuario>("SELECT * FROM usuarios").ToList();
            return Ok(usuarios);
        }

        // GET api/<UsuariosController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var usuario = _connection.QuerySingleOrDefault<Usuario>("SELECT * FROM usuarios WHERE id = @id", new { id });
            if (usuario == null)
                return NotFound();
            return Ok(usuario);
        }

        // POST api/<UsuariosController>
        [HttpPost]
        public IActionResult Post([FromBody] Usuario usuario)
        {
            var query = "INSERT INTO usuarios (nombre, email, contraseña, rol) VALUES(@Nombre, @Email, @Contraseña, @Rol)";
            _connection.Execute(query, usuario);

            var uObtenido = _connection.QuerySingle<Usuario>("SELECT * FROM usuarios WHERE email = @Email", new { usuario.Email });
            if (uObtenido == null)
            {
                return NotFound("User not found.");
            }
            return CreatedAtAction(nameof(Get), new { id = uObtenido.Id }, uObtenido);
        }

        // PUT api/<UsuariosController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Usuario usuario)
        {
            var query = "UPDATE usuarios SET nombre = @Nombre, email = @Email, contraseña = @Contraseña, rol = @Rol WHERE id = @Id";
            var filasAfectadas = _connection.Execute(query, new { usuario.Nombre, usuario.Email, usuario.Contraseña, usuario.Rol, Id = id });
            if (filasAfectadas == 0)
                return NotFound();
            return NoContent();
        }

        // DELETE api/<UsuariosController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var query = "DELETE FROM usuarios WHERE id = @id";
            var filasAfectadas = _connection.Execute(query, new { id });
            if (filasAfectadas == 0)
                return NotFound();
            return NoContent();
        }
    }
}
