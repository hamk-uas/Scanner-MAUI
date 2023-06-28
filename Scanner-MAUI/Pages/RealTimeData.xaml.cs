using Esri.ArcGISRuntime.UI;
using Scanner_MAUI.Helpers;
using Location = Scanner_MAUI.Helpers.Location;
using System.IO.Ports;
using Scanner_MAUI.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.System;

namespace Scanner_MAUI.Pages;

public partial class RealTimeData : ContentPage
{
    private GraphicsDrawable graphicsDrawable;
    private Markers mapMarkers;
    private SerialPortConn scannerConn;

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
        scannerConn = new SerialPortConn();
        NetworkListView.ItemsSource = scannerConn.NetworkNames;
        
        //BindingContext = this;
    }

    // Dynamically populating the network name based on the selected network from the list view
    private void OnNetworkNameSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Network selectedNetwork)
        {
            NetworkNameLabel.Text = $"Network Name: {selectedNetwork.Name}";

            // Update the signal strength based on the selected network
            int signalStrength = GetSignalStrength(selectedNetwork.Name);
            graphicsDrawable.SignalStrength = signalStrength;

            // Refresh the Canvas
            Canvas.Invalidate();
            
            // Create the mapMarkers object and set the longitude and latitude
            mapMarkers = new Markers
            {
                Longitude = GetLongitude(selectedNetwork.Name),
                Latitude = GetLatitude(selectedNetwork.Name)
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

    private void StartMenuItem_Clicked(object sender, EventArgs e)
    {
        // TODO: Connect to the LoRa scanner and start scanning
        scannerConn.ConnectToScanner();
    }

    private void StopMenuItem_Clicked(object sender, EventArgs e)
    {
        scannerConn.Disconnect();
    }
}

