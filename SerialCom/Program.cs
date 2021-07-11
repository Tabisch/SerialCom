using System;
using System.IO.Ports;
using System.Threading;

namespace SerialCom
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] ports = SerialPort.GetPortNames();
            string chosenPort = "";

            if (ports.Length == 1)
            {
                chosenPort = ports[0];
            }
            else
            {
                if (ports.Length > 1)
                {
                    Console.WriteLine("Choose a Serial Port:");

                    int counter = 1;
                    foreach (string port in ports)
                    {
                        Console.WriteLine(counter++ + ": " + port);
                    }

                    Console.Write("Number: ");

                    chosenPort = ports[Convert.ToInt32(Console.ReadLine()) - 1];
                }
                else
                {
                    Console.WriteLine("No Serial Port found");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }

            Console.WriteLine("Serial setup");
            Serial_Updater.SerialSetup(chosenPort);

            Thread su = new Thread(Serial_Updater.writeToSerial);
            Console.WriteLine("Thread start");
            su.Start();

            Thread.Sleep(2000);

            Console.WriteLine("Controller Reader");
            ControllerReader c = new ControllerReader();

            string temp = "";

            while (true)
            {
                Console.WriteLine("Input");
                temp = Console.ReadLine();

                switch(temp.ToLower())
                {
                    case "exit":
                        Environment.Exit(0);
                        break;
                    case "long":
                        pressHomeButton(1000);
                        break;
                    case "short":
                        pressHomeButton(100);
                        break;
                }
            }
        }

        static void pressHomeButton(int time)
        {
            Serial_Updater.sendSerial(0, 1, "Home");
            Thread.Sleep(time);
            Serial_Updater.sendSerial(0, 0, "Home");
        }
    }
}
