/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: calcular hash md5 de strings
*/

using System;
using System.Security.Cryptography;
using System.Text;

namespace MD5Service 
{
    public class MD5Service
    {
        public static string CalculateMD5(string input)
        {
            // Converte a string de entrada em bytes
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Cria um objeto MD5
            using (MD5 md5 = MD5.Create())
            {
                // Calcula o hash MD5 dos bytes de entrada
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Converte o array de bytes em uma string hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2" formata o byte como uma string hexadecimal
                }

                return sb.ToString();
            }
        }
    }
}