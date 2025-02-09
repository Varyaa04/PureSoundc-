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
using static PureSound.pages.player.playlistsPage;

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
                var favouriteTracks = _dbContext.tableFavourite
                    .Where(f => f.idUser == userId)
                    .Select(f => f.idTrack)
                    .ToList();

                Tracks.Clear();

                foreach (var trackId in favouriteTracks)
                {
                    var track = await GetTrackFromDeezerAsync(trackId);
                    if (track != null)
                    {
                        Tracks.Add(track);
                    }
                }

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
                    Debug.WriteLine($"Ответ от API Deezer для трека {trackId}: {response}");

                    var deezerTrack = JsonConvert.DeserializeObject<DeezerTrack>(response);

                    if (deezerTrack == null || deezerTrack.Artist == null || deezerTrack.Album == null)
                    {
                        Debug.WriteLine($"Ошибка: данные о треке {trackId} отсутствуют или некорректны.");
                        return null;
                    }

                    return new Track
                    {
                        Id = deezerTrack.Id,
                        Title = deezerTrack.Title,
                        Artist = deezerTrack.Artist.Name,
                        Duration = FormatDuration(deezerTrack.Duration),
                        CoverUrl = deezerTrack.Album.CoverMedium
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
            FilterTracks(query);
        }

        private void FilterTracks(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                TracksList.ItemsSource = Tracks;
            }
            else
            {
                var filteredTracks = Tracks
                    .Where(track => track.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                    track.Artist.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                TracksList.ItemsSource = filteredTracks;
            }
        }

        private void btndel_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedTrack = button.DataContext as Track;

            if (selectedTrack != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы точно хотите удалить выбранный трек из 'Избранного'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    var trackToDelete = _dbContext.tableFavourite
                        .FirstOrDefault(p => p.idTrack == selectedTrack.Id && p.idUser == userId);

                    if (trackToDelete != null)
                    {
                        _dbContext.tableFavourite.Remove(trackToDelete);
                        _dbContext.SaveChanges();
    
                        Tracks.Remove(selectedTrack);
                        MessageBox.Show(
                            "Трек успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information
                        );
                    }
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            LoadFavouriteTracksAsync();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();
            FilterTracks(query);
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