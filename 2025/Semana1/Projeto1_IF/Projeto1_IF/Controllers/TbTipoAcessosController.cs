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
    public class TbTipoAcessosController : Controller
    {
        private readonly db_IFContext _context;

        public TbTipoAcessosController(db_IFContext context)
        {
            _context = context;
        }

        // GET: TbTipoAcessos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TbTipoAcesso.ToListAsync());
        }

        // GET: TbTipoAcessos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbTipoAcesso = await _context.TbTipoAcesso
                .FirstOrDefaultAsync(m => m.IdTipoAcesso == id);
            if (tbTipoAcesso == null)
            {
                return NotFound();
            }

            return View(tbTipoAcesso);
        }

        // GET: TbTipoAcessos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TbTipoAcessos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoAcesso,Nome,FlagAtivo")] TbTipoAcesso tbTipoAcesso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbTipoAcesso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tbTipoAcesso);
        }

        // GET: TbTipoAcessos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbTipoAcesso = await _context.TbTipoAcesso.FindAsync(id);
            if (tbTipoAcesso == null)
            {
                return NotFound();
            }
            return View(tbTipoAcesso);
        }

        // POST: TbTipoAcessos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoAcesso,Nome,FlagAtivo")] TbTipoAcesso tbTipoAcesso)
        {
            if (id != tbTipoAcesso.IdTipoAcesso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbTipoAcesso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbTipoAcessoExists(tbTipoAcesso.IdTipoAcesso))
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
            return View(tbTipoAcesso);
        }

        // GET: TbTipoAcessos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbTipoAcesso = await _context.TbTipoAcesso
                .FirstOrDefaultAsync(m => m.IdTipoAcesso == id);
            if (tbTipoAcesso == null)
            {
                return NotFound();
            }

            return View(tbTipoAcesso);
        }

        // POST: TbTipoAcessos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbTipoAcesso = await _context.TbTipoAcesso.FindAsync(id);
            if (tbTipoAcesso != null)
            {
                _context.TbTipoAcesso.Remove(tbTipoAcesso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbTipoAcessoExists(int id)
        {
            return _context.TbTipoAcesso.Any(e => e.IdTipoAcesso == id);
        }
    }
}
