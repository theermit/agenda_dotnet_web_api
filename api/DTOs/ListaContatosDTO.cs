/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: DTO usado na view de lista de contatos
*/

using api.Models;

namespace api.DTOs
{
    public class ListaContatosDTO
    {
        public string? nome_usuario{get;set;}
        public IEnumerable<Contato>? contatos{get;set;}
    }
}