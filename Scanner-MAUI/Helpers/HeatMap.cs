using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using Esri.ArcGISRuntime.UI;

using Scanner_MAUI.Pages;
using System.Reflection;
using Map = Esri.ArcGISRuntime.Mapping.Map;
using MapView = Esri.ArcGISRuntime.Maui.MapView;

namespace Scanner_MAUI.Helpers
{
    public class HeatMap
    {
        //store the longitude in its own property
        public double Longitude { get; set; }

        //store the longitude in its own property
        public double Latitude { get; set; }

        public int RSSI { get;set; }

        public async Task HeatMarkers(MapView MyMapView)
        {

            //get value by key
            var apiKey = Keys.Instance["Settings:API-key"];

            double longitude = Longitude;
            double latitude = Latitude;

            int rssi = RSSI;    

            // Create several map points using the WGS84 coordinates (latitude and longitude).
            MapPoint networkPoint1 = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);
            MapPoint networkPoint2 = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);
            MapPoint networkPoint3 = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);
            MapPoint networkPoint4 = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);

            // Use the two points farthest apart to create an envelope.
            Envelope initialEnvelope = new Envelope(networkPoint1, networkPoint4);

            // Use the envelope to define the map views visible area (include some padding around the extent).
            MyMapView.Map.InitialViewpoint = new Viewpoint(initialEnvelope);

            // Set the viewpoint to the envelope with padding.
            await MyMapView.SetViewpointGeometryAsync(initialEnvelope, 50);

            // Create a graphics overlay .
            GraphicsOverlay myGraphicOverlay = new GraphicsOverlay();

            Graphic network1Graphic = new Graphic(networkPoint1);
            Graphic network2Graphic = new Graphic(networkPoint2);
            Graphic network3Graphic = new Graphic(networkPoint3);
            Graphic network4Graphic = new Graphic(networkPoint4);

            myGraphicOverlay.Graphics.Add(network1Graphic);
            myGraphicOverlay.Graphics.Add(network2Graphic);
            myGraphicOverlay.Graphics.Add(network3Graphic);
            myGraphicOverlay.Graphics.Add(network4Graphic);

            // Get current assembly that contains the image
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            // Get image as a stream from the resources
            // Picture is defined as EmbeddedResource and DoNotCopy
            Stream resourceStream = currentAssembly.GetManifestResourceStream(
                "Scanner_MAUI.Resources.Images.signal.png");

            // Create new symbol using asynchronous factory method from stream
            PictureMarkerSymbol pinSymbol = await PictureMarkerSymbol.CreateAsync(resourceStream);
            pinSymbol.Height = 50;
            pinSymbol.Width = 50;
            // Create a simple renderer based on the simple marker symbol.
            SimpleRenderer myRenderer = new SimpleRenderer(pinSymbol);

            // Apply the renderer to the graphics overlay (all graphics use the same symbol).
            myGraphicOverlay.Renderer = myRenderer;

            //var uniqueValueRenderer = new UniqueValueRenderer();
            //uniqueValueRenderer.DefaultSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Diamond, System.Drawing.Color.Purple, 15).ToMultilayerSymbol();
            //MultilayerPointSymbol circleMultilayerSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Green, 30).ToMultilayerSymbol();
            //myGraphicOverlay.Renderer = uniqueValueRenderer;

            // Create a new HeatMapRenderer with info provided by the user.
            HeatMapRenderer heatMapRendererInfo = new HeatMapRenderer
            {
                BlurRadius = 10,
                MaxPixelIntensity = 1000.0,
                MinPixelIntensity = 0.0,
            };

            //heatMapRendererInfo.Field = "mill_damages";
            // Add color stops to the HeatMapRenderer
            heatMapRendererInfo.AddColorStop(0.0, Colors.Transparent);
            heatMapRendererInfo.AddColorStop(0.10, new Color(1, 0, 0, (float)0.6));    // Red color at position 0
                                                                                       //heatMapRendererInfo.AddColorStop(0.5, new Color(1, 1, 0, (float)0.6));  // Yellow color at position 0.5
            heatMapRendererInfo.AddColorStop(1.0, new Color(0, 1, 0, (float)0.6));    // Green color at position 1

            // Get the JSON representation of the renderer class.
            string heatMapJson = heatMapRendererInfo.ToJson();

            // Use the static Renderer.FromJson method to create a new renderer from the JSON string.
            var heatMapRenderer = Renderer.FromJson(heatMapJson);
            //myGraphicOverlay.Renderer = heatMapRenderer;
            // Add the graphics overlay to the map view.
            MyMapView.GraphicsOverlays.Add(myGraphicOverlay);

        }
    }
}
