using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtiquetaZebra {

  // ---
  // converte strings
  public class StringPtr {

    byte[] buff = new byte[1024];

    // ---
    // StringPtr()
    public StringPtr() {

      buff[0] = 0;

    }
    // ------------------------------------------------------------------------

    // ---
    // StringPtr()
    public StringPtr(string s) {

      fromString(s);

    }
    // ------------------------------------------------------------------------

    // ---
    // StringPtr()
    public unsafe StringPtr(byte* b) {

      fromPtr(b);

    }
    // ------------------------------------------------------------------------

    // ---
    // string p/ ptr
    public void fromString(string s) {

      var i = 0;
      foreach(var c in s)
        buff[i++] = (byte)c;  
      buff[i] = 0;

    }
    // ------------------------------------------------------------------------

    // ---
    // ptr p/ string
    public unsafe void fromPtr(byte* ptr) {

      var i = 0;
      while (ptr[i] != 0)
        buff[i] = ptr[i++];
      buff[i] = 0;

    }
    // ------------------------------------------------------------------------

    // ---
    // retorna ptr
    public unsafe byte* getPtr() {

      fixed (byte* ptr = @buff) {
        return ptr;
      }

    }
    // ------------------------------------------------------------------------

    // ---
    // copia buff p/ ptr
    public unsafe void copyTo(byte* dst) {

      var i = 0;
      while (buff[i] != 0)
        dst[i] = buff[i++];
      dst[i] = 0;

    }
    // ------------------------------------------------------------------------

    // ---
    // ptr p/ string
    public string toString() {

      StringBuilder ret = new StringBuilder();

      var i = 0;
      while (buff[i] != 0)
        ret.Append((char)buff[i++]);

      return ret.ToString();

    }
    // ------------------------------------------------------------------------

  }
  // --------------------------------------------------------------------------
  // --------------------------------------------------------------------------

}
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
