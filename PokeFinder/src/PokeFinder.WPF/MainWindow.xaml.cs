using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Input;
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
        private object _lock = new object();
        private readonly IPokemonService _pokemonService = new PokemonService();

        public ObservableCollection<Pokemon> VisiblePokemon { get; } = new ObservableCollection<Pokemon>();
        public ObservableCollection<Pokemon> NearbyPokemon { get; } = new ObservableCollection<Pokemon>();


  
        public MainWindow() {
            InitializeComponent();
            DataContext = this;

            ScanDistance.Text = "0.001";
            // ST johann
            TbPoint1.Text = "47.3505269339223,13.201210498809816";
            TbPoint2.Text = "47.34212340756677,13.206746578216555";

            //TbPoint1.Text = "47.939993793103035,13.064954280853271";
            //TbPoint2.Text = "47.93204398514115,13.088707923889162";

            _pokemonService.OnException += exception => {
                PokemonLoadingFailedMessage.Text = exception.ToString();
                PokemonLoadingFailedMessage.Visibility = Visibility.Visible;
            };
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            try {
                LoadPokemonButton.IsEnabled = false;
                NearbyPokemon.Clear();
                VisiblePokemon.Clear();
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

                await LoadPokemon(latitude1, longitude1, latitude2, longitude2, double.Parse(ScanDistance.Text));
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
            var higherLatitude = latitude1 > latitude2 ? latitude1 : latitude2;
            var lowLatitude = latitude1 > latitude2 ? latitude2 : latitude1;
            var higherLongitude = longitude1 > longitude2 ? longitude1 : longitude2;
            var lowerLongitude = longitude1 > longitude2 ? longitude2 : longitude1;

            var tasks = new List<Task>();
            
            for (var i = lowLatitude; i < higherLatitude; i += interval) {
                var j = lowerLongitude;
                do {
                    var i1 = i.ToString("G");
                    var j1 = j.ToString("G");
                    var cacheTask = Task.Run(() => ExecuteCacheRequest(j1, i1));
                    var apiTask = Task.Run(() => ExecuteApiRequest(j1, i1));
                    tasks.Add(cacheTask);
                    tasks.Add(apiTask);
                    j += interval;
                } while (j < higherLongitude);
            }

            Console.WriteLine(tasks.Count + " Threads to start.");
            await Task.WhenAll(tasks.ToArray());
        }

        private async Task ExecuteCacheRequest(string longitude, string latitude) {
            try {
                var pokemons = await _pokemonService.ExecuteCacheRequest(latitude, longitude);
                foreach (var pokemon in pokemons) {
                    lock (_lock) {
                        AddPokemon(pokemon);
                    }
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
                var pokemons = await _pokemonService.ExecuteApiRequest(latitude, longitude);
                foreach (var pokemon in pokemons) {
                    lock (_lock) {
                        AddPokemon(pokemon);
                    }
                }
            }
            catch (Exception ex) {
                Dispatcher.Invoke(() => {
                    PokemonLoadingFailedMessage.Text += "\n" + ex.Message;
                    PokemonLoadingFailedMessage.Visibility = Visibility.Visible;
                });
            }
        }

        private void AddPokemon(Pokemon pokemon) {
            if (pokemon.PokemonType != PokemonType.Nearby && VisiblePokemon.Any(x => x.SpawnId == pokemon.SpawnId))
                return;
            if (pokemon.PokemonType == PokemonType.Nearby && NearbyPokemon.Any(x => x.Id == pokemon.Id))
                return;
            Dispatcher.Invoke(() => {
                if (pokemon.PokemonType == PokemonType.Nearby) {
                    NearbyPokemon.AddSorted(pokemon, (x, y) => x.Id >= y.Id);
                }
                else {
                    VisiblePokemon.AddSorted(pokemon, (x, y) => x.Id >= y.Id);
                }
            });
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e) {
            var image = sender as Image;
            if (image == null)
                return;

            var pokemon = VisiblePokemon.First(x => x.SpawnId == image.Tag.ToString());
            Process.Start($"http://maps.google.com/maps?q={pokemon.Latitude},{pokemon.Longitude}");
        }
    }
}