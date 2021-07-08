using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialCom
{
    class Serial_Updater
    {
        static SerialPort serialPort;
        static Queue<ControllerActionCalls> cacq;

        static bool waitForAwnser = true;


        public static void sendSerial(short selection, short message)
        {
            cacq.Enqueue(new ControllerActionCalls(selection, message));
        }

        public static void writeToSerial()
        {
            while (true)
            {
                if (cacq.Count > 0)
                {
                    ControllerActionCalls temp = cacq.Dequeue();

                    serialPort.WriteLine(temp.getSelection().ToString());
                    serialPort.WriteLine(temp.getInput().ToString());
                    Console.WriteLine("Send: selection " + temp.getSelection().ToString() + " " + temp.getInput().ToString());

                    while (waitForAwnser)
                    {
                        Thread.Sleep(10);
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        public static void SerialSetup(string port)
        {
            serialPort = new SerialPort("COM4", 9600);
            serialPort.NewLine = "\n";
            serialPort.DataReceived += dataReceived;

            serialPort.Open();

            cacq = new Queue<ControllerActionCalls>();
        }

        static void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            waitForAwnser = false;

            Console.WriteLine("Data Received: " + serialPort.ReadLine());
        }
    }
}
