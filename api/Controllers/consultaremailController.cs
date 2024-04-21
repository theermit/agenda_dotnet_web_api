/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: controller de consulta se o email já está cadastrado
*/

using Microsoft.AspNetCore.Mvc;
using System.Data.Odbc;
using System.Data.Common;
using api.DTOs;

namespace api.Controllers;
//TODO: testar
[ApiController]
[Route("[controller]")]
public class consultaremailController : ControllerBase
{
    private readonly ILogger<consultaremailController> _logger;
    private OdbcConnection _conn;

    public consultaremailController(ILogger<consultaremailController> logger)
    {
        _logger = logger;
        _conn = DbODBCConnFactory.DatabaseConnFactory.CreateConn("DSN=agenda");
    }
    [HttpPost]
    public async Task<IActionResult> PostEmail([FromBody] ConsultarEmailDTO consultaEmail)
    {
        List<string> listaEmails = new List<string>();
        try 
        {
            if(consultaEmail.email == null)
            {
                return StatusCode(400);
            }

            if(consultaEmail.email.Length == 0)
            {
                return StatusCode(400);
            }

            string queryString = "Select txt_email from tb_usuario where txt_email = '" + consultaEmail.email + "'";
            OdbcCommand command = new OdbcCommand(queryString, _conn);
            await _conn.OpenAsync();
            DbDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                listaEmails.Add((string)reader[0]);
            }
            await reader.CloseAsync();
            VerificaEmailDTO verificaEmailDTO = new VerificaEmailDTO()
            {
                email_jah_cadastrado = listaEmails.Count > 0,
            };
            return Ok(verificaEmailDTO);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Ocorreu um erro ao consultar a base de emails.");
            return StatusCode(500, "Falha interna");
        }
        finally 
        {
            await _conn.CloseAsync();
        }
    }
}
