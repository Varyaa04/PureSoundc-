using PureSound.appCurr;
using PureSound.pages.player;
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

namespace PureSound.pages.userprofile
{
    /// <summary>
    /// Логика взаимодействия для userProfileWindow.xaml
    /// </summary>
    /// 
    public partial class userProfileWindow : Window
    {
        int authId = Convert.ToInt32(App.Current.Properties["idUser"].ToString());
        private usersTable curUser;

        public userProfileWindow(usersTable selUser)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (selUser == null)
            {
                MessageBox.Show("Пользователь не выбран!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close(); 
                return;
            }
            
            curUser = selUser;
            curUser.idUser = authId; 
            DataContext = curUser;

            Console.WriteLine("PROFILE ID " +curUser.idUser);

            tbEmail.Text = AppConn.modeldb.usersTable
                .Where(x => x.idUser == curUser.idUser)
                .Select(x => x.userEmail)
                .FirstOrDefault() ?? string.Empty;

            tbName.Text = AppConn.modeldb.usersTable
               .Where(x => x.idUser == curUser.idUser)
               .Select(x => x.userName)
               .FirstOrDefault() ?? string.Empty;

            tbLogin.Text = AppConn.modeldb.usersTable
               .Where(x => x.idUser == curUser.idUser)
               .Select(x => x.userLogin)
               .FirstOrDefault() ?? string.Empty;

            tbPassword.Password = AppConn.modeldb.usersTable
               .Where(x => x.idUser == curUser.idUser)
               .Select(x => x.userPassword)
               .FirstOrDefault() ?? string.Empty;

            tbEmail.IsEnabled = false;
            tbLogin.IsEnabled = false;
            tbName.IsEnabled = false;
            tbPassword.IsEnabled = false;
            btnSave.IsEnabled = false;
            tbPasswordVisible.IsEnabled = false;
            cbSPass.IsEnabled = false;

            tbPassword.PasswordChar = '☭';
            tbPasswordVisible.Visibility = Visibility.Hidden;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            WindowPlayer secondWindow = new WindowPlayer();
            secondWindow.Show();
            this.Close();
        }

        private void cbEdit_Checked(object sender, RoutedEventArgs e)
        {
            tbEmail.IsEnabled = true;
            tbLogin.IsEnabled = true;
            tbName.IsEnabled = true;
            tbPassword.IsEnabled = true;
            cbSPass.IsEnabled = true;
            tbPasswordVisible.IsEnabled = true;
            btnSave.IsEnabled=true;
        }

        private void cbSPass_Checked(object sender, RoutedEventArgs e)
        {
            tbPasswordVisible.Text = tbPassword.Password; 
            tbPasswordVisible.Visibility = Visibility.Visible;
            tbPassword.Visibility = Visibility.Hidden;
        }

        private void cbSPass_Unchecked(object sender, RoutedEventArgs e)
        {
            tbPassword.PasswordChar = '☭';
            tbPassword.Password = tbPasswordVisible.Text; 
            tbPasswordVisible.Visibility = Visibility.Hidden; 
            tbPassword.Visibility = Visibility.Visible;
        }

        private void cbEdit_Unchecked(object sender, RoutedEventArgs e)
        {
            tbEmail.IsEnabled = false;
            tbLogin.IsEnabled = false;
            tbName.IsEnabled = false;
            tbPassword.IsEnabled = false;
            cbSPass.IsEnabled = false;
            tbPasswordVisible.IsEnabled = false;
            btnSave.IsEnabled = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int userId = curUser.idUser;
                var userToUpdate = AppConn.modeldb.usersTable.FirstOrDefault(u => u.idUser == userId);

                if (userToUpdate != null)
                {
                    userToUpdate.userName = tbName.Text;
                    userToUpdate.userEmail = tbEmail.Text;
                    userToUpdate.userPassword = tbPassword.Password;
                    userToUpdate.userLogin = tbLogin.Text;

                    AppConn.modeldb.SaveChanges();
                    MessageBox.Show("Данные успешно изменены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    WindowPlayer playerw = new WindowPlayer();
                    playerw.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь не найден!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
