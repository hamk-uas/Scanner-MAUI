using System.Globalization;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Scanner_MAUI.Model;
using System.ComponentModel;

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
                }
                //else if (networkData.Lat == 0 || networkData.Lon == 0 || (networkData.Lat == 0 && networkData.Lon == 0))
                //{
                //    networkData.Lat = 60.996010;
                //    networkData.Lon = 24.464230;
                //}

                networkData.RSSI = int.Parse(fields[5].Split(':')[1].Trim());
                //networkData.SNR = double.Parse(fields[6].Split(':')[1].Trim(), CultureInfo.DefaultThreadCurrentCulture);
                networkData.SNR = double.Parse(fields[6].Split(':')[1].Trim(), CultureInfo.InvariantCulture);
                //scannerData.TimeStamp = ParseDateTime(fields[7].Split(':')[1].Trim());
            }
            else
            {
                Debug.WriteLine("Insufficient fields in the data string.");
            }

            return networkData;
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
