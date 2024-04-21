/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: DTO usado para informar se um email já foi usado
*/

namespace api.DTOs
{
    public class VerificaEmailDTO
    {
        public bool? email_jah_cadastrado{get;set;}
    }
}