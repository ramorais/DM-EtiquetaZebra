using System;
using System.IO;

namespace EtiquetaZebra
{
    class Program
    {
        static unsafe void Main(string[] args)
        {

          var cStrInput = new StringPtr();
          var cStrReturn = new StringPtr();

          cStrInput.fromString(
            @"
            {
              ""impressora"": ""Generic"",
              ""arquivoEtiqueta"": ""c:\\temp\\etiqueta.prn"",
              ""campos"": {
                ""nf"": ""1234""
              }
            }
            "
          );

          ProtheusDll.ExecInClientDLL(0, cStrInput.getPtr(), cStrReturn.getPtr(), 1);

        }
    }
}