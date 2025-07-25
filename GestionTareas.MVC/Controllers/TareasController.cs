using GestionTareas.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace SistemaGestion.MVC.Controllers
{
    public class TareasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TareasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Tareas
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync("https://localhost:7134/api/tareas");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var tareas = JsonSerializer.Deserialize<IEnumerable<Tarea>>(jsonString);
                return View(tareas);
            }
            return View(new List<Tarea>());
        }

        // GET: Tareas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"https://localhost:7134/api/tareas/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonString);
                var tarea = JsonSerializer.Deserialize<Tarea>(jsonString);
                return View(tarea);
            }
            return NotFound();
        }

        // GET: Tareas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PostAsJsonAsync("https://localhost:7134/api/tareas", tarea);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(tarea);
        }

        // GET: Tareas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"https://localhost:7134/api/tareas/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var tarea = JsonSerializer.Deserialize<Tarea>(jsonString);
                return View(tarea);
            }
            return NotFound();
        }

        // POST: Tareas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tarea tarea)
        {
            if (id != tarea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PutAsJsonAsync($"https://localhost:7134/api/tareas/{id}", tarea);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(tarea);
        }

        // GET: Tareas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"https://localhost:7134/api/tareas/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var tarea = JsonSerializer.Deserialize<Tarea>(jsonString);
                return View(tarea);
            }
            return NotFound();
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"https://localhost:7134/api/tareas/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
