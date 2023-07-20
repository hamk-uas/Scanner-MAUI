using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.Reflection;
using MapView = Esri.ArcGISRuntime.Maui.MapView;

namespace Scanner_MAUI.Helpers
{
    public class Markers
    {
        //store the longitude in its own property
        public double Longitude { get; set; }

        //store the longitude in its own property
        public double Latitude { get; set; }


        public async Task MapMarkers(MapView MyMapView)
        {
            //get value by key
            var apiKey = Keys.Instance["Settings:API-key"];

            double longitude = Longitude;
            double latitude = Latitude;

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

            // Add the graphics overlay to the map view.
            MyMapView.GraphicsOverlays.Add(myGraphicOverlay);

        }
    }
}
