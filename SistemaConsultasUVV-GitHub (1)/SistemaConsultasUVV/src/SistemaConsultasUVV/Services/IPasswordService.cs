using SistemaConsultasUVV.Models;

namespace SistemaConsultasUVV.Services;

public interface IPasswordService
{
    string GerarHash(Usuario usuario, string senha);
    bool Verificar(Usuario usuario, string senhaHash, string senhaInformada);
}
