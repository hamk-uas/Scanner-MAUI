using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Scanner_MAUI.Helpers
{
    // Create a class to hold the extracted information
    public class ScannerData
    {
        public string Message { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int RSSI { get; set; }
        public double SNR { get; set; }
        public DateTime TimeStamp { get; set; }
 
    }

    class SerialPortConn
    {
        private SerialPort serialPort;
        private List<ScannerData> scannerDataList = new List<ScannerData>();
        // static list to store the network names
        public static List<string> NetworkNames { get; } = new List<string>();
        public void ConnectToScanner()
        {
            // Clear the NetworkNames list before connecting to the scanner
            NetworkNames.Clear();
            try
            {
                serialPort = new SerialPort("COM6", 115200);

                serialPort.Open();
                Debug.WriteLine("Serial Port conn created");
                Debug.WriteLine("Serial Port Is Open: " + serialPort.IsOpen);

                var data = new byte[] { (byte)'1', 13 };
                serialPort.Write(data, 0, data.Length);

                // TODO: Handle the scanner's output and display the information in your app
                serialPort.DataReceived += SerialPort_DataReceived;
                //Debug.WriteLine("test");
            }
            catch (Exception ex)
            {
                // Handle the exception
                Debug.WriteLine($"Failed to connect to the scanner: {ex.Message}");
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    string DataIn = serialPort.ReadLine();
                    //Debug.WriteLine("Data received from the scanner: " + DataIn);

                    // Process the received data and extract the fields
                    ScannerData scannerData = ProcessReceivedData(DataIn);

                    // Save the extracted data for later use
                    scannerDataList.Add(scannerData);

                    foreach(string name  in NetworkNames)
                    {
                        Debug.WriteLine($"Name: {name}");
                    }

                    //Debug.WriteLine("Data list: " + scannerData);
                    //foreach (ScannerData data in scannerDataList)
                    //{
                    //    Debug.WriteLine($"Message: {data.Message}");
                    //    Debug.WriteLine($"Name: {data.Name}");
                    //    Debug.WriteLine($"Type: {data.Type}");
                    //    Debug.WriteLine($"Latitude: {data.Latitude}");
                    //    Debug.WriteLine($"Longitude: {data.Longitude}");
                    //    Debug.WriteLine($"RSSI: {data.RSSI}");
                    //    Debug.WriteLine($"SNR: {data.SNR}");
                    //    //Debug.WriteLine($"Time: {data.TimeStamp}");
                    //    Debug.WriteLine(""); // Add an empty line between each scanner data
                    //}
                }
            }
            catch (OperationCanceledException)
            {
                // Handle the cancellation if needed
                Debug.WriteLine("Reading data from the scanner was canceled.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Debug.WriteLine($"Failed to read data from the scanner: {ex.Message}");
            }
        }

        private ScannerData ProcessReceivedData(string data)
        {
            ScannerData scannerData = new ScannerData();

            //Debug.WriteLine($"Data received from the scanner before spliting: {data}");

            // Parse and extract relevant information from the data string
            string[] fields = data.Split(new string[] { ", " }, StringSplitOptions.None);

            Debug.WriteLine($"Received data fields count: {fields.Length}");
            //Debug.WriteLine($"Received data fields: {string.Join(", ", fields)}");

            if (fields.Length >= 8)
            {
                scannerData.Message = fields[0].Split(':')[1].Trim();
                string input = fields[1];
                string startMarker = "b'";
                string endMarker = "'";

                int startIndex = input.IndexOf(startMarker) + startMarker.Length;
                int endIndex = input.IndexOf(endMarker, startIndex);

                if (startIndex >= 0 && endIndex >= 0)
                {
                    string extractedString = input.Substring(startIndex, endIndex - startIndex);
                    scannerData.Name = extractedString;
                    NetworkNames.Add(extractedString); // Add network name to the list
                }
                scannerData.Type = fields[2].Split(':')[1].Trim();
                scannerData.Latitude = ParseNullableDouble(fields[3].Split(':')[1].Trim());
                scannerData.Longitude = ParseNullableDouble(fields[4].Split(':')[1].Trim());
                scannerData.RSSI = int.Parse(fields[5].Split(':')[1].Trim());
                scannerData.SNR = double.Parse(fields[6].Split(':')[1].Trim(), CultureInfo.InvariantCulture);
                //scannerData.TimeStamp = ParseDateTime(fields[7].Split(':')[1].Trim());
            }
            else
            {
                Debug.WriteLine("Insufficient fields in the data string.");
            }

            return scannerData;
        }

        private double? ParseNullableDouble(string value)
        {
            if (double.TryParse(value, out double result))
                return result;

            return null;
        }

    }
}
