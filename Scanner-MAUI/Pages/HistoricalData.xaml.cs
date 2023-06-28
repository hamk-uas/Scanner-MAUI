using Scanner_MAUI.Helpers;
using Scanner_MAUI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;

namespace Scanner_MAUI.Pages;

public partial class HistoricalData : ContentPage
{
    //public ObservableCollection<Network> NetworkNames { get; set; }
    private SerialPortConn scannerConn;


    public HistoricalData()
	{
		InitializeComponent();
        scannerConn = new SerialPortConn();
        Network network = new Network();
        //network.NetworkNames = new ObservableCollection<Network>();
        // Access the NetworkNames property to retrieve the collection of network items
        //ObservableCollection<Network> NetworkNames = network.NetworkNames;
        //BindingContext = this;
        NetworkListView.ItemsSource = scannerConn.NetworkNames;

    }
    private void StartMenuItem_Clicked(object sender, EventArgs e)
    {
        // TODO: Connect to the LoRa scanner and start scanning
        scannerConn.ConnectToScanner();
        //Task.Delay(1000).ContinueWith(task => 
        //{
        //    scannerConn.NetworkNames = new ObservableCollection<Network>();
        //    BindingContext = this;
        //});

    }

    //public void ConnectToScanner()
    //{
    //    NetworkNames.Clear();
    //    try
    //    {
    //        serialPort = new SerialPort("COM6", 115200);

    //        serialPort.Open();
    //        Debug.WriteLine("Serial Port conn created");
    //        Debug.WriteLine("Serial Port Is Open: " + serialPort.IsOpen);

    //        var data = new byte[] { (byte)'1', 13 };
    //        serialPort.Write(data, 0, data.Length);

    //        // Handle the scanner's output and display the information
    //        serialPort.DataReceived += SerialPort_DataReceived;

    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle the exception
    //        Debug.WriteLine($"Failed to connect to the scanner: {ex.Message}");
    //    }
    //}

    //public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    //{
    //    try
    //    {
    //        while (serialPort.BytesToRead > 0)
    //        {
    //            string DataIn = serialPort.ReadLine();
    //            //Debug.WriteLine("Data received from the scanner: " + DataIn);

    //            // Process the received data and extract the fields
    //            Network data = ProcessReceivedData(DataIn);

    //            MainThread.BeginInvokeOnMainThread(() =>
    //            {
    //                if (!string.IsNullOrEmpty(data.Name))
    //                {
    //                    NetworkNames.Add(new Network { Name = data.Name });
    //                }
    //            });
    //        }
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        // Handle the cancellation if needed
    //        Debug.WriteLine("Reading data from the scanner was canceled.");
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle other exceptions
    //        Debug.WriteLine($"Failed to read data from the scanner: {ex.Message}");
    //    }

    //}

    //private Network ProcessReceivedData(string data)
    //{
    //    Network networkData = new Network();

    //    // Parse and extract relevant information from the data string
    //    string[] fields = data.Split(new string[] { ", " }, StringSplitOptions.None);

    //    Debug.WriteLine($"Received data fields count: {fields.Length}");
    //    //Debug.WriteLine($"Received data fields: {string.Join(", ", fields)}");

    //    if (fields.Length >= 8)
    //    {
    //        networkData.Message = fields[0].Split(':')[1].Trim();
    //        string input = fields[1];
    //        string startMarker = "b'";
    //        string endMarker = "'";

    //        int startIndex = input.IndexOf(startMarker) + startMarker.Length;
    //        int endIndex = input.IndexOf(endMarker, startIndex);

    //        if (startIndex >= 0 && endIndex >= 0)
    //        {
    //            string extractedString = input.Substring(startIndex, endIndex - startIndex);
    //            networkData.Name = extractedString;

    //            // Add non-null and non-empty names to the collection
    //            //if (!string.IsNullOrEmpty(scannerData.Name) && !NetworkNames.Contains(extractedString))
    //            //if (!string.IsNullOrEmpty(networkData.Name))
    //            //{
    //            //    NetworkNames.Add(new Network { Name = networkData.Name });
    //            //}
    //            //NetworkNames.Add(scannerData.Name); // Add network name to the list
    //        }
    //        networkData.Type = fields[2].Split(':')[1].Trim();
    //        networkData.Latitude = ParseNullableDouble(fields[3].Split(':')[1].Trim());
    //        networkData.Longitude = ParseNullableDouble(fields[4].Split(':')[1].Trim());
    //        networkData.RSSI = int.Parse(fields[5].Split(':')[1].Trim());
    //        networkData.SNR = double.Parse(fields[6].Split(':')[1].Trim(), CultureInfo.InvariantCulture);
    //        //scannerData.TimeStamp = ParseDateTime(fields[7].Split(':')[1].Trim());
    //    }
    //    else
    //    {
    //        Debug.WriteLine("Insufficient fields in the data string.");
    //    }

    //    return networkData;
    //}

    //private double? ParseNullableDouble(string value)
    //{
    //    if (double.TryParse(value, out double result))
    //        return result;

    //    return null;
    //}

    //public void Disconnect()
    //{
    //    // Stop scanning and close the serial port
    //    if (serialPort != null && serialPort.IsOpen)
    //    {
    //        var data = new byte[] { (byte)'0', 13 };
    //        serialPort.Write(data, 0, data.Length);
    //        //serialPort.WriteLine("0"); // Send the command to stop scanning
    //        serialPort.Close();
    //        Debug.WriteLine("Serial Port conn closed");
    //    }
    //}


    private void StopMenuItem_Clicked(object sender, EventArgs e)
        {
            scannerConn.Disconnect();
        }
}