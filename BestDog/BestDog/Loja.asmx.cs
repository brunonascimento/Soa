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
    public class Loja : System.Web.Services.WebService
    {

        [WebMethod]
        public bool VerificaEstoque(int tipoHotDog, int tipoBebida, int filial)
        {
            DatabaseHelper obj = new DatabaseHelper();


            //Se o produto existe no estoque, 
            int QtdeEstoqueHotDog = obj.LOJA_VerificaProdutoEstoque(tipoHotDog, filial);

            //Se o produto existe no estoque, 
            int QtdeEstoqueBebida = obj.LOJA_VerificaProdutoEstoque(tipoBebida, filial);

            if (QtdeEstoqueBebida > 0 && QtdeEstoqueHotDog > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [WebMethod]
        public decimal CalculaPreco(int tipoHotDog, int tipoBebida,bool desconto,int filial)
        {
            decimal total = 0;
            
            DatabaseHelper obj = new DatabaseHelper();
            total += obj.LOJA_ObtemPrecoProduto(tipoHotDog, filial);

            total += obj.LOJA_ObtemPrecoProduto(tipoBebida, filial);
            
            if (desconto)
            {
                total = total * 0.5M;
            }
            return total;
        }

        [WebMethod]
        public void RegistraVenda(int tipoHotDog, int QtdeHotDog, int tipoBebida, int QtdeBebida  , String cpf, int idFilial)
        {
            DatabaseHelper obj = new DatabaseHelper();

            obj.LOJA_SalvaVenda(cpf, tipoHotDog, QtdeHotDog, tipoBebida, QtdeBebida, idFilial);
        }

        [WebMethod]
        public void AtualizaEstoqueLocal(int idProduto, int qtdeVendida, int filial)
        {
            DatabaseHelper obj = new DatabaseHelper();


            //Se o produto existe no estoque, 
            int QtdeEstoque = obj.LOJA_VerificaProdutoEstoque(idProduto, filial);

            if (QtdeEstoque != -1)
            {
                //Atualizar a quantidade disponível no estoque
                obj.LOJA_AtualizaEstoque(idProduto, qtdeVendida, QtdeEstoque);
            }

        }
        
    }
}
