namespace Scanner_MAUI;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

    // adding A title for the app
    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        if(window != null )
        {
            window.Title = "HAMK Scanner-App";
        }

        return window;
    }
}
