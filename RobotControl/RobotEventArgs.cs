using System;
using System.Collections.Generic;
using System.Text;

namespace RobotControl
{
    class RobotEventArgs : EventArgs
    {
        public AGIle_Robotics.Interfaces.INeuralNetwork Network;
        public double[] LookData;
        public double[] AxisData;
        public double[] SensorData;

        public RobotEventArgs(AGIle_Robotics.Interfaces.INeuralNetwork network, double[] lookData, double[] axisData, double[] sensorData)
        {
            Network = network;
            LookData = lookData;
            AxisData = axisData;
            SensorData = sensorData;
        }
    }
}
