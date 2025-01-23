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

namespace PureSound.pages.player
{
    /// <summary>
    /// Логика взаимодействия для mainPlayer.xaml
    /// </summary>
    public partial class mainPlayer : Page
    {
        private usersTable _auth = new usersTable();

        private const string DeezerApiUrl = "https://api.deezer.com/search?q=top";
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
        public mainPlayer(usersTable authUser)
        {
            InitializeComponent();

            if (authUser != null)
            {
                _auth = authUser;
            }
            DataContext = _auth;

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
                MessageBox.Show("Введите название песни или исполнителя.");
            }
        }
        private async Task SearchTracksAsync(string query)
        {
            try
            {
                Tracks.Clear();
                string requestUrl = DeezerApiUrl + Uri.EscapeDataString(query);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<DeezerResponse>(jsonResponse);

                foreach (var item in result.Data)
                {
                    Tracks.Add(new Track
                    {
                        Title = item.Title,
                        Artist = item.Artist.Name,
                        Duration = FormatDuration(item.Duration),
                        CoverUrl = item.Album.CoverMedium
                    });
                }

                if (Tracks.Count == 0)
                {
                    MessageBox.Show("Ничего не найдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
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
                            Title = item.Title,
                            Artist = item.Artist.Name,
                            Duration = TimeSpan.FromSeconds(item.Duration).ToString(@"mm\:ss"),
                            CoverUrl = item.Album.CoverMedium
                        });
                    }

                    MessageBox.Show($"Загружено треков: {Tracks.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private string FormatDuration(int durationInSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(durationInSeconds);
            return time.ToString(@"mm\:ss");
        }

        public class Track
        {
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
    }
}
