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

                DateTime referenceDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                int year3 = referenceDate.Year;
                int month3 = referenceDate.Month;
                int day3 = referenceDate.Day;
                int hour3 = referenceDate.Hour;
                int minute3 = referenceDate.Minute;
                int second3 = referenceDate.Second;
                int milliseconds = referenceDate.Millisecond;

                string formattedString3 = $"time: ({year3}";
                fields[7] = formattedString3;

                string formattedstring4 = $"{month3}";
                fields[8] = formattedstring4;

                string formattedstring5 = $"{day3}";
                fields[9] = formattedstring5;

                string formattedstring6 = $"{hour3}";
                fields[10] = formattedstring6;

                string formattedstring7 = $"{minute3}";
                fields[11] = formattedstring7;

                string formattedstring8 = $"{second3}";
                fields[12] = formattedstring8;

                string formattedstring9 = $"{milliseconds}";
                fields[13] = formattedstring9;

                string updatedDatastream = string.Join(", ", fields);
;
                //string updatedDatastream2 = string.Join(", ", fields);
                serialPort.WriteLine(updatedDatastream);
                networkData.Timestamp = referenceDate;

                //// Extract the timestamp tuple from the data stream
                //int startIndex1 = data.IndexOf("time: (") + 7;
                //int endIndex1 = data.IndexOf(", None)");
                //string timestampStr = data.Substring(startIndex1, endIndex1 - startIndex1);


                //// Split the timestamp string by commas and convert to integers
                //string[] timestampParts = timestampStr.Split(',');
                //int year = int.Parse(timestampParts[0].Trim());
                //int month = int.Parse(timestampParts[1].Trim());
                //int day = int.Parse(timestampParts[2].Trim());
                //int hour = int.Parse(timestampParts[3].Trim());
                //int minute = int.Parse(timestampParts[4].Trim());
                //int second = int.Parse(timestampParts[5].Trim());
                //int rtc = int.Parse(timestampParts[6].Trim());

                //DateTime referenceDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                //DateTime dateTime = referenceDate.AddMilliseconds(rtc);

                //timestampParts[6] = dateTime.ToString();

                //timestampStr = timestampParts[6];


                //// Parse the string into a DateTime object
                //DateTime dateTime2 = DateTime.ParseExact(timestampStr, "d.M.yyyy H.m.s", null);
                //// Extract individual components
                //int year2 = dateTime2.Year;
                //int month2 = dateTime2.Month;
                //int day2 = dateTime2.Day;
                //int hour2 = dateTime2.Hour;
                //int minute2 = dateTime2.Minute;
                //int second2 = dateTime2.Second;
                //int millisecond2 = dateTime2.Millisecond;
                //string formattedString = $"{year2}, {month2}, {day2}, {hour2}, {minute2}, {second2}, {millisecond2}";

                ////fields[7].Replace(" time: (2023, 6, 15, 17, 30, 17, 845006, None)", "");
                //fields[7] = "time: (" + formattedString + endIndex1;
                //fields[8].Remove(0);
                //fields[9].Remove(0);
                //fields[10].Remove(0);
                //fields[11].Remove(0);
                //fields[12].Remove(0);
                //fields[13].Remove(0);
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
