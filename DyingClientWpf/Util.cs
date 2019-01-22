using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DyingClientWpf
{
  public static class Util
  {
    private static readonly MD5 _md5 = MD5.Create();

    public static string HashMd5(string s)
    {
      return BitConverter.ToString(_md5.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", "");
    }
  }
}
