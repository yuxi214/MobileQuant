using System;
using QuantEngine;

namespace MobileQuant
{
    class Program
    {
        static void Main()
        {
            //确保没有交易在运行
            System.Diagnostics.Process current = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in processes)
            {
                if (process.Id != current.Id && current.ProcessName.Equals(process.ProcessName)) //忽略当前进程 
                {
                        return;
                }
            }

            //启动引擎
            Engine eg = Engine.Instance;
            eg.Run();

            //退出
            while (true)
            {
                Console.WriteLine("退出按q。");
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.Q)
                {
                    Console.WriteLine("继续退出按y。");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                        break;
                }
            }
        }
    }
}
