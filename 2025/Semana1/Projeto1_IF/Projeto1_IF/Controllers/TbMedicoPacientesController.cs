using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto1_IF.Models;

namespace Projeto1_IF.Controllers
{
    public class TbMedicoPacientesController : Controller
    {
        private readonly db_IFContext _context;

        public TbMedicoPacientesController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbMedicoPacientes
        public async Task<IActionResult> Index()
        {
            var db_IFContext = _context.TbMedicoPaciente.Include(t => t.IdPacienteNavigation).Include(t => t.IdProfissionalNavigation);
            return View(await db_IFContext.ToListAsync());
        }

        // GET: TbMedicoPacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMedicoPaciente = await _context.TbMedicoPaciente
                .Include(t => t.IdPacienteNavigation)
                .Include(t => t.IdProfissionalNavigation)
                .FirstOrDefaultAsync(m => m.IdMedicoPaciente == id);
            if (tbMedicoPaciente == null)
            {
                return NotFound();
            }

            return View(tbMedicoPaciente);
        }

        // GET: TbMedicoPacientes/Create
        public IActionResult Create()
        {
            ViewData["IdPaciente"] = new SelectList(_context.TbPaciente, "IdPaciente", "Cpf");
            ViewData["IdProfissional"] = new SelectList(_context.TbProfissional, "IdProfissional", "Bairro");
            return View();
        }

        // POST: TbMedicoPacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMedicoPaciente,IdPaciente,IdProfissional,InformacaoResumida")] TbMedicoPaciente tbMedicoPaciente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbMedicoPaciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPaciente"] = new SelectList(_context.TbPaciente, "IdPaciente", "Cpf", tbMedicoPaciente.IdPaciente);
            ViewData["IdProfissional"] = new SelectList(_context.TbProfissional, "IdProfissional", "Bairro", tbMedicoPaciente.IdProfissional);
            return View(tbMedicoPaciente);
        }

        // GET: TbMedicoPacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMedicoPaciente = await _context.TbMedicoPaciente.FindAsync(id);
            if (tbMedicoPaciente == null)
            {
                return NotFound();
            }
            ViewData["IdPaciente"] = new SelectList(_context.TbPaciente, "IdPaciente", "Cpf", tbMedicoPaciente.IdPaciente);
            ViewData["IdProfissional"] = new SelectList(_context.TbProfissional, "IdProfissional", "Bairro", tbMedicoPaciente.IdProfissional);
            return View(tbMedicoPaciente);
        }

        // POST: TbMedicoPacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMedicoPaciente,IdPaciente,IdProfissional,InformacaoResumida")] TbMedicoPaciente tbMedicoPaciente)
        {
            if (id != tbMedicoPaciente.IdMedicoPaciente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbMedicoPaciente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMedicoPacienteExists(tbMedicoPaciente.IdMedicoPaciente))
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
            ViewData["IdPaciente"] = new SelectList(_context.TbPaciente, "IdPaciente", "Cpf", tbMedicoPaciente.IdPaciente);
            ViewData["IdProfissional"] = new SelectList(_context.TbProfissional, "IdProfissional", "Bairro", tbMedicoPaciente.IdProfissional);
            return View(tbMedicoPaciente);
        }

        // GET: TbMedicoPacientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMedicoPaciente = await _context.TbMedicoPaciente
                .Include(t => t.IdPacienteNavigation)
                .Include(t => t.IdProfissionalNavigation)
                .FirstOrDefaultAsync(m => m.IdMedicoPaciente == id);
            if (tbMedicoPaciente == null)
            {
                return NotFound();
            }

            return View(tbMedicoPaciente);
        }

        // POST: TbMedicoPacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbMedicoPaciente = await _context.TbMedicoPaciente.FindAsync(id);
            if (tbMedicoPaciente != null)
            {
                _context.TbMedicoPaciente.Remove(tbMedicoPaciente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbMedicoPacienteExists(int id)
        {
            return _context.TbMedicoPaciente.Any(e => e.IdMedicoPaciente == id);
        }
    }
}
