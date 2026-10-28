# ExoApi — Atividade 06: Login (JWT) e CORS

Web API em ASP.NET Core (.NET 6) da empresa **ExoApi**, com:

- **Login** via `POST /api/usuarios`, que devolve um **token JWT** (expira em 30 min);
- **Autorização**: `PUT` e `DELETE` de `/api/usuarios/{id}` exigem o token (`[Authorize]`);
- **CORS** habilitado (`CorsPolicy`).

## Como executar

1. No SSMS, execute o script `cria-db.sql` (cria o banco `ExoApi` e os usuários de teste).
2. Em `Exo.WebApi/Contexts/ExoContext.cs`, ajuste a string de conexão para o seu SQL Server
   (padrão: `Server=localhost\SQLEXPRESS01;Database=ExoApi;Trusted_Connection=True;`).
3. Na pasta `Exo.WebApi`:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
4. A API sobe em `https://localhost:7154` (Swagger em `/swagger`).

## Como testar (Insomnia / Postman)

Usuários criados pelo script: `email@sp.br` e `email_dois@sp.br`, ambos com senha `1234`.

| # | Requisição | Resultado esperado |
|---|------------|--------------------|
| 1 | `PUT https://localhost:7154/api/usuarios/2` (sem token) | `401 Unauthorized` |
| 2 | `DELETE https://localhost:7154/api/usuarios/2` (sem token) | `401 Unauthorized` |
| 3 | `POST https://localhost:7154/api/usuarios` com `{"email":"email@sp.br","senha":"1234"}` | `200 OK` + `{"token":"..."}` |
| 4 | `POST` com credenciais erradas | `404` — "E-mail ou senha inválidos!" |
| 5 | `PUT https://localhost:7154/api/usuarios/2` com Auth → Bearer Token e corpo `{"email":"email_novo@sp.br","senha":"aaaa1234"}` | `204 No Content` |
| 6 | `GET https://localhost:7154/api/usuarios` | `200 OK` com o usuário 2 alterado |
| 7 | `DELETE https://localhost:7154/api/usuarios/2` com o Bearer Token | `204 No Content` |
| 8 | `GET https://localhost:7154/api/usuarios` | `200 OK` sem o usuário 2 |

> Em testes com `curl` em ambiente local, use `-k` por causa do certificado de desenvolvimento HTTPS.

## Observações

- O método `Cadastrar` (POST) foi comentado e substituído pelo POST de login, conforme a atividade.
- A chave do token (`exoapi-chaveautenticacao`) é a mesma no `Program.cs` e no `UsuariosController.cs`;
  se forem diferentes, o token é rejeitado e as rotas protegidas retornam `401`.
