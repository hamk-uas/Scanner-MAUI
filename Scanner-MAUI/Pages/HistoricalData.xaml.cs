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
    private Markers mapMarkers;

    public HistoricalData()
	{
		InitializeComponent();
        NetworkListView.ItemSelected += OnNetworkNameSelected;
        scannerConn = new SDCard();
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
            WebView webView;
            webView = new WebView
            {
                Source = new HtmlWebViewSource
                {
                    Html = @" 
                        <!DOCTYPE html>
                        <html>
                          <head>
                            <meta charset=""utf-8"" />
                            <meta
                              name=""viewport""
                              content=""initial-scale=1,maximum-scale=1,user-scalable=no""
                            />
                            <title>Intro to CSVLayer - 4.15</title>

                            <style>
                              html,
                              body,
                              #viewDiv {
                                padding: 0;
                                margin: 0;
                                height: 100%;
                                width: 100%;
                                background-color: aliceblue;
                              }
                            </style>

                            <link
                              rel=""stylesheet""
                              href=""https://js.arcgis.com/4.15/esri/themes/light/main.css""
                            />
                            <script src=""https://js.arcgis.com/4.15/""></script>

                            <script>
                              require([
                                ""esri/Map"",
                                ""esri/views/MapView"",
                                ""esri/layers/CSVLayer"",
                                ""esri/widgets/Legend""
                              ], (Map, MapView, CSVLayer, Legend) => {
        
                                const renderer = {
                                  type: ""heatmap"",
                                  colorStops: [
                                    { color: ""rgba(63, 40, 102, 0)"", ratio: 0 },
                                    { color: ""#472b77"", ratio: 0.083 },
                                    { color: ""#4e2d87"", ratio: 0.166 },
                                    { color: ""#563098"", ratio: 0.249 },
                                    { color: ""#5d32a8"", ratio: 0.332 },
                                    { color: ""#6735be"", ratio: 0.415 },
                                    { color: ""#7139d4"", ratio: 0.498 },
                                    { color: ""#7b3ce9"", ratio: 0.581 },
                                    { color: ""#853fff"", ratio: 0.664 },
                                    { color: ""#a46fbf"", ratio: 0.747 },
                                    { color: ""#c29f80"", ratio: 0.83 },
                                    { color: ""#e0cf40"", ratio: 0.913 },
                                    { color: ""#ffff00"", ratio: 1 }
                                  ],
                                  maxDensity: 0.01,
                                  minDensity: 0
                                };
                                 const template = {
                                  title: ""{name}"",
                                  content: ""Rssi: {rssi}, Type: {type}.""
                                };
                                let fileInputField;
                                let fileInput = document.getElementById(""file-input"");
                                fileInput.addEventListener('change', () => {          
                                  const url = URL.createObjectURL(fileInput.files[0]);
                                  const csvLayer = new CSVLayer({
                                    url: url, 
                                    title: ""RSSI Strength Values"",
                                    popupTemplate: template,
                                    renderer: renderer
                                  });
          
                                  map.add(csvLayer);          
                                },false);        

                                const map = new Map({
                                  basemap: ""osm"",
                                  // layers: [csvLayer]
                                });
        
                                const view = new MapView({
                                  container: ""viewDiv"",
                                  map: map,
                                  center: [24.4590, 60.9929],
                                  zoom: 3,
                                  scale: 62223.819286
                                });
                                 view.ui.add(
                                  new Legend({
                                    view: view
                                  }),
                                  ""bottom-left""
                                );
                              });
                            </script>
                          </head>

                          <body>
                            <span>
                              Browse... <input type=""file"" id=""file-input"" name=""files[]"" accept=""text/csv"">
                            </span>    
                            <div id=""viewDiv""></div>
                          </body>
                        </html> "
                }
            };
            MapView.Add(webView);
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
        pg.ClearCSV(scannerConn.SDContent);
        //tableSection.Clear();
        //pg.showHeader(tableSection);
    }
}