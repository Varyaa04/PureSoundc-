using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using PureSound.appCurr;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Логика взаимодействия для playlistTracks.xaml
    /// </summary>
    public partial class playlistTracks : Page
    {
        public playlistsTable _currentPlaylist;
        private pureSoundEntities _dbContext;
        private const string DeezerApiUrl = "https://api.deezer.com/track/";
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();

        public playlistTracks(playlistsTable currentPlaylist)
        {
            InitializeComponent();
            _currentPlaylist = currentPlaylist;
            DataContext = this;
            _dbContext = new pureSoundEntities();

            // Устанавливаем название плейлиста
            namePlTb.Text = _dbContext.playlistsTable
                .Where(x => x.idPlaylist == currentPlaylist.idPlaylist)
                .Select(x => x.namePlaylist)
                .FirstOrDefault() ?? "Название плейлиста";

            // Загружаем треки плейлиста
            LoadPlaylistTracksAsync();
        }

        private async void LoadPlaylistTracksAsync()
        {
            try
            {
                var playlistTracks = _dbContext.playlistTracksTable
                    .Where(f => f.idPlaylist == _currentPlaylist.idPlaylist)
                    .Select(f => f.idTracks)
                    .ToList();

                Tracks.Clear();

                foreach (var trackId in playlistTracks)
                {
                    var track = await GetTrackFromDeezerAsync(trackId);
                    if (track != null)
                    {
                        Application.Current.Dispatcher.Invoke(() => Tracks.Add(track));
                    }
                }

                Application.Current.Dispatcher.Invoke(() => TracksList.ItemsSource = Tracks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке треков плейлиста: {ex.Message}");
            }
            finally
            {
                _dbContext.Dispose();
            }
        }

        private async Task<Track> GetTrackFromDeezerAsync(string trackId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"{DeezerApiUrl}{trackId}");
                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Ошибка: {response.StatusCode} для трека {trackId}");
                        return null;
                    }

                    var responseData = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Ответ от API Deezer для трека {trackId}: {responseData}");

                    var deezerTrack = JsonConvert.DeserializeObject<DeezerTrack>(responseData);

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
            var selectedTrack = button?.DataContext as Track;

            if (selectedTrack != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы точно хотите удалить выбранный трек из плейлиста?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    var trackToDelete = _dbContext.playlistTracksTable
                        .FirstOrDefault(p => p.idTracks == selectedTrack.Id && p.idPlaylist == _currentPlaylist.idPlaylist);

                    if (trackToDelete != null)
                    {
                        _dbContext.playlistTracksTable.Remove(trackToDelete);
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
            LoadPlaylistTracksAsync();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();
            FilterTracks(query);
        }

        private void TracksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        public DeezerArtist Artist { get; set; }
        public int Duration { get; set; }
        public DeezerAlbum Album { get; set; }
    }

    public class DeezerArtist
    {
        public string Name { get; set; }
    }

    public class DeezerAlbum
    {
        public string CoverMedium { get; set; }
    }
}