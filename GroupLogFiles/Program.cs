using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GroupLogFiles
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("This application groups log files and sorts them by day.");
            Console.WriteLine("Created by David Finley.\n\n");


            bool cont;
            HandleFiles fileHandler = new HandleFiles();
            
            if (fileHandler.DoesLogPathExist() == false)
            {
                Console.WriteLine("There is not a path to the log file.\nPress Enter to find log file....");
                
                if (ContinueOrExit() == true)
                {
                    string logFile = fileHandler.PromptForLogFilePath();
                    Console.WriteLine($"The selected log file path : {logFile}\nPress Enter to continue....");
                    if (ContinueOrExit() == true)
                    {
                        Console.Write("The log file saved in this location will be copied and saved to the log file directory.");
                    }
                    
                }
                else 
                { 
                    Environment.Exit(0); 
                }
            }
        }

        public static bool ContinueOrExit()
        {
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    return true; 
                }
                else
                {
                    Console.WriteLine("Exiting Application....");
                    Thread.Sleep(5000);
                    return false;
                }
            }
        }
    }
}
