using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;
using SistemaConsultasUVV.Services;
using SistemaConsultasUVV.ViewModels;

namespace SistemaConsultasUVV.Controllers;

public class AccountController(
    ApplicationDbContext context,
    IPasswordService passwordService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Cadastro()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Consultas");

        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastro(CadastroViewModel model)
    {
        var emailNormalizado = (model.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (await context.Usuarios.AnyAsync(u => u.Email == emailNormalizado))
            ModelState.AddModelError(nameof(model.Email), "Já existe uma conta com este e-mail.");

        if (!ModelState.IsValid)
            return View(model);

        var usuario = new Usuario
        {
            Nome = model.Nome.Trim(),
            Email = emailNormalizado,
            DataCadastro = DateTime.UtcNow
        };
        usuario.SenhaHash = passwordService.GerarHash(usuario, model.Senha);

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        await EntrarAsync(usuario, false);

        TempData["Sucesso"] = "Conta criada com sucesso. Bem-vindo(a)!";
        return RedirectToAction("Index", "Consultas");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Consultas");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var emailNormalizado = (model.Email ?? string.Empty).Trim().ToLowerInvariant();
        var usuario = await context.Usuarios.SingleOrDefaultAsync(u => u.Email == emailNormalizado);

        if (usuario is null || !passwordService.Verificar(usuario, usuario.SenhaHash, model.Senha))
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        await EntrarAsync(usuario, model.LembrarMe);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return LocalRedirect(model.ReturnUrl);

        return RedirectToAction("Index", "Consultas");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AcessoNegado() => View();

    private async Task EntrarAsync(Usuario usuario, bool persistente)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = persistente,
                ExpiresUtc = persistente ? DateTimeOffset.UtcNow.AddDays(7) : null
            });
    }
}
