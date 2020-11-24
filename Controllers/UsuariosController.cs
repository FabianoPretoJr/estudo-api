using Microsoft.AspNetCore.Mvc;
using estudo_api.Data;
using estudo_api.Models;
using System.Linq;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;

namespace estudo_api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        public UsuariosController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody]Usuario usuario)
        {
            // Implementar validações
            // Verificar se os dados são validos (senha e e-mail)
            // Verificar se e-mail já existe no banco de dados
            // Encriptar a senha antes de mandar para o banco
            database.Add(usuario);
            database.SaveChanges();
            return Ok(new {msg = "Usuário cadastrado com sucesso!"});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario credenciais)
        {
            // Buscar um usuário por E-mail
            // Verificar se a senha está correta
            // Gerar um token JWT e retornar esse token para o usuário

            try
            {
                Usuario usuario = database.Usuarios.First(u => u.Email.Equals(credenciais.Email));

                if(usuario != null)
                {
                    if(usuario.Senha.Equals(credenciais.Senha))
                    {
                        string chaveDeSeguranca = "hdjflj5fv5v45fv54v65v";

                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca)); // dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 3.0.0
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id", usuario.Id.ToString()));
                        claims.Add(new Claim("email", usuario.Email));
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                        var JWT = new JwtSecurityToken(
                            issuer: "Fabiano Preto", // Quem está fornecendo o JWT para o usuário
                            expires: DateTime.Now.AddHours(1), // Tempo de válidade do token
                            audience: "usuario_comum", // Defini nível do usuário
                            signingCredentials: credenciaisDeAcesso,
                            claims: claims
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                    }
                    else
                    {
                        Response.StatusCode = 401;
                        return new ObjectResult("");
                    }
                }
                else
                {
                    Response.StatusCode = 401;
                    return new ObjectResult("");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 401;
                return new ObjectResult("");
            }
        }
    }
}