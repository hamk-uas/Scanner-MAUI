using Esri.ArcGISRuntime.UI;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Scanner_MAUI.Helpers;
using Scanner_MAUI.Model;
using SkiaSharp;
using System.Collections.ObjectModel;
using Location = Scanner_MAUI.Helpers.Location;

namespace Scanner_MAUI.Pages;

public partial class HistoricalData : ContentPage
{
    private SDCard scannerConn;
    private PopulateTable pg;
    private TableSection tableSection;
    private ObservableCollection<ObservableValue> _observableValues;
    private ObservableCollection<ObservableValue> _observableValues2;
    //private ObservableCollection<ISeries> Series { get; set; }
    private HeatMap heatMap;
    private Markers mapMarkers;

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
        _observableValues = new ObservableCollection<ObservableValue>();
        _observableValues2 = new ObservableCollection<ObservableValue>();
        CartesianChart.Title = new LabelVisual
        {
            Text = "SNR Chart",
            TextSize = 20,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };
        CartesianChart2.Title = new LabelVisual
        {
            Text = "RSSI Chart",
            TextSize = 20,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };
        heatMap = new HeatMap();
        mapMarkers = new Markers();
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
            pg.WriteNetworkDataToCSV(scannerConn.SDContent);
            foreach (Network network in scannerConn.SDContent)
            {
                double snr = network.SNR;
                int rssi = network.RSSI;
                _observableValues.Add(new(snr));
                _observableValues2.Add(new(rssi));
                //mapMarkers.Longitude = network.Lon;
                //mapMarkers.Latitude = network.Lat;
                
                //mapMarkers.Longitude = network.Lon;
                //mapMarkers.Latitude = network.Lat;
                //_ = mapMarkers.MapMarkers(MyMapView);

                heatMap.Longitude = network.Lon;
                heatMap.Latitude = network.Lat;
                heatMap.RSSI = rssi;
                _ = heatMap.HeatMarkers(MyMapView);
            }
            CartesianChart.Series = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values =  _observableValues,
                    Fill = null,
                    Name = "SNR Value"
                }
            };

            CartesianChart2.Series = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values =  _observableValues2,
                    Fill = null,
                    Name = "RSSI Value"
                }
            };
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

    private void ClearSd_Clicked(object sender, EventArgs e)
    {
        scannerConn.ClearSD();
        //tableSection.Clear();
        //pg.showHeader(tableSection);
    }
}