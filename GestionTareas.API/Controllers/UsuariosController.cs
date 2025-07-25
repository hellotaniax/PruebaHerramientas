using Dapper;
using GestionTareas.API.Models;
using GestionTareas.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestionTareas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IDbConnection _connection;
        private readonly JwtService _jwtService;

        public UsuariosController(IDbConnection connection, JwtService jwtService)
        {
            _connection = connection;
            _jwtService = jwtService;
        }
        // GET: api/<UsuariosController>
        [HttpGet]
        public IActionResult GetAll()
        {
            var usuarios = _connection.Query<Usuario>("SELECT * FROM usuarios").ToList();
            return Ok(usuarios);
        }

        // POST api/<UsuariosController>/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var query = "SELECT * FROM usuarios WHERE email = @Email AND contraseña = @Contraseña";
            var usuario = _connection.QuerySingleOrDefault<Usuario>(query, new { model.Email, model.Contraseña });

            if (usuario == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            // Generar el token JWT utilizando el servicio
            var token = _jwtService.GenerateToken(usuario.Email, usuario.Rol);

            return Ok(new { Token = token }); // Devolver el token al cliente
        }

        // POST api/<UsuariosController>/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            // Verificar si ya existe un usuario con el mismo email
            var queryCheckEmail = "SELECT COUNT(1) FROM usuarios WHERE email = @Email";
            var emailExists = _connection.ExecuteScalar<int>(queryCheckEmail, new { model.Email });

            if (emailExists > 0)
            {
                return BadRequest("Email already in use.");
            }

            // Insertar el nuevo usuario en la base de datos
            var query = "INSERT INTO usuarios (nombre, email, contraseña, rol) VALUES(@Nombre, @Email, @Contraseña, @Rol)";
            _connection.Execute(query, model);

            // Obtener el usuario recién creado para devolverlo
            var usuario = _connection.QuerySingle<Usuario>("SELECT * FROM usuarios WHERE email = @Email", new { model.Email });

            // Generar el token JWT para el nuevo usuario
            var token = _jwtService.GenerateToken(usuario.Email, usuario.Rol);

            return CreatedAtAction(nameof(Get), new { id = usuario.Id }, new { Token = token, Usuario = usuario });
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
