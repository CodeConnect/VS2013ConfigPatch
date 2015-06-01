using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VS2013ConfigPatch
{
    class Program
    {
        const string LOCATION_WITHIN_PROGRAM_FILES = @"Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe.config";
        const string ASSEMBLY_BASE = @"Microsoft.Build";
        const string ASSEMBLY1 = @"Microsoft.Build";
        const string ASSEMBLY2 = @"Microsoft.Build.Engine";
        const string ASSEMBLY3 = @"Microsoft.Build.Framework";
        const string OLD_VERSION_PATCHED = @"2.0.0.0 - 12.0.0.0";
        const string OLD_VERSION_UNDO = @"2.0.0.0 - 4.0.0.0";
        const string NEW_VERSION_PATCHED = @"14.0.0.0";
        const string NEW_VERSION_UNDO = @"12.0.0.0";

        static void Main(string[] args)
        {
            bool undo = StartUp();
            try
            {
                var targetFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), LOCATION_WITHIN_PROGRAM_FILES);
                BackUpFile(targetFileName, undo);
                ModifyXml(targetFileName, undo);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("\nRun this program with administrative privileges.");
            }
            catch (FileNotFoundException fe)
            {
                Console.WriteLine($"\nCouldn't find file {fe.FileName}");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nExecution aborted: " + e.ToString());
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static bool StartUp()
        {
            Console.WriteLine(@"
           _ _           
     /\   | (_)          
    /  \  | |___   _____ 
   / /\ \ | | \ \ / / _ \
  / ____ \| | |\ V /  __/
 /_/    \_\_|_| \_/ \___|
Patch for Visual Studio 2013

Do you want to apply the patch or undo it?
Press [A] to Apply
Press [U] to Undo

");

            do
            {
                var key = Console.ReadKey().KeyChar.ToString();
                if (key.Equals("A", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine();
                    return false;
                }
                else if (key.Equals("U", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine();
                    return true;
                }
                else
                {
                    Console.WriteLine(" (you need to press either A or U. You most likely should press A)");
                }
            } while (true);
        }

        private static void BackUpFile(string targetFileName, bool undo = false)
        {
            var backupFileName = undo ? targetFileName + ".undoBackup" : targetFileName + ".backup";
            Console.WriteLine($"Backing up {targetFileName} into {backupFileName}...");
            try
            {
                File.Copy(targetFileName, backupFileName);
            }
            catch (IOException)
            {
                // destination file already exists. We don't want to do any work without creating a backup,
                // but we also don't want to overwrite an existing backup.
                Console.WriteLine($"Backup file {backupFileName} already exists. Save it elsewhere, remove it manually, then re-run the script.");
                throw;
            }
            Console.WriteLine($"Backup successful.");
        }

        public static void ModifyXml(string path, bool undo = false)
        {
            bool patchedAssembly1 = false;
            bool patchedAssembly2 = false;
            bool patchedAssembly3 = false;

            XDocument doc = XDocument.Load(path);
            XNamespace ns = "urn:schemas-microsoft-com:asm.v1";

            var level1 = doc.Descendants("runtime").First();
            var level2 = level1.Descendants(ns + "assemblyBinding").First();
            var assemblies = level2.Descendants(ns + "dependentAssembly");
            foreach (var assembly in assemblies)
            {
                var identity = assembly.Descendants(ns + "assemblyIdentity");
                var assemblyName = identity.Attributes().Where(attr => attr.Name == "name").First().Value;
                if (assemblyName.StartsWith(ASSEMBLY_BASE))
                {
                    var redirect = assembly.Descendants(ns + "bindingRedirect").First();
                    redirect.SetAttributeValue("oldVersion", undo ? OLD_VERSION_UNDO : OLD_VERSION_PATCHED);
                    redirect.SetAttributeValue("newVersion", undo ? NEW_VERSION_UNDO : NEW_VERSION_PATCHED);
                    patchedAssembly1 |= assemblyName == ASSEMBLY1;
                    patchedAssembly2 |= assemblyName == ASSEMBLY2;
                    patchedAssembly3 |= assemblyName == ASSEMBLY3;
                }
                if (patchedAssembly1 && patchedAssembly2 && patchedAssembly3)
                {
                    // We're done early
                    break;
                }
            }
            if (patchedAssembly1 && patchedAssembly2 && patchedAssembly3)
            {
                // Save the xml
                doc.Save(path);
                Console.WriteLine(@"

Patch successful :)

Remember to install Visual Studio 2015 Build Tools.

");
            }
            else
            {
                if (!patchedAssembly1)
                    Console.WriteLine("Unable to patch {ASSEMBLY1}");
                if (!patchedAssembly2)
                    Console.WriteLine("Unable to patch {ASSEMBLY2}");
                if (!patchedAssembly3)
                    Console.WriteLine("Unable to patch {ASSEMBLY3}");
                throw new ApplicationException("Unable to patch the configuration file.");
            }
        }
    }
}
