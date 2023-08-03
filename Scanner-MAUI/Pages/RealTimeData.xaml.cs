using Esri.ArcGISRuntime.UI;
using Scanner_MAUI.Helpers;
using Location = Scanner_MAUI.Helpers.Location;
using Scanner_MAUI.Model;
using System.Diagnostics;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;

namespace Scanner_MAUI.Pages;

public partial class RealTimeData : ContentPage
{

    private GraphicsDrawable graphicsDrawable;
    private Markers mapMarkers;
    private SerialPortConn scannerConn;
    private Dictionary<string, string> RssiValues;
    private ObservableCollection<ObservableValue> _observableValues;
    public ObservableCollection<ISeries> Series { get; set; }
    private Dictionary<string, int> BaudRates;
    private Dictionary<string, string> COM;

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

        mapMarkers = new Markers();

        scannerConn = new SerialPortConn();
        NetworkListView.ItemsSource = scannerConn.NetworkNames;
        RssiValues = new Dictionary<string, string>();
        _observableValues = new ObservableCollection<ObservableValue>();
        CartesianChart.Title = new LabelVisual
        {
            Text = "SNR Chart",
            TextSize = 20,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };
        BaudRates = new Dictionary<string, int>();
        COM = new Dictionary<string, string>();
    }

    private void ScannerConn_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(scannerConn.Strength))
        {
            //RssiValues = new Dictionary<string, string>();
            int rssi = scannerConn.Strength;
            int signalStrength = ConvertRSSIToSignalStrength(rssi);

            RssiValues[rssi.ToString()] = signalStrength.ToString();

            graphicsDrawable.SignalStrength = signalStrength;
            Canvas.Invalidate();

            NetworkRSSILabel.Text = $"RSSI Value:  {rssi}dB";
            SignalStrengthPercentage.Text = $"Signal Strength %:  {signalStrength}%";

            RssiLabel.Text = $"RSSI Values:  {string.Join("dB,\n ", RssiValues.Keys)}dB";
            SignalStrengthPercentages.Text = $"Signal Strength %s:  {string.Join("%,\n ", RssiValues.Values)}%";

            Type.Text = $"Type:  {scannerConn.Type}";

            Latitude.Text = $"Latitude:  {scannerConn.Lat}";
            Longitude.Text = $"Longitude:  {scannerConn.Lon}";

            //Show the marker location on the map based on the network name
            mapMarkers.Longitude = scannerConn.Lon;
            mapMarkers.Latitude = scannerConn.Lat;
            _ = mapMarkers.MapMarkers(MyMapView);
            //MyMapView.GraphicsOverlays.Clear();

            Debug.WriteLine("Signal strength % = " + signalStrength);

            rangePointer.Value = signalStrength;
            Number.Text = signalStrength.ToString() + "%";
            //needlePointer.Value = signalStrength;

            needlePointer2.Value = rssi;
            rangePointer2.Value = rssi;
            Number2.Text = "RSSI: " + rssi.ToString() + "dB";

            double snr = scannerConn.SNR;
            _observableValues.Add(new (snr));
            MyMapView.GraphicsOverlays.Clear();

            DateTimeLabel.Text = $"Date and Time: {scannerConn.Datetime}";

        }
    }

    // Dynamically populating the network name based on the selected network from the list view
    private void OnNetworkNameSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Network selectedNetwork)
        {
            NetworkNameLabel.Text = $"Network Name:  {selectedNetwork.Name}";
            

            //Show the marker location on the map based on the network name
            //mapMarkers = new Markers
            //{
            //    Longitude = scannerConn.Lon,
            //    Latitude = scannerConn.Lat
            //};

            //_ = mapMarkers.MapMarkers(MyMapView);
            //MyMapView.GraphicsOverlays.Clear();


            scannerConn.PropertyChanged += ScannerConn_PropertyChanged;

            Canvas.Invalidate();
            
            CartesianChart.Series = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values = _observableValues,
                    Fill = null,
                    Name = "SNR Value"
                }
            };
            MyMapView.GraphicsOverlays.Clear();


        }
    }

    private int ConvertRSSIToSignalStrength(int rssi)
    {
        // RSSI ranges from -110 to 5 and signal strength ranges from 0 to 100
        int signalStrength = (rssi - (-110)) * 100 / (5 - (-110)); //or
        //int signalStrength = (rssi + 110) * 100 / (115);
        signalStrength = Math.Max(0, Math.Min(100, signalStrength)); // Ensure the signal strength is within the valid range

        return signalStrength;
    }

    private void MyLocationButton_Clicked(object sender, EventArgs e)
    {
        // Starts location display with auto pan mode set to Compass Navigation.
        MyMapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.CompassNavigation;

        _ = Location.StartDeviceLocationTask(MyMapView);
    }

    private async void StartMenuItem_Clicked(object sender, EventArgs e)
    {
        if (BaudRates.Count > 0 && COM.Count > 0)
        {
            int firstBaudRate = BaudRates.Values.First();
            string firstComValue = COM.Values.First();
            try
            {
                scannerConn.ConnectToScanner(firstBaudRate, firstComValue);
            }
            catch (Exception ex)
            {
                 await DisplayAlert("Alert", ex.ToString(), "OK");
            }
        }
        else
        {
            BaudRates.Clear();
            COM.Clear();
            BaudRate.Text = "Selected Baud Rate: ";
            COMNumber.Text = "Selected COM Port Number: ";
            await DisplayAlert("Alert", "Please select a baud rate and a com port number", "OK");
        }        
    }

    private void StopMenuItem_Clicked(object sender, EventArgs e)
    {
        scannerConn.Disconnect();
        RssiValues.Clear();
        NetworkNameLabel.Text = $"Network Name:  ";
        NetworkRSSILabel.Text = $"RSSI Value:  ";
        SignalStrengthPercentage.Text = $"Signal Strength %:  ";

        RssiLabel.Text = $"RSSI Values:  ";
        SignalStrengthPercentages.Text = $"Signal Strength %s:  ";

        Type.Text = $"Type:  ";

        Latitude.Text = $"Latitude:  ";
        Longitude.Text = $"Longitude:  ";

        MyMapView.GraphicsOverlays.Clear();
        
        _observableValues.Clear();
        BaudRates.Clear();
        COM.Clear();

    }

    private void BaudRate110_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 110;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate300_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 300;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate600_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 600;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate1200_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 1200;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate2400_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 2400;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate4800_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 4800;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate9600_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 9600;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate19200_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 19200;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate28800_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 28800;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate38400_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 38400;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate57600_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 57600;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate76800_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 76800;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate115200_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 115200;
        BaudRates["baudrate13"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate230400_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 230400;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }
    private void BaudRate460800_Clicked(object sender, EventArgs e)
    {
        BaudRates.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        int baudRate = 460800;
        BaudRates["baudrate"] = baudRate;
        BaudRate.Text = "Selected Baud Rate: " + baudRate;
    }


    private void Com1_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "1";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com2_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "2";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com3_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "3";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com4_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "4";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com5_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "5";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com6_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "6";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com7_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "7";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com8_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "8";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com9_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "9";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
    private void Com10_Clicked(object sender, EventArgs e)
    {
        COM.Clear();
        // TODO: Connect to the LoRa scanner and start scanning
        string com = "10";
        COM["com"] = com;
        COMNumber.Text = "Selected COM Port Number: " + com;
    }
}

