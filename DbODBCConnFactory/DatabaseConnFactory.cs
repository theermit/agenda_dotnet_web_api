/*
    AUTOR: Benhur Alencar Azevedo
    UTILIDADE: criar objetos de conexao
*/

using System.Data.Odbc;

namespace DbODBCConnFactory
{
    public class DatabaseConnFactory
    {
        public static OdbcConnection CreateConn(string dsn)
        {
            OdbcConnection conn = new OdbcConnection(dsn);
            return conn;
        }
    }
}