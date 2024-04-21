/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: controller de cadastro de usuário
*/

using Microsoft.AspNetCore.Mvc;
using System.Data.Odbc;
using System.Data.Common;
using api.DTOs;

namespace api.Controllers;
//TODO: testar
[ApiController]
[Route("[controller]")]
public class cadastrarusuarioController : ControllerBase
{
    private readonly ILogger<cadastrarusuarioController> _logger;
    private OdbcConnection _conn;

    public cadastrarusuarioController(ILogger<cadastrarusuarioController> logger)
    {
        _logger = logger;
        _conn = DbODBCConnFactory.DatabaseConnFactory.CreateConn("DSN=agenda");
    }
    [HttpPost]
    public async Task<IActionResult> PostCadastrarUsuario([FromBody] CadastroDTO cadastroDTO)
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

            List<string> listaEmails = new List<string>();
            string queryString = "Select txt_email from tb_usuario where txt_email = '" + cadastroDTO.email + "'";
            OdbcCommand command = new OdbcCommand(queryString, _conn);
            await _conn.OpenAsync();
            DbDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                listaEmails.Add((string)reader[0]);
            }
            await reader.CloseAsync();
            if(listaEmails.Count > 0)
                return StatusCode(406);
            
            if(cadastroDTO.nome == null)
            {
                return StatusCode(400);
            }
            
            if(cadastroDTO.nome.Length == 0)
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

            if(cadastroDTO.confirmacao_senha == null)
            {
                return StatusCode(400);
            }
            
            if(cadastroDTO.confirmacao_senha.Length == 0)
            {
                return StatusCode(400);
            }

            if(cadastroDTO.confirmacao_senha != cadastroDTO.senha)
            {
                return StatusCode(400);
            }

            string senha_md5 = MD5Service.MD5Service.CalculateMD5(cadastroDTO.senha);

            queryString = "Insert into tb_usuario (txt_nome, txt_email, txt_senha) values ('" + cadastroDTO.nome + "', '" + cadastroDTO.email + "', '" + senha_md5 + "')";
            command = new OdbcCommand(queryString, _conn);
            await command.ExecuteNonQueryAsync();
            return StatusCode(201);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Ocorreu um erro ao cadastrar usuário.");
            return StatusCode(500, "Falha interna");
        }
        finally 
        {
            await _conn.CloseAsync();
        }
    }
}
