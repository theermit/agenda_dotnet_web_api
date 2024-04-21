/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: modelo de contato
*/

namespace api.Models
{
    public class Contato
    {
        public Int32? id{get;set;}
        public string? nome{get;set;}
        public string? telefone{get;set;}
        public Int32? id_usuario{get;set;}
    }
}