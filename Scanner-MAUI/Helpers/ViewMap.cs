using Esri.ArcGISRuntime.Mapping;
using Map = Esri.ArcGISRuntime.Mapping.Map;
using Esri.ArcGISRuntime.Ogc;
using Esri.ArcGISRuntime.Maui;
using Scanner_MAUI.Helpers;
using System.Net.Http.Headers;
using System.Net;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using Windows.Media.Protection.PlayReady;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Symbology;
using System.Reflection;

using Colors = System.Drawing.Color;

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

                _ = apiKey;

                // Create a new map.
                Map myMap = new();

                // Get the basemap from the map.
                Basemap myBasemap = myMap.Basemap;

                // Get the layer collection for the base layers.
                LayerCollection myLayerCollection = myBasemap.BaseLayers;

                // Create an instance for the WMTS layer.
                WmtsLayer myWmtsLayer;

                //specify to use TLS 1.2 as default connection (Maube not needed)
                //System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // Define the Uri to the WMTS service.
                //Uri wmtsUri = new("https://avoin-karttakuva.maanmittauslaitos.fi/avoin/wmts/1.0.0/WMTSCapabilities.xml?api-key=" + apiKey);

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
                //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, wmtsUri);

                //request.Headers.Add("Authorization", "Basic " + user_id);
                //httpClient.Send(request);

                // Send the request and get the response.
                HttpResponseMessage response = await httpClient.SendAsync(request);
                //var response = httpClient.GetAsync(wmtsUri).Result;


                // Check if the request was successful.
                if (response.IsSuccessStatusCode)
                {
                    // Get the response content.
                    HttpContent content = response.Content;

                    //    // Read the response as a string.
                    //    string responseString = await content.ReadAsStringAsync();

                    //// Define a new instance of the WMTS service.
                    //WmtsService myWmtsService = new(wmtsUri);

                    //// Load the WMTS service.
                    //await myWmtsService.LoadAsync();

                    //// Get the service information (i.e., metadata) about the WMTS service.
                    //WmtsServiceInfo myWmtsServiceInfo = myWmtsService.ServiceInfo;

                    //// Obtain the read-only list of WMTS layer info objects and select the one with the desired Id value.
                    //WmtsLayerInfo info = myWmtsServiceInfo.LayerInfos.Single(l => l.Id == "taustakartta");

                    //// Create a WMTS layer using WMTS layer info.
                    //myWmtsLayer = new WmtsLayer(info);

                    // Create a WMTS layer using a Uri and provide an Id value.
                    myWmtsLayer = new WmtsLayer(wmtsUri, "taustakartta");

                    // Add the WMTS layer to the layer collection of the map.
                    myLayerCollection.Add(myWmtsLayer);

                    // Assign the map to the MapView.
                    MyMapView.Map = myMap;
                 
                    //var Test = "This is a test to check if the code is being executed untill this point";
                  
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

        public static async Task OpenstreetMaps(MapView MyMapView)
        {
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = Keys.Instance["Settings:EsriAPI"];

            // Create the OpenStreetMap basemap.
            Basemap osmBasemap = new Basemap(BasemapStyle.OSMStandard);

            // Create the map with the OpenStreetMap basemap.
            Map map = new Map(osmBasemap);
           
            MyMapView.Map = map;
            MyMapView.SetViewpoint(new Viewpoint(
                latitude: 60.9763,
                longitude: 24.4783,
                scale: 22223.819286
                ));
        }

    }
}