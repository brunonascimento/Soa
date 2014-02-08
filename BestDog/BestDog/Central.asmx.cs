using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace BestDog
{
    /// <summary>
    /// Summary description for Matriz
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Central : System.Web.Services.WebService
    {
        //Dictionary<int, Dictionary<int, int>> Filial = new Dictionary<int, Dictionary<int, int>>();
        Dictionary<int, int> MetaFilial = new Dictionary<int, int>();

        public Central() : base()
        {   
        }

        [WebMethod]
        public bool VerificaFidelidade(String CPF)
        {
            DatabaseHelper dh = new DatabaseHelper();
            return dh.VerificaFidelidade(CPF);
        }

        [WebMethod]
        public void AtualizaEstoqueCentral(int idProduto, int qtdeVendida)
        {
            DatabaseHelper obj = new DatabaseHelper();

             

            //Se o produto existe no estoque, 
            int QtdeEstoque = obj.CENTRAL_VerificaProdutoEstoque(idProduto, qtdeVendida);

            if (QtdeEstoque != -1)
            {
                //Atualizar a quantidade disponível no estoque
                obj.CENTRAL_AtualizaEstoque(idProduto, qtdeVendida, QtdeEstoque);
            }

        }


        [WebMethod]
        public int VerificaEstoqueCentral(int idProduto, int qtdeVendida)
        {
            DatabaseHelper obj = new DatabaseHelper();


            //Se o produto existe no estoque, 
            return  obj.CENTRAL_VerificaProdutoEstoque(idProduto, qtdeVendida);
 

        }


        [WebMethod]
        public void ConfirmaCompra(int IDVenda)
        {
            DatabaseHelper obj = new DatabaseHelper();

            //Se o produto existe no estoque, 
            obj.FORNECEDOR_CentralConfirmaCompra(IDVenda);

        }


        [WebMethod]
        public bool VerificaEstoqueFilial(int idFilial,int idProduto)
        {
            DatabaseHelper obj = new DatabaseHelper();
            //Se o produto existe no estoque, 
            int QtdeEstoque = obj.LOJA_VerificaProdutoEstoque(idProduto, idFilial);

            if (QtdeEstoque > 0)
            {
                return true;
            }
            return false;
        }
        [WebMethod]
        public void CadastraFilial(String nome, int meta)
        {
            DatabaseHelper obj = new DatabaseHelper();
            obj.CENTRAL_CadastraFilial(nome, meta);

        }


        [WebMethod]
        public bool VerificaMetaFilial(int IDFilial)
        {
            DatabaseHelper obj = new DatabaseHelper();

            //Buscar a meta da filial
            int meta = obj.CENTRAL_ObtemMeta(IDFilial);

           
            //Agendar todas as compras do cliente para a mesma data de entrega
            double pontuacao = obj.CENTRAL_SelecionaPontuacao(IDFilial);

            if (pontuacao > meta)
            {
                return true;
            }
            else
            {
                return false;
            }

        }



        

    }
}
