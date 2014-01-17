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
        public bool VerificaEstoque(int tipoHotDog, int tipoBebida)
        {
            bool disponibilidade = true;
            switch (tipoHotDog)
            {
                case 1:
                   break;
                case 2:
                    disponibilidade = false;
                    break;
                default:
                    disponibilidade = false;
                    break;
            }

            switch (tipoBebida)
            {
                case 1:
                    break;
                case 2:
                    break;
                default:
                    disponibilidade = false;
                    break;
            }
            
            return disponibilidade;
        }

        [WebMethod]
        public decimal CalculaPreco(int tipoHotDog, int tipoBebida,bool desconto)
        {
            decimal total = 0;
            if (tipoHotDog == 1)
            {
                total += 5.90M;
            }
            if (tipoHotDog == 2)
            {
                total += 7.90M;
            }

            if (tipoBebida == 1)
            {
                total += 1.00M;
            }

            if (tipoBebida == 2)
            {
                total += 1.50M;
            }

            if (desconto)
            {
                total = total * 0.5M;
            }
            return total;
        }

        [WebMethod]
        public void RegistraVenda(int tipoHotDog, int tipoBebida,String cpf)
        {

        }

        [WebMethod]
        public void AtualizaEstoqueLocal(int tipoHotDog, int tipoBebida)
        {

        }
        
    }
}
