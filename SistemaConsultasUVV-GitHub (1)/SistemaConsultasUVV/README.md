# Sistema de Gestão de Consultas UVV

Aplicação Web desenvolvida em **C# com ASP.NET Core MVC e Entity Framework Core**, criada para o trabalho prático da disciplina de Desenvolvimento Web Back-end. O sistema permite cadastrar usuários, autenticar o acesso e gerenciar consultas vinculadas ao usuário conectado.

## Funcionalidades

- Cadastro de usuário com validação de nome, e-mail e senha;
- login e logout por autenticação baseada em cookie;
- senha persistida com hash, nunca em texto puro;
- cadastro, listagem, detalhamento, edição e exclusão de consultas;
- isolamento dos registros: cada usuário acessa somente suas próprias consultas;
- rotas de consultas protegidas com `[Authorize]`;
- validação no servidor com Data Annotations;
- prevenção de CSRF com Anti-Forgery Token;
- persistência no SQL Server utilizando EF Core e Code First;
- migration inicial incluída no repositório;
- interface responsiva para computador e celular.

## Tecnologias

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server / SQL Server LocalDB
- Razor Views
- HTML e CSS

## Arquitetura

```text
src/SistemaConsultasUVV/
├── Controllers/     # Recebem requisições e coordenam os fluxos
├── Data/            # DbContext do Entity Framework Core
├── Migrations/      # Versionamento da estrutura do banco
├── Models/          # Entidades Usuario e Consulta
├── Services/        # Serviço de hash e verificação de senha
├── ViewModels/      # Modelos específicos de cadastro e login
├── Views/           # Telas Razor organizadas por controller
└── wwwroot/         # Arquivos estáticos e estilos
```

Essa divisão aplica **Separation of Concerns (SoC)**: apresentação, regras de entrada, acesso a dados e segurança permanecem em responsabilidades separadas.

## Pré-requisitos

Instale antes de executar:

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2019 ou superior, SQL Server Express ou LocalDB
- Opcional: Visual Studio 2022 com a carga “Desenvolvimento ASP.NET e Web”

## Configuração do banco de dados

A conexão padrão usa SQL Server LocalDB no Windows:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SistemaConsultasUVV;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Ela está no arquivo `src/SistemaConsultasUVV/appsettings.json`. Se estiver usando SQL Server Express, Docker ou autenticação por usuário e senha, altere apenas o valor de `DefaultConnection`.

Exemplo com SQL Server Express:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=SistemaConsultasUVV;Trusted_Connection=True;TrustServerCertificate=True"
```

Não envie senhas reais do banco ao GitHub. Em ambiente real, utilize User Secrets ou variáveis de ambiente.

## Executar pelo terminal

Na raiz do repositório, execute:

```bash
dotnet restore
dotnet tool install --global dotnet-ef --version 8.*
dotnet ef database update --project src/SistemaConsultasUVV
dotnet run --project src/SistemaConsultasUVV
```

Abra a URL HTTPS exibida no terminal. Pelas configurações do projeto, normalmente será `https://localhost:7248`.

Caso o comando HTTPS apresente erro de certificado local:

```bash
dotnet dev-certs https --trust
```

## Executar pelo Visual Studio

1. Abra `SistemaConsultasUVV.sln` no Visual Studio 2022.
2. Aguarde a restauração dos pacotes NuGet.
3. Confira a Connection String em `appsettings.json`.
4. Abra **Ferramentas > Gerenciador de Pacotes NuGet > Console do Gerenciador de Pacotes**.
5. Selecione `SistemaConsultasUVV` como projeto padrão.
6. Execute:

```powershell
Update-Database
```

7. Pressione `F5` ou clique no botão HTTPS para iniciar.

## Criar uma nova migration

O projeto já contém a migration inicial. Quando houver mudanças futuras nos Models, utilize:

```bash
dotnet ef migrations add NomeDaAlteracao --project src/SistemaConsultasUVV
dotnet ef database update --project src/SistemaConsultasUVV
```

## Fluxo para demonstração

1. Acesse **Criar conta** e faça o cadastro.
2. Saia e realize o login com o usuário criado.
3. Clique em **Nova consulta**.
4. Preencha especialidade, data/hora futura e descrição.
5. Abra os detalhes, edite e depois exclua o registro.
6. Tente abrir `/Consultas` sem estar autenticado para demonstrar a proteção da rota.

Um roteiro mais detalhado está em [`ROTEIRO-VIDEO.md`](ROTEIRO-VIDEO.md).

## Segurança implementada

- `UseAuthentication()` aparece antes de `UseAuthorization()` no pipeline;
- o controller de consultas possui `[Authorize]`;
- consultas são sempre filtradas pelo `NameIdentifier` do usuário autenticado;
- tentativas de acessar uma consulta de outro usuário retornam `404`;
- as senhas usam `PasswordHasher<TUser>` com salt e algoritmo seguro;
- formulários POST usam `[ValidateAntiForgeryToken]`;
- o retorno após login aceita somente URLs locais;
- o e-mail possui índice único no banco.

> A coluna exigida como `Senha` no banco armazena exclusivamente o **hash da senha**. O código usa a propriedade `SenhaHash` para deixar essa proteção explícita.

## Verbos HTTP utilizados

| Operação | Método | Rota principal |
|---|---:|---|
| Exibir cadastro | GET | `/Account/Cadastro` |
| Cadastrar usuário | POST | `/Account/Cadastro` |
| Exibir login | GET | `/Account/Login` |
| Autenticar | POST | `/Account/Login` |
| Listar consultas | GET | `/Consultas` |
| Cadastrar consulta | GET/POST | `/Consultas/Criar` |
| Editar consulta | GET/POST | `/Consultas/Editar/{id}` |
| Excluir consulta | GET/POST | `/Consultas/Excluir/{id}` |

## Publicar no GitHub

Crie um repositório vazio no GitHub e, dentro desta pasta, execute:

```bash
git init
git add .
git commit -m "Entrega do Sistema de Gestão de Consultas UVV"
git branch -M main
git remote add origin URL_DO_SEU_REPOSITORIO
git push -u origin main
```

Também é possível usar o GitHub Desktop: escolha **Add an Existing Repository**, selecione esta pasta, faça o primeiro commit e clique em **Publish repository**.

## Vídeo demonstrativo

**Link:** COLE_AQUI_O_LINK_DO_VIDEO

Antes da entrega, substitua o texto acima pelo link público ou acessível ao professor no Loom, YouTube ou serviço semelhante.

## Participantes

Preencha em ordem alfabética:

- NOME COMPLETO DO ALUNO 1
- NOME COMPLETO DO ALUNO 2

## Checklist antes do envio

- [ ] Atualizar os nomes dos participantes neste README;
- [ ] executar `Update-Database` sem erros;
- [ ] testar cadastro, logout e login;
- [ ] testar criação, detalhes, edição e exclusão de consulta;
- [ ] gravar o vídeo demonstrativo;
- [ ] inserir o link do vídeo no README;
- [ ] publicar o repositório e conferir se está acessível ao professor;
- [ ] preencher `ENTREGA.md`, exportá-lo como PDF e enviar pelo representante.
