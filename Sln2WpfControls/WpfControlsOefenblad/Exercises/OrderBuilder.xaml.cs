using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Order Builder", Description = "CheckBox + RadioButton met samenvatting en reset.", Order = 5, IsVisible = true)]
    public partial class OrderBuilder : Page
    {
        public OrderBuilder()
        {
            InitializeComponent();
        }
        private void btnBevestig_Click(object sender, RoutedEventArgs e)
        {
            string levering = "";

            if (rdbAfhalen.IsChecked == true) levering = "Afhalen";
            else if (rdbLevering.IsChecked == true) levering = "Levering";
            else if (rdbTerPlaatse.IsChecked == true) levering = "Ter plaatse";
            else
            {
                txtSummary.Text = "Kies eerst een leveringsmethode";
                return;
            }

            List<string> extras = new List<string>();
            if (chkKaas.IsChecked == true) extras.Add("Kaas");
            if (chkSpek.IsChecked == true) extras.Add("Spek");
            if (chkSaus.IsChecked == true) extras.Add("Extra saus");
            if (chkUi.IsChecked == true) extras.Add("Ui");

            txtSummary.Text = $"Levering: {levering}\nExtra's: {string.Join(", ", extras)}";
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            chkKaas.IsChecked = true;
            chkSpek.IsChecked = false;
            chkSaus.IsChecked = true;
            chkUi.IsChecked = false;

            rdbAfhalen.IsChecked = false;
            rdbLevering.IsChecked = false;
            rdbTerPlaatse.IsChecked = false;

            txtSummary.Text = "...";
        }
    }
}
