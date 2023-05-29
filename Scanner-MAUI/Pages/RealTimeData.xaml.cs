
using System.Diagnostics.CodeAnalysis;

namespace Scanner_MAUI.Pages
{
    public partial class RealTimeData : ContentPage
    {
        public RealTimeData()
        {
            InitializeComponent();
            NetworkListView.ItemSelected += OnNetworkNameSelected;
        }

        //Dynamically populating the network name
        private void OnNetworkNameSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is string selectedNetwork)
            {
                NetworkNameLabel.Text = $"Network Name: {selectedNetwork}";
            }
        }
    }
}
