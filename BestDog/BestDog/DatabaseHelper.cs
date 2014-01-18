using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace BestDog
{
    public class DatabaseHelper
    {
        private SqlConnection connection;
        public String connectionString = @"Data Source=.\SQLEXPRESS;
                                          AttachDbFilename=C:\SOA_WEB\Soa\BestDog\BestDog\App_Data\database.mdf;
                                          Integrated Security=True;
                                          Connect Timeout=30;
                                          User Instance=True";
        public bool VerificaFidelidade(String CPF)
        {
            connection = new SqlConnection();
            try
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                String query = @"SELECT     Id, CPF, Pontos
                                FROM         Clientes
                                WHERE     (CPF = @cpf)";

                SqlCommand cmd = new SqlCommand(query, connection);
                IDataReader dr;
                cmd.Parameters.Add(new SqlParameter("@cpf",CPF));

                int id = 0;
                int pontos = 0;

                Boolean adicionaCliente = false;
                using (dr = cmd.ExecuteReader())
                {
                    
                    if (!dr.Read())
                    {
                        //adiciona fidelidade
                        adicionaCliente = true;
                    }
                    else
                    {
                        id = dr.GetInt32(0);
                        pontos = dr.GetInt32(2);
                    }
                }

                if (adicionaCliente)
                {
                    AdicionaCliente(CPF);
                    return false;
                }

                if (pontos > 4)
                {
                    //fornece desconto e zera os pontos
                    ZeraPontos(id);
                    return true;
                }
                return false;
            }
            finally
            {
                connection.Close();
            }
            
        }

        private void ZeraPontos(int Id)
        {
            String query = @"UPDATE Clientes
                            SET Pontos=0
                            WHERE Id=@id";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.Add(new SqlParameter("@id", Id));
            cmd.ExecuteNonQuery();
        }

        private void AdicionaCliente(string CPF)
        {
            String query = @"INSERT INTO Clientes(CPF)
                             VALUES(@cpf);";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlParameter par = new SqlParameter("@cpf", SqlDbType.VarChar);
            par.Value = CPF;
            cmd.Parameters.Add(par);
            cmd.ExecuteNonQuery();
        }

    }
}