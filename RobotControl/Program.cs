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

        static void Main(string[] args)
        {
            RobotDataReceived += OnRobotDataReceived;

            string networkFileName = "network.json";
            networkFileName = args[0];
            string networkJson = File.ReadAllText(networkFileName);
            Network = NeuralNetwork.Deserialize(networkJson);

            if(args.Length > 1)
                serverPort = int.Parse(args[1]);

            tcpClient = new TcpClient("localhost", serverPort);
            clientStream = tcpClient.GetStream();

            var data = new Byte[256];
            String responseMessage = String.Empty;

            while (true)
            {
                Int32 dataLength = clientStream.Read(data, 0, data.Length);
                responseMessage = System.Text.Encoding.ASCII.GetString(data, 0, dataLength);
                if (responseMessage.ToLower().Trim() == "exit")
                    break;
                var responseData = ParseResponse(responseMessage);
                RobotDataReceived?.Invoke(tcpClient, responseData);
            }
        }

        private static RobotEventArgs ParseResponse(string message)
        {
            var splitted = message.Split(' ');
            var data = new List<double>();
            foreach (var s in splitted)
                data.Add(double.Parse(s));

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

            return new RobotEventArgs(Network, lookData, axisData, sensorData);
        }

        private static void OnRobotDataReceived(object sender, RobotEventArgs e)
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

        private static void WriteArrayToStream(double[] array)
        {
            string message = String.Join(' ', array);
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);   
            clientStream.Write(data, 0, data.Length);
        }

    }
}
