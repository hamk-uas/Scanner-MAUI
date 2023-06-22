using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Mapping;
using Microsoft.UI.Xaml.Controls;
using Scanner_MAUI.Helpers;
using System.Diagnostics;
using Location = Scanner_MAUI.Helpers.Location;
using Esri.ArcGISRuntime.Maui;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System.Reflection;
using PointerEventArgs = Microsoft.Maui.Controls.PointerEventArgs;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Diagnostics;
using MenuFlyoutItem = Microsoft.Maui.Controls.MenuFlyoutItem;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using Esri.ArcGISRuntime.Data;

namespace Scanner_MAUI.Pages;

public partial class RealTimeData : ContentPage
{
    private GraphicsDrawable graphicsDrawable;
    private Markers mapMarkers;
    //private SerialPortConn scannerData;
    private SerialPort serialPort;


    public RealTimeData()
    {
        InitializeComponent();
        graphicsDrawable = new GraphicsDrawable();
        Canvas.Drawable = graphicsDrawable;
        NetworkListView.ItemSelected += OnNetworkNameSelected;
        TimeStamp.TimeStampViewr(DateTimeLabel);
        _ = ViewMap.LoadWMTSLayer(MyMapView);
        _ = Location.StartDeviceLocationTask(MyMapView); // Gets current location
        //_ = Markers.MapMarkers(MyMapView);
        //scannerConn = new SerialPortConn();
        //scannerConn.ScannerData();
    }

    // Dynamically populating the network name based on the selected network from the list view
    private void OnNetworkNameSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is string selectedNetwork)
        {
            NetworkNameLabel.Text = $"Network Name: {selectedNetwork}";

            // Update the signal strength based on the selected network
            int signalStrength = GetSignalStrength(selectedNetwork);
            graphicsDrawable.SignalStrength = signalStrength;

            // Refresh the Canvas
            Canvas.Invalidate();

            // Create the mapMarkers object and set the longitude and latitude
            mapMarkers = new Markers
            {
                Longitude = GetLongitude(selectedNetwork),
                Latitude = GetLatitude(selectedNetwork)
            };

            //Show the marker location on the map based on the network name
            _ = mapMarkers.MapMarkers(MyMapView);
            MyMapView.GraphicsOverlays.Clear();
        }
    }

    private double GetLongitude(string selectedNetwork)
    {
        if (selectedNetwork == "Network-1")
            return 24.477205; 
        else if (selectedNetwork == "Network-2")
            return 24.477339;
        else if (selectedNetwork == "Network-3")
            return 24.477532;
        else if (selectedNetwork == "Network-4")
            return 24.478310;

        return 0;
    }

    private double GetLatitude(string selectedNetwork)
    {
        if (selectedNetwork == "Network-1")
            return 60.977689;
        else if (selectedNetwork == "Network-2")
            return 60.977603;
        else if (selectedNetwork == "Network-3")
            return 60.977515;
        else if (selectedNetwork == "Network-4")
            return 60.977208;

        return 0;
    }

    private int GetSignalStrength(string networkName)
    {
        if (networkName == "Network-1")
            return 10; // Maximum signal strength
        else if (networkName == "Network-2")
            return 7; // Medium signal strength
        else if (networkName == "Network-3")
            return 4; // Low signal strength
        else if (networkName == "Network-4")
            return 1; // Minimum signal strength

        return 1; // Default signal strength
    }

   
    private void MyLocationButton_Clicked(object sender, EventArgs e)
    {
        // Starts location display with auto pan mode set to Compass Navigation.
        MyMapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.CompassNavigation;

        _ = Location.StartDeviceLocationTask(MyMapView);
    }

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();

    //    // Event handler for the "Start" menu item
    //    var startMenuItem = (MenuFlyoutItem)FindByName("StartMenuItem");
    //    startMenuItem.Clicked += StartMenuItem_Clicked;
    //}

    private void StartMenuItem_Clicked(object sender, EventArgs e)
    {

        // TODO: Connect to the LoRa scanner and start scanning
        ConnectToScanner();

    }

    private void ConnectToScanner()
    {
        try
        {
            serialPort = new SerialPort("COM6", 115200);
            //serialPort.ReadTimeout = 80000;
            //serialPort.WriteTimeout = 80000;
            serialPort.Open();
            Debug.WriteLine("Serial Port conn created");
            Debug.WriteLine("Serial Port Is Open: " + serialPort.IsOpen);

            var data = new byte[] {(byte)'1', 13 };
            serialPort.Write(data, 0, data.Length);
     
            Debug.WriteLine("test");
            // TODO: Handle the scanner's output and display the information in your app
            serialPort.DataReceived += SerialPort_DataReceived;
            Debug.WriteLine("test2");
        }
        catch (Exception ex)
        {
            // Handle the exception
            Debug.WriteLine($"Failed to connect to the scanner: {ex.Message}");
        }
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        //try
        //{
        //    // Handle the received data from the scanner
        //    string data = serialPort.ReadLine();
        //    // Process the data as needed
        //    ProcessReceivedData(data);
        //    Debug.WriteLine("Data received from the scanner: " + data);
        //}
        try
        {
            while (serialPort.BytesToRead > 0)
            {
                string DataIn = serialPort.ReadLine();
                Debug.WriteLine("Data received from the scanner: " + DataIn);
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

    //private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    //{
    //    try
    //    {
    //        // Read the available bytes from the serial port
    //        int bytesToRead = serialPort.BytesToRead;
    //        byte[] buffer = new byte[bytesToRead];
    //        serialPort.Read(buffer, 0, bytesToRead);

    //        // Convert the received bytes to a string
    //        string data = Encoding.ASCII.GetString(buffer);

    //        // Split the data into separate messages
    //        string[] messages = data.Split(new[] { "message: datastearm," }, StringSplitOptions.RemoveEmptyEntries);

    //        // Process each message separately
    //        foreach (string message in messages)
    //        {
    //            // Remove leading and trailing whitespace
    //            string trimmedMessage = message.Trim();

    //            // Process the received data
    //            ProcessReceivedData(trimmedMessage);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle exceptions
    //        Debug.WriteLine($"Failed to read data from the scanner: {ex.Message}");
    //    }
    //}

    private void ProcessReceivedData(string data)
    {
        // Parse and extract relevant information from the data string
        // Example: Parse data and display specific fields in the console
        var fields = data.Split(',');
        if (fields.Length >= 8)
        {
            string name = fields[1].Trim();
            string type = fields[3].Trim();
            double? latitude = ParseNullableDouble(fields[4].Trim());
            double? longitude = ParseNullableDouble(fields[5].Trim());
            int rssi = int.Parse(fields[6].Trim());
            double snr = double.Parse(fields[7].Trim());

            Debug.WriteLine($"Name: {name}, Type: {type}, Latitude: {latitude}, Longitude: {longitude}, RSSI: {rssi}, SNR: {snr}");
        }
        //string[] fields = data.Split(',');

        //if (fields.Length >= 8)
        //{
        //    string name = fields[1].Split(':')[1].Trim();
        //    string type = fields[2].Split(':')[1].Trim();
        //    double? latitude = ParseNullableDouble(fields[3].Split(':')[1].Trim());
        //    double? longitude = ParseNullableDouble(fields[4].Split(':')[1].Trim());
        //    int rssi = int.Parse(fields[5].Split(':')[1].Trim());
        //    double snr = double.Parse(fields[6].Split(':')[1].Trim());

        //    Debug.WriteLine($"Name: {name}, Type: {type}, Latitude: {latitude}, Longitude: {longitude}, RSSI: {rssi}, SNR: {snr}");
        //}
    }

    private double? ParseNullableDouble(string value)
    {
        if (double.TryParse(value, out double result))
            return result;

        return null;
    }

    private void StopMenuItem_Clicked(object sender, EventArgs e)
    {
        // Stop scanning and close the serial port
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine("0"); // Send the command to stop scanning
            serialPort.Close();
            Debug.WriteLine("Serial Port conn closed");
        }
    }

    //private async void StartMenuItem_Clicked(object sender, EventArgs e)
    //{
    //    scannerData = new SerialPortConn();
    //    await Task.Run(() =>
    //    {
    //        scannerData.ScannerData();
    //    });

    //    //scannerData.SendInput(1);
    //}
}

