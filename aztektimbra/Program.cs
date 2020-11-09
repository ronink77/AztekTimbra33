/*
||||||||||||||||||||||||||||||||||||||||||
|||||   Software desarrollado por    ||||| 
|||||   Juliam Maya Diaz             |||||
|||||   Septiembre 2020        v.01  |||||
||||||||||||||||||||||||||||||||||||||||||
*/
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using aztektimbra.Utilerias;

namespace aztektimbra
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (Environment.GetCommandLineArgs().Length != 7)
            {

                string commandLineArg1 = Environment.GetCommandLineArgs()[1];//aztektimbra
                string commandLineArg2 = Environment.GetCommandLineArgs()[2];//directorio .cer
                string commandLineArg3 = Environment.GetCommandLineArgs()[3];//directorio .key
                string commandLineArg4 = Environment.GetCommandLineArgs()[4];//firectorio paswword
                string commandLineArg5 = Environment.GetCommandLineArgs()[5];//directorio xml.xml
                string commandLineArg6 = Environment.GetCommandLineArgs()[6];//directorio serie+folio+.xml
                string commandLineArg7 = Environment.GetCommandLineArgs()[7];//ruta .xslt
                byte[] numArray1 = (byte[])null;

                //Imprime un fichero con los argumentos pasados por AztekCFDI.
                System.IO.File.WriteAllText(@"C:\\cfdi\\Ruta-Timbrado.txt", commandLineArg1 + " - " + commandLineArg2 + " - " + commandLineArg3 + " - " + commandLineArg5 + " - " + commandLineArg6 + " - " + commandLineArg7);

                //Carga el transformador xslt
                string CadenaOriginal = "";
                System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
                transformador.Load(commandLineArg7);

                //Carga el xml y obtiene la Cadena-orginal
                using (StringWriter sw = new StringWriter())
                using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
                {
                    transformador.Transform(commandLineArg5, xwo);
                    CadenaOriginal = sw.ToString();
                    System.IO.File.WriteAllText(@"C:\\cfdi\\Cadena-Original.txt", CadenaOriginal);

                }

                //Se declara la variable par el sello y se guardan en un ficheros con el valor de retorno.       
                string selloCFD = "";
                SelloDigital oSelloDigital = new SelloDigital();

                selloCFD = oSelloDigital.Sellar(CadenaOriginal, commandLineArg3, commandLineArg4);
                System.IO.File.WriteAllText(@"C:\\cfdi\\Sello-Emisor.txt", selloCFD);

                //Se leé el xml  generado por AztekCFDI
                FileStream fileStream1 = new FileStream(commandLineArg5, FileMode.OpenOrCreate, FileAccess.Read);
                XmlDocument oXML = new XmlDocument();
                oXML.Load((Stream)fileStream1);

                if (commandLineArg1 == "aztektimbra")
                {
                    //Si algo sale mal se imprime un fichero y se muestra el error en consola
                    if (selloCFD == null)
                    {
                        Console.Write("Error en sellado");
                        Console.ReadKey();
                        System.IO.File.WriteAllText(commandLineArg6 + ".resp", "Error en sellado");
                        Environment.Exit(0);
                    }

                    //Se leén los atributos del xml y modifica con los valores obtenidos.
                    oXML.ChildNodes[1].Attributes["Sello"].Value = selloCFD;

                    X509Certificate2 x509Certificate2 = new X509Certificate2(commandLineArg2);

                    if (x509Certificate2 != null)
                    {
                        string base64String = Convert.ToBase64String(x509Certificate2.RawData);

                        //Reescribe el xml con los atributos modificados.
                        numArray1 = Encoding.UTF8.GetBytes(oXML.InnerXml);
                        FileStream fileStream2 = new FileStream(commandLineArg6, FileMode.Create, FileAccess.ReadWrite);
                        BinaryWriter binaryWriter = new BinaryWriter((Stream)fileStream2, Encoding.Unicode);
                        fileStream2.Position = 0L;
                        binaryWriter.Write(numArray1);
                        binaryWriter.Close();
                        fileStream2.Close();

                    }

                    // Se cierran  los archivos usados.
                    StreamReader streamReader = new StreamReader(commandLineArg6);
                    string end = streamReader.ReadToEnd();
                    streamReader.Close();
                    FileStream fileStream3 = new FileStream(commandLineArg2, FileMode.Open, FileAccess.Read);
                    int length1 = (int)fileStream3.Length;
                    byte[] numArray2 = new byte[length1];
                    int num = fileStream3.Read(numArray2, 0, length1);
                    fileStream3.Close();
                    FileStream fileStream4 = new FileStream(commandLineArg3, FileMode.Open, FileAccess.Read);
                    int length2 = (int)fileStream4.Length;
                    byte[] numArray3 = new byte[length2];
                    num = fileStream4.Read(numArray3, 0, length2);
                    fileStream4.Close();

                    //Cargamos el XML sellado  en una matriz  y lo convierte en Base64         
                    byte[] rutaxmlsellado = System.IO.File.ReadAllBytes(commandLineArg6);
                    string xmlBase64 = Convert.ToBase64String(rutaxmlsellado);

                    //Imprime un fichero con el resultado de la converción del XML sellado.
                    System.IO.File.WriteAllText(@"C:\\cfdi\\xmlBase64.txt", xmlBase64);

                    /*
                   //---------------nicia proceso de timbrado del XML---------------
                   //Crear el objeto cliente
                   ServiceReference1.timbrado_cfdi33_portClient cliente_timbrar = new ServiceReference1.timbrado_cfdi33_portClient();

                    //Crear el objeto de la respuesta
                
                    ServiceReference1.timbrar_cfdi_result response = new ServiceReference1.timbrar_cfdi_result();
                 

                    //llamar el método de timbrado enviándole los 
                    //parámetros con las credenciales y el xml en formato base64
                   response = cliente_timbrar.timbrar_cfdi("AAA010101000", "h6584D56fVdBbSmmnB", codificado);

                 */                                  }

            }

        }
    }
}
