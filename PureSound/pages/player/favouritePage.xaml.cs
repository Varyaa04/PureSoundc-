using Newtonsoft.Json;
using PureSound.appCurr;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
    /// Логика взаимодействия для favouritePage.xaml
    /// </summary>
    public partial class favouritePage : Page
    {
        private const string DeezerApiUrl = "https://api.deezer.com/chart/0/tracks";
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();


        private async Task<IEnumerable<string>> GetFavoriteTrackIdsAsync()
        {
            return await pureSoundEntities.GetContext().tableFavourite  .Select(t => t.idTrack).ToListAsync();
        }


        public favouritePage()
        {
            InitializeComponent();
            LoadFavoriteTracks();


        }

        private async Task LoadFavoriteTracks()
        {
            try
            {
                var favoriteTrackIds = await GetFavoriteTrackIdsAsync();

                using (HttpClient client = new HttpClient())
                {
                    string jsonResponse = await client.GetStringAsync("https://api.deezer.com/track/");
                    var allTracks = JsonConvert.DeserializeObject<DeezerTrack[]>(jsonResponse).Where(t => favoriteTrackIds.Contains(t.Id));

                    var tracks = new ObservableCollection<Track>(allTracks.Select(t => new Track
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Artist = t.Artist?.Name ?? "Unknown Artist",
                        Duration = FormatDuration(t.Duration),
                        CoverUrl = t.Album?.CoverMedium ?? "https://via.placeholder.com/150"
                    }));

                    Dispatcher.Invoke(() =>
                    {
                        TracksList.ItemsSource = tracks;
                        counterTB.Text = "Загружено треков: " + tracks.Count;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show("Введите название песни или исполнителя.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task SearchTracksAsync(string query)
        {
            //try
            //{
            //    using (HttpClient client = new HttpClient())
            //    {
            //        string jsonResponse = await client.GetStringAsync(DeezerApiUrl);

            //        Console.WriteLine(jsonResponse);


            //        var result = JsonConvert.DeserializeObject<DeezerResponse>(jsonResponse);

            //        Tracks.Clear();
            //        foreach (var item in result.Data)
            //        {
            //            Tracks.Add(new Track
            //            {
            //                Id = item.Id,
            //                Title = item.Title,
            //                Artist = item.Artist.Name,
            //                Duration = TimeSpan.FromSeconds(item.Duration).ToString(@"mm\:ss"),
            //                CoverUrl = item.Album.CoverMedium
            //            });
            //        }

            //        counterTB.Text = "Загружено треков: " + Tracks.Count;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        //private async void LoadTracks()
        //{
        //    try
        //    {
        //        using (var context = new pureSoundEntities())
        //        {
        //            var favoriteIds = context.tableFavourite.Select(f => f.idTrack).ToList();

        //            foreach (var id in favoriteIds)
        //            {
        //                using (HttpClient client = new HttpClient())
        //                {
        //                    string trackUrl = $"https://api.deezer.com/track/{id}";
        //                    string jsonResponse = await client.GetStringAsync(trackUrl);

        //                    var result = JsonConvert.DeserializeObject<DeezerTrack>(jsonResponse);

        //                    FavoriteTracks.Add(new Track
        //                    {
        //                        Id = result.Id,
        //                        Title = result.Title,
        //                        Artist = result.Artist.Name,
        //                        Duration = FormatDuration(result.Duration),
        //                        CoverUrl = result.Album.CoverMedium
        //                    });
        //                }
        //            }

        //            counterTB.Text = "Загружено треков: " + FavoriteTracks.Count;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

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

      


    }
}

