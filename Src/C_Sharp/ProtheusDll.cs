using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
// ---
using RGiesecke.DllExport;
using Newtonsoft.Json.Linq;
using RawPrint;

namespace EtiquetaZebra {

  // ---
  // dll
  public class ProtheusDll {

    // ---
    // remove os caracteres de controle
    // do começo do arquivo
    private static String removerComecoArquivo(String conteudo) {

      var pos = conteudo.IndexOf("^XA");
      if (pos > 0)
        return conteudo.Substring(pos);
      else
        return conteudo;

    }
    // ------------------------------------------------------------------------ 

    // ---
    // ExeDLLRun2
    [DllExport("ExecInClientDLL", CallingConvention.StdCall)]
    public static unsafe void ExecInClientDLL(int nOpc, byte* cStrInput, byte* cStrReturn, int nRetMaxSize) {

      string ret = "";
      // ---
      StringPtr strInput = new StringPtr(cStrInput);  // entrada
      StringPtr strReturn = new StringPtr();          // retorno

      try {

        var json = JObject.Parse(strInput.toString());

        var impressora = json["impressora"].ToString();           // nome da impressora
        var arquivoEtiqueta = json["arquivoEtiqueta"].ToString(); // arquivo a ser interpretado
        var conteudoArquivo = removerComecoArquivo(
          File.ReadAllText(arquivoEtiqueta)
        );

        // substitui as macros pelos valores
        JObject campos = (JObject)json["campos"];
        foreach (JProperty j in campos.Properties()) {

          var campo = "%" + j.Name + "%";
          var valor = j.Value.ToString();

          conteudoArquivo = conteudoArquivo.Replace(campo, valor);

        }

        // se a impressora não foi 
        // especificada, ignora a impressão
        if (impressora.Length != 0) {

          var sufixo = DateTime.Now.ToString("_ddMMyyyyHHmmssffffff");

          IPrinter printer = new Printer();
          printer.PrintRawStream(
            impressora,
            new MemoryStream(Encoding.ASCII.GetBytes(conteudoArquivo)), "EtiquetaZebra" + sufixo
          );

        }

        // mas retorna uma string
        // com o código ZPL formatado
        ret = conteudoArquivo;

      } catch (Exception e) {
        ret = "#Erro: " + e.Message;
      }

      // cria o buffer
      if (ret.Length > nRetMaxSize) {
        strReturn.fromString(ret.Substring(0, nRetMaxSize));
      } else {
        strReturn.fromString(ret);
      }

      // copia buffer p/ retornar
      strReturn.copyTo(cStrReturn);

    }
    // ------------------------------------------------------------------------

  }
  // --------------------------------------------------------------------------
  // --------------------------------------------------------------------------

}
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------