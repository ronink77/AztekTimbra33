/*
||||||||||||||||||||||||||||||||||||||||
||||    Código Desarrollado por    |||||
||||    Juliam Maya Diaz           |||||
||||    Octubre 2020       V 0.1   |||||
||||||||||||||||||||||||||||||||||||||||
 */ 
using System.Security.Cryptography;
using System.Text;

namespace aztektimbra.Utilerias
{
   public class Encrypt
    {
        public static string GetSHA1(string str)
        {
            SHA1 sha1 = SHA1Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha1.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}
