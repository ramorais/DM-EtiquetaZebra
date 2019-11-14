using Newtonsoft.Json.Linq;
using RawPrint;
using RGiesecke.DllExport;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EtiquetaZebra {

  // ---
  public class Main {

    // ---
    // ExeDLLRun
    [DllExport("ExecInClientDLL", CallingConvention.StdCall)]
    public static unsafe void ExecInClientDLL(int nOpc, byte* cStrInput, byte* cStrReturn, int nRetMaxSize) {

      StringPtr strInput = new StringPtr(cStrInput);  // entrada
      StringPtr strReturn = new StringPtr();          // retorno
      // ---
      JObject json = null;
      string impressora = "";
      string arquivoEtiqueta = "";
      // ---
      string ret = "";

      try {

        json = JObject.Parse(strInput.toString());

        impressora = json["impressora"].ToString();
        arquivoEtiqueta = json["arquivoEtiqueta"].ToString();
        var corpo = File.ReadAllText(arquivoEtiqueta);

        JObject campos = (JObject)json["campos"];
        foreach (JProperty j in campos.Properties()) {

          var campo = "%" + j.Name + "%";
          var valor = j.Value.ToString();

          corpo = corpo.Replace(campo, valor);
          
        }

        ret = corpo;

        if (impressora.Length != 0) {

          var sufixo = DateTime.Today.ToString("_ddmmyyyy_hhMMss");

          IPrinter printer = new Printer();
          printer.PrintRawStream(impressora, new MemoryStream(Encoding.ASCII.GetBytes(corpo)), "EtiquetaZebra" + sufixo);

        }

      }
      catch(Exception e) {

        ret = "#Erro: " + e.Message;

      }

      strReturn.fromString(ret);

      // copia para retornar
      strReturn.copyTo(cStrReturn);

    }
    // ------------------------------------------------------------------------

  }
  // --------------------------------------------------------------------------
  // --------------------------------------------------------------------------

}
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------