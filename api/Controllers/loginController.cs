/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: controller de login
*/

using Microsoft.AspNetCore.Mvc;
using System.Data.Odbc;
using System.Data.Common;
using api.DTOs;

namespace api.Controllers;
//TODO: testar
[ApiController]
[Route("[controller]")]
public class loginController : ControllerBase
{
    private readonly ILogger<loginController> _logger;
    private OdbcConnection _conn;

    public loginController(ILogger<loginController> logger)
    {
        _logger = logger;
        _conn = DbODBCConnFactory.DatabaseConnFactory.CreateConn("DSN=agenda");
    }
    [HttpPost]
    public async Task<IActionResult> PostLogar([FromBody] CadastroDTO cadastroDTO)
    {
        try 
        {
            if(cadastroDTO.email == null)
            {
                return StatusCode(400);
            }

            if(cadastroDTO.email.Length == 0)
            {
                return StatusCode(400);
            }

            if(cadastroDTO.senha == null)
            {
                return StatusCode(400);
            }
            
            if(cadastroDTO.senha.Length == 0)
            {
                return StatusCode(400);
            }

            string senha_md5 = MD5Service.MD5Service.CalculateMD5(cadastroDTO.senha);

            List<Int32> ListId = new List<Int32>();
            string queryString = "Select id from tb_usuario where txt_email = '" + cadastroDTO.email + "' and txt_senha = '" + senha_md5 + "'";
            OdbcCommand command = new OdbcCommand(queryString, _conn);
            await _conn.OpenAsync();
            DbDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ListId.Add((Int32)reader[0]);
            }
            await reader.CloseAsync();
            if(ListId.Count == 0)
                return StatusCode(403);
            
            var JWT = new JWTService.JwtService(
                "sua-issuer",
                "sua-audience",
                "this is my custom Secret key for authentication",
                30
            );

            string token = JWT.GenerateToken(ListId.ToArray()[0].ToString());
            
            return Ok(new { Token = token });
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Ocorreu um erro ao logar usuário.");
            return StatusCode(500, "Falha interna. ");
        }
        finally 
        {
            await _conn.CloseAsync();
        }
    }
}
