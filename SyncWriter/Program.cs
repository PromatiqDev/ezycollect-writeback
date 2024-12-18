using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Write-Back Module ...");
            Console.WriteLine("Loading settings:");
            string ArgumentType = "";
            string FolderPath = "";
            string TestRowstring = "";
            int TestRows = 0;
            bool Debug = false;
            
            try
            {
                foreach (string arg in args)
                {
                    Console.WriteLine("Argument = " + arg);
                    if (arg.Length == 2 && arg.Substring(0, 1) == "-")
                        ArgumentType = arg;
                    else
                    {
                        switch (ArgumentType.ToLower())
                        {
                            case "-f":
                                //config project Folder
                                FolderPath = arg;
                                break;
                            case "-t":
                                //test row count
                                TestRowstring = arg;
                                break;
                            
                        }

                        ArgumentType = "";
                    }
                    if (ArgumentType == "-d")
                        Debug = true;
                }
                if (TestRowstring != "")
                    int.TryParse(TestRowstring, out TestRows);

                if (TestRows < 0)
                    TestRows = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Argument read error encountered: " + ex.Message);
            }
            try
            {
                
                
                Console.WriteLine("Calling the transaction agent...");
                if (TestRows > 0)
                {
                    Console.WriteLine("Testing first " + TestRows.ToString() + " events");
                }

                Agent _clsAgent = new Agent(FolderPath, TestRows, Debug);
                _clsAgent.RunAgent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Startup error encountered: " + ex.Message);
            }
        
        }
    }
}
