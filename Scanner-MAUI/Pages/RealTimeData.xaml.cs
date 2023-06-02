using Esri.ArcGISRuntime.Mapping;
using Map = Esri.ArcGISRuntime.Mapping.Map;
using ArcGISTiledLayer = Esri.ArcGISRuntime.Mapping.Map;
using Esri.ArcGISRuntime.Ogc;

namespace Scanner_MAUI.Pages;

public partial class RealTimeData : ContentPage
{

	public RealTimeData()
	{
		InitializeComponent();
        _ = LoadWMTSLayer(true);
        //_ = LoadWMTSLayer(true);
    }

    //private void UriButton_Click(object sender, EventArgs e)
    //{
    //    //Load the WMTS layer using Uri method.
    //    _ = LoadWMTSLayer(true);

    //    // Disable and enable the appropriate buttons.
    //    UriButton.IsEnabled = false;
    //    InfoButton.IsEnabled = true;
    //}

    //private void InfoButton_Click(object sender, EventArgs e)
    //{
    //    //Load the WMTS layer using layer info.
    //    _ = LoadWMTSLayer(false);

    //    // Disable and enable the appropriate buttons.
    //    UriButton.IsEnabled = true;
    //    InfoButton.IsEnabled = false;
    //}

    //private async Task LoadWMTSLayer(bool uriMode)
    private async Task LoadWMTSLayer(bool uriMode)

    {
        try
        {
            // Create a new map.
            Map myMap = new Map();

            // Get the basemap from the map.
            Basemap myBasemap = myMap.Basemap;

            // Get the layer collection for the base layers.
            LayerCollection myLayerCollection = myBasemap.BaseLayers;

            // Create an instance for the WMTS layer.
            WmtsLayer myWmtsLayer;

            // Define the Uri to the WMTS service.
            Uri wmtsUri = new Uri("https://gibs.earthdata.nasa.gov/wmts/epsg4326/best");
            //Uri wmtsUri = new Uri(" https://avoin-karttakuva.maanmittauslaitos.fi/avoin/wmts/1.0.0/WMTSCapabilities.xml");
            //Uri wmtsUri = new Uri("https://avoin-karttakuva.maanmittauslaitos.fi/avoin/wmts/1.0.0/taustakartta/default/WGS84_Pseudo-Mercator/%7Bz%7D/%7By%7D/%7Bx%7D.png?api-key=9b0c804a-d74e-404e-b3f3-8165d8e3f69a");

            if (uriMode)
            {
               //Create a WMTS layer using a Uri and provide an Id value.
               myWmtsLayer = new WmtsLayer(wmtsUri, "SRTM_Color_Index");
            }
            else
            {
                // Define a new instance of the WMTS service.
                WmtsService myWmtsService = new WmtsService(wmtsUri);

                // Load the WMTS service.
                await myWmtsService.LoadAsync();

                // Get the service information (i.e. metadata) about the WMTS service.
                WmtsServiceInfo myWmtsServiceInfo = myWmtsService.ServiceInfo;

                // Obtain the read only list of WMTS layer info objects, and select the one with the desired Id value.
                WmtsLayerInfo info = myWmtsServiceInfo.LayerInfos.Single(l => l.Id == "SRTM_Color_Index");

                // Create a WMTS layer using WMTS layer info.
                myWmtsLayer = new WmtsLayer(info);
            }

            // Add the WMTS layer to the layer collection of the map.
            myLayerCollection.Add(myWmtsLayer);

            // Assign the map to the MapView.
            MyMapView.Map = myMap;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Sample error", ex.ToString(), "OK");
        }
    }
}