using GestionTareas.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SistemaGestion.MVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly HttpClient _client;

        public UsuariosController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("API");
        }

        // Vista de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Acción para el login
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var response = await _client.PostAsJsonAsync("https://localhost:7134/api/Usuarios/login", model);
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<dynamic>();
                string token = tokenResponse?.Token;

                // Guardar el token en la sesión o en una cookie
                HttpContext.Session.SetString("JwtToken", token);

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Login failed.");
            return View();
        }

        // Vista de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Acción para el registro
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var response = await _client.PostAsJsonAsync("https://localhost:7134/api/Usuarios/register", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "Registration failed.");
            return View();
        }
    }
}
