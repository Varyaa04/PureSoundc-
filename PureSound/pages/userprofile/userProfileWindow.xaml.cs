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
        public userProfileWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            tbEmail.IsEnabled = false;
            tbLogin.IsEnabled = false;
            tbName.IsEnabled = false;
            tbPassword.IsEnabled = false;
            cbSPass.IsEnabled = false;
            tbPassword.PasswordChar = '☭';
            string email = AppConn.modeldb.usersTable.Where(x => x.idUser == authId).Select(x => x.userEmail).FirstOrDefault();
            tbEmail.Text = email;
            tbLogin.Text = AppConn.modeldb.usersTable.Where(x => x.idUser == authId).Select(x => x.userLogin).ToString();
            tbPassword.Password = AppConn.modeldb.usersTable.Where(x => x.idUser == authId).Select(x => x.userPassword).ToString();
            tbName.Text = AppConn.modeldb.usersTable.Where(x => x.idUser == authId).Select(x => x.userName).ToString();
            
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
        }

        private void cbSPass_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void cbSPass_Unchecked(object sender, RoutedEventArgs e)
        {
            tbPassword.PasswordChar = '☭';
        }

        private void cbEdit_Unchecked(object sender, RoutedEventArgs e)
        {
            tbEmail.IsEnabled = false;
            tbLogin.IsEnabled = false;
            tbName.IsEnabled = false;
            tbPassword.IsEnabled = false;
            cbSPass.IsEnabled = false;
        }
    }
}
