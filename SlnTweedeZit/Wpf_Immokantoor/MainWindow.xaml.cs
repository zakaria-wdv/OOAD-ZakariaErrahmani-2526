using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CLImmo.Data;
using CLImmo.Enums;
using CLImmo.Models;

namespace WpfImmo
{
    public partial class MainWindow : Window
    {
        private readonly PandRepository pandRepository = new();
        private readonly MakelaarRepository makelaarRepository = new();

        private List<Pand> alleWoningen = new();
        private List<Makelaar> alleMakelaars = new();

        public MainWindow()
        {
            InitializeComponent();

            // Databank aanmaken indien nodig (enkel de eerste keer)
            DatabaseInitializer.ZorgDatabankBestaat();

            LaadGegevens();
        }

        private void LaadGegevens()
        {
            alleMakelaars = makelaarRepository.GetAllMakelaars();
            alleWoningen = pandRepository.GetAllPanden();

            // "Alle makelaars" optie toevoegen bovenaan de filterlijst
            var makelaarKeuzes = new List<Makelaar> { new Makelaar("", "Alle", "makelaars") };
            makelaarKeuzes.AddRange(alleMakelaars);
            MakelaarFilterComboBox.ItemsSource = makelaarKeuzes;
            MakelaarFilterComboBox.SelectedIndex = 0;

            var labelKeuzes = new List<string> { "Alle" };
            labelKeuzes.AddRange(Enum.GetNames(typeof(Energielabel)));
            EnergielabelFilterComboBox.ItemsSource = labelKeuzes;
            EnergielabelFilterComboBox.SelectedIndex = 0;

            ToepassenFilters();
        }

        private void ToepassenFilters()
        {
            IEnumerable<Pand> resultaat = alleWoningen;

            if (EnergielabelFilterComboBox.SelectedItem is string label && label != "Alle")
                resultaat = resultaat.Where(p => p.Energielabel.ToString() == label);

            if (MakelaarFilterComboBox.SelectedItem is Makelaar makelaar && !string.IsNullOrEmpty(makelaar.Id))
                resultaat = resultaat.Where(p => p.MakelaarId == makelaar.Id);

            if (EnkelTeKoopCheckBox.IsChecked == true)
                resultaat = resultaat.Where(p => !p.IsVerkocht);

            GalerijListBox.ItemsSource = resultaat.ToList();
        }

        private void FilterGewijzigd(object sender, SelectionChangedEventArgs e) => ToepassenFilters();

        private void EnkelTeKoopGewijzigd(object sender, RoutedEventArgs e) => ToepassenFilters();

        private void GalerijListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DetailPanel.DataContext = GalerijListBox.SelectedItem;
        }

        private void MarkeerVerkochtButton_Click(object sender, RoutedEventArgs e)
        {
            if (DetailPanel.DataContext is Pand geselecteerd && !geselecteerd.IsVerkocht)
            {
                pandRepository.MarkeerVerkocht(geselecteerd.Id);
                LaadGegevens();
            }
        }

        private void NieuwPandButton_Click(object sender, RoutedEventArgs e)
        {
            var venster = new NieuwPandWindow(alleMakelaars) { Owner = this };

            if (venster.ShowDialog() == true)
            {
                LaadGegevens();
            }
        }
    }
}
