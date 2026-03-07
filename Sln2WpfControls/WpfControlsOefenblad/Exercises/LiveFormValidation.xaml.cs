using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;
using System.Windows.Media;
using System.Linq;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Live Form Validation", Description = "TextChanged gebruiken voor live validatie en IsEnabled.", Order = 4, IsVisible = true)]
    public partial class LiveFormValidation : Page
    {
        public LiveFormValidation()
        {
            InitializeComponent();
        }
        private void txtPaswoord_TextChanged(object sender, TextChangedEventArgs e) 
        {
            string password = txtPaswoord.Text; 
            if (string.IsNullOrWhiteSpace(password)) 
            { 
                txtStatus.Text = "..."; 
                btnSave.IsEnabled = false; 
                return; 
            } 
            bool lengte = password.Length >= 8; 
            bool hoofdletter = password.Any(c => char.IsUpper(c)); 
            bool cijfer = password.Any(c => char.IsDigit(c)); 
            if (lengte && hoofdletter && cijfer) 
            { 
                txtStatus.Text = "Geldig paswoord"; 
                txtStatus.Foreground = Brushes.DarkGreen; 
                btnSave.IsEnabled = true; 
            } 
            else 
            { 
                txtStatus.Text = "Ongeldig paswoord"; 
                txtStatus.Foreground = Brushes.Red; 
                btnSave.IsEnabled = false; 
            } 
        }
    }
}
