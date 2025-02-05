using PureSound.appCurr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для registration.xaml
    /// </summary>
    public partial class registration : Page
    {
        public registration()
        {
            InitializeComponent();
            tbname.MaxLength = 25;
            tblogin.MaxLength = 30;
            tbpass.MaxLength = 30;
            tbrepass.PasswordChar = '☭';

        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frame.Navigate(new authorization());
        }

        private void btnreg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    if (AppConn.modeldb.usersTable.Any(x => x.userLogin == tblogin.Text && x.userEmail == tbemail.Text))
                    {
                        MessageBox.Show("Пользователь с таким логином и email'ом уже существует!",
                            "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при регистрации: " + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (string.IsNullOrWhiteSpace(tblogin.Text) ||
                    string.IsNullOrWhiteSpace(tbname.Text) ||
                    string.IsNullOrWhiteSpace(tbpass.Text))
                {
                    MessageBox.Show("Все поля должны быть заполнены!",
                        "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (tbpass.Text != tbrepass.Password)
                {
                    MessageBox.Show("Пароли не совпадают!",
                       "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

                if (!Regex.IsMatch(tbemail.Text, emailPattern))
                {
                    MessageBox.Show("Введите корректный email!",
                        "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                usersTable userObj = new usersTable()
                {
                    userName = tbname.Text,
                    userEmail = tbemail.Text,
                    userLogin = tblogin.Text,
                    userPassword = tbpass.Text
                };
                AppConn.modeldb.usersTable.Add(userObj);
                AppConn.modeldb.SaveChanges();
                MessageBox.Show("Вы успешно зарегистрировались!",
                    "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                AppFrame.frame.Navigate(new authorization());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void tbrepass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tbpass.Text != tbrepass.Password || tbrepass.Password == "")
            {
                btnreg.IsEnabled = false;
                tbrepass.Background = Brushes.LightCoral;
                tbrepass.BorderBrush = Brushes.Red;
            }
            else
            {
                btnreg.IsEnabled = true;
                tbrepass.Background = Brushes.LightGreen;
                tbrepass.BorderBrush = Brushes.Green;

            }
        }

        private void tbpass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbpass.Text != tbrepass.Password || tbrepass.Password == "")
            {
                btnreg.IsEnabled = false;
                tbrepass.Background = Brushes.LightCoral;
                tbrepass.BorderBrush = Brushes.Red;
            }
            else
            {
                btnreg.IsEnabled = true;
                tbrepass.Background = Brushes.LightGreen;
                tbrepass.BorderBrush = Brushes.Green;

            }
        }

        private void tbname_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key < Key.A || e.Key > Key.Z) && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }
    }
}