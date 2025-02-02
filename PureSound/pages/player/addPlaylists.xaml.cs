using PureSound.appCurr;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PureSound.pages.player
{
    /// <summary>
    /// Логика взаимодействия для addPlaylists.xaml
    /// </summary>
    public partial class addPlaylists : Window
    {
        public addPlaylists()
        {
            InitializeComponent();
        }

        private void addPlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nameP = namePl.Text;
                int idUsers = Convert.ToInt32(App.Current.Properties["idUser"]);

                var existingFavorite = pureSoundEntities.GetContext().playlistsTable.FirstOrDefault(o => o.idUser == idUsers && o.namePlaylist == nameP);

                if (existingFavorite != null)
                {
                    MessageBox.Show("Плейлист с таким именем уже существует!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var plTable = new playlistsTable()
                    {
                        idUser = idUsers,
                        namePlaylist = nameP,
                    };
                    pureSoundEntities.GetContext().playlistsTable.Add(plTable);
                    pureSoundEntities.GetContext().SaveChanges();

                    MessageBox.Show("Плейлист успешно добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        
    }
    }
}
