using System;
using System.Threading;

namespace SerialCom
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread su = new Thread(Serial_Updater.writeToSerial);
            su.Start();

            Thread.Sleep(2000);

            ControllerReader c = new ControllerReader();

            Console.ReadLine();
        }
    }
}
