using PureSound.appCurr;
using PureSound.pages.userprofile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
    /// Логика взаимодействия для playlistsPage.xaml
    /// </summary>
    public partial class playlistsPage : Page
    {
        private int userId = Convert.ToInt32(App.Current.Properties["idUser"]);
        private ObservableCollection<playlist> playlists { get; set; }
        private pureSoundEntities _dbContext;

        public playlistsPage()
        {
            InitializeComponent();
            _dbContext = new pureSoundEntities();
            UpdateUI(); // Обновляем интерфейс
            playlists = new ObservableCollection<playlist>();
            LoadPlaylists();
        }

        private void UpdateUI()
        {
            if (_dbContext.playlistsTable.Any(x => x.idUser == userId))
            {
                btnAddNull.Visibility = Visibility.Hidden;
                nullL.Visibility = Visibility.Hidden;
                btnAdd.Visibility = Visibility.Visible;
                playlistList.Visibility = Visibility.Visible;
            }
            else
            {
                btnAddNull.Visibility = Visibility.Visible;
                nullL.Visibility = Visibility.Visible;
                btnAdd.Visibility = Visibility.Hidden;
                playlistList.Visibility = Visibility.Hidden;
            }
        }

        public async void LoadPlaylists()
        {
            playlists.Clear(); // Очищаем коллекцию перед загрузкой

            var playlistsTable = _dbContext.playlistsTable
                .Where(x => x.idUser == userId)
                .ToList();

            foreach (var playlist in playlistsTable)
            {
                int trackCount = _dbContext.playlistTracksTable
                    .Count(pt => pt.idPlaylist == playlist.idPlaylist);

                playlists.Add(new playlist
                {
                    namePlaylist = playlist.namePlaylist,
                    TrackCount = trackCount,
                    idPlaylist = playlist.idPlaylist // Добавьте это, если нужно
                });
            }

            playlistList.ItemsSource = playlists;
        }

        private void btnAddNull_Click(object sender, RoutedEventArgs e)
        {
            addPlaylists firstWindow = new addPlaylists();
            firstWindow.Show();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            addPlaylists firstWindow = new addPlaylists();
            firstWindow.Show();
        }

        private void playlistBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylist = playlistList.SelectedItem as playlist;

            if (selectedPlaylist != null)
            {
                var currentPlaylist = _dbContext.playlistsTable
                    .FirstOrDefault(p => p.namePlaylist == selectedPlaylist.namePlaylist && p.idUser == userId);

                if (currentPlaylist != null)
                {
                    AppFramePl.frame.Navigate(new playlistTracks(currentPlaylist));
                }
                else
                {
                    MessageBox.Show("Плейлист не найден в базе данных.");
                }
            }
            else
            {
                MessageBox.Show("Плейлист не выбран.");
            }
        }

        private void delPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedPlaylist = button.DataContext as playlist;

            if (selectedPlaylist != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы точно хотите удалить выбранный плейлист?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    var playlistToDelete = _dbContext.playlistsTable
                        .FirstOrDefault(p => p.namePlaylist == selectedPlaylist.namePlaylist && p.idUser == userId);

                    if (playlistToDelete != null)
                    {
                        _dbContext.playlistsTable.Remove(playlistToDelete);
                        _dbContext.SaveChanges();

                        playlists.Remove(selectedPlaylist);
                        playlistList.ItemsSource = null;
                        playlistList.ItemsSource = playlists;

                        UpdateUI(); // Обновляем интерфейс после удаления

                        MessageBox.Show(
                            "Плейлист успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information
                        );
                    }
                }
            }
        }

        public class playlist
        {
            public string namePlaylist { get; set; }
            public int TrackCount { get; set; }
            public int idPlaylist { get; set; } // Добавьте это, если нужно
        }
    }
}