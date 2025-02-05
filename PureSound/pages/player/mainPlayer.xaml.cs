using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using PureSound.appCurr;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
using static NuGet.Packaging.PackagingConstants;

namespace PureSound.pages.player
{
    /// <summary>   
    /// Логика взаимодействия для mainPlayer.xaml
    /// </summary>
    public partial class mainPlayer : Page
    {
        private const string DeezerApiUrl = "https://api.deezer.com/chart/0/tracks?limit=100";
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
        public mainPlayer()
        {
            InitializeComponent();
            TracksList.ItemsSource = Tracks;
            LoadTracks();

        }


        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();
            if (!string.IsNullOrEmpty(query))
            {
                await SearchTracksAsync(query);
            }
            else
            {
                MessageBox.Show("Введите название песни или исполнителя.","Ошибка!",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private async Task SearchTracksAsync(string query)
        {
            try
            {
                Tracks.Clear();
                string DeezerApiUrlSearch = "https://api.deezer.com/search?q="; 
                string requestUrl = DeezerApiUrlSearch + Uri.EscapeDataString(query);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<DeezerResponse>(jsonResponse);

                foreach (var item in result.Data)
                {
                    Tracks.Add(new Track
                    {
                        Id = item.Id,   
                        Title = item.Title,
                        Artist = item.Artist.Name,
                        Duration = FormatDuration(item.Duration),
                        CoverUrl = item.Album.CoverMedium
                    });
                }

                if (Tracks.Count == 0)
                {
                    counterTB.Text = "Ничего не найдено.";
                }
                else
                {
                    counterTB.Text = "Найдено треков: " + Tracks.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadTracks()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonResponse = await client.GetStringAsync(DeezerApiUrl);

                    Console.WriteLine(jsonResponse);


                    var result = JsonConvert.DeserializeObject<DeezerResponse>(jsonResponse);

                    Tracks.Clear();
                    foreach (var item in result.Data)
                    {
                        Tracks.Add(new Track
                        {      
                            Id = item.Id,
                            Title = item.Title,
                            Artist = item.Artist.Name,
                            Duration = TimeSpan.FromSeconds(item.Duration).ToString(@"mm\:ss"),
                            CoverUrl = item.Album.CoverMedium
                        });
                    }

                    counterTB.Text = "Загружено треков: " + Tracks.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}","Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string FormatDuration(int durationInSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(durationInSeconds);
            return time.ToString(@"mm\:ss");
        }

        public class Track
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Duration { get; set; }
            public string CoverUrl { get; set; }
        }

        public class DeezerResponse
        {
            public DeezerTrack[] Data { get; set; }
        }

        public class DeezerTrack
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public int Duration { get; set; }
            public DeezerArtist Artist { get; set; }
            public DeezerAlbum Album { get; set; }
        }

        public class DeezerArtist
        {
            public string Name { get; set; }
        }

        public class DeezerAlbum
        {
            [JsonProperty("cover_medium")]
            public string CoverMedium { get; set; }
        }

        private void btnFav_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button b = sender as Button;
                string ID = Convert.ToString(float.Parse(((b.Parent as StackPanel).Children[0] as TextBlock).Text));

                int idUsers = Convert.ToInt32(App.Current.Properties["idUser"]);

                var existingFavorite = pureSoundEntities.GetContext().tableFavourite.FirstOrDefault(o => o.idUser == idUsers && o.idTrack == ID);

                if (existingFavorite != null)
                {
                    MessageBox.Show("Данный трек уже в 'Избранном'!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var newFavorite = new tableFavourite()
                    {
                        idUser = idUsers,
                        idTrack = ID,
                    };
                    pureSoundEntities.GetContext().tableFavourite.Add(newFavorite);
                    pureSoundEntities.GetContext().SaveChanges();

                    MessageBox.Show("успешно добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppFrame.frame.Navigate(new mainPlayer());
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TracksList_Selected(object sender, RoutedEventArgs e)
        {
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            LoadTracks();
        }

        private void btnPlaylist_Click(object sender, RoutedEventArgs e)
        {
            addPlaylistsTrack secondWindow = new addPlaylistsTrack( (sender as Button).DataContext as Track);
            secondWindow.Show();
        }
    }
}

