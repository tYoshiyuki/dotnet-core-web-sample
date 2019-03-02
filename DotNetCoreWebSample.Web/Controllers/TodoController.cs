using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreWebSample.Web.Models;
using DotNetCoreWebSample.Web.Services;

namespace DotNetCoreWebSample.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class TodoController : Controller
    {
        private readonly IToDoService _service;

        public TodoController(DotnetCoreWebSampleContext context, IToDoService service)
        {
            _service = service;
        }

        // GET: Todo
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetList());
        }

        // GET: Todo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _service.Get(id.Value);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // GET: Todo/Create
        public IActionResult Create()
        {
            return View(new Todo { CreatedDate = DateTime.Now });
        }

        // POST: Todo/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Description,CreatedDate")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                await _service.Create(todo);
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _service.Get(id.Value);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        // POST: Todo/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,CreatedDate")] Todo todo)
        {
            if (id != todo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.Edit(todo);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _service.Get(id.Value);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todo/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _service.Get(id);
            await _service.Delete(todo);
            return RedirectToAction(nameof(Index));
        }

        private bool TodoExists(int id)
        {            
            return _service.Exists(id);
        }
    }
}
