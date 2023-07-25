using Esri.ArcGISRuntime.UI;
using Scanner_MAUI.Helpers;
using Scanner_MAUI.Model;
using Location = Scanner_MAUI.Helpers.Location;

namespace Scanner_MAUI.Pages;

public partial class HistoricalData : ContentPage
{
    private SDCard scannerConn;
    private PopulateTable pg;
    private TableSection tableSection;

    public HistoricalData()
	{
		InitializeComponent();
        NetworkListView.ItemSelected += OnNetworkNameSelected;
        scannerConn = new SDCard();
        NetworkListView.ItemsSource = scannerConn.NetworkNames;
        _ = ViewMap.LoadWMTSLayer(MyMapView);
        _ = Location.StartDeviceLocationTask(MyMapView); // Gets current location
        tableSection = TableView.Root[0];
        pg = new PopulateTable();
    }

    private void MyLocationButton_Clicked(object sender, EventArgs e)
    {
        // Starts location display with auto pan mode set to Compass Navigation.
        MyMapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.CompassNavigation;

        _ = Location.StartDeviceLocationTask(MyMapView);
    }

    private void OnNetworkNameSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Network selectedNetwork)
        {
            // Get the existing TableSection (assuming it's the first one in the TableRoot)
            
            pg.populateTable(scannerConn.SDContent, tableSection);
        }
    }

    private void StartMenuItem_Clicked(object sender, EventArgs e)
    {
        // TODO: Connect to the LoRa scanner and start scanning
        scannerConn.ConnectToSD();
    }

    private void StopMenuItem_Clicked(object sender, EventArgs e)
    {
        scannerConn.Disconnect();
        //tableSection.Clear();
        //pg.showHeader(tableSection);
    }
}