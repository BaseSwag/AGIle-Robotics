using System;
using Newtonsoft.Json;
using AGIle_Robotics;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RobotControl
{
    class Program
    {
        static NeuralNetwork Network;
        static event EventHandler<RobotEventArgs> RobotDataReceived;

        static int serverPort = 1337;
        static TcpClient tcpClient;
        static Stream clientStream;
        static int maxErrors = 3;

        static void Main(string[] args)
        {
            RobotDataReceived += OnRobotDataReceived;

            string networkFileName = "network.json";
            if (args.Length > 0)
                networkFileName = args[0];

            if (GetNetwork(networkFileName, out Network))
            {
                if (args.Length > 1)
                {
                    int temp;
                    if (int.TryParse(args[1], out temp))
                        serverPort = temp;
                }

                ServerLoop(serverPort);
            }
        }

        private static RobotEventArgs ParseResponse(string message)
        {
            var splitted = message.Split(' ');
            var data = new List<double>();
            int counter = 0;
            double temp;
            foreach (var s in splitted)
            {
                if(double.TryParse(s, out temp))
                {
                    data.Add(temp);
                    counter++;
                }
                else
                    break;
            }

            if (counter == 6)
            {
                double[] lookData = new double[]
                {
                data[0],
                data[1],
                };

                double[] axisData = new double[]
                {
                data[2],
                data[3],
                };

                double[] sensorData = new double[]
                {
                data[4],
                data[5],
                };

                try
                {
                    return new RobotEventArgs(Network, lookData, axisData, sensorData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            return null;
        }

        private static void OnRobotDataReceived(object sender, RobotEventArgs e)
        {
            if (e != null)
            {
                try
                {
                    List<double> input = new List<double>();
                    foreach (double i in e.LookData)
                        input.Add(i);
                    foreach (double i in e.AxisData)
                        input.Add(i);
                    foreach (double i in e.SensorData)
                        input.Add(i);

                    double[] output = e.Network.Activate(input.ToArray());
                    WriteArrayToStream(output);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void WriteArrayToStream(double[] array)
        {
            try
            {
                string message = String.Join(' ', array);
                byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                clientStream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool GetNetwork(string fileName, out NeuralNetwork network)
        {
            try
            {
                string networkJson = File.ReadAllText(fileName);
                network = NeuralNetwork.Deserialize(networkJson);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                network = null;
                return false;
            }

        }

        private static void ServerLoop(int port)
        {
            try
            {
                tcpClient = new TcpClient("localhost", serverPort);
                clientStream = tcpClient.GetStream();

                var data = new Byte[256];
                String responseMessage = String.Empty;
                int errorCount = 0;

                while (true)
                {
                    try
                    {
                        int dataLength = clientStream.Read(data, 0, data.Length);
                        responseMessage = System.Text.Encoding.ASCII.GetString(data, 0, dataLength);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        if (++errorCount >= maxErrors)
                        {
                            Console.WriteLine("Too many errors, aborting...");
                            break;
                        }
                    }

                    if (responseMessage.ToLower().Trim() == "exit")
                    {
                        Console.WriteLine("Exit requested. Shutting down...");
                        break;
                    }
                    var responseData = ParseResponse(responseMessage);
                    RobotDataReceived?.Invoke(tcpClient, responseData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                tcpClient.Dispose();
                clientStream.Dispose();
                Console.WriteLine("Connection closed");
            }
        }
    }
}
