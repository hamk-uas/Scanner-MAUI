using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Map = Esri.ArcGISRuntime.Mapping.Map;
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

            // Create a new map.
            Map myMap = new();

            //// Create several map points using the WGS84 coordinates (latitude and longitude).
            MapPoint networkPoint1 = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);
            MapPoint networkPoint2 = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);
            MapPoint networkPoint3 = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);
            MapPoint networkPoint4 = new MapPoint(longitude, latitude, SpatialReferences.Wgs84);

            // Get the basemap from the map.
            Basemap myBasemap = myMap.Basemap;

            // Get the layer collection for the base layers.
            LayerCollection myLayerCollection = myBasemap.BaseLayers;

            // Create an instance for the WMTS layer.
            WmtsLayer myWmtsLayer;

            // Define the Uri to the WMTS service.
            Uri wmtsUri = new("https://avoin-karttakuva.maanmittauslaitos.fi/avoin/wmts/1.0.0/taustakartta/default/ETRS-TM35FIN/3/5/3.png?api-key=" + apiKey);

            // Attach the Authorization header with the API key as the username.
            string headAuth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{apiKey}:"));

            // Create a new HttpClient.
            HttpClient httpClient = new HttpClient();

            // Set the base address of the HttpClient.
            httpClient.BaseAddress = wmtsUri;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", headAuth);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Create a new HttpRequestMessage.
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, wmtsUri);

            // Send the request and get the response.
            HttpResponseMessage response = await httpClient.SendAsync(request);

            // Check if the request was successful.
            if (response.IsSuccessStatusCode)
            {
                // Get the response content.
                HttpContent content = response.Content;
                // Create a WMTS layer using a Uri and provide an Id value.
                myWmtsLayer = new WmtsLayer(wmtsUri, "taustakartta");

                // Add the WMTS layer to the layer collection of the map.
                myLayerCollection.Add(myWmtsLayer);

                // Use the two points farthest apart to create an envelope.
                Envelope initialEnvelope = new Envelope(networkPoint1, networkPoint4);

                // Use the envelope to define the map views visible area (include some padding around the extent).
                myMap.InitialViewpoint = new Viewpoint(initialEnvelope);

                // Assign the map to the MapView.
                MyMapView.Map = myMap;

                // Set the viewpoint to the envelope with padding.
                _ = MyMapView.SetViewpointGeometryAsync(initialEnvelope, 50);

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

                //var Test = "This is a test to check if the code is being executed untill this point";

            }
            else
            {
                // Handle the error response.
                string errorMessage = $"Request failed with status code {response.StatusCode}: {response.ReasonPhrase}";
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
        }
    }
}
