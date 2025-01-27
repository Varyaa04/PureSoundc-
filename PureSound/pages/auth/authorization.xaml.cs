using NuGet.Protocol.Plugins;
using PureSound.appCurr;
using PureSound.pages.player;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PureSound.pages.auth
{
    /// <summary>
    /// Логика взаимодействия для authorization.xaml
    /// </summary>
    public partial class authorization : Page
    {
        public authorization()
        {
            InitializeComponent();
        }

        private void btnreg_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frame.Navigate(new registration());
        }

        private void btnauth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var password = AppConn.modeldb.usersTable.FirstOrDefault(x => x.userPassword == passtxt.Password);
                var login = AppConn.modeldb.usersTable.FirstOrDefault(x => x.userLogin == logtxt.Text);
                var userObj = AppConn.modeldb.usersTable.FirstOrDefault(x => x.userLogin == logtxt.Text && x.userPassword == passtxt.Password);
                if (password == null)
                {
                    MessageBox.Show("Неверный пароль",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (userObj == null)
                {
                    MessageBox.Show("Такого пользователя не существует!", "Ошибка авторизации!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Здравствуйте, " + userObj.userName + "!",
                                "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    App.Current.Properties["idUser"] = userObj.idUser;
                    WindowPlayer secondWindow = new WindowPlayer();
                    secondWindow.Show();
                    Application.Current.MainWindow.Close();

                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ошибка " + Ex.Message.ToString() + " Критическая работа приложения!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
