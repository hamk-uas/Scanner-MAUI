using Scanner_MAUI.Functions;

namespace Scanner_MAUI.Pages
{
    public partial class RealTimeData : ContentPage
    {
        public RealTimeData()
        {
            InitializeComponent();
            NetworkListView.ItemSelected += OnNetworkNameSelected;
            TimeStamp.TimeStampViewr(DateTimeLabel);
        }

        //Dynamically populating the network name based on the selected network fromthe list view
        private void OnNetworkNameSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is string selectedNetwork)
            {
                NetworkNameLabel.Text = $"Network Name: {selectedNetwork}";
            }
        }
    }
}
