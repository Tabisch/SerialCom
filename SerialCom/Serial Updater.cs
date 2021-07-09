using System;
using System.Collections.Concurrent;
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
        static ConcurrentQueue<ControllerActionCalls> cacq;

        static bool waitForAwnser = true;

        static bool desynced = false;

        static int sleeptime = 10;

        static int lastUpdate;


        public static void sendSerial(short selection, short message, string info)
        {
            cacq.Enqueue(new ControllerActionCalls(selection, message, info));
        }

        public static bool getDesynced()
        {
            return desynced;
        }

        public static void writeToSerial()
        {
            while (true)
            {
                if(desynced)
                {
                    Thread.Sleep(sleeptime);
                    cacq.Clear();
                    desynced = false;
                    continue;
                }

                if (cacq.Count > 0)
                {
                    ControllerActionCalls temp;
                    cacq.TryDequeue(out temp);

                    serialPort.WriteLine(temp.getSelection().ToString());
                    serialPort.WriteLine(temp.getInput().ToString());
                    Console.WriteLine("Send: selection " + temp.getSelection().ToString() + " " + temp.getInput().ToString() + " " + temp.getInfo());

                    lastUpdate = Environment.TickCount;

                    waitForAwnser = true;

                    while (waitForAwnser)
                    {
                        Thread.Sleep(sleeptime);
                    }
                }
                else
                {
                    if(Environment.TickCount - lastUpdate > 1000)
                    {
                        sendSerial(-1,0, "Keep Alive");
                        lastUpdate = Environment.TickCount;
                    }

                    Thread.Sleep(sleeptime);
                }
            }
        }

        public static void SerialSetup(string port)
        {
            serialPort = new SerialPort(port, 19200);
            serialPort.NewLine = "\n";
            serialPort.DataReceived += dataReceived;

            serialPort.Open();

            cacq = new ConcurrentQueue<ControllerActionCalls>();

            lastUpdate = Environment.TickCount;
        }

        static void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string temp = serialPort.ReadLine();

            Console.WriteLine("Data Received: " + temp);

            if(temp == "Desynced")
            {
                desynced = true;
            }

            waitForAwnser = false;
        }
    }
}
