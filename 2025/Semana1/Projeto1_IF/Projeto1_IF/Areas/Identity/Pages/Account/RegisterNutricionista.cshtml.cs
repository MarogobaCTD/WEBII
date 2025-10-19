// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Projeto1_IF.Controllers;
using Projeto1_IF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using static Projeto1_IF.Controllers.TbProfissionaisController;

namespace Projeto1_IF.Areas.Identity.Pages.Account
{
    public class RegisterModelNutricionista : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly db_IFContext _context; //acrescentado para trabalhar junto com o cadastro do Profissional

        public RegisterModelNutricionista(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            db_IFContext context  ///Registra o context do profissional para ser preenchido no cadastro
            )            
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///    Incluída a classe do TbProfissional para que os campos sejam reconhecidos no Input   
            /// </summary>
            public TbProfissional tbProfissional { get; set; }

        }

        // MÉTODO PARA BUSCAR APENAS OS PLANOS DOS NUTRICIONISTAS
        public List<TbPlano> BuscaPlanos()
        {
            return this._context.TbPlano
                    .Where(p => p.Nome.Contains("Nutricionista"))
                    .ToList();            
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            // BUSCA APENAS OS PLANOS DOS NUTRICIONISTA PARA SER EXIBIDO
            var planos = BuscaPlanos();

            // Necessário para receber os dados de outras informações que liga com o Profissional
            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome");
            // ViewData["IdPlano"] = new SelectList(_context.TbPlano, "IdPlano", "Nome");
            ViewData["IdPlano"] = new SelectList(planos, "IdPlano", "Nome");
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome");           
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {            
                returnUrl ??= Url.Content("~/");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                // Para ignorar o IdUser do profissional pois ele está sendo criado aqui e repassado para o context
                ModelState.Remove("Input.tbProfissional.IdUser");
                ModelState.Remove("Input.tbProfissional.IdContrato");

                if (ModelState.IsValid)
                {
                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        //Atribuir a role específica ao usuário
                        await _userManager.AddToRoleAsync(user, "Nutricionista");
                        
                        #region Inputs do Profissional
                        //Adiciona-se os inputs do Profissional para o cadastro aqui
                        Input.tbProfissional.IdContratoNavigation.DataInicio = DateTime.UtcNow;
                        Input.tbProfissional.IdContratoNavigation.DataFim = Input.tbProfissional.IdContratoNavigation.DataInicio.Value.AddMonths(1);
                        _context.Add(Input.tbProfissional.IdContratoNavigation);
                        await _context.SaveChangesAsync();
                        
                        Input.tbProfissional.IdUser = user.Id;
                        Input.tbProfissional.IdContrato = Input.tbProfissional.IdContratoNavigation.IdContrato;                            
                        _context.Add(Input.tbProfissional);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                        ///////////////////////////////////////////////////////////////////
                        #endregion

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro Geral: " + ex.Message);
            }

            // BUSCA APENAS OS PLANOS DOS NUTRICIONISTAS
            var planos = BuscaPlanos();

            ViewData["IdCidade"] = new SelectList(_context.TbCidade, "IdCidade", "Nome", Input.tbProfissional.IdCidade);
            //ViewData["IdContrato"] = new SelectList(_context.TbPlano, "IdPlano", "Nome", Input.tbProfissional.IdContratoNavigation.IdPlano);
            ViewData["IdContrato"] = new SelectList(planos, "IdPlano", "Nome", Input.tbProfissional.IdContratoNavigation.IdPlano);
            ViewData["IdTipoAcesso"] = new SelectList(_context.TbTipoAcesso, "IdTipoAcesso", "Nome", Input.tbProfissional.IdTipoAcesso);            
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
