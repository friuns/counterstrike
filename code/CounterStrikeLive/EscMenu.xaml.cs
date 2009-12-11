﻿using doru;
using CounterStrikeLive.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CounterStrikeLive
{
    public partial class EscMenu : ChildWindow
    {

        public static new EscMenu _This;
        public EscMenu()
        {
            _This = this;
            InitializeComponent();
			_ZoomText.Text = _Game._Scale.ToString();
            Loaded += new RoutedEventHandler(EscMenu_Loaded);
        }

        void EscMenu_Loaded(object sender, RoutedEventArgs e)
        {
            Volume.Value = LocalDatabase._This._Volume;
        }

    
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void VoteMap_Click(object sender, RoutedEventArgs e)
        {
            MapSelect _MapSelect = new MapSelect();
            _MapSelect.Success += delegate
            {

                Menu._This._Sender.Send(PacketType.voteMap, _MapSelect.Name.ToBytes());
                Menu._This._Chat.Text += Game._This._LocalClient._Nick + " Voted MapInfo " + _MapSelect.Name;
            };
        }

        private void full_screen_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Host.Content.IsFullScreen = App.Current.Host.Content.IsFullScreen == true ? false : true;
            this.DialogResult = true;
        }

        private void select_team_Click(object sender, RoutedEventArgs e)
        {
            new TeamSelect();
            this.DialogResult = true;
        }

        private void show_scoreboard_Click(object sender, RoutedEventArgs e)
        {
            Menu._This._ScoreBoard.Toggle();
            this.DialogResult = true;
        }

        private void text_message_Click(object sender, RoutedEventArgs e)
        {
            new ChatBox();
            this.DialogResult = true;
        }

        private void Change_Nick_Click(object sender, RoutedEventArgs e)
        {
            new EnterNick();
            this.DialogResult = true;
        }
		Game _Game = Game._This;
        
		private void ZoomOut_Click(object sender, RoutedEventArgs e)
		{
			_Game.ZoomOut();
			_ZoomText.Text = _Game._Scale.ToString();
		}
		
		private void ZoomIn_Click(object sender, RoutedEventArgs e)
		{
			_Game.ZoomIn();
			_ZoomText.Text = _Game._Scale.ToString();
		}

        

        

        private void Hard_Bots_Click(object sender, RoutedEventArgs e)
        {
            Easy_Bots.IsChecked = !Hard_Bots.IsChecked;
            
        }

        private void Easy_Bots_Click(object sender, RoutedEventArgs e)
        {
            Hard_Bots.IsChecked = !Easy_Bots.IsChecked;
            
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (Mute.IsChecked ?? false) Volume.Value = 0;
            else Volume.Value = .5;

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(Volume!=null)
                LocalDatabase._This._Volume = Volume.Value;
        }
        
        


		
    }
}
