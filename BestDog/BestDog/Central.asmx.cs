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
        public Central() : base()
        {
            lstCPF.Add("377.743.768-95");
        }

        [WebMethod]
        public bool VerificaFidelidade(String CPF)
        {
            DatabaseHelper dh = new DatabaseHelper();
            return dh.VerificaFidelidade(CPF);
        }

        [WebMethod]
        public void AtualizaEstoqueCentral(int tipoHotDog, int tipoBebida)
        {

        }
    }
}
