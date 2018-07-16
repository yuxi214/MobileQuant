using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuantEngine;

namespace MobileQuant
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
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
            catch(Exception ex)
            {
                Utils.Log(ex.StackTrace);
            }
        }
    }
}
