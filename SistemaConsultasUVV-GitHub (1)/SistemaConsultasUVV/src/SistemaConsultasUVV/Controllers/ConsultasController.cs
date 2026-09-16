using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;

namespace SistemaConsultasUVV.Controllers;

[Authorize]
public class ConsultasController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var usuarioId = ObterUsuarioId();
        var consultas = await context.Consultas
            .AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.DataHora)
            .ToListAsync();

        return View(consultas);
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var consulta = await EncontrarConsultaDoUsuarioAsync(id, false);
        return consulta is null ? NotFound() : View(consulta);
    }

    [HttpGet]
    public IActionResult Criar() => View(new Consulta
    {
        DataHora = DateTime.Now.AddDays(1).Date.AddHours(9)
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar([Bind("Especialidade,DataHora,Descricao")] Consulta consulta)
    {
        ValidarDataHora(consulta.DataHora);
        if (!ModelState.IsValid)
            return View(consulta);

        consulta.UsuarioId = ObterUsuarioId();
        context.Consultas.Add(consulta);
        await context.SaveChangesAsync();

        TempData["Sucesso"] = "Consulta cadastrada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var consulta = await EncontrarConsultaDoUsuarioAsync(id, false);
        return consulta is null ? NotFound() : View(consulta);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, [Bind("Id,Especialidade,DataHora,Descricao")] Consulta entrada)
    {
        if (id != entrada.Id)
            return BadRequest();

        ValidarDataHora(entrada.DataHora);
        if (!ModelState.IsValid)
            return View(entrada);

        var consulta = await EncontrarConsultaDoUsuarioAsync(id, true);
        if (consulta is null)
            return NotFound();

        consulta.Especialidade = entrada.Especialidade.Trim();
        consulta.DataHora = entrada.DataHora;
        consulta.Descricao = entrada.Descricao.Trim();
        await context.SaveChangesAsync();

        TempData["Sucesso"] = "Consulta atualizada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Excluir(int id)
    {
        var consulta = await EncontrarConsultaDoUsuarioAsync(id, false);
        return consulta is null ? NotFound() : View(consulta);
    }

    [HttpPost, ActionName("Excluir")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarExclusao(int id)
    {
        var consulta = await EncontrarConsultaDoUsuarioAsync(id, true);
        if (consulta is null)
            return NotFound();

        context.Consultas.Remove(consulta);
        await context.SaveChangesAsync();
        TempData["Sucesso"] = "Consulta excluída com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private int ObterUsuarioId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Usuário autenticado sem identificador."));

    private Task<Consulta?> EncontrarConsultaDoUsuarioAsync(int id, bool rastrear)
    {
        var query = rastrear ? context.Consultas.AsQueryable() : context.Consultas.AsNoTracking();
        var usuarioId = ObterUsuarioId();
        return query.SingleOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
    }

    private void ValidarDataHora(DateTime dataHora)
    {
        if (dataHora <= DateTime.Now)
            ModelState.AddModelError(nameof(Consulta.DataHora), "Escolha uma data e um horário futuros.");
    }
}
