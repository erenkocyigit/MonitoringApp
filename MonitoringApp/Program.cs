using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace MonitoringApp
{
    internal class Program
    {
        static string inspectedProcessName = null;
        static int nCounter ,nOverFlowCount = 0;
        static Thread thread;
        static bool abortProgram = false;

        static void Main(string[] args)
        {
            int nArgsCount = args.Length;
            int nTempSamplingTime = 0;
            int nTempLifeTime = 0;

            if (nArgsCount != 3)
            {
                Console.WriteLine("Arguments count not equal 3. Please check it.\n");
                return;
            }

            inspectedProcessName = args[0];

            thread = new Thread(new ThreadStart(thread_Callback));
            thread.Priority = ThreadPriority.AboveNormal;
            thread.Start();

            nTempLifeTime = int.Parse(args[1]);
            nTempSamplingTime = int.Parse(args[2]);
            

            nCounter = nOverFlowCount = nTempLifeTime / nTempSamplingTime;

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = nTempSamplingTime * 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            while (true)
            {
                if(abortProgram)
                {
                    thread.Join(100);
                    thread.Abort();
                    Environment.Exit(0);
                    break;
                }
            }

            Console.WriteLine("The process has been terminated.");
            Console.ReadLine();

        }

        static void thread_Callback()
        {
            while (true)
            {
                ConsoleKeyInfo cKeyInfo = Console.ReadKey();

                if(cKeyInfo.Key == ConsoleKey.Q)
                {
                    abortProgram = true;
                }
            }
        }

        static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Process[] procs = System.Diagnostics.Process.GetProcessesByName(inspectedProcessName);
            if (procs.Length > 0)
            {
                nCounter--;

                if (nCounter == 0)
                {
                    Process proc = System.Diagnostics.Process.GetProcessById(procs[0].Id);
                    try
                    {
                        proc.Kill();
                    }
                    catch (Exception)
                    { }

                    abortProgram = true;
                }
            }
            else
            {
                nCounter = nOverFlowCount;
            }


            
        }
    }
}
