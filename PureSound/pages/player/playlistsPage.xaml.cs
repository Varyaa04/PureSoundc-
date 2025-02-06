using PureSound.appCurr;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            playlists = new ObservableCollection<playlist>();
            LoadPlaylists();




        }

        public async void LoadPlaylists()
        {
            var playlistsTable = _dbContext.playlistsTable
         .Where(x => x.idUser == userId && x.idUser != null)
         .ToList();

            foreach (var playlist in playlistsTable)
            {
                int trackCount = _dbContext.playlistTracksTable
                    .Count(pt => pt.idPlaylist == playlist.idPlaylist);

                playlists.Add(new playlist
                {
                    namePlaylist = playlist.namePlaylist,
                    TrackCount = trackCount
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
                        MessageBox.Show(
                            "Плейлист успешно удален!", "Успех", MessageBoxButton.OK,MessageBoxImage.Information
                        );
                    }
                }
            }
        }
        public class playlist
        {
            public string namePlaylist { get; set; }
            public int TrackCount { get; set; }
        }
    }
}
