using System.Net;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Newtonsoft.Json.Linq;

namespace gsuit_api_maui
{
    public static class Auth
    {
        public static string ConstructGoogleSignInUrl()
        {
            // Specify the necessary parameters for the Google Sign-In URL
            const string clientId = "your_app_client_id";
            const string responseType = "code";
            const string accessType = "offline";
            const string redirect_uri = "http://localhost";
            const string scope = "https://www.googleapis.com/auth/drive";

            // Construct the Google Sign-In URL
            return "https://accounts.google.com/o/oauth2/v2/auth" +
                            $"?client_id={Uri.EscapeDataString(clientId)}" +
                            $"&redirect_uri={Uri.EscapeDataString(redirect_uri)}" +
                            $"&response_type={Uri.EscapeDataString(responseType)}" +
                            $"&scope={Uri.EscapeDataString(scope)}" +
                            $"&access_type={Uri.EscapeDataString(accessType)}" +
                            "&include_granted_scopes=true" +
                            "&prompt=consent";


        }


        public static string OnWebViewNavigating(WebNavigatingEventArgs e, ContentPage signInContentPage)
        {


            if (e.Url.StartsWith("http://localhost"))
            {

                Uri uri = new Uri(e.Url);
                string query = WebUtility.UrlDecode(uri.Query);
                var queryParams = System.Web.HttpUtility.ParseQueryString(query);
                string authorizationCode = queryParams.Get("code");

                signInContentPage.Navigation.PopModalAsync();
                return authorizationCode;


            }


            return null;
        }

        public static UserCredential ExchangeCodeForAccessToken(string code)
        {
            UserCredential userCredential = null;
            // Configure the necessary parameters for the token exchange
            const string clientId = "your_app_client_id";
            const string clientSecret = "your_app_client_secret";
            const string redirectUri = "http://localhost";

            // Create an instance of HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Construct the token exchange URL
                const string tokenUrl = "https://oauth2.googleapis.com/token";

                // Create a FormUrlEncodedContent object with the required parameters
                FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
              {
                   { "code", code },
                   { "client_id", clientId },
                   { "client_secret", clientSecret },
                   { "redirect_uri", redirectUri },
                   { "grant_type", "authorization_code" }
              });

                // Send a POST request to the token endpoint to exchange the code for an access token
                HttpResponseMessage response = client.PostAsync(tokenUrl, content).Result;

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    string responseContent = response.Content.ReadAsStringAsync().Result;

                    // Parse the JSON response to extract the access token
                    JObject json = JObject.Parse(responseContent);
                    string accessToken = json.GetValue("access_token").ToString();
                    string refreshToken = json.GetValue("refresh_token").ToString();
                    string[] Scopes = { DriveService.Scope.Drive };


                    userCredential = GetCredentialFromAccessToken(accessToken, refreshToken);
                    return userCredential;
                }

            }

            return userCredential;
        }

        private static UserCredential GetCredentialFromAccessToken(string accessToken, string refreshToken)
        {
            string[] Scopes = { DriveService.Scope.Drive };

            TokenResponse tokenResponse = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "your_app_client_id",
                    ClientSecret = "your_app_client_secret"
                },
                Scopes = Scopes,
            };

            return new UserCredential(new GoogleAuthorizationCodeFlow(initializer), userId: "user", tokenResponse);

        }
    }
}