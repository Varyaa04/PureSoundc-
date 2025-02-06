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
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PureSound.Properties;

namespace PureSound.pages.player
{
    /// <summary>
    /// Логика взаимодействия для favouritePage.xaml
    /// </summary>
    public partial class favouritePage : Page
    {
        private const string DeezerApiUrl = "https://api.deezer.com/track/";
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
        private int userId = Convert.ToInt32(App.Current.Properties["idUser"]);

        private pureSoundEntities _dbContext;

        public favouritePage()
        {
            InitializeComponent();
            _dbContext = new pureSoundEntities();
            LoadFavouriteTracksAsync();
        }

        private async void LoadFavouriteTracksAsync()
        {
            try
            {
                // Получаем избранные треки пользователя из базы данных
                var favouriteTracks = _dbContext.tableFavourite
                    .Where(f => f.idUser == userId)
                    .Select(f => f.idTrack)
                    .ToList();

                // Очищаем текущий список треков
                Tracks.Clear();

                // Загружаем данные о каждом треке из API Deezer
                foreach (var trackId in favouriteTracks)
                {
                    var track = await GetTrackFromDeezerAsync(trackId);
                    if (track != null)
                    {
                        Tracks.Add(track);
                    }
                }

                // Привязываем список треков к ListView
                TracksList.ItemsSource = Tracks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке избранных треков: {ex.Message}");
            }
        }

        private async Task<Track> GetTrackFromDeezerAsync(string trackId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync($"{DeezerApiUrl}{trackId}");
                    var deezerTrack = JsonConvert.DeserializeObject<DeezerTrack>(response);

                    return new Track
                    {
                        Id = deezerTrack.Id,
                        Title = deezerTrack.Title,
                        Artist = deezerTrack.Artist     ,
                        Duration = FormatDuration(deezerTrack.Duration),
                        CoverUrl = deezerTrack.Album
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка при получении трека {trackId}: {ex.Message}");
                    return null;
                }
            }
        }

        private string FormatDuration(int durationInSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(durationInSeconds);
            return time.ToString(@"mm\:ss");
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.Trim();
            if (!string.IsNullOrEmpty(query))
            {
                // Реализуйте поиск, если необходимо
            }
        }

        private void btndel_Click(object sender, RoutedEventArgs e)
        {
            // Реализуйте удаление треков, если необходимо
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализуйте сброс, если необходимо
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Track
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Duration { get; set; }
        public string CoverUrl { get; set; }
    }

    public class DeezerTrack
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
    }

}