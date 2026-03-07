using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    /// <summary>
    /// Interaction logic for SenderOxo.xaml
    /// </summary>
    [NavPage(Title = "Sender Oxo", Description = "OXO spel met één click event handler", Order = 8, IsVisible = true)]
    public partial class SenderOxo : Page
    {
        private bool isX = true;
        public SenderOxo()
        {
            InitializeComponent();
        }
        private void OxoButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
                return;

            if (isX)
                btn.Content = "X";
            else
                btn.Content = "O";

            btn.IsEnabled = false;

            isX = !isX;
        }
    }
}
