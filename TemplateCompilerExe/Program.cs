using System;
using System.Collections.Generic;
using System.Linq;
using Bendyline.Base;
using Bendyline.UI.TemplateCompiler;

namespace Bendyline.UI.TemplateCompiler.exe
{
    public static class Program
    {
        static void Main(string[] args)
        {
            CommandLineLogger cll = new CommandLineLogger();
            cll.Initialize();

            TemplateCompilerEngine tce = new TemplateCompilerEngine();

            for (int i=0; i<args.Length; i++)
            {
                String arg = args[i];
                String argCanon = arg.ToLower();

                if (argCanon.StartsWith("/"))
                {
                    argCanon = "-" + argCanon.Substring(1);
                }

                switch (argCanon)
                {
                    case "-?":
                    case "-h":
                    case "-help":
                        OutputUsage();
                        break;

                    case "-if":
                    case "-inputfolder":
                        if (i < args.Length - 1)
                        {
                            i++;
                            tce.InputFolder = args[i];
                        }
                        break;

                    case "-of":
                    case "-outputfolder":
                        if (i < args.Length - 1)
                        {
                            i++;
                            tce.OutputFolder = args[i];
                        }
                        break;

                    case "-n":
                    case "-name":
                        if (i < args.Length - 1)
                        {
                            i++;
                            tce.Name= args[i];
                        }
                        break;

                }
            }

            if (tce.InputFolder == null)
            {
                Console.WriteLine("No input folder was specified.\r\n");

                OutputUsage();

                return;
            }

            try
            {
                if (tce.OutputFolder == null)
                {
                    Console.WriteLine("No output folder was specified.");

                    OutputUsage();

                    return;
                }

                tce.Execute();
            }
            catch (Exception)
            {
                ;
            }
        }

        private static void OutputUsage()
        {
            Console.WriteLine(@"Usage: bltemplate -inputfolder <folder for input> -outputfolder <folder for output>

    inputfolder: Path to folder with .htm/.html files to compile in.
    outputfolder: Path to export compiled file.");
        }
    }
}
