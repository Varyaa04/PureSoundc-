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
        private const string DeezerApiUrl = "https://api.deezer.com/search?q=id:";
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
        private int userId = Convert.ToInt32(App.Current.Properties["idUser"]);

        private pureSoundEntities _dbContext;


        public favouritePage()
        {
            InitializeComponent();
            _dbContext = new pureSoundEntities();
            Loaded += (sender, args) => LoadFavoriteTracks();

        }

        public async Task LoadFavoriteTracks()
        {
            Console.WriteLine(userId);
            if (_dbContext != null)
            {
                List<tableFavourite> favoriteTracks = _dbContext.tableFavourite.Where(f => f.idUser == userId).ToList();
                Console.WriteLine($"Favorite tracks count: {favoriteTracks.Count}");
                if (favoriteTracks.Count == 0)
                {
                    counterTB.Text = "Нет любимых треков.";
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"https://api.deezer.com/search?q=id:{(favoriteTracks.Select(f => f.idTrack))}";

                    try
                    {
                        string jsonResponse = await client.GetStringAsync(apiUrl);
                        Console.WriteLine($"JSON Response:\n{jsonResponse}");

                        if (string.IsNullOrEmpty(jsonResponse) || jsonResponse == "{\"data\":[],\"total\":0}")
                        {
                            counterTB.Text = "Нет любимых треков.";
                            return;
                        }

                        JObject json = JObject.Parse(jsonResponse);
                        JArray data = (JArray)json["data"];

                        if (data.Count == 0)
                        {
                            counterTB.Text = "Нет любимых треков.";
                            return;
                        }

                        Tracks.Clear();
                        foreach (JObject item in data)
                        {
                            if (item == null)
                            {
                                continue;
                            }

                            Tracks.Add(new Track
                            {
                                Id = (string)item["id"],
                                Title = (string)item["title"],
                                Artist = (string)item["artist"]["name"],
                                Duration = FormatDuration((int)item["duration"]),
                                CoverUrl = item["album"]["cover_medium"] != null ? (string)item["album"]["cover_medium"] : null
                            });
                        }

                        counterTB.Text = $"Загружено любимых треков: {Tracks.Count}";
                    }
                    catch (HttpRequestException ex)
                    {
                        Debug.WriteLine("\nException Caught!");
                        Debug.WriteLine("Message: {0}", ex.Message);
                        MessageBox.Show("Ошибка при загрузке любимых треков. Пожалуйста, попробуйте позже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Database context is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            
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
                //await SearchTracksAsync(query);
            }
        }

        private void btndel_Click(object sender, RoutedEventArgs e)
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

