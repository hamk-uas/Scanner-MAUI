using Esri.ArcGISRuntime.Mapping;
using Map = Esri.ArcGISRuntime.Mapping.Map;
using Esri.ArcGISRuntime.Ogc;
using Esri.ArcGISRuntime.Maui;
using Scanner_MAUI.Helpers;
using System.Net.Http.Headers;
using System.Net;

namespace Scanner_MAUI.Helpers
{
    public static class ViewMap
    {
        public static async Task LoadWMTSLayer(MapView MyMapView)
        {
            try
            {
                //get value by key
                var apiKey = Keys.Instance["Settings:API-key"];

                // Create a new map.
                Map myMap = new();

                // Get the basemap from the map.
                Basemap myBasemap = myMap.Basemap;

                // Get the layer collection for the base layers.
                LayerCollection myLayerCollection = myBasemap.BaseLayers;

                // Create an instance for the WMTS layer.
                WmtsLayer myWmtsLayer;

                // Create a new HttpClient.
                HttpClient httpClient = new HttpClient();

                //specify to use TLS 1.2 as default connection (Maube not needed)
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // Define the Uri to the WMTS service.
                Uri wmtsUri = new("https://avoin-karttakuva.maanmittauslaitos.fi/avoin/wmts/1.0.0/WMTSCapabilities.xml?api-key=" + apiKey);

                // Set the base address of the HttpClient.
                httpClient.BaseAddress = wmtsUri;

                // Attach the Authorization header with the API key as the username.
                string authHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(apiKey + ":"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                // Create a new HttpRequestMessage.
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, wmtsUri);

                // Send the request and get the response.
                HttpResponseMessage response = await httpClient.SendAsync(request);

                // Check if the request was successful.
                if (response.IsSuccessStatusCode)
                {
                    // Define a new instance of the WMTS service.
                    WmtsService myWmtsService = new(wmtsUri);

                    // Load the WMTS service.
                    await myWmtsService.LoadAsync();

                    // Get the service information (i.e., metadata) about the WMTS service.
                    WmtsServiceInfo myWmtsServiceInfo = myWmtsService.ServiceInfo;

                    // Obtain the read-only list of WMTS layer info objects and select the one with the desired Id value.
                    WmtsLayerInfo info = myWmtsServiceInfo.LayerInfos.Single(l => l.Id == "taustakartta");

                    // Create a WMTS layer using WMTS layer info.
                    myWmtsLayer = new WmtsLayer(info);

                    // Add the WMTS layer to the layer collection of the map.
                    myLayerCollection.Add(myWmtsLayer);

                    // Assign the map to the MapView.
                    MyMapView.Map = myMap;
                }
                else
                {
                    // Handle the error response.
                    string errorMessage = $"Request failed with status code {response.StatusCode}: {response.ReasonPhrase}";
                    await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
    }
}