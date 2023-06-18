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

namespace Scanner_MAUI.Pages;

public partial class RealTimeData : ContentPage
{
    private GraphicsDrawable graphicsDrawable;
    private Markers mapMarkers;

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
        //mapMarkers = new Markers();
        //_ = mapMarkers.MapMarkers(MyMapView);
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

            // Show the marker location on the map based on the network name
            //Markers.MapMarkers(MyMapView, selectedNetwork);

            //MyMapView.GraphicsOverlays.Clear();

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
}

