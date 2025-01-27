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
    /// Логика взаимодействия для WindowPlayer.xaml
    /// </summary>
    public partial class WindowPlayer : Window
    {
        public WindowPlayer()
        {
            InitializeComponent();
            AppFramePl.frame = MainFrame;
            AppConn.modeldb = new pureSoundEntities();
            AppFramePl.frame.Navigate(new player.mainPlayer());
            int authId = Convert.ToInt32(App.Current.Properties["idUser"].ToString());
            usernameTb.Text = AppConn.modeldb.usersTable
                .Where(x => x.idUser == authId)
                .Select(x => x.userName)
                .FirstOrDefault();
        }

        private void btnFav_Click(object sender, RoutedEventArgs e)
        {
            AppFramePl.frame.Navigate(new favouritePage());
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите вернуться в главное меню?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MainWindow firstWindow = new MainWindow();
                firstWindow.Show();
                AppFrame.frame.Navigate(new auth.authorization());
                this.Close();
            }
        }
    }
}
