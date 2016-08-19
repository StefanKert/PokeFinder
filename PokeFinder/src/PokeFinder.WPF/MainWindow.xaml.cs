using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PokeFinder.Misc;
using PokeFinder.Models;
using PokeFinder.Services;

namespace PokeFinder.WPF
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {

        private List<Pokemon> Visible_Pokemon = new List<Pokemon>();
        private List<Pokemon> Nearby_Pokemon = new List<Pokemon>();

        private readonly Dictionary<int, string> pokemonPng = new Dictionary<int, string>();

        private readonly IPokemonService _pokemonService = new PokemonService();

        public MainWindow() {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


            Interval.Text = "0.001";
            // ST johann
            //TbPoint1.Text = "47.3505269339223,13.201210498809816";
            //TbPoint2.Text = "47.34212340756677,13.206746578216555";

            TbPoint1.Text = "47.939993793103035,13.064954280853271";
            TbPoint2.Text = "47.93204398514115,13.088707923889162";

            var pokemons = new HttpClient().GetStringAsync("https://gist.githubusercontent.com/anonymous/50c284e815df6c81aa53497a305a29f2/raw").Result.Split('\n');
            foreach (string t in pokemons) {
                var data = t.Split(':');
                if (data.Length == 2) {
                    pokemonPng.Add(Convert.ToInt32(data[0]), data[1]);
                }
            }
            _pokemonService.OnException += exception => {
                PokemonLoadingFailedMessage.Text = exception.ToString();
                PokemonLoadingFailedMessage.Visibility = Visibility.Visible;
            };
        }

        private void InitClient(HttpClient client) {
            client.DefaultRequestHeaders.Add("Origin", "https://fastpokemap.com");
            client.DefaultRequestHeaders.Referrer = new Uri("https://fastpokemap.com/secret/");
            client.DefaultRequestHeaders.Host = "api.fastpokemap.com";
            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            client.Timeout = TimeSpan.FromMinutes(1);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            try {
                LoadPokemonButton.IsEnabled = false;
                VisiblePokemon.Children.Clear();
                NearByPokemon.Children.Clear();
                Visible_Pokemon.Clear();
                PokemonLoadingFailedMessage.Text = "";
                PokemonLoadingSucceededMessage.Visibility = Visibility.Collapsed;
                PokemonLoadingFailedMessage.Visibility = Visibility.Collapsed;
                PokemonLoadingMessage.Visibility = Visibility.Visible;
                var point1 = TbPoint1.Text.Split(',');
                var point2 = TbPoint2.Text.Split(',');

                var latitude1 = double.Parse(point1[0]);
                var longitude1 = double.Parse(point1[1]);

                var latitude2 = double.Parse(point2[0]);
                var longitude2 = double.Parse(point2[1]);

                await LoadPokemon(latitude1, longitude1, latitude2, longitude2, double.Parse(Interval.Text));
                PokemonLoadingSucceededMessage.Visibility = Visibility.Visible;
                PokemonLoadingMessage.Visibility = Visibility.Collapsed;
                LoadPokemonButton.IsEnabled = true;
            }
            catch (Exception ex) {
                PokemonLoadingFailedMessage.Text += "\n" + ex;
                PokemonLoadingSucceededMessage.Visibility = Visibility.Collapsed;
                PokemonLoadingFailedMessage.Visibility = Visibility.Visible;
                PokemonLoadingMessage.Visibility = Visibility.Collapsed;
            }
        }

        private async Task LoadPokemon(double latitude1, double longitude1, double latitude2, double longitude2, double interval) {
            double higherLatitude = latitude1 > latitude2 ? latitude1 : latitude2;
            double lowLatitude = latitude1 > latitude2 ? latitude2 : latitude1;
            double higherLongitude = longitude1 > longitude2 ? longitude1 : longitude2;
            double lowerLongitude = longitude1 > longitude2 ? longitude2 : longitude1;

            var tasks = new List<Task>();

            for (double i = lowLatitude; i < higherLatitude; i += interval) {
                double j = lowerLongitude;
                do {
                    var i1 = i.ToString("G");
                    var j1 = j.ToString("G");
                    tasks.Add(ExecuteCacheRequest(j1, i1));
                    tasks.Add(ExecuteApiRequest(j1, i1));
                    j += interval;
                } while (j < higherLongitude);
            }

            await Task.WhenAll(tasks.ToArray());
        }

        private async Task ExecuteCacheRequest(string longitude, string latitude) {
            try {
                var pokemons = await _pokemonService.ExecuteApiRequest(latitude, longitude);
                foreach (var pokemon in pokemons) {
                    AddPokemonToView(pokemon);
                }
            }
            catch (Exception ex) {
                Dispatcher.Invoke(() => {
                    PokemonLoadingFailedMessage.Text += "\n" + ex.Message;
                    PokemonLoadingFailedMessage.Visibility = Visibility.Visible;
                });
            }
        }

        private async Task ExecuteApiRequest(string longitude, string latitude) {
            try {
                var pokemons = await _pokemonService.ExecuteCacheRequest(latitude, longitude);
                foreach (var pokemon in pokemons) {
                    AddPokemonToView(pokemon);
                }
            }
            catch (Exception ex) {
                Dispatcher.Invoke(() => {
                    PokemonLoadingFailedMessage.Text += "\n" + ex.Message;
                    PokemonLoadingFailedMessage.Visibility = Visibility.Visible;
                });
            }
        }

        private void AddPokemonToView(Pokemon pokemon) {
            if (pokemon.PokemonType != PokemonType.Nearby && Visible_Pokemon.Any(x => x.SpawnId == pokemon.SpawnId))
                return;
            if (pokemon.PokemonType == PokemonType.Nearby && Nearby_Pokemon.Any(x => x.Id == pokemon.Id))
                return;
            var png = pokemonPng[pokemon.Id];
            byte[] binaryData = Convert.FromBase64String(png);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            var panel = new StackPanel();
            var image = new Image {Source = bi, Width = 40};
            if (pokemon.PokemonType != PokemonType.Nearby) {
                image.MouseDown += (sender, args) => { Process.Start($"http://maps.google.com/maps?q={pokemon.Latitude},{pokemon.Longitude}"); };
                var text = pokemon.ExpiresAt.ToLongTimeString();
                var textBlock = new TextBlock {Text = text};
                panel.Children.Add(textBlock);
            }
            panel.Children.Add(image);

            var border = new Border {BorderThickness = new Thickness(5), Child = panel,};
            if (pokemon.PokemonType == PokemonType.Nearby)
                border.BorderBrush = new SolidColorBrush(Colors.Transparent);
            if (pokemon.PokemonType == PokemonType.Api)
                border.BorderBrush = new SolidColorBrush(Colors.CornflowerBlue);
            if (pokemon.PokemonType == PokemonType.Cache)
                border.BorderBrush = new SolidColorBrush(Colors.Tomato);
            Dispatcher.Invoke(() => {
                if (pokemon.PokemonType == PokemonType.Nearby) {
                    NearByPokemon.Children.Add(border);
                    Nearby_Pokemon.Add(pokemon);
                }
                else {
                    VisiblePokemon.Children.Add(border);
                    Visible_Pokemon.Add(pokemon);
                }
            });
        }


        private async Task<HttpResponseMessage> GetResponseFromUrl(string url) {
            using (var client = new HttpClient()) {
                InitClient(client);
                return await client.GetAsync(url);
            }
        }

        public static string UnZipStr(byte[] input) {
            using (MemoryStream inputStream = new MemoryStream(input)) {
                using (DeflateStream gzip = new DeflateStream(inputStream, CompressionMode.Decompress)) {
                    return Encoding.UTF8.GetString(gzip.ReadAsBytes());
                }
            }
        }

    }
}