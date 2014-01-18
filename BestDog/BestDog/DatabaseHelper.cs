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
                                          AttachDbFilename=C:\Documents and Settings\Administrador\Meus documentos\GitHub\Soa\BestDog\BestDog\App_Data;
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

        public void LOJA_SalvaVenda(string CPFCLiente, int TipoHotDog, int QtdeHotDog, int TipoBebida, int QtdeBebida)
        {

            String query = @"INSERT INTO VendaLoja
                             (CPFCLiente, TipoHotDog, QtdeHotDog, TipoBebida, QtdeBebida)
                             Values
                            (@CPFCLiente, @TipoHotDog, @QtdeHotDog, @TipoBebida, @QtdeBebida)";

            SqlCommand cmd = new SqlCommand(query, connection);

            cmd.Parameters.Add(new SqlParameter("@CPFCLiente", CPFCLiente));
            cmd.Parameters.Add(new SqlParameter("@TipoHotDog", TipoHotDog));
            cmd.Parameters.Add(new SqlParameter("@QtdeHotDog", QtdeHotDog));
            cmd.Parameters.Add(new SqlParameter("@TipoBebida", TipoBebida));
            cmd.Parameters.Add(new SqlParameter("@QtdeBebida", QtdeBebida));
 


            cmd.ExecuteNonQuery();
        }




        public int FORNECEDOR_VerificaProdutoEstoque(int Id, int Qtde)
        {
            String query = @"SELECT qtdeEstoque
                            From  ProdutosFornecedor 
                            WHERE IdProduto=@id
                            AND QtdeEstoque >= @qtde";


            SqlCommand cmd = new SqlCommand(query, connection);
            IDataReader dr;
            cmd.Parameters.Add(new SqlParameter("@id", Id));
            cmd.Parameters.Add(new SqlParameter("@qtde", Qtde));
            int retorno = -1;


            using (dr = cmd.ExecuteReader())
            {

                if (!dr.Read())
                {
                    //adiciona fidelidade
                    retorno = -1;
                }
                else
                {
                    retorno = dr.GetInt32(0);
                }
            }


            return retorno;

        }


        public void FORNECEDOR_AtualizaEstoque(int id, int QtdeVendida, int QtdeEstoque )
        {

            int QtdeFinal = QtdeEstoque - QtdeVendida;

            String query = @"UPDATE Produtos 
                             SET QtdeEstoque = @Qtde
                             WHERE IdProduto = @Id";

            SqlCommand cmd = new SqlCommand(query, connection);
            SqlParameter par = new SqlParameter("@Qtde", SqlDbType.Int );
            SqlParameter par1 = new SqlParameter("@Id", SqlDbType.Int);

            par.Value = QtdeFinal;
            par1.Value = id;


            cmd.Parameters.Add(par);
            cmd.Parameters.Add(par1);

            cmd.ExecuteNonQuery();
        }


        public void FORNECEDOR_SalvaVenda(int idCliente, int IdProduto , int QtdeVendida)
        {

            String query = @"INSERT INTO VendaFornecedor 
                             (IDCliente, IDProduto, QtdeProduto, Status)
                             Values
                            (@IDCliente, @IDProduto, @QtdeProduto, @Status)";

            SqlCommand cmd = new SqlCommand(query, connection);

            SqlParameter par = new SqlParameter("@IDCliente", SqlDbType.Int);
            SqlParameter par1 = new SqlParameter("@IDProduto", SqlDbType.Int);
            SqlParameter par2 = new SqlParameter("@QtdeProduto", SqlDbType.Int);
            SqlParameter par3 = new SqlParameter("@Status", SqlDbType.Int);

            par.Value = idCliente;
            par1.Value = IdProduto;
            par2.Value = QtdeVendida;
            //Status 0 = Sem entrega agendada...
            par3.Value = 0;

            cmd.Parameters.Add(par);
            cmd.Parameters.Add(par1);
            cmd.Parameters.Add(par2);
            cmd.Parameters.Add(par3);


            cmd.ExecuteNonQuery();
        }

        public void FORNECEDOR_AgendaEntrega(int IDCliente, DateTime DataEntrega)
        {
        

            String query = @"UPDATE VendaFornecedor
                            SET  DataEntrega = @DataEntrega 
                            WHERE IDCliente = @IDCliente 
                            AND DataEntrega is null
                            AND Status = 0 ";


            SqlCommand cmd = new SqlCommand(query, connection);
       
            cmd.Parameters.Add(new SqlParameter("@DataEntrega", DataEntrega));
            cmd.Parameters.Add(new SqlParameter("@IDCliente", IDCliente));


            cmd.ExecuteNonQuery();

        }

        public List<string> FORNECEDOR_SelecionaEntregasPeriodo(int IDCliente, DateTime DataInicial, DateTime DataFinal)
        {

            List<string> mensagens = new List<string>();
          

            String query = @"SELECT     SELECT     'Prezado Cliente ' + Clientes.Nome + ' ( ID = ' + CONVERT(varchar, Clientes.Id) + '), a venda ' + CONVERT(varchar, VendaFornecedor.IDVenda) 
                      + ' do produto ' + Produtos.NomeProduto + ' ( Quantidade: ' + CONVERT(varchar, VendaFornecedor.QtdeVendida) 
                      + ') foi entregue no dia ' + CONVERT(varchar, VendaFornecedor.DataEntrega) AS Mensagem
                            FROM            VendaFornecedor INNER JOIN
                                            Produtos ON VendaFornecedor.IDProduto = Produtos.IdProduto INNER JOIN
                                            Clientes ON VendaFornecedor.IDCliente = Clientes.Id
                             WHERE      IDCliente = @IDCliente
                             AND        VendaFornecedor.DataEntrega BETWEEN @DataInicial AND @DataFinal";


            SqlCommand cmd = new SqlCommand(query, connection);
            IDataReader dr;
            cmd.Parameters.Add(new SqlParameter("@IDCliente", IDCliente));
            cmd.Parameters.Add(new SqlParameter("@DataInicial", DataInicial));
            cmd.Parameters.Add(new SqlParameter("@DataFinal", DataFinal));


            using (dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    mensagens.Add(dr.GetString(0));
                }
            }

            return mensagens;

        }


        public double FORNECEDOR_FazOrcamento(int Id)
        {
            String query = @"SELECT PrecoUnitatioProduto
                            From  Produtos 
                            WHERE IdProduto =@id";


            SqlCommand cmd = new SqlCommand(query, connection);
            IDataReader dr;
            cmd.Parameters.Add(new SqlParameter("@id", Id));
       


            double retorno = -1;


            using (dr = cmd.ExecuteReader())
            {

                if (!dr.Read())
                {
                    //adiciona fidelidade
                    retorno = -1;
                }
                else
                {
                    retorno = dr.GetDouble(0);
                }
            }


            return retorno;

        }



    }
}