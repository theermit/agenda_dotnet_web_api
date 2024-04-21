/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: DTO usado no cadastro de novo usuario
*/

namespace api.DTOs
{
    public class CadastroDTO
    {
        public string? nome{get;set;}
        public string? email{get;set;}
        public string? confirmacao_senha{get;set;}
        public string? senha{get;set;}
    }
}