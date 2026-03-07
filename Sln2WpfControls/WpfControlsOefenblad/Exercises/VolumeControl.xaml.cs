using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Volume Control", Description = "Slider gebruiken met ValueChanged en property-aanpassing.", Order = 7, IsVisible = true)]
    public partial class VolumeControl : Page
    {
        public VolumeControl()
        {
            InitializeComponent();
            sldVolume.ValueChanged += sldVolume_ValueChanged;
        }

        private void sldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double volume = sldVolume.Value;

            txtVolume.Text = $"Volume: {(int)volume}%";
            bdrVolume.Width = stpVolumeControl.ActualWidth * volume / 100;

            if (volume < 20)
                bdrVolume.Background = Brushes.Green;
            else if (volume < 40)
                bdrVolume.Background = Brushes.Yellow;
            else if (volume < 60)
                bdrVolume.Background = Brushes.Orange;
            else if (volume < 80)
                bdrVolume.Background = Brushes.Red;
            else
                bdrVolume.Background = Brushes.DarkRed;
        }
    }
}
