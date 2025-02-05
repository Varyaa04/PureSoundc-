using PureSound.appCurr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PureSound.pages.player
{
    /// <summary>
    /// Логика взаимодействия для addPlaylistsTrack.xaml
    /// </summary>
    public partial class addPlaylistsTrack : Window
    {
        int authId = Convert.ToInt32(App.Current.Properties["idUser"].ToString());
        public addPlaylistsTrack()
        {
            InitializeComponent();
            MessageBox.Show("Received ID: " + ReceivedId);
            cbPlaylists.ItemsSource = pureSoundEntities.GetContext().playlistsTable.Where(x => x.idUser == authId).Select(x => x.namePlaylist).ToList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button b = sender as Button;
                int ID = Convert.ToInt32(float.Parse(((b.Parent as StackPanel).Children[0] as TextBlock).Text));
                new playlistTracksTable()
                {
                   idPlaylist = cbPlaylists.SelectedIndex,
                   idPlTracks = ID
                };
                AppConn.modeldb.SaveChanges();
                MessageBox.Show("Песня успешно добавлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + ex.Message, "Ошибка!", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        public string ReceivedId { get; set; }
    }
}

