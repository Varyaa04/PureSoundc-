using PureSound.appCurr;
using PureSound.pages.userprofile;
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
        int authId;
        private usersTable user = new usersTable();

        public WindowPlayer()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AppFramePl.frame = MainFrame;
            AppConn.modeldb = new pureSoundEntities();
            AppFramePl.frame.Navigate(new player.mainPlayer());
            if (App.Current.Properties["idUser"] == null)
            {
                MessageBox.Show("Ошибка: ID пользователя не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            authId = Convert.ToInt32(App.Current.Properties["idUser"].ToString());
            user.idUser = authId;
            DataContext = user;
            tbId.Text = user.idUser.ToString();
            var userName = AppConn.modeldb.usersTable
                .Where(x => x.idUser == authId)
                .Select(x => x.userName)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Ошибка: Имя пользователя не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                usernameTb.Text = userName;
            }
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
                AppFramePl.frame.Navigate(new auth.authorization());
                this.Close();
            }
        }

        private void btnPlaylists_Click(object sender, RoutedEventArgs e)
        {
            AppFramePl.frame.Navigate(new playlistsPage());
        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            AppFramePl.frame.Navigate(new mainPlayer());
        }

        private void profileSee_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                MessageBox.Show("Ошибка: Кнопка не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dataContext = button.DataContext;
            if (dataContext == null)
            {
                MessageBox.Show("Ошибка: DataContext не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (button.Parent is StackPanel parentPanel)
            {
                if (parentPanel.Children[0] is TextBlock idTextBlock && int.TryParse(idTextBlock.Text, out int ID))
                {
                    if (dataContext is usersTable user)
                    {
                        user.idUser = ID; // Убедитесь, что это необходимо
                        userProfileWindow userProfile = new userProfileWindow(user);
                        userProfile.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: Данные пользователя не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"DataContext: {dataContext}");
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка: Невозможно получить ID пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Ошибка: Невозможно найти родительский StackPanel.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}