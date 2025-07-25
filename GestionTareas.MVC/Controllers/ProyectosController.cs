using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using GestionTareas.API.Models;
using GestionTareas.MVC.Models;

namespace SistemaGestion.MVC.Controllers
{
    public class ProyectosController : Controller
    {
        private readonly HttpClient _client;

        public ProyectosController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("API");
        }

        // Vista de lista de proyectos
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("https://localhost:7134/api/Proyectos");
            if (response.IsSuccessStatusCode)
            {
                var proyectos = await response.Content.ReadFromJsonAsync<List<Proyecto>>();
                return View(proyectos);
            }
            return View(new List<Proyecto>());
        }

        // Vista de detalles de un proyecto con tareas asociadas
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Obtener los detalles del proyecto
            var proyectoResponse = await _client.GetAsync($"https://localhost:7134/api/Proyectos/{id}");
            if (!proyectoResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var proyecto = await proyectoResponse.Content.ReadFromJsonAsync<Proyecto>();

            // Obtener las tareas asociadas al proyecto
            var tareasResponse = await _client.GetAsync($"https://localhost:7134/api/Proyectos/{id}/tareas");
            if (!tareasResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var tareas = await tareasResponse.Content.ReadFromJsonAsync<List<Tarea>>();

            // Pasar el proyecto y las tareas a la vista
            var model = new ProyectoDetallesViewModel
            {
                Proyecto = proyecto,
                Tareas = tareas
            };

            return View(model);
        }

        // Vista de creación de proyecto
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Acción para crear un proyecto
        [HttpPost]
        public async Task<IActionResult> Create(Proyecto model)
        {
            var response = await _client.PostAsJsonAsync("https://localhost:7134/api/Proyectos", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Error al crear el proyecto.");
            return View();
        }

        // Vista de edición de proyecto
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _client.GetAsync($"https://localhost:7134/api/Proyectos/{id}");
            if (response.IsSuccessStatusCode)
            {
                var proyecto = await response.Content.ReadFromJsonAsync<Proyecto>();
                return View(proyecto);
            }
            return NotFound();
        }

        // Acción para editar un proyecto
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Proyecto model)
        {
            var response = await _client.PutAsJsonAsync($"https://localhost:7134/api/Proyectos/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Error al actualizar el proyecto.");
            return View(model);
        }

        // Acción para eliminar un proyecto
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.DeleteAsync($"https://localhost:7134/api/Proyectos/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Error al eliminar el proyecto.");
            return RedirectToAction("Index");
        }
    }
}
