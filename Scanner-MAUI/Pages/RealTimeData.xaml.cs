using Esri.ArcGISRuntime.UI;
using Scanner_MAUI.Helpers;
using Location = Scanner_MAUI.Helpers.Location;
using Scanner_MAUI.Model;
using System.Diagnostics;
using Syncfusion.Maui.Gauges;
using System.Globalization;

namespace Scanner_MAUI.Pages;

public partial class RealTimeData : ContentPage
{

    private GraphicsDrawable graphicsDrawable;
    private Markers mapMarkers;
    private SerialPortConn scannerConn;
    private Dictionary<string, string> RssiValues;

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

            NetworkRSSILabel.Text = $"RSSI Value:  {rssi}db";
            SignalStrengthPercentage.Text = $"Signal Strength %:  {signalStrength}%";

            RssiLabel.Text = $"RSSI Values:  {string.Join("db,\n ", RssiValues.Keys)}db";
            SignalStrengthPercentages.Text = $"Signal Strength %s:  {string.Join("%,\n ", RssiValues.Values)}%";

            Type.Text = $"Type:  {scannerConn.Type}";

            Latitude.Text = $"Latitude:  {scannerConn.Lat}";
            Longitude.Text = $"Longitude:  {scannerConn.Lon}";


            mapMarkers.Longitude = scannerConn.Lon;
            mapMarkers.Latitude = scannerConn.Lat;

            //Show the marker location on the map based on the network name
            _ = mapMarkers.MapMarkers(MyMapView);
            MyMapView.GraphicsOverlays.Clear();


            Debug.WriteLine("Signal strength % = " + signalStrength);

            rangePointer.Value = signalStrength;
            Number.Text = signalStrength.ToString() + "%";
            //needlePointer.Value = signalStrength;

            needlePointer2.Value = rssi;
            rangePointer2.Value = rssi;
            Number2.Text = "RSSI: " + rssi.ToString() + "db";
        }
    }

    // Dynamically populating the network name based on the selected network from the list view
    private void OnNetworkNameSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Network selectedNetwork)
        {
            NetworkNameLabel.Text = $"Network Name:  {selectedNetwork.Name}";

            //Show the marker location on the map based on the network name
            mapMarkers = new Markers
            {
                Longitude = scannerConn.Lon,
                Latitude = scannerConn.Lat
            };

            _ = mapMarkers.MapMarkers(MyMapView);
            MyMapView.GraphicsOverlays.Clear();

            scannerConn.PropertyChanged += ScannerConn_PropertyChanged;

            Canvas.Invalidate();

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

    private void StartMenuItem_Clicked(object sender, EventArgs e)
    {
        // TODO: Connect to the LoRa scanner and start scanning
        scannerConn.ConnectToScanner();
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

        _ = mapMarkers.MapMarkers(MyMapView);

    }
}

