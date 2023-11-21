using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            HandleFiles fileHandler = new HandleFiles();
            //bool logPathExists = fileHandler.DoesLogPathExist();
            //bool dirPathExists = fileHandler.DoesDirectoryExist();
            bool logPathExists = false;
            bool dirPathExists = false; 
            try
            {
                // handles locating the log file and the directory if they are not in the app settings. 
                if (logPathExists == false || dirPathExists == false)
                {
                    if (logPathExists == false)
                    {
                        if (HandleLogFile(fileHandler) == true)
                        {
                            fileHandler.LogFile = fileHandler.GetLogFilePathFromConfig();
                            logPathExists = true;
                        }
                    }
                    if (dirPathExists == false)
                    {
                        if (HandleLogFileDirectory(fileHandler) == true)
                        {
                            fileHandler.LogFileDirectory = fileHandler.GetDirectoryFromConfig();
                            dirPathExists = true;
                        }
                    }
                }

                // if the log file and the directory are in the app settings then we can run the application as normal.
                // the functionality is every morning before Wash Entrance Controller runs (or any app that creates log files),
                // we want to copy yesterdays log file to the log file directory so we can store and analyze them.

                bool stopThread = false; 

                Thread copyLogFile = new Thread(() =>
                {
                    while (stopThread == false)
                    {
                        DateTime currentTime = DateTime.Now;
                        DateTime date = currentTime.Date;
                        DateTime target = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 13, 23, 0);

                        if (currentTime >= target)
                        {
                            if (File.Exists(fileHandler.LogFile) == true)
                            {
                                //string newLogFile = Path.Combine(fileHandler.LogFileDirectory, $"log:{date.ToString("yyyy-MM-dd")}.txt");
                                string newLogFile = fileHandler.LogFileDirectory.ToString() + "\\log:" + date.ToString("yyyy-MM-dd") + ".txt";
                                Directory.CreateDirectory(fileHandler.LogFileDirectory);
                                File.Copy(fileHandler.LogFile.ToString(), newLogFile);
                                Console.WriteLine($"{date.ToString("yyyy-MM-dd")} : Log file successfully copied.");
                            }
                            else
                            {
                                Console.WriteLine($"{date.ToString("yyyy-MM-dd")} : Log file does not exist.");
                            }
                        }

                        Thread.Sleep(60000);
                    }
                });

                copyLogFile.Start();

                while (true)
                {
                     
                    Console.Clear();
                    Console.WriteLine("Program running.....\n");
                    Console.WriteLine("1. Edit variables\n2. Exit Application");

                    var key = Console.ReadKey(intercept : true);
                    if (key.Key == ConsoleKey.D1)
                    {
                        // show the log file and log file directory variables and allow them to be edited. 
                        bool changesMade = ChangeVars(fileHandler);
                    }
                    else if (key.Key == ConsoleKey.D2)
                    {
                        // end the thread and close the application.
                        stopThread = true;
                        copyLogFile.Join();
                        Console.Clear();
                        Console.WriteLine("Exiting Application....");
                        Thread.Sleep(10000);
                        break;
                    }
                }
                
               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            
        }

        public static string RemoveDoubleSlashes(string inputPath)
        {
            string cleanedPath = Regex.Replace(inputPath, @"\\+", @"\");
            return cleanedPath;
        }

        public static bool ChangeVars(HandleFiles filehandler)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select the variable you would like to edit.");
                Console.WriteLine($"1. Log File : {filehandler.LogFile}");
                Console.WriteLine($"2. Log File Directory : {filehandler.LogFileDirectory}");
                Console.WriteLine("3. Return");
                var key = Console.ReadKey(intercept : true);
                if (key.Key == ConsoleKey.D1)
                {
                    string newLogFIle = filehandler.PromptForLogFilePath();
                    filehandler.UpdateLogPathInConfig(newLogFIle);
                    Console.WriteLine("Successfully updated log file.");
                }
                else if (key.Key == ConsoleKey.D2)
                {
                    string newLogFileDirectory = filehandler.PromptForLogFileDirectory();
                    filehandler.UpdateLogDirectoryInConfig(newLogFileDirectory);
                    Console.WriteLine("Successfully updated log file directory");
                }
                else if (key.Key == ConsoleKey.D3)
                {
                    return true; 
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

        public static bool HandleLogFile(HandleFiles fileHandler)
        {
            Console.WriteLine("There is not a path to the log file. Press Enter to locate log file....");

            if (ContinueOrExit() == true)
            {
                string logFile = fileHandler.PromptForLogFilePath();
                Console.WriteLine($"The selected log file path : {logFile}\n\nPress Enter to continue....\n");
                if (ContinueOrExit() == true)
                {
                    fileHandler.UpdateLogPathInConfig(logFile);
                    return true;
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Environment.Exit(0);
            }
            return false;
        }

        public static bool HandleLogFileDirectory(HandleFiles fileHandler)
        {
            Console.WriteLine("There is not a path to the Log File Directory. Press Enter to locate directory....");
            if (ContinueOrExit() == true)
            {
                string logFileDirectory = fileHandler.PromptForLogFileDirectory();
                Console.WriteLine($"The selected log file directory : {logFileDirectory}\n\nPress Enter to continue....\n");
                if (ContinueOrExit() == true)
                {
                    fileHandler.UpdateLogDirectoryInConfig(logFileDirectory);
                    return true;
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Environment.Exit(0);
            }
            return false;
        }
    }
}
