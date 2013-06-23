using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Core;

namespace FileEncrypt
{
    class Program
    {
        static int Main(string[] args)
        {
            if (!args.Any() || args.Length > 2)
            {
                Usage();
                return Pause();
            }

            var sourceFilename = new FileInfo(args[0]);
            if (!sourceFilename.Exists)
                throw new FileNotFoundException("File not found: " + sourceFilename.FullName);

            var destFilename = Path.Combine(sourceFilename.Directory.FullName, 
                sourceFilename.Name.Replace(sourceFilename.Extension, DefaultExt));

            if (args.Length == 2)
                destFilename = args[1];

            var sourceData = File.ReadAllText(sourceFilename.FullName);
            var encrypted = CryptoManager.Encrypt3DES(sourceData);
            File.WriteAllText(destFilename, encrypted);

            Console.WriteLine("Encrypted contents of {0} to {1}", sourceFilename, destFilename);
            Pause();

            return 0;
        }

        private static void Usage()
        {
            var sb = new StringBuilder();
            var exeName = Assembly.GetExecutingAssembly().GetName().Name + ".exe";
            var defaultExt = DefaultExt;
            sb.AppendFormat( "{0} sourceInputFilename [destFilename]", exeName);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("[destFilename] - optional, defaults to sourceInputFilename dir with {0} ext",
                            defaultExt);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("Examples:");
            sb.AppendLine();
            sb.AppendFormat("{0} C:\\Dropbox\\Public\\WIO.json{1}", exeName, Environment.NewLine);
            sb.AppendFormat("\tCreates C:\\Dropbox\\Public\\WIO{0}", defaultExt);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("{0} C:\\Dropbox\\Public\\WIO.json C:\\Dropbox\\Public\\WIO.cfg", exeName);
            Console.WriteLine(sb.ToString());
        }

        private static string DefaultExt
        {
            get { return ConfigurationManager.AppSettings["DefaultExt"]; }
        }

        private static int Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            return 0;
        }
    }
}
