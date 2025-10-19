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
    public class TbTipoProfissionalController : Controller
    {
        private readonly db_IFContext _context;

        public TbTipoProfissionalController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbTipoProfissional
        public async Task<IActionResult> Index()
        {
            return View(await _context.TbTipoProfissional.ToListAsync());
        }

        // GET: TbTipoProfissional/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbTipoProfissional = await _context.TbTipoProfissional
                .FirstOrDefaultAsync(m => m.IdTipoProfissional == id);
            if (tbTipoProfissional == null)
            {
                return NotFound();
            }

            return View(tbTipoProfissional);
        }

        // GET: TbTipoProfissional/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TbTipoProfissional/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoProfissional,Nome")] TbTipoProfissional tbTipoProfissional)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbTipoProfissional);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tbTipoProfissional);
        }

        // GET: TbTipoProfissional/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbTipoProfissional = await _context.TbTipoProfissional.FindAsync(id);
            if (tbTipoProfissional == null)
            {
                return NotFound();
            }
            return View(tbTipoProfissional);
        }

        // POST: TbTipoProfissional/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoProfissional,Nome")] TbTipoProfissional tbTipoProfissional)
        {
            if (id != tbTipoProfissional.IdTipoProfissional)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbTipoProfissional);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbTipoProfissionalExists(tbTipoProfissional.IdTipoProfissional))
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
            return View(tbTipoProfissional);
        }

        // GET: TbTipoProfissional/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbTipoProfissional = await _context.TbTipoProfissional
                .FirstOrDefaultAsync(m => m.IdTipoProfissional == id);
            if (tbTipoProfissional == null)
            {
                return NotFound();
            }

            return View(tbTipoProfissional);
        }

        // POST: TbTipoProfissional/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbTipoProfissional = await _context.TbTipoProfissional.FindAsync(id);
            if (tbTipoProfissional != null)
            {
                _context.TbTipoProfissional.Remove(tbTipoProfissional);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbTipoProfissionalExists(int id)
        {
            return _context.TbTipoProfissional.Any(e => e.IdTipoProfissional == id);
        }
    }
}
