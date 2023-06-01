using Microsoft.Maui.Controls;
using Scanner_MAUI.Functions;

namespace Scanner_MAUI.Pages
{
    public partial class RealTimeData : ContentPage
    {
        private GraphicsDrawable graphicsDrawable;

        public RealTimeData()
        {
            InitializeComponent();
            graphicsDrawable = new GraphicsDrawable();
            Canvas.Drawable = graphicsDrawable;
            NetworkListView.ItemSelected += OnNetworkNameSelected;
            TimeStamp.TimeStampViewr(DateTimeLabel);
        }

        // Dynamically populating the network name based on the selected network from the list view
        private void OnNetworkNameSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is string selectedNetwork)
            {
                NetworkNameLabel.Text = $"Network Name: {selectedNetwork}";

                // Update the signal strength based on the selected network
                int signalStrength = GetSignalStrength(selectedNetwork);
                graphicsDrawable.SignalStrength = signalStrength;

                // Refresh the Canvas
                Canvas.Invalidate();
               
            }
        }

        private int GetSignalStrength(string networkName)
        {
            if (networkName == "Network-1")
                return 10; // Maximum signal strength
            else if (networkName == "Network-2")
                return 7; // Medium signal strength
            else if (networkName == "Network-3")
                return 4; // Low signal strength
            else if (networkName == "Network-4")
                return 1; // Minimum signal strength

            return 1; // Default signal strength
        }
    }
}
