using System.Diagnostics;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            //  C:\Users\ioan.scafa\Teste SAF-T\Romanian_SAFT_Financial_Schema_v_2_3_090821.xsd
            Console.WriteLine(@"Enter the file path for schema file (.xsd) for example C:\Romanian_SAF-T_Financial_Schema_v_2_4_6_09032022.xsd");
            string? schemaPath = Console.ReadLine();

            Console.WriteLine(@"Enter the file path for the generated model for example C:\AuditFileModel.cs");
            string? destFileName = Console.ReadLine();

            string workingDirectory = Directory.GetCurrentDirectory();
            string schemaFileName = Path.GetFileName(schemaPath);


            File.Copy(schemaPath, $"{ workingDirectory }\\{ schemaFileName }", true);

            string? xsdFileName = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xsd").First();

            var proc = Process.Start(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe", $@"{ xsdFileName } /c");
            proc.WaitForExit();

            var generatedCsFileName = (Path.GetFileName(schemaPath).Replace(".xsd", ".cs"));

            AddSchemaLocation(generatedCsFileName, schemaFileName);
            //AddXmlDocumentation(generatedCsFileName, schemaPath);

            File.Delete(@$"{ xsdFileName }");

            string sourceFolderName = Directory.GetCurrentDirectory();
            File.Move(@$"{ sourceFolderName }\{ generatedCsFileName }", destFileName, true);


            Console.WriteLine("Model has been generated.");
            Console.ReadLine();
        }

        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        static void AddSchemaLocation(string generatedCsFileName, string schemaFileName)
        {
            lineChanger("using System.Xml;" + Environment.NewLine + "using System.Xml.Schema;" + Environment.NewLine + "using System.Xml.Serialization;", generatedCsFileName, 11);
            lineChanger(Environment.NewLine, generatedCsFileName, 76);

            // atentie la schema location - ar trebui sa preia denumirea de la 'schemaPath'
            lineChanger("    [XmlAttribute(\"schemaLocation\", Namespace = XmlSchema.InstanceNamespace)]" + Environment.NewLine +
                        $"    public string xsiSchemaLocation = \"urn:StandardAuditFile-Taxation-Financial:RO {schemaFileName}\";", generatedCsFileName, 77);

            lineChanger("}" + Environment.NewLine, generatedCsFileName, 79);
        }

        //static void AddXmlDocumentation(string generatedCsFileName, string schemaFile)
        //{
        //    var csFileLines = File.ReadAllLines(generatedCsFileName);
        //    var xsdFileLines = File.ReadAllLines(schemaFile);

        //    string[] xsdComments = xsdFileLines.Where(x => x.Contains("<xs:documentation>")).ToArray();

        //    Dictionary<string, string> commentsDictionary = new Dictionary<string, string>();

        //    for (int i = 4; i < xsdFileLines.Length; i++)
        //    {
        //        if (xsdFileLines[i].Contains("<xs:documentation>"))
        //        {

        //            int indexToStart = xsdFileLines[i - 2].IndexOf("\"") + 1;
        //            if (indexToStart == -1)
        //            {
        //                indexToStart = 0;
        //            }

        //            int indexofsecond = xsdFileLines[i - 2].IndexOf("\"", xsdFileLines[i - 2].IndexOf("\"") + 1);
        //            if (indexofsecond == -1)
        //            {
        //                indexofsecond = 0;
        //            }

        //            int length = indexofsecond - indexToStart;
        //            string value = xsdFileLines[i - 2].Substring(indexToStart, length);

        //            string key = xsdFileLines[i].Replace("<xs:documentation>", "").Replace("</xs:documentation>", "");

        //            commentsDictionary.Add($"{ key } { i }" , value);
        //        }
        //    }


        //    foreach (var record in commentsDictionary.Where(x => x.Key.Contains("Unit of measure applicable to this product.") == false))
        //    {
        //        for (int i = 0; i < csFileLines.Length - 1; i++)
        //        {
        //            if (csFileLines[i].Contains("/// <remarks/>") && csFileLines[i + 1].Trim().StartsWith("["))
        //            {
        //                if (csFileLines[i + 1].Trim().StartsWith("["))
        //                {
        //                    int y = 1;
        //                    while (csFileLines[i + y].Trim().StartsWith("["))
        //                    {
        //                        y++;
        //                    }

        //                    if (csFileLines[i + y].Split(" ")[3].ToString().Trim() == record.Value)
        //                    {
        //                        csFileLines[i] = csFileLines[i].Replace("/// <remarks/>", $"/// <summary>{ record.Key.Trim().Substring(0, record.Key.Trim().IndexOf(".") + 1) }</summary>");
        //                    }
        //                }
        //            }
        //        }

        //    }
            //File.WriteAllLines(generatedCsFileName, csFileLines);
        //}
    }
}

