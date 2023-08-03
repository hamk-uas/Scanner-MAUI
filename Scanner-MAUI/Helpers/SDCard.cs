using Microsoft.Maui.Controls;
using Scanner_MAUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner_MAUI.Helpers
{
    public class SDCard
    {
        private SerialPort serialPort;
        public ObservableCollection<Network> NetworkNames { get; set; } = new ObservableCollection<Network>();
        public ObservableCollection<Network> SDContent { get; set; } = new ObservableCollection<Network>();
        private Network ExistingNetwork { get; set; }

        public void ConnectToSD()
        {
            NetworkNames.Clear();
            SDContent.Clear();
            try
            {
                serialPort = new SerialPort("COM6", 115200);

                serialPort.Open();
                Debug.WriteLine("Serial Port conn created");
                Debug.WriteLine("Serial Port Is Open: " + serialPort.IsOpen);

                var data = new byte[] { (byte)'3', 13 };
                serialPort.Write(data, 0, data.Length);

                // Handle the scanner's output and display the information
                serialPort.DataReceived += SerialPort_DataReceived;

            }
            catch (Exception ex)
            {
                // Handle the exception
                Debug.WriteLine($"Failed to connect to the scanner: {ex.Message}");
            }
        }

        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    string DataIn = serialPort.ReadLine();
                    Debug.WriteLine("Data received from the scanner: " + DataIn);
                    //Task.Delay(1000).Wait();   
                    // Process the received data and extract the fields
                    Network data = ProcessReceivedData(DataIn);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Check if the data name is not empty or null
                        if (!string.IsNullOrEmpty(data.Name))
                        {
                            // Find the existing network with the same name, if it exists
                            ExistingNetwork = NetworkNames.FirstOrDefault(network => network.Name == data.Name);

                            if (ExistingNetwork != null)
                            {
                                if (data.Name == ExistingNetwork.Name)
                                {
                                    SDContent.Add(new Network { 
                                                        Name = data.Name,
                                                        Type = data.Type,
                                                        Lat = data.Lat,
                                                        Lon = data.Lon,
                                                        RSSI = data.RSSI,
                                                        SNR = data.SNR,
                                                        Timestamp = data.Timestamp,
                                                            });
                                    Debug.WriteLine("ExistingNetwork: " + ExistingNetwork.Name + " " + ExistingNetwork.RSSI);
                                    Debug.WriteLine("Type: " + data.Type);
                                }

                            }
                            else
                            {
                                // Create a new network with the name and initialize its RSSIList with the current RSSI value
                                NetworkNames.Add(new Network { Name = data.Name });
                            }

                            foreach (Network network in NetworkNames)
                            {
                                Debug.WriteLine("NetworkName: " + network.Name + " " + network.RSSI);
                            }
                            foreach (Network network in SDContent)
                            {
                                Debug.WriteLine("SDContent: " + network.Name + " " 
                                    + network.Type + " " 
                                     + network.Lat + " "
                                     + network.Lon + " "
                                     + network.RSSI + " "
                                     + network.SNR + " "
                                     + network.Timestamp
                                    );
                            }
                        }
                    });

                    Debug.WriteLine(NetworkNames.Count);

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

        private Network ProcessReceivedData(string data)
        {
            Network networkData = new Network();

            // Remove the leading 'b' and trailing whitespaces from the data string
            data = data.Trim('b').Trim();

            // Parse and extract relevant information from the data string
            string[] fields = data.Split(new string[] { "," }, StringSplitOptions.None);

            Debug.WriteLine($"Received data fields count: {fields.Length}");
            //Debug.WriteLine($"Received data fields: {string.Join(", ", fields)}");

            if (fields.Length >= 8)
            {
                networkData.Name = fields[0].Trim('\'');
                networkData.Type = fields[1].Trim();

                // The latitude and longitude information can be parsed directly without extra handling
                if (double.TryParse(fields[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double latValue))
                {
                    networkData.Lat = latValue;
                }
                else
                {
                    // Handle the error when parsing latitude fails
                    //networkData.Lat = 0; // Assign a default value or handle the error as needed
                    networkData.Lat = 60.996010;
                }


                if (double.TryParse(fields[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double lonValue))
                {
                    networkData.Lon = lonValue;
                }
                else
                {
                    // Handle the error when parsing longitude fails
                    //networkData.Lon = 0; // Assign a default value or handle the error as needed
                    networkData.Lon = 24.464230;
                }

                if (int.TryParse(fields[4], out int rssiValue))
                {
                    networkData.RSSI = rssiValue;
                }

                if (double.TryParse(fields[5], NumberStyles.Float, CultureInfo.InvariantCulture, out double snrValue))
                {
                    networkData.SNR = snrValue;
                }

                DateTime referenceDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                string rtc = fields[12].Trim();


                long rtc2 = long.Parse(rtc);
                long ticks = rtc2 * 10;
                //DateTime dateTime = new DateTime(ticks);
                DateTime dateTime = referenceDate.AddMicroseconds(ticks);

                networkData.Timestamp = dateTime;

            }
            else
            {
                Debug.WriteLine("Insufficient fields in the data string.");
            }

            return networkData;
        }

        public void Disconnect()
        {
            // Stop scanning and close the serial port
            if (serialPort != null && serialPort.IsOpen)
            {
                var data = new byte[] { (byte)'0', 13 };
                serialPort.Write(data, 0, data.Length);
                //serialPort.WriteLine("0"); // Send the command to stop scanning
                serialPort.Close();
                NetworkNames.Clear();
                SDContent.Clear();
                Debug.WriteLine("Serial Port conn closed");
            }
        }

        public async void ClearSD()
        {
            // Stop scanning and close the serial port
            if (serialPort != null && serialPort.IsOpen)
            {
                var data = new byte[] { (byte)'4', 13 };
                serialPort.Write(data, 0, data.Length);
                //serialPort.WriteLine("0"); // Send the command to stop scanning
                NetworkNames.Clear();
                SDContent.Clear();
                Debug.WriteLine("SD card Cleared");
                await Application.Current.MainPage.DisplayAlert("Alert", "The SD card has been cleard", "Close");
            }
        }

    }

}
