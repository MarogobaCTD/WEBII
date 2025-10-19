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
    public class TbContratoesController : Controller
    {
        private readonly db_IFContext _context;

        public TbContratoesController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbContratoes
        public async Task<IActionResult> Index()
        {
            var db_IFContext = _context.TbContrato.Include(t => t.IdPlanoNavigation);
            return View(await db_IFContext.ToListAsync());
        }

        // GET: TbContratoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbContrato = await _context.TbContrato
                .Include(t => t.IdPlanoNavigation)
                .FirstOrDefaultAsync(m => m.IdContrato == id);
            if (tbContrato == null)
            {
                return NotFound();
            }

            return View(tbContrato);
        }

        // GET: TbContratoes/Create
        public IActionResult Create()
        {
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome");
            return View();
        }

        // POST: TbContratoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdContrato,IdPlano,DataInicio,DataFim")] TbContrato tbContrato)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbContrato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", tbContrato.IdPlano);
            return View(tbContrato);
        }

        // GET: TbContratoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbContrato = await _context.TbContrato.FindAsync(id);
            if (tbContrato == null)
            {
                return NotFound();
            }
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", tbContrato.IdPlano);
            return View(tbContrato);
        }

        // POST: TbContratoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdContrato,IdPlano,DataInicio,DataFim")] TbContrato tbContrato)
        {
            if (id != tbContrato.IdContrato)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbContrato);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbContratoExists(tbContrato.IdContrato))
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
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", tbContrato.IdPlano);
            return View(tbContrato);
        }

        // GET: TbContratoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbContrato = await _context.TbContrato
                .Include(t => t.IdPlanoNavigation)
                .FirstOrDefaultAsync(m => m.IdContrato == id);
            if (tbContrato == null)
            {
                return NotFound();
            }

            return View(tbContrato);
        }

        // POST: TbContratoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbContrato = await _context.TbContrato.FindAsync(id);
            if (tbContrato != null)
            {
                _context.TbContrato.Remove(tbContrato);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbContratoExists(int id)
        {
            return _context.TbContrato.Any(e => e.IdContrato == id);
        }
    }
}
