using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Welkom", Description = "TextBox met eenvoudige validatie.", Order = 2, IsVisible = true)]
    public partial class Welkom : Page
    {
        public Welkom()
        {
            InitializeComponent();
        }

        private void btnZegHallo_Click(object sender, RoutedEventArgs e)
        {
            string naam = txtNaam.Text.Trim();

            if (string.IsNullOrWhiteSpace(naam))
            {
                txtFout.Visibility = Visibility.Visible;
                txtWelkom.Text = "...";
            }
            else
            {
                txtFout.Visibility = Visibility.Hidden;
                txtWelkom.Text = $"Welkom, {naam}!";
            }
        }
    }
}