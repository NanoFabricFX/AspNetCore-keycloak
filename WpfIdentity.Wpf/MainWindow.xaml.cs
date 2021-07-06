using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Newtonsoft.Json;

namespace WpfIdentity.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OidcClient _oidcClient = null;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Start;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public async void Start(object sender, RoutedEventArgs e)
        {
            var options = new OidcClientOptions()
            {
                Authority = "https://keycloak-ctos.apps.hkhit-ocp1.sit.cmft.com/auth/realms/basic/",
                ClientId = "wpf",
                Scope = "openid profile email offline_access",
                RedirectUri = "https://notused",
                Browser = new WpfEmbeddedBrowser(),
                RefreshDiscoveryOnSignatureFailure = false,
                BackchannelHandler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) => true }
        };

            _oidcClient = new OidcClient(options);

            LoginResult result;
            try
            {
                result = await _oidcClient.LoginAsync();
            }
            catch (Exception ex)
            {
                Message.Text = $"Unexpected Error: {ex.Message}";
                return;
            }

            if (result.IsError)
            {
                Message.Text = result.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : result.Error;
            }
            else
            {
                //var Http = new HttpClient();
                //Http.SetBearerToken(result.AccessToken);

                //var response = await Http.GetAsync("https://localhost:5001/WeatherForecast");
                //if (!response.IsSuccessStatusCode) throw new Exception(response.StatusCode.ToString());
                //var apiResult = JsonConvert.DeserializeObject<List<WeatherForecast>>(await response.Content.ReadAsStringAsync());

                var name = result.User.Identity.Name;
                Message.Text = $"Hello {name}";
            }
        }
    }
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF { get; set; }// Modified Line
    }
}
