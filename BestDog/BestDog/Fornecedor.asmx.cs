using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace BestDog
{
    /// <summary>
    /// Summary description for Loja
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Fornecedor : System.Web.Services.WebService
    {

        [WebMethod]
        public bool fazVenda(int IdProduto, int QtdeVendida, int idCliente)
        {

       
            DatabaseHelper obj = new DatabaseHelper();


            //Se o produto existe no estoque, 
            int QtdeEstoque = obj.FORNECEDOR_VerificaProdutoEstoque(IdProduto, QtdeVendida);

            if (QtdeEstoque != -1)
            {
                //Atualizar a quantidade disponível no estoque
                obj.FORNECEDOR_AtualizaEstoque(IdProduto, QtdeVendida, QtdeEstoque);

                //Cadastra a venda
                obj.FORNECEDOR_SalvaVenda(idCliente, IdProduto, QtdeVendida);

                return true;
            }
            else
            {
                return false;
            }

           
        }



        [WebMethod]
        public void AgendaEntrega(int idCliente, DateTime DataEntrega)
        {

             DatabaseHelper obj = new DatabaseHelper();

            //Agendar todas as compras do cliente para a mesma data de entrega
             obj.FORNECEDOR_AgendaEntrega(idCliente, DataEntrega);

        }

        [WebMethod]
        public List<string> ComunicaEntrega(int idCliente, DateTime dataInicioPeriodo, DateTime dataFimPeriodo)
        {

            List<string> mensagens = new List<string>();

            //Gera mensagens de entrega
                  DatabaseHelper obj = new DatabaseHelper();


                  mensagens = obj.FORNECEDOR_SelecionaEntregasPeriodo(idCliente, dataInicioPeriodo, dataFimPeriodo);


                  return mensagens;
        }




        [WebMethod]
        public double RecebeeFazOrcamento(int idProduto)
        {

            double precoProduto = -1;

             DatabaseHelper obj = new DatabaseHelper();


             precoProduto = obj.FORNECEDOR_FazOrcamento(idProduto);


             return precoProduto;

            
        }
        
    }
}
