using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto1_IF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Projeto1_IF.Controllers
{
    [Authorize]
    public class TbPacientesController : Controller
    {
        private readonly db_IFContext _context;

        public TbPacientesController(db_IFContext context)
        {
            _context = context;
        }

        IQueryable<TbPacienteDTO> db_IFContext;

        // GET: TbPacientes

        public async Task<IActionResult> Index()
        {
             // REALIZA A BUSCA DE PACIENTES SOMENTE DO USUÁRIO 
             db_IFContext = (from pac in _context.TbPaciente
                             join medpac in _context.TbMedicoPaciente on pac.IdPaciente equals medpac.IdPaciente
                             join cid in _context.TbCidade on pac.IdCidade equals cid.IdCidade
                             where medpac.IdProfissional == BuscaProfissional()
                             select new TbPacienteDTO
                             {
                                 IdPaciente = pac.IdPaciente,
                                 Nome = pac.Nome,
                                 Rg = pac.Rg,
                                 Cpf = pac.Cpf,
                                 DataNascimento = pac.DataNascimento,
                                 NomeResponsavel = pac.NomeResponsavel,
                                 Sexo = pac.Sexo,
                                 Etnia = pac.Etnia,
                                 Endereco = pac.Endereco,
                                 Bairro = pac.Bairro,
                                 TelResidencial = pac.TelResidencial,
                                 TelComercial = pac.TelComercial,
                                 TelCelular = pac.TelCelular,
                                 Profissao = pac.Profissao,
                                 Atleta = ((bool)pac.FlgAtleta ? "Sim" : "Não"),
                                 Gestante =((bool)pac.FlgGestante ? "Sim" : "Não"),
                                 NomeCidade = cid.Nome
                             });

            //var db_IFContext = _context.TbPaciente.Include(t => t.IdCidadeNavigation);
            return View(await db_IFContext.ToListAsync());
        }

        // GET: TbPacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbPaciente = await _context.TbPaciente
                .Include(t => t.IdCidadeNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id);
            if (tbPaciente == null)
            {
                return NotFound();
            }

            return View(tbPaciente);
        }

        // GET: TbPacientes/Create
        public IActionResult Create()
        {
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome");
            ViewBag.SexoList = new List<SelectListItem>
            {
                new SelectListItem { Value = "M", Text = "Masculino" },
                new SelectListItem { Value = "F", Text = "Feminino" },
                new SelectListItem { Value = "B", Text = "Não Binário" },
                new SelectListItem { Value = "N", Text = "Não Informado" }
            };
            ViewBag.EtniaList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Branca" },
                new SelectListItem { Value = "2", Text = "Preta" },
                new SelectListItem { Value = "3", Text = "Parda" },
                new SelectListItem { Value = "4", Text = "Amarela" },
                new SelectListItem { Value = "5", Text = "Indígena" },
                new SelectListItem { Value = "9", Text = "Não Informada" }
            };
            ViewBag.AtletaList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Sim" },
                new SelectListItem { Value = "false", Text = "Não" }
            };
            ViewBag.GestanteList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Sim" },
                new SelectListItem { Value = "false", Text = "Não" }
            };
            return View();
        }

        // POST: TbPacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Rg,Cpf,DataNascimento,NomeResponsavel,Sexo,Etnia,Endereco,Bairro,IdCidade,TelResidencial,TelComercial,TelCelular,Profissao,FlgAtleta,FlgGestante")] TbPaciente tbPaciente, [Bind("IdMedicoPaciente")] TbMedicoPaciente idMedicoPacienteNavigation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(tbPaciente);
                    await _context.SaveChangesAsync();

                    idMedicoPacienteNavigation.IdProfissional = BuscaProfissional();
                    idMedicoPacienteNavigation.IdPaciente = tbPaciente.IdPaciente;
                    idMedicoPacienteNavigation.InformacaoResumida = "Inserido pelo sistema!";
                    _context.Add(idMedicoPacienteNavigation);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbPaciente.IdCidade);
                ViewBag.SexoList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "M", Text = "Masculino" },
                    new SelectListItem { Value = "F", Text = "Feminino" },
                    new SelectListItem { Value = "B", Text = "Não Binário" },
                    new SelectListItem { Value = "N", Text = "Não Informado" }
                };
                ViewBag.EtniaList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Branca" },
                    new SelectListItem { Value = "2", Text = "Preta" },
                    new SelectListItem { Value = "3", Text = "Parda" },
                    new SelectListItem { Value = "4", Text = "Amarela" },
                    new SelectListItem { Value = "5", Text = "Indígena" },
                    new SelectListItem { Value = "9", Text = "Não Informada" }
                };
                ViewBag.AtletaList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Sim" },
                    new SelectListItem { Value = "false", Text = "Não" }
                };
                ViewBag.GestanteList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Sim" },
                    new SelectListItem { Value = "false", Text = "Não" }
                };
            }
            catch (DbUpdateException ex )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Erro ao criar o paciente. " +
                    "Erro: " + ex.Message);                    
            }
           
            return View(tbPaciente);
        }

        public int BuscaProfissional()
        {
            // Pega o id do Usuário no Claims com as informações já repassada no User
            var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _context.TbProfissional
                   .Where(prof => prof.IdUser == idUser)
                   .Select(prof => prof.IdProfissional)
                   .FirstOrDefault();
        }

        // GET: TbPacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }            

            var tbPaciente = await _context.TbPaciente.Include(t => t.IdCidadeNavigation).FirstOrDefaultAsync(s => s.IdPaciente == id);
           
            if (tbPaciente == null)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbPaciente.IdCidade);
            ViewBag.SexoList = new List<SelectListItem>
            {
                new SelectListItem { Value = "M", Text = "Masculino" },
                new SelectListItem { Value = "F", Text = "Feminino" },
                new SelectListItem { Value = "B", Text = "Não Binário" },
                new SelectListItem { Value = "N", Text = "Não Informado" }
            };
            ViewBag.EtniaList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Branca" },
                new SelectListItem { Value = "2", Text = "Preta" },
                new SelectListItem { Value = "3", Text = "Parda" },
                new SelectListItem { Value = "4", Text = "Amarela" },
                new SelectListItem { Value = "5", Text = "Indígena" },
                new SelectListItem { Value = "9", Text = "Não Informada" }
            };
            ViewBag.AtletaList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Sim" },
                new SelectListItem { Value = "false", Text = "Não" }
            };
            ViewBag.GestanteList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Sim" },
                new SelectListItem { Value = "false", Text = "Não" }
            };
            return View(tbPaciente);
        }

        // POST: TbPacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var tbPaciente = await _context.TbPaciente.Include(t => t.IdCidadeNavigation).FirstOrDefaultAsync(s => s.IdPaciente == id);

            if (await TryUpdateModelAsync<TbPaciente>(
                tbPaciente,
                "",
                s => s.Nome, s => s.Rg, s => s.Cpf, s => s.DataNascimento, s => s.NomeResponsavel, s => s.Sexo, s => s.Etnia,
                s => s.Endereco, s => s.Bairro, s => s.IdCidade, s => s.TelResidencial, s => s.TelComercial, s => s.TelCelular,
                s => s.Profissao, s => s.FlgAtleta, s => s.FlgGestante))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Erro ao atualizar paciente. " +
                        "Erro: " + ex.Message);
                }
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbPaciente.IdCidade);
            ViewBag.SexoList = new List<SelectListItem>
            {
                new SelectListItem { Value = "M", Text = "Masculino" },
                new SelectListItem { Value = "F", Text = "Feminino" },
                new SelectListItem { Value = "B", Text = "Não Binário" },
                new SelectListItem { Value = "N", Text = "Não Informado" }
            };
            ViewBag.EtniaList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Branca" },
                new SelectListItem { Value = "2", Text = "Preta" },
                new SelectListItem { Value = "3", Text = "Parda" },
                new SelectListItem { Value = "4", Text = "Amarela" },
                new SelectListItem { Value = "5", Text = "Indígena" },
                new SelectListItem { Value = "9", Text = "Não Informada" }
            };
            ViewBag.AtletaList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Sim" },
                new SelectListItem { Value = "false", Text = "Não" }
            };
            ViewBag.GestanteList = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Sim" },
                new SelectListItem { Value = "false", Text = "Não" }
            };
            return View(tbPaciente);
        }

        // GET: TbPacientes/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var tbPaciente = await _context.TbPaciente
                .Include(t => t.IdCidadeNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdPaciente == id);
            if (tbPaciente == null)
            {
                return RedirectToAction("Error", "Home");
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Erro ao excluir. Tente novamente mais tarde.";
            }

            return View(tbPaciente);
        }

        // POST: TbPacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbPaciente = await _context.TbPaciente.FindAsync(id);
            if (tbPaciente == null)
            {
                return RedirectToAction(nameof(Index));                
            }

            try
            {
                var tbMedicoPaciente = BuscaPacientes(tbPaciente.IdPaciente);

                if (tbMedicoPaciente.Count() > 0)
                {
                    foreach (var item in tbMedicoPaciente)
                    {
                        _context.TbMedicoPaciente.Remove(item.tbMedicoPaciente);
                        await _context.SaveChangesAsync();
                    }                   
                }

                _context.TbPaciente.Remove(tbPaciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }         
            
        }


        private bool TbPacienteExists(int id)
        {
            return _context.TbPaciente.Any(e => e.IdPaciente == id);
        }

        // METODO PARA BUSCAR OS PACIENTES PARA DEPOIS REALIZAR A EXCLUSÃO
        public IQueryable<TbMedicoPacienteDTO> BuscaPacientes(int id)
        {
            return (from pac in _context.TbPaciente
                    join medpac in _context.TbMedicoPaciente on pac.IdPaciente equals medpac.IdPaciente
                    join cid in _context.TbCidade on pac.IdCidade equals cid.IdCidade
                    where medpac.IdPaciente == id
                    select new TbMedicoPacienteDTO
                    {
                        tbPaciente = pac,
                        tbMedicoPaciente = medpac
                    });
        }
    }
}
