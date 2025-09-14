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
    public class TbCategoriaMedicamentosController : Controller
    {
        private readonly db_IFContext _context;

        public TbCategoriaMedicamentosController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbCategoriaMedicamentos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TbCategoriaMedicamento.ToListAsync());
        }

        // GET: TbCategoriaMedicamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbCategoriaMedicamento = await _context.TbCategoriaMedicamento
                .FirstOrDefaultAsync(m => m.IdCategoriaMedicamento == id);
            if (tbCategoriaMedicamento == null)
            {
                return NotFound();
            }

            return View(tbCategoriaMedicamento);
        }

        // GET: TbCategoriaMedicamentos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TbCategoriaMedicamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategoriaMedicamento,Nome,InformacaoComplementar")] TbCategoriaMedicamento tbCategoriaMedicamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbCategoriaMedicamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tbCategoriaMedicamento);
        }

        // GET: TbCategoriaMedicamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbCategoriaMedicamento = await _context.TbCategoriaMedicamento.FindAsync(id);
            if (tbCategoriaMedicamento == null)
            {
                return NotFound();
            }
            return View(tbCategoriaMedicamento);
        }

        // POST: TbCategoriaMedicamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoriaMedicamento,Nome,InformacaoComplementar")] TbCategoriaMedicamento tbCategoriaMedicamento)
        {
            if (id != tbCategoriaMedicamento.IdCategoriaMedicamento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbCategoriaMedicamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbCategoriaMedicamentoExists(tbCategoriaMedicamento.IdCategoriaMedicamento))
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
            return View(tbCategoriaMedicamento);
        }

        // GET: TbCategoriaMedicamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbCategoriaMedicamento = await _context.TbCategoriaMedicamento
                .FirstOrDefaultAsync(m => m.IdCategoriaMedicamento == id);
            if (tbCategoriaMedicamento == null)
            {
                return NotFound();
            }

            return View(tbCategoriaMedicamento);
        }

        // POST: TbCategoriaMedicamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbCategoriaMedicamento = await _context.TbCategoriaMedicamento.FindAsync(id);
            if (tbCategoriaMedicamento != null)
            {
                _context.TbCategoriaMedicamento.Remove(tbCategoriaMedicamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbCategoriaMedicamentoExists(int id)
        {
            return _context.TbCategoriaMedicamento.Any(e => e.IdCategoriaMedicamento == id);
        }
    }
}
