using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CLImmo.Data;
using CLImmo.Enums;
using CLImmo.Models;
using CLImmo.Validation;

namespace WpfImmo
{
    public partial class NieuwPandWindow : Window
    {
        private readonly List<Makelaar> makelaars;
        private readonly PandValidator validator = new();
        private readonly PandRepository pandRepository = new();

        public NieuwPandWindow(List<Makelaar> makelaars)
        {
            InitializeComponent();
            this.makelaars = makelaars;

            TypeComboBox.ItemsSource = new[] { "Huis", "Appartement" };
            TypeComboBox.SelectedIndex = 0;

            EnergielabelComboBox.ItemsSource = Enum.GetNames(typeof(Energielabel));
            EnergielabelComboBox.SelectedIndex = 0;

            MakelaarComboBox.ItemsSource = makelaars;
            if (makelaars.Count > 0)
                MakelaarComboBox.SelectedIndex = 0;
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TuinPanel/LiftPanel bestaan nog niet bij het opstarten van de constructor
            // (SelectionChanged vuurt al bij ItemsSource toekennen), dus check op null.
            if (TuinPanel == null || LiftPanel == null) return;

            bool isHuis = TypeComboBox.SelectedItem as string == "Huis";
            TuinPanel.Visibility = isHuis ? Visibility.Visible : Visibility.Collapsed;
            LiftPanel.Visibility = isHuis ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OpslaanButton_Click(object sender, RoutedEventArgs e)
        {
            FoutTextBlock.Text = "";

            var adres = AdresTextBox.Text.Trim();
            if (!validator.IsGeldigAdres(adres))
            {
                FoutTextBlock.Text = $"Ongeldig adres: minstens {validator.MinimumAantalTekensAdres} tekens, " +
                                      "enkel letters, cijfers, spaties en de tekens , . -";
                return;
            }

            if (!double.TryParse(PrijsTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double prijs)
                || !validator.IsGeldigePrijs(prijs))
            {
                FoutTextBlock.Text = "Ongeldige prijs: moet een getal groter dan 0 zijn.";
                return;
            }

            if (!int.TryParse(OppervlakteTextBox.Text, out int oppervlakte) || oppervlakte <= 0)
            {
                FoutTextBlock.Text = "Ongeldige oppervlakte: moet een geheel getal groter dan 0 zijn.";
                return;
            }

            if (!int.TryParse(BouwjaarTextBox.Text, out int bouwjaar) || bouwjaar < 1800 || bouwjaar > DateTime.Now.Year)
            {
                FoutTextBlock.Text = $"Ongeldig bouwjaar: moet tussen 1800 en {DateTime.Now.Year} liggen.";
                return;
            }

            if (MakelaarComboBox.SelectedItem is not Makelaar gekozenMakelaar)
            {
                FoutTextBlock.Text = "Kies een makelaar.";
                return;
            }

            var energielabel = Pand.ParseEnergielabel(EnergielabelComboBox.SelectedItem as string ?? "C");
            string? foto = string.IsNullOrWhiteSpace(FotoTextBox.Text) ? null : FotoTextBox.Text.Trim();

            Pand nieuwPand;

            if (TypeComboBox.SelectedItem as string == "Huis")
            {
                if (!int.TryParse(TuinTextBox.Text, out int tuin) || tuin < 0)
                {
                    FoutTextBlock.Text = "Ongeldige tuinoppervlakte: moet een geheel getal zijn (0 of meer).";
                    return;
                }

                nieuwPand = new Huis(adres, gekozenMakelaar.Id, prijs, oppervlakte, energielabel, bouwjaar, tuin, foto);
            }
            else
            {
                bool heeftLift = LiftCheckBox.IsChecked == true;
                nieuwPand = new Appartement(adres, gekozenMakelaar.Id, prijs, oppervlakte, energielabel, bouwjaar, heeftLift, foto);
            }

            pandRepository.VoegPandToe(nieuwPand);

            DialogResult = true;
            Close();
        }

        private void AnnulerenButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
