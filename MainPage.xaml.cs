using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace gsuit_api_maui;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnLoginBtnClicked(object sender, EventArgs e)
	{
		WebView _signInWebView = new WebView
		{
			Source = Auth.ConstructGoogleSignInUrl(),
			VerticalOptions = LayoutOptions.Fill,
		};
		Grid grid = new Grid();
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star }); // Row for the WebView
		Grid.SetRow(_signInWebView, 0);
		grid.Children.Add(_signInWebView);

		ContentPage signInContentPage = new ContentPage
		{
			Content = grid,
		};

		try
		{
			Application.Current.MainPage.Navigation.PushModalAsync(signInContentPage);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		_signInWebView.Navigating += (sender, e) =>
	   {

		   string code = Auth.OnWebViewNavigating(e, signInContentPage);
		   if (e.Url.StartsWith("http://localhost") && code != null)
		   {
			   UserCredential userCredential = Auth.ExchangeCodeForAccessToken(code);
		   	if (userCredential != null)
			   {
				   Drive _drive = new Drive();
				   var driveService = new DriveService(new BaseClientService.Initializer()
				   {
					   HttpClientInitializer = userCredential,
					   ApplicationName = "your_app_name"
				   });
				   string[] keywords = new string[] { "content", "dummy" };

				   _drive.GetDriveFiles(keywords, driveService);
			   }
		   }

	   };

	}


}

