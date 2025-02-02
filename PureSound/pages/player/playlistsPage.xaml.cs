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
            var playlistsTable = _dbContext.playlistsTable.Where(x => x.idUser == userId && x.idUser != null).ToList();

            foreach (var playlist in playlistsTable)
            {
                playlists.Add(new playlist { namePlaylist = playlist.namePlaylist });
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
    }
    public class playlist
    {
        public string namePlaylist { get; set; }
    }
}
