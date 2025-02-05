﻿using PureSound.appCurr;
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
        private mainPlayer.Track tracks = new mainPlayer.Track();

        public addPlaylistsTrack(mainPlayer.Track track)
        {
            InitializeComponent();
            if(track != null)
            {
                tracks = track;
            }
            DataContext = track;
            ReceivedId = tracks.Id;
            Console.WriteLine("Received ID: " + ReceivedId);
            cbPlaylists.ItemsSource = pureSoundEntities.GetContext().
                playlistsTable.Where(x => x.idUser == authId).
                Select(x => x.namePlaylist).ToArray();
        }
            

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cbPlaylists.Text))
                {
                    MessageBox.Show("Выберите плейлист!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int idPlaylist = AppConn.modeldb.playlistsTable
                    .Where(x => x.namePlaylist == cbPlaylists.Text && x.idUser == authId)
                    .Select(x => x.idPlaylist)
                    .FirstOrDefault();

                if (idPlaylist == 0)
                {
                    MessageBox.Show("Плейлист не найден!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newPlaylistTrack = new playlistTracksTable()
                {
                    idPlaylist = idPlaylist,
                    idTracks = ReceivedId
                };

                AppConn.modeldb.playlistTracksTable.Add(newPlaylistTrack);
                AppConn.modeldb.SaveChanges();

                MessageBox.Show("Песня успешно добавлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public string ReceivedId { get; set; }
    }
}

