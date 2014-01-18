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
        List<String> lstCPF = new List<String>();
        Dictionary<int, Dictionary<int, int>> Filial = new Dictionary<int, Dictionary<int, int>>();

        public Central() : base()
        {
            lstCPF.Add("377.743.768-95");
            Filial = ListaFilial();
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
        public void ConfirmaCOmpra(int IDVenda)
        {
            DatabaseHelper obj = new DatabaseHelper();


            //Se o produto existe no estoque, 
            obj.FORNECEDOR_CentralConfirmaCompra(IDVenda);


        }


        [WebMethod]
        public Dictionary<int, int> VerificaEstoqueFilial(int idFilial)
        {
            Dictionary<int, int> Produto = new Dictionary<int, int>();

            if (Filial.ContainsKey(idFilial))
            {
                Produto = Filial[idFilial];

            }

            return Produto;
        }


        [WebMethod]
        public void ConfirmaCompra(int idCompra)
        {

            DatabaseHelper obj = new DatabaseHelper();

            //Agendar todas as compras do cliente para a mesma data de entrega
            obj.FORNECEDOR_AgendaEntrega(idCliente, DataEntrega);

        }



        private   Dictionary<int, Dictionary<int, int>> ListaFilial()
        {
            Dictionary<int, Dictionary<int, int>> Filial = new Dictionary<int, Dictionary<int, int>>();

            Filial.Add(1, EstoqueProduto());
            Filial.Add(2, EstoqueProduto());

            return Filial;

        }

        private Dictionary<int, int> EstoqueProduto()
        {

            //Gerar tabela de produto e quantidade
            Dictionary<int, int> Produto = new Dictionary<int, int>();

            for (int i = 1; i <= 20; i++)
            {
                Produto.Add(1, i + 5);
                Produto.Add(2, i + 6);
                Produto.Add(3, i + 3);
                Produto.Add(4, i + 8);
            }

            return Produto;

        }

    }
}
