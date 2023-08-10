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
using System.Diagnostics;
using Location = Scanner_MAUI.Helpers.Location;

namespace Scanner_MAUI.Pages;

public partial class HistoricalData : ContentPage
{
    private ReadCSV readCSV;
    private SDCard scannerConn;
    private PopulateTable pg;
    private TableSection tableSection;
    private MapWebView mapWebView;
    private ObservableCollection<ObservableValue> _observableValues;
    private ObservableCollection<ObservableValue> _observableValues2;
    //private ObservableCollection<ISeries> Series { get; set; }
    private Markers mapMarkers;

    public HistoricalData()
	{
		InitializeComponent();
        NetworkListView.ItemSelected += OnNetworkNameSelected;
        scannerConn = new SDCard();
        mapMarkers = new Markers();
        readCSV = new ReadCSV();
        mapWebView = new MapWebView();
        NetworkListView.ItemsSource = scannerConn.NetworkNames;
       
        //_ = ViewMap.LoadWMTSLayer(MyMapView); //Maanmittauslaitos WMTS layer
        _ = ViewMap.OpenstreetMaps(MyMapView);
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

                mapMarkers.Longitude = network.Lon;
                mapMarkers.Latitude = network.Lat;

                mapMarkers.Longitude = network.Lon;
                mapMarkers.Latitude = network.Lat;
                _ = mapMarkers.MapMarkers(MyMapView);

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
            WebView webView = new WebView();
            mapWebView.GetWebMap(MapView, webView);
            
        }
    }

    private async void StartMenuItem_Clicked(object sender, EventArgs e)
    {
        var watch = Stopwatch.StartNew();
        scannerConn.ConnectToSD();
        watch.Stop();
        double ms = watch.Elapsed.TotalMilliseconds;
        int lapsedTime = Convert.ToInt32(ms);
        Debug.WriteLine("lapsedTime " + lapsedTime);
        Debug.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        ActivityIndicator activityIndicator = new ActivityIndicator()
        {
            IsRunning = true,
            Color = Colors.Blue,
            HeightRequest = 50,
            WidthRequest = 50
        };
        //NetworkListView2.IsVisible = false;
        listFrame.IsVisible = true;
        listFrame.IsEnabled = false;
        NetworkListView.IsEnabled = false;
        listFrame.Opacity = 0.5;
        listFrame.BorderColor = null;
        listFrame.Content = activityIndicator;

        await Task.Delay(TimeSpan.FromMilliseconds(lapsedTime*1000));

        listFrame.Opacity = 1;
        NetworkListView.IsEnabled = true;
        listFrame.IsEnabled = true;
        listFrame.BorderColor = null;
        listFrame.Content = NetworkListView;

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
        pg.ClearCSV(scannerConn.SDContent);
        //tableSection.Clear();
        //pg.showHeader(tableSection);
    }

    private void ReadCSV_Clicked(object sender, EventArgs e)
    {
        // TODO: Read csv
        readCSV.ReadCSVFile();
        NetworkListView.ItemsSource = readCSV.NetworkNames;
        pg.PopulateTableCSV(readCSV.CSVContent, tableSection);
        foreach (Network network in readCSV.CSVContent)
        {
            double snr = network.SNR;
            int rssi = network.RSSI;
            _observableValues.Add(new(snr));
            _observableValues2.Add(new(rssi));

            mapMarkers.Longitude = network.Lon;
            mapMarkers.Latitude = network.Lat;

            mapMarkers.Longitude = network.Lon;
            mapMarkers.Latitude = network.Lat;
            _ = mapMarkers.MapMarkers(MyMapView);

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
            WebView webView = new WebView();
            mapWebView.GetWebMap(MapView, webView);
        }
    }
}