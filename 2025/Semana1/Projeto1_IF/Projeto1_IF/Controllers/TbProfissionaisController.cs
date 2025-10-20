using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.Win32;
using Projeto1_IF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Projeto1_IF.Controllers
{
    [Authorize]
    public class TbProfissionaisController : Controller
    {
        private readonly db_IFContext _context;
        private string idUser;

        public TbProfissionaisController(db_IFContext context)
        {
            _context = context;
        }

        //Criado um Enum para os Planos
        public enum Planos
        {
            MedicoTotal = 1,
            MedicoParcial = 2,
            NutricionistaTotal = 3,
            NutricionistaParcial = 4
        }

        public IQueryable<TbPlano> BuscarPlanosPorPalavras(List<string> palavras)
        {
            return _context.TbPlano
                           .Where(p => palavras.Any(filtro => p.Nome.Contains(filtro)));
        }

        // GET: TbProfissionais
        //[Authorize(Roles = "GerenteMedico,GerenteNutricionista")]
        //[Authorize(Roles = "Medico,Nutricionista")]
        public async Task<IActionResult> Index()
        {
            #region Outras formas de busca
            // Método 1
            //var db_IFContext = _context.TbProfissional
            //                    .Include(t => t.IdCidadeNavigation)
            //                    .Include(t => t.IdContratoNavigation)
            //                         .ThenInclude(t => t.IdPlanoNavigation)
            //                    .Include(t => t.IdTipoAcessoNavigation)
            //                    .Where(t => t.IdContratoNavigation.IdPlano == 1);

            // Método 2
            //var db_IFContext = (from pro in _context.TbProfissional
            //                    where (Plano)pro.IdContratoNavigation.IdPlano == Plano.MedicoTotal
            //                    select pro)
            //                      .Include(t => t.IdCidadeNavigation)
            //                      .Include(t => t.IdContratoNavigation)
            //                      .Include(pro => pro.IdContratoNavigation)
            //                        .ThenInclude(t => t.IdPlanoNavigation);

            // Método 3
            //var db_IFContext = from pro in _context.TbProfissional
            //                   join contrato in _context.TbContrato on pro.IdContrato equals contrato.IdContrato
            //                   join plano in _context.TbPlano on contrato.IdPlano equals plano.IdPlano
            //                   where plano.IdPlano == 1
            //                   select pro;

            #endregion

            IQueryable<TbProfissionalDTO> db_IFContext;
            IQueryable<TbPlano> planos;

            var ehGerente = User.IsInRole("GerenteGeral") || User.IsInRole("GerenteMedico") || User.IsInRole("GerenteNutricionista");

            if ((!ehGerente) && (User.IsInRole("Medico") || User.IsInRole("Nutricionista")))
            {
                // Pega o id do Usuário no Claims com as informações já repassada no User
                var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

                db_IFContext = (from pro in _context.TbProfissional
                                where pro.IdUser == idUser //Filtra apenas a informação do usuário logado
                                select new TbProfissionalDTO
                                {
                                    IdProfissional = pro.IdProfissional,
                                    Nome = pro.Nome,
                                    NomeCidade = pro.IdCidadeNavigation.Nome,
                                    NomeUF = pro.IdCidadeNavigation.IdEstadoNavigation.Uf,
                                    NomePlano = pro.IdContratoNavigation.IdPlanoNavigation.Nome,
                                    Cpf = pro.Cpf,
                                    CrmCrn = pro.CrmCrn,
                                    Especialidade = pro.Especialidade,
                                    Logradouro = pro.Logradouro,
                                    Numero = pro.Numero,
                                    Bairro = pro.Bairro,
                                    Cep = pro.Cep,
                                    Ddd1 = pro.Ddd1,
                                    Ddd2 = pro.Ddd2,
                                    Telefone1 = pro.Telefone1,
                                    Telefone2 = pro.Telefone2,
                                    Salario = pro.Salario,
                                });
            }
            else
            {
                var palavras = new List<string> { "Médico", "Nutricionista" };

                if (User.IsInRole("GerenteMedico"))
                {
                   planos = BuscarPlanosPorPalavras(new List<string> { "Médico" });                                   
                }
                else if (User.IsInRole("GerenteNutricionista"))
                {
                    planos = BuscarPlanosPorPalavras(new List<string> { "Nutricionista" });                                   
                }
                else
                {
                    planos = BuscarPlanosPorPalavras(new List<string> { "Médico", "Nutricionista" });
            ;
                }
                db_IFContext = BuscaInfoProfissionais(planos);
            }
            
            return View(await db_IFContext.ToListAsync());
        }

        /// <summary>
        /// Criado um método para buscar as informações dos profissionais com base nos planos
        /// </summary>
        /// <param name="tpPlano">Lista dos planos</param>
        /// <returns></returns>
        public IQueryable<TbProfissionalDTO> BuscaInfoProfissionais(IQueryable<TbPlano> tpPlano)
        {
            return (from pro in _context.TbProfissional
                    where tpPlano.Any(filtro => filtro.IdPlano == pro.IdContratoNavigation.IdPlano)
                    select new TbProfissionalDTO
                    {
                        IdProfissional = pro.IdProfissional,
                        Nome = pro.Nome,
                        NomeCidade = pro.IdCidadeNavigation.Nome,
                        NomePlano = pro.IdContratoNavigation.IdPlanoNavigation.Nome,
                        Cpf = pro.Cpf,
                        CrmCrn = pro.CrmCrn,
                        Especialidade = pro.Especialidade,
                        Logradouro = pro.Logradouro,
                        Numero = pro.Numero,
                        Bairro = pro.Bairro,
                        Cep = pro.Cep,
                        Ddd1 = pro.Ddd1,
                        Ddd2 = pro.Ddd2,
                        Telefone1 = pro.Telefone1,
                        Telefone2 = pro.Telefone2,
                        Salario = pro.Salario,
                    });
        }

        // GET: TbProfissionais/Details/5
        [Authorize(Roles = "Medico,Nutricionista")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProfissional = await _context.TbProfissional
                .Include(t => t.IdCidadeNavigation)
                .Include(t => t.IdContratoNavigation)
                .Include(t => t.IdTipoAcessoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdProfissional == id);
            if (tbProfissional == null)
            {
                return NotFound();
            }

            return View(tbProfissional);
        }

        // GET: TbProfissionais/Create
        [Authorize(Roles = "Medico,Nutricionista")]
        public IActionResult Create()
        {            

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome");
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome");
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome");
            return View();
        }

        // POST: TbProfissionais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoProfissional,IdTipoAcesso,IdCidade,IdUser,Nome,Cpf,CrmCrn,Especialidade,Logradouro,Numero,Bairro,Cep,Ddd1,Ddd2,Telefone1,Telefone2,Salario")] TbProfissional tbProfissional, [Bind("IdPlano")] TbContrato IdContratoNavigation)
        {
            try
            {
                ModelState.Remove("IdUser");
                ModelState.Remove("IdContrato");
                if (ModelState.IsValid)
                {
                    IdContratoNavigation.DataInicio = DateTime.UtcNow;
                    IdContratoNavigation.DataFim = IdContratoNavigation.DataInicio.Value.AddMonths(1);
                    _context.Add(IdContratoNavigation);
                    await _context.SaveChangesAsync();

                    var userManager = HttpContext.RequestServices.GetService<UserManager<IdentityUser>>();

                    if (userManager != null)
                    {
                        var email = User.Identity?.Name;
                        if (email != null)
                        {
                            var user = await userManager.FindByEmailAsync(email);
                            if (user != null)
                            {
                                tbProfissional.IdUser = user.Id;
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }

                    tbProfissional.IdContrato = IdContratoNavigation.IdContrato;
                    _context.Add(tbProfissional);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Não é possível salvar o registro! Erro: " + ex.Message);
            } catch (Exception ex)
            {
                ModelState.AddModelError("", "Não é possível salvar o registro! Erro: " + ex.Message);
            }
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbProfissional.IdCidade);
            ViewData["IdContrato"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", IdContratoNavigation.IdPlano);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // GET: TbProfissionais/Edit/5        
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var tbProfissional = await _context.TbProfissional.Include(t => t.IdContratoNavigation).FirstOrDefaultAsync(s => s.IdProfissional == id); 

            if (tbProfissional == null)
            {
                return NotFound();
            }
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbProfissional.IdCidade);
            ViewData["IdContrato"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", tbProfissional.IdContratoNavigation.IdPlano);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // POST: TbProfissionais/Edit/5
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

            var tbProfissional = await _context.TbProfissional.Include(t => t.IdContratoNavigation).FirstOrDefaultAsync(s => s.IdProfissional == id);
            if (tbProfissional == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<TbProfissional>(
               tbProfissional, "",
               s => s.IdProfissional, s => s.IdTipoAcesso, s => s.IdCidade, s => s.Nome, s => s.Cpf, s => s.CrmCrn,
               s => s.Especialidade, s => s.Logradouro, s => s.Numero, s => s.Bairro, s => s.Cep, 
               s => s.Ddd1, s => s.Ddd2, s => s.Telefone1, s => s.Telefone2, s => s.Salario))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Erro ao realizar as atualizações! Erro: " + ex.Message);
                }
            }

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", tbProfissional.IdCidade);
            ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", tbProfissional.IdContratoNavigation.IdPlano);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", tbProfissional.IdTipoAcesso);
            return View(tbProfissional);
        }

        // GET: TbProfissionais/Delete/5
        [Authorize(Roles = "GerenteGeral,GerenteMedico,GerenteNutricionista")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            IQueryable<TbMedicoPacienteDTO> tbMedicoPaciente;

            if (id == null)
            {
                return NotFound();
            }

            
            /*var tbProfissional = await _context.TbProfissional
                .Include(t => t.IdCidadeNavigation)
                .ThenInclude(e => e.IdEstadoNavigation)
                .Include(t => t.IdContratoNavigation)
                .ThenInclude(s => s.IdPlanoNavigation)
                .Include(t => t.IdTipoAcessoNavigation)                
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdProfissional == id);*/
             
            // MUDADO PARA OUTRO REPOSITORIO DE PROFISSIONAIS PARA VALIDAR SE TEM PACIENTES
             var tbProfissionalDTO = (from pro in _context.TbProfissional
                                      where pro.IdProfissional == id //Filtra apenas a informação do usuário logado
                                        select new TbProfissionalDTO
                                        {
                                            IdProfissional = pro.IdProfissional,
                                            Nome = pro.Nome,
                                            NomeCidade = pro.IdCidadeNavigation.Nome,
                                            NomeUF = pro.IdCidadeNavigation.IdEstadoNavigation.Uf,
                                            NomePlano = pro.IdContratoNavigation.IdPlanoNavigation.Nome,
                                            Cpf = pro.Cpf,
                                            CrmCrn = pro.CrmCrn,
                                            Especialidade = pro.Especialidade,
                                            Logradouro = pro.Logradouro,
                                            Numero = pro.Numero,
                                            Bairro = pro.Bairro,
                                            Cep = pro.Cep,
                                            Ddd1 = pro.Ddd1,
                                            Ddd2 = pro.Ddd2,
                                            Telefone1 = pro.Telefone1,
                                            Telefone2 = pro.Telefone2,
                                            Salario = pro.Salario,
                                            TemPacientes = false
                                        }).FirstOrDefault();

            if (tbProfissionalDTO == null)
            {
                return NotFound();
            }

            // VALIDA SE TEM PACIENTES E MARCA O PROFISSIONAL COM A MENSAGEM
            tbMedicoPaciente = BuscaPacientes(tbProfissionalDTO.IdProfissional);

            if (tbMedicoPaciente.Count() > 0)
            {
                tbProfissionalDTO.TemPacientes = true;
                tbProfissionalDTO.Mensagem = "Profissional possui pacientes! Eles serão excluídos!";            
            } 

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(tbProfissionalDTO);
        }

        // POST: TbProfissionais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            IQueryable<TbMedicoPacienteDTO> tbPacientes;

            var tbProfissional = await _context.TbProfissional.FindAsync(id);

            if (tbProfissional == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // VALIDA SE TEM PACIENTES E OS EXCLUÍ PRIMEIRO
                tbPacientes = BuscaPacientes(tbProfissional.IdProfissional);

                if (tbPacientes.Count() > 0)
                {
                    foreach (var item in tbPacientes)
                    {

                        //EXCLUI A LIGACAO ENTRE AS TABELAS
                        _context.TbMedicoPaciente.Remove(item.tbMedicoPaciente);
                        await _context.SaveChangesAsync();

                        _context.TbPaciente.Remove(item.tbPaciente);
                        await _context.SaveChangesAsync();
                    }
                }

                _context.TbProfissional.Remove(tbProfissional);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            } catch
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
            
        }

        // METODO PARA BUSCAR OS PACIENTES PARA DEPOIS REALIZAR A EXCLUSÃO
        public IQueryable<TbMedicoPacienteDTO> BuscaPacientes(int id)
        {
            return  (from pac in _context.TbPaciente
                    join medpac in _context.TbMedicoPaciente on pac.IdPaciente equals medpac.IdPaciente
                    join cid in _context.TbCidade on pac.IdCidade equals cid.IdCidade
                    where medpac.IdProfissional == id
                    select new TbMedicoPacienteDTO 
                    {
                        tbPaciente = pac,
                        tbMedicoPaciente = medpac
                    });
        }
    }
}
