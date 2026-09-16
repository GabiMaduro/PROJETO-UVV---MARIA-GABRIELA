using Microsoft.AspNetCore.Identity;
using SistemaConsultasUVV.Models;

namespace SistemaConsultasUVV.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public string GerarHash(Usuario usuario, string senha) =>
        _passwordHasher.HashPassword(usuario, senha);

    public bool Verificar(Usuario usuario, string senhaHash, string senhaInformada) =>
        _passwordHasher.VerifyHashedPassword(usuario, senhaHash, senhaInformada)
        is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
}
