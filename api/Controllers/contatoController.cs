/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: controller de contato
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Data.Odbc;
using System.Data.Common;
using api.DTOs;
using api.Models;

namespace api.Controllers;
//TODO: testar
[Authorize]
[ApiController]
[Route("[controller]")]
public class contatoController : ControllerBase
{
    private readonly ILogger<contatoController> _logger;
    private OdbcConnection _conn;

    public contatoController(ILogger<contatoController> logger)
    {
        _logger = logger;
        _conn = DbODBCConnFactory.DatabaseConnFactory.CreateConn("DSN=agenda");
    }
    [HttpPost]
    public async Task<IActionResult> criar_contato([FromBody] Contato contato)
    {
        try 
        {
            if(contato.nome == null)
            {
                return StatusCode(400);
            }

            if(contato.nome.Length == 0)
            {
                return StatusCode(400);
            }

            if(contato.telefone == null)
            {
                return StatusCode(400);
            }

            if(contato.telefone.Length == 0)
            {
                return StatusCode(400);
            }

            var usuario = HttpContext.User;

            var IdUsuario = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string queryString = "INSERT INTO tb_contato (txt_nome, txt_telefone, fk_id_usuario) VALUES ('" + contato.nome + "', '" + contato.telefone + "', " + IdUsuario + ")";
            OdbcCommand command = new OdbcCommand(queryString, _conn);
            await _conn.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return StatusCode(201);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Ocorreu um erro ao cadastrar o contato.");
            return StatusCode(500, "Falha interna");
        }
        finally 
        {
            await _conn.CloseAsync();
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> alterar_contato(string id, [FromBody] Contato contato)
    {
        try 
        {
            if(contato.nome == null)
            {
                return StatusCode(400);
            }

            if(contato.nome.Length == 0)
            {
                return StatusCode(400);
            }

            if(contato.telefone == null)
            {
                return StatusCode(400);
            }

            if(contato.telefone.Length == 0)
            {
                return StatusCode(400);
            }

            if(contato.id_usuario == null)
            {
                return StatusCode(400);
            }

            if(contato.id == null)
            {
                return StatusCode(400);
            }

            var usuario = HttpContext.User;

            var IdUsuario = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(contato.id_usuario.ToString() == IdUsuario)
            {
                return StatusCode(400);
            }

            string queryString = "UPDATE tb_contato set txt_nome = '" + contato.nome + "', txt_telefone = '" + contato.telefone + "', fk_id_usuario = " + IdUsuario + " where id = " + contato.id;
            OdbcCommand command = new OdbcCommand(queryString, _conn);
            await _conn.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return StatusCode(202);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Ocorreu um erro ao atualizar o contato.");
            return StatusCode(500, "Falha interna");
        }
        finally 
        {
            await _conn.CloseAsync();
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> apagar_contato(string id)
    {
        try 
        {   
            int id_contato = 0;
            string queryString = "SELECT fk_id_usuario FROM tb_contato where id = " + id;
            OdbcCommand command = new OdbcCommand(queryString, _conn);
            await _conn.OpenAsync();
            DbDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                id_contato = (int)reader[0];
            }

            var usuario = HttpContext.User;

            var IdUsuario = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(id_contato.ToString() == IdUsuario)
            {
                return StatusCode(400);
            }

            queryString = "DELETE FROM tb_contato  where id = " + id;
            command = new OdbcCommand(queryString, _conn);
            await command.ExecuteNonQueryAsync();
            return StatusCode(204);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Ocorreu um erro ao apagar o contato.");
            return StatusCode(500, "Falha interna");
        }
        finally 
        {
            await _conn.CloseAsync();
        }
    }
    [HttpGet("{idContato}")]
    public async Task<IActionResult> recuperar_contato(string idContato)
    {
        try 
        {   
            var usuario = HttpContext.User;

            var IdUsuario = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string queryString = "SELECT id, txt_nome, txt_telefone, fk_id_usuario FROM tb_contato where id = " + idContato + " and  fk_id_usuario = " + IdUsuario;
            OdbcCommand command = new OdbcCommand(queryString, _conn);
            await _conn.OpenAsync();
            DbDataReader reader = await command.ExecuteReaderAsync();
            
            if(!reader.HasRows)
                return StatusCode(404);

            Contato contato = new Contato();
            while (await reader.ReadAsync())
            {
                contato = new Contato(){
                    id = (int)reader[0],
                    nome = reader[1].ToString(),
                    telefone = reader[2].ToString(),
                    id_usuario = (int)reader[3]
                };
            }

            return Ok(contato);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Ocorreu um erro ao consultar o contato.");
            return StatusCode(500, "Falha interna");
        }
        finally 
        {
            await _conn.CloseAsync();
        }
    }
    [HttpGet]
    public async Task<IActionResult> recuperar_todos_contatos()
    {
        try 
        {   
            var usuario = HttpContext.User;

            var IdUsuario = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string queryString = "SELECT tb_usuario.txt_nome, tb_contato.id, tb_contato.txt_nome, tb_contato.txt_telefone, tb_contato.fk_id_usuario FROM tb_usuario left outer join tb_contato on tb_usuario.id = tb_contato.fk_id_usuario where tb_usuario.id = " + IdUsuario;
            OdbcCommand command = new OdbcCommand(queryString, _conn);
            await _conn.OpenAsync();
            DbDataReader reader = await command.ExecuteReaderAsync();
            
            if(!reader.HasRows)
                return StatusCode(404);

            ListaContatosDTO listaContatosDTO = new ListaContatosDTO()
            {
                contatos = new List<Contato>()
            };
            Contato contato = new Contato();
            for (int cont = 0; await reader.ReadAsync(); cont++)
            {
                if(cont == 0)
                    listaContatosDTO.nome_usuario = reader[0].ToString();
                
                if(!reader.IsDBNull(1))
                {
                    contato = new Contato(){
                        id = (int)reader[1],
                        nome = reader[2].ToString(),
                        telefone = reader[3].ToString(),
                        id_usuario = (int)reader[4]
                    };
                    listaContatosDTO.contatos = listaContatosDTO.contatos.Append(contato);
                }
            }

            return Ok(listaContatosDTO);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Ocorreu um erro ao consultar a lista de contatos.");
            return StatusCode(500, "Falha interna");
        }
        finally 
        {
            await _conn.CloseAsync();
        }
    }
}