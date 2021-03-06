﻿using System;
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
                                          AttachDbFilename=C:\Documents and Settings\Administrador\Meus documentos\GitHub\Soa\BestDog\BestDog\App_Data\database.mdf;
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

        public void LOJA_SalvaVenda(string CPFCLiente, int TipoHotDog, int QtdeHotDog, int TipoBebida, int QtdeBebida,int IdFilial)
        {

            String query = @"INSERT INTO VendaLoja
                             (CPFCLiente, TipoHotDog, QtdeHotDog, TipoBebida, QtdeBebida, Ponto,IdFilial)
                             Values
                            (@CPFCLiente, @TipoHotDog, @QtdeHotDog, @TipoBebida, @QtdeBebida,3,@IdFilial)";
            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            SqlCommand cmd = new SqlCommand(query, connection);

            cmd.Parameters.Add(new SqlParameter("@CPFCLiente", CPFCLiente));
            cmd.Parameters.Add(new SqlParameter("@TipoHotDog", TipoHotDog));
            cmd.Parameters.Add(new SqlParameter("@QtdeHotDog", QtdeHotDog));
            cmd.Parameters.Add(new SqlParameter("@TipoBebida", TipoBebida));
            cmd.Parameters.Add(new SqlParameter("@QtdeBebida", QtdeBebida));
            cmd.Parameters.Add(new SqlParameter("@IdFilial", IdFilial));


            cmd.ExecuteNonQuery();
        }

        public Decimal LOJA_ObtemPrecoProduto(int IdProduto, int idFilial)
        {
            String query = @"SELECT     PrecoUnitario
                            FROM         EstoqueLoja 
                            WHERE IdProduto=@id and IDFilial=@idFilial";

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            SqlCommand cmd = new SqlCommand(query, connection);
            IDataReader dr;
            cmd.Parameters.Add(new SqlParameter("@id", IdProduto));
            cmd.Parameters.Add(new SqlParameter("@IDFilial", idFilial));


            Decimal valor = 0;
            using (dr = cmd.ExecuteReader())
            {

                if (dr.Read())
                {
                    valor = dr.GetDecimal(0);
                }
            }


            return valor;
        }
        public int LOJA_VerificaProdutoEstoque(int Id, int idFilial)
        {
            String query = @"SELECT QtdeEstoque
                            From  EstoqueLoja 
                            WHERE IdProduto=@id and IDFilial=@idFilial";

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            SqlCommand cmd = new SqlCommand(query, connection);
            IDataReader dr;
            cmd.Parameters.Add(new SqlParameter("@id", Id));
            cmd.Parameters.Add(new SqlParameter("@IDFilial", idFilial));

            int retorno = -1;


            using (dr = cmd.ExecuteReader())
            {

                if (!dr.Read())
                {
                    retorno = -1;
                }
                else
                {
                    retorno = dr.GetInt32(0);
                }
            }


            return retorno;

        }

        public void LOJA_AtualizaEstoque(int id, int QtdeVendida, int QtdeEstoque)
        {

            int QtdeFinal = QtdeEstoque - QtdeVendida;

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            String query = @"UPDATE ProdutosFornecedor 
                             SET QtdeEstoque = @Qtde
                             WHERE IdProduto = @Id";

            SqlCommand cmd = new SqlCommand(query, connection);
            SqlParameter par = new SqlParameter("@Qtde", SqlDbType.Int);
            SqlParameter par1 = new SqlParameter("@Id", SqlDbType.Int);

            par.Value = QtdeFinal;
            par1.Value = id;


            cmd.Parameters.Add(par);
            cmd.Parameters.Add(par1);

            cmd.ExecuteNonQuery();
        }

        public void CENTRAL_AtualizaEstoque(int id, int QtdeVendida, int QtdeEstoque)
        {

            int QtdeFinal = QtdeEstoque - QtdeVendida;

            String query = @"UPDATE EstoqueCentral 
                             SET QtdeEstoque = @Qtde
                             WHERE IdProduto = @Id";

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            SqlCommand cmd = new SqlCommand(query, connection);
            SqlParameter par = new SqlParameter("@Qtde", SqlDbType.Int);
            SqlParameter par1 = new SqlParameter("@Id", SqlDbType.Int);

            par.Value = QtdeFinal;
            par1.Value = id;


            cmd.Parameters.Add(par);
            cmd.Parameters.Add(par1);

            cmd.ExecuteNonQuery();
        }

        public int CENTRAL_VerificaProdutoEstoque(int Id, int Qtde)
        {
            String query = @"SELECT qtdeEstoque
                            From  EstoqueCentral 
                            WHERE IdProduto=@id
                            AND QtdeEstoque >= @qtde";

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

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

        public double CENTRAL_SelecionaPontuacao(int IdFilial)
        {
            String query = @"SELECT     SUM(Ponto) AS pontos
                            FROM         VendaLoja
                            WHERE     IdFilial = @IdFilial";


            SqlCommand cmd = new SqlCommand(query, connection);
            IDataReader dr;
            cmd.Parameters.Add(new SqlParameter("@IdFilial", IdFilial));

            int retorno = -1;


            using (dr = cmd.ExecuteReader())
            {

                if (dr.Read())
                {
                    retorno = dr.GetInt32(0);
                }
            }


            return retorno;

        }


        public void FORNECEDOR_CentralConfirmaCompra(int IDVenda)
        {

            String query = @"UPDATE VendaFornecedor
                            SET  Status = 1 
                            WHERE IDVenda = @IDVenda";

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            SqlCommand cmd = new SqlCommand(query, connection);

            cmd.Parameters.Add(new SqlParameter("@IDVenda", IDVenda));

            cmd.ExecuteNonQuery();
        }



        public int FORNECEDOR_VerificaProdutoEstoque(int Id, int Qtde)
        {
            String query = @"SELECT qtdeEstoque
                            From  ProdutosFornecedor 
                            WHERE IdProduto=@id
                            AND QtdeEstoque >= @qtde";

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

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

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

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

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

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
                            SET  DataEntrega = @DataEntrega , Status = 2 
                            WHERE IDCliente = @IDCliente 
                            AND DataEntrega is null
                            ";


            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

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


        public int CENTRAL_ObtemMeta(int IdFilial)
        {
            String query = @"SELECT     Meta
                            FROM         Filial
                            WHERE     IDFilial =@id";

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            SqlCommand cmd = new SqlCommand(query, connection);
            IDataReader dr;
            cmd.Parameters.Add(new SqlParameter("@id", IdFilial));

            int meta = 0;
            using (dr = cmd.ExecuteReader())
            {

                if (dr.Read())
                {
                    meta = dr.GetInt32(0);
                }
            }


            return meta;
        }



        internal void CENTRAL_CadastraFilial(String NomeFilial, int Meta)
        {
            String query = @"INSERT INTO Filial(Nome,Meta)
                             Values(@Nome, @Meta)";

            connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            SqlCommand cmd = new SqlCommand(query, connection);

            SqlParameter par = new SqlParameter("@Nome", SqlDbType.VarChar);
            SqlParameter par1 = new SqlParameter("@Meta", SqlDbType.Int);

            par.Value = NomeFilial;
            par1.Value = Meta;
            
            cmd.Parameters.Add(par);
            cmd.Parameters.Add(par1);
            
            cmd.ExecuteNonQuery();
        }
    }
}