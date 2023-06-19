using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scanner_MAUI.Pages;
using MapView = Esri.ArcGISRuntime.Maui.MapView;

namespace Scanner_MAUI.Helpers
{
    public static class Location
    {
        public static async Task StartDeviceLocationTask(MapView MyMapView)
        {
            try
            {
                // Check if location permission granted.
                var status = Microsoft.Maui.ApplicationModel.PermissionStatus.Unknown;
                status = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<Microsoft.Maui.ApplicationModel.Permissions.LocationWhenInUse>();

                // Request location permission if not granted.
                if (status != Microsoft.Maui.ApplicationModel.PermissionStatus.Granted)
                {
                    status = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<Microsoft.Maui.ApplicationModel.Permissions.LocationWhenInUse>();
                }

                // Start the location display once permission is granted.
                if (status == Microsoft.Maui.ApplicationModel.PermissionStatus.Granted)
                {
                    await MyMapView.LocationDisplay.DataSource.StartAsync();
                    MyMapView.LocationDisplay.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Application.Current.MainPage.DisplayAlert("Couldn't start location", ex.Message, "OK");
            }

        }

    }
}