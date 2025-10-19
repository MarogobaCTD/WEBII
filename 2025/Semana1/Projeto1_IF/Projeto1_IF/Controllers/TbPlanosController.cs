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
    public class TbPlanosController : Controller
    {
        private readonly db_IFContext _context;

        public TbPlanosController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbPlanos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TbPlano.ToListAsync());
        }

        // GET: TbPlanos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbPlano = await _context.TbPlano
                .FirstOrDefaultAsync(m => m.IdPlano == id);
            if (tbPlano == null)
            {
                return NotFound();
            }

            return View(tbPlano);
        }

        // GET: TbPlanos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TbPlanos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPlano,Nome,Validade,Valor")] TbPlano tbPlano)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbPlano);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tbPlano);
        }

        // GET: TbPlanos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbPlano = await _context.TbPlano.FindAsync(id);
            if (tbPlano == null)
            {
                return NotFound();
            }
            return View(tbPlano);
        }

        // POST: TbPlanos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPlano,Nome,Validade,Valor")] TbPlano tbPlano)
        {
            if (id != tbPlano.IdPlano)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbPlano);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbPlanoExists(tbPlano.IdPlano))
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
            return View(tbPlano);
        }

        // GET: TbPlanos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbPlano = await _context.TbPlano
                .FirstOrDefaultAsync(m => m.IdPlano == id);
            if (tbPlano == null)
            {
                return NotFound();
            }

            return View(tbPlano);
        }

        // POST: TbPlanos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbPlano = await _context.TbPlano.FindAsync(id);
            if (tbPlano != null)
            {
                _context.TbPlano.Remove(tbPlano);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbPlanoExists(int id)
        {
            return _context.TbPlano.Any(e => e.IdPlano == id);
        }
    }
}
