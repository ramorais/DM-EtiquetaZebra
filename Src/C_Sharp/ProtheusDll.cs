using Newtonsoft.Json.Linq;
using RawPrint;
using RGiesecke.DllExport;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EtiquetaZebra {

  // ---
  public class ProtheusDll {

    // ---
    private static String removerNaoZPL(String conteudo) {

      var pos = conteudo.IndexOf("^XA");
      if(pos > 0)
        return conteudo.Substring(pos);
      else
        return conteudo;

    }
    // ------------------------------------------------------------------------ 

    // ---
    // ExeDLLRun
    [DllExport("ExecInClientDLL", CallingConvention.StdCall)]
    public static unsafe void ExecInClientDLL(int nOpc, byte* cStrInput, byte* cStrReturn, int nRetMaxSize) {

      string ret = "";
      // ---
      StringPtr strInput = new StringPtr(cStrInput);  // entrada
      StringPtr strReturn = new StringPtr();          // retorno

      try {

        var json = JObject.Parse(strInput.toString());

        var impressora = json["impressora"].ToString();
        var arquivoEtiqueta = json["arquivoEtiqueta"].ToString();
        var conteudoArquivo = removerNaoZPL(File.ReadAllText(arquivoEtiqueta));

        JObject campos = (JObject)json["campos"];
        foreach (JProperty j in campos.Properties()) {

          var campo = "%" + j.Name + "%";
          var valor = j.Value.ToString();

          conteudoArquivo = conteudoArquivo.Replace(campo, valor);
          
        }

        if (impressora.Length != 0) {

          var sufixo = DateTime.Today.ToString("_ddmmyyyy_hhMMss");

          IPrinter printer = new Printer();
          printer.PrintRawStream(
            impressora,
            new MemoryStream(Encoding.ASCII.GetBytes(conteudoArquivo)), "EtiquetaZebra" + sufixo
          );

        }

        ret = conteudoArquivo;

      }
      catch(Exception e) {
        ret = "#Erro: " + e.Message;
      }

      // cria o buffer
      strReturn.fromString(ret.Substring(0, nRetMaxSize));

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