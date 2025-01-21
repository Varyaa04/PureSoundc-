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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PureSound.pages.player
{
    /// <summary>
    /// Логика взаимодействия для mainPlayer.xaml
    /// </summary>
    public partial class mainPlayer : Page
    {
        private usersTable _auth = new usersTable();

        public mainPlayer(usersTable authUser)
        {
            InitializeComponent();

            if (authUser != null)
            {
                _auth = authUser;
            }
            DataContext = _auth;

        }

    }
}
