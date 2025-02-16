﻿using Newtonsoft.Json;
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
using static PureSound.pages.player.mainPlayer;

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

        private async Task LoadFavouriteTracksAsync()
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

                    //string jsonResponse = await client.GetStringAsync(DeezerApiUrl);

                    //Console.WriteLine(jsonResponse);


                    //var result = JsonConvert.DeserializeObject<DeezerResponse>(jsonResponse);

                    //Tracks.Clear();
                    //foreach (var item in result.Data)
                    //{
                    //    Tracks.Add(new Track
                    //    {
                    //        Id = item.Id,
                    //        Title = item.Title,
                    //        Artist = item.Artist.Name,
                    //        Duration = TimeSpan.FromSeconds(item.Duration).ToString(@"mm\:ss"),
                    //        CoverUrl = item.Album.CoverMedium
                    //    });
                    //}

                    var response = await client.GetStringAsync($"{DeezerApiUrl}{trackId}");
                    Debug.WriteLine($"Ответ от API Deezer для трека {trackId}: {response}");

                    var deezerTrack = JsonConvert.DeserializeObject<DeezerTrack>(response);

                    if (deezerTrack == null)
                    {
                        Debug.WriteLine($"Ошибка: данные о треке {trackId} отсутствуют или некорректны.");
                        return null;
                    }
                    else
                    {
                        Tracks.Clear();

                        return new Track
                        {
                            Id = deezerTrack.Id,
                            Title = deezerTrack.Title,
                            Artist = deezerTrack.Artist.Name,
                            Duration = FormatDuration(deezerTrack.Duration),
                            CoverUrl = deezerTrack.Album?.CoverMedium ?? "https://img.freepik.com/free-vector/vector-beam-musical-note-sticker_53876-127376.jpg?t=st=1739215131~exp=1739218731~hmac=96c235e09b80c4d7e3c775db47c6a317acb5b24aadf32514c2e81187bde1bc39&w=740" // Запасной URL
                        };
                    }

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
}