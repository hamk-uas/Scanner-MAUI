using System.Globalization;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Scanner_MAUI.Model;
using System.ComponentModel;
using System.Text;

namespace Scanner_MAUI.Helpers
{

    public class SerialPortConn : INotifyPropertyChanged
    { 
        //public ObservableCollection<Network> NetworkNames { get; set; }
        private SerialPort serialPort;
        Network networkNames = new Network();
        public ObservableCollection<Network> NetworkNames { get; set; } = new ObservableCollection<Network>();
        //public ObservableCollection<Network> NetworkSnrValues { get; set; } = new ObservableCollection<Network>();
        private Network ExistingNetwork { get; set; }
        //public int strength {  get; set; }
        private int strength;

        public int Strength
        {
            get { return strength; }
            //get => strength;
            set
            {
                if (strength != value)
                {
                    strength = value;
                    OnPropertyChanged(nameof(Strength));
                    //OnPropertyChanged();
                }
            }
        }
        private string type;

        public string Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }
        private double lat;

        public double Lat
        {
            get { return lat; }
            set
            {
                if (lat != value)
                {
                    lat = value;
                    OnPropertyChanged(nameof(Lat));
                }
            }
        }

        private double lon;

        public double Lon
        {
            get { return lon; }
            set
            {
                if (lon != value)
                {
                    lon = value;
                    OnPropertyChanged(nameof(Lon));
                }
            }
        }

        private double snr;

        public double SNR
        {
            get { return snr; }
            set
            {
                if (snr != value)
                {
                    snr = value;
                    OnPropertyChanged(nameof(SNR));
                }
            }
        }

        private DateTime datetime;

        public DateTime Datetime
        {
            get { return datetime; }
            set
            {
                if (datetime != value)
                {
                    datetime = value;
                    OnPropertyChanged(nameof(Datetime));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ConnectToScanner()
        {
            NetworkNames.Clear();
            //NetworkSnrValues.Clear();
            Strength = -110;
            try
            {
                serialPort = new SerialPort("COM6", 115200);

                serialPort.Open();
                Debug.WriteLine("Serial Port conn created");
                Debug.WriteLine("Serial Port Is Open: " + serialPort.IsOpen);

                var data = new byte[] { (byte)'1', 13 };
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
                                if(data.Name == ExistingNetwork.Name)
                                {
                                    Strength = data.RSSI;
                                    Type = data.Type;
                                    Lat = data.Lat;
                                    Lon = data.Lon;
                                    SNR = data.SNR;
                                    Datetime = data.Timestamp; 
                                    ExistingNetwork.RSSI = data.RSSI;
                                    ExistingNetwork.Name = data.Name;

                                    //NetworkNames.Add(new Network { Name = data.Name });
                            
                                    Debug.WriteLine("Strength: " + Strength);
                                    Debug.WriteLine("ExistingNetwork: " + ExistingNetwork.Name + " " + ExistingNetwork.RSSI);
                                    Debug.WriteLine("coords: " + Lat + " " + Lon);
                                    Debug.WriteLine("snr: " + data.SNR);

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
                        }
                    });

                    Debug.WriteLine(NetworkNames.Count);

                    if (NetworkNames.Count >= 12 || NetworkNames.Count >= 14)
                    {
                        // Close the serial port
                        Disconnect();
                    }
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

            // Parse and extract relevant information from the data string
            string[] fields = data.Split(new string[] { ", " }, StringSplitOptions.None);

            Debug.WriteLine($"Received data fields count: {fields.Length}");
            //Debug.WriteLine($"Received data fields: {string.Join(", ", fields)}");

            if (fields.Length >= 8)
            {
                networkData.Message = fields[0].Split(':')[1].Trim();
                string input = fields[1];
                string startMarker = "b'";
                string endMarker = "'";

                int startIndex = input.IndexOf(startMarker) + startMarker.Length;
                int endIndex = input.IndexOf(endMarker, startIndex);

                if (startIndex >= 0 && endIndex >= 0)
                {
                    string extractedString = input.Substring(startIndex, endIndex - startIndex);
                    networkData.Name = extractedString;

                }
                networkData.Type = fields[2].Split(':')[1].Trim();
               
                //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                string latString = fields[3].Split(':')[1].Trim();
                Debug.WriteLine("latstring :" + latString);
                if (double.TryParse(latString, NumberStyles.Float, CultureInfo.InvariantCulture, out double latValue))
                {
                    Debug.WriteLine("latvalue: " + latValue);
                    networkData.Lat = latValue;
                    Debug.WriteLine("netLat: " +  networkData.Lat);
                }
                else
                {
                    // Handle the error when parsing latitude fails
                    //networkData.Lat = 0; // Assign a default value or handle the error as needed
                    networkData.Lat = 60.996010;
                    //double desiredLatitude= 60.996018;
                    //latString = $" {desiredLatitude}";
                    //fields[3] = "lat: " + latString;
                    //string updatedDatastream = string.Join(", ", fields);
                    //serialPort.WriteLine(updatedDatastream);
                }

                string lonString = fields[4].Split(':')[1].Trim();
                if (double.TryParse(lonString, NumberStyles.Any, CultureInfo.InvariantCulture, out double lonValue))
                {
                    networkData.Lon = lonValue;
                }
                else
                {
                    // Handle the error when parsing longitude fails
                    //networkData.Lon = 0; // Assign a default value or handle the error as needed
                    networkData.Lon = 24.464230;
                    double desiredLongitude = 24.464238;
                    lonString = $" {desiredLongitude}";
                    fields[4] = "lon: " + lonString;
                    string updatedDatastream3 = string.Join(", ", fields);
                    serialPort.WriteLine(updatedDatastream3);
                }

                networkData.RSSI = int.Parse(fields[5].Split(':')[1].Trim());
                //networkData.SNR = double.Parse(fields[6].Split(':')[1].Trim(), CultureInfo.DefaultThreadCurrentCulture);
                networkData.SNR = double.Parse(fields[6].Split(':')[1].Trim(), CultureInfo.InvariantCulture);

                //double desiredLongitude = 24.464238;
                //lonString = $" {desiredLongitude}";
                //fields[4] = "lon: " + lonString;
                //string updatedDatastream = string.Join(", ", fields);
                //serialPort.WriteLine(updatedDatastream);

                // Extract the timestamp tuple from the data stream
                int startIndex1 = data.IndexOf("time: (") + 7;
                int endIndex1 = data.IndexOf(", None)");
                string timestampStr = data.Substring(startIndex1, endIndex1 - startIndex1);

                int streamI1 = data.IndexOf("message");
                int streamI2 = data.IndexOf(", time:");
                string streamNoTime = data.Substring(streamI1, streamI2 - streamI1);
                // Split the timestamp string by commas and convert to integers
                string[] timestampParts = timestampStr.Split(',');
                int year = int.Parse(timestampParts[0].Trim());
                int month = int.Parse(timestampParts[1].Trim());
                int day = int.Parse(timestampParts[2].Trim());
                int hour = int.Parse(timestampParts[3].Trim());
                int minute = int.Parse(timestampParts[4].Trim());
                int second = int.Parse(timestampParts[5].Trim());
                int rtc = int.Parse(timestampParts[6].Trim());

                DateTime referenceDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                DateTime dateTime = referenceDate.AddMilliseconds(rtc);
                timestampParts[6] = dateTime.ToString();
                foreach(var value in timestampParts)
                {
                   
                }
                string date = fields[7].Split(':')[1].Trim();
                string updatedDatastream = string.Join(", ", timestampParts);
                string updatedDatastream2 = string.Join(", ", fields);
                serialPort.WriteLine(updatedDatastream);
                networkData.Timestamp = dateTime;
                //Debug.WriteLine("rtc " + rtc);
                //string timeField = timestampStr;
                //DateTime currentTime = DateTime.Now;
                //year = currentTime.Year;
                //month = currentTime.Month;
                //day = currentTime.Day;
                //hour = currentTime.Hour;
                //minute = currentTime.Minute;
                //second = currentTime.Second;
                //rtc = currentTime.Millisecond;

                //// Create a new timestamp string with the current time values
                //string updatedTimestamp = $"({year}, {month}, {day}, {hour}, {minute}, {second}, {rtc}, None)";

                //fields[7] = streamNoTime + ", time: " + updatedTimestamp;
                //// Replace the original timestamp in the datastream with the updated one
                ////string updatedDatastream = data.Replace(data.Split(new[] { "time:" }, StringSplitOptions.None)[1].Split(',')[0].Trim(), updatedTimestamp);
                //string updatedDatastream = fields[7];
                //// Send the updated datastream back to the scanner through the serial port
                //serialPort.Write(updatedDatastream);
                ////networkData.Timestamp = dateTime;
                //Debug.WriteLine("Timestamp " + networkData.Timestamp);

                //scannerData.TimeStamp = ParseDateTime(fields[7].Split(':')[1].Trim());
            }
            else
            {
                Debug.WriteLine("Insufficient fields in the data string.");
            }

            return networkData;
        }

        private static long ConvertToUnixTimestamp(DateTime dateTime)
        {
            // Unix timestamp starts from January 1, 1970 (UTC)
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Calculate the number of seconds between the given DateTime and Unix Epoch
            TimeSpan timeSpan = dateTime.ToUniversalTime() - unixEpoch;

            // Return the total seconds (Unix timestamp)
            return (long)timeSpan.TotalSeconds;
        }

        private double? ParseNullableDouble(string value)
        {
            if (double.TryParse(value, out double result))
                return result;

            return 0;
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
                Strength = -110;
                Debug.WriteLine("Serial Port conn closed");
            }
        }

       
    }
}
