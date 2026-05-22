using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFDokter.Pages;

// Overzicht van alle patiënten als grid van contact-cards
// Cards worden dynamisch aangemaakt in code-behind (zoals in SlnDemoItemsPanel)
public partial class PatientenOverzichtPage : Page
{
    // Volledige lijst van patiënten (om te filteren zonder telkens DB-call)
    private List<Patient> allePatienten = new List<Patient>();

    public PatientenOverzichtPage()
    {
        InitializeComponent();
        LaadPatienten();
    }

    // Haalt alle patiënten op en toont ze
    private void LaadPatienten()
    {
        txtFoutmelding.Text = "";
        try
        {
            allePatienten = Patient.GeefAllePatienten();
            TekenCards(allePatienten);
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij laden patiënten: " + ex.Message;
        }
    }

    // Bouwt de cards op uit de gegeven lijst
    private void TekenCards(List<Patient> patienten)
    {
        // Eerst de panel leegmaken
        cardsPanel.Children.Clear();

        for (int i = 0; i < patienten.Count; i++)
        {
            Patient patient = patienten[i];
            Border card = MaakPatientCard(patient);
            cardsPanel.Children.Add(card);
        }
    }

    // Maakt een visuele card voor een patiënt
    // Bevat foto, naam, email, gsm en drie knoppen (details, wijzig, verwijder)
    private Border MaakPatientCard(Patient patient)
    {
        Border border = new Border();
        border.Width = 260;
        border.Height = 320;
        border.Margin = new Thickness(8);
        border.Background = new SolidColorBrush(Color.FromRgb(0xF7, 0xFA, 0xFA));
        border.BorderBrush = new SolidColorBrush(Color.FromRgb(0xCB, 0xD5, 0xE0));
        border.BorderThickness = new Thickness(1);
        border.CornerRadius = new CornerRadius(8);
        border.Padding = new Thickness(15);

        StackPanel stack = new StackPanel();

        // Profielfoto in een cirkel
        Border fotoBorder = new Border();
        fotoBorder.Width = 80;
        fotoBorder.Height = 80;
        fotoBorder.CornerRadius = new CornerRadius(40);
        fotoBorder.Background = new SolidColorBrush(Color.FromRgb(0xE2, 0xE8, 0xF0));
        fotoBorder.HorizontalAlignment = HorizontalAlignment.Center;
        fotoBorder.Margin = new Thickness(0, 0, 0, 10);
        fotoBorder.ClipToBounds = true;

        Image foto = new Image();
        foto.Stretch = Stretch.UniformToFill;
        if (patient.ProfielFotoData != null)
        {
            try
            {
                BitmapImage bm = new BitmapImage();
                bm.BeginInit();
                bm.CacheOption = BitmapCacheOption.OnLoad;
                bm.StreamSource = new MemoryStream(patient.ProfielFotoData);
                bm.EndInit();
                foto.Source = bm;
            }
            catch
            {
                // Negeer foto-fouten
            }
        }
        fotoBorder.Child = foto;
        stack.Children.Add(fotoBorder);

        // Naam
        TextBlock txtNaam = new TextBlock();
        txtNaam.Text = patient.VolledigeNaam;
        txtNaam.FontWeight = FontWeights.Bold;
        txtNaam.FontSize = 16;
        txtNaam.HorizontalAlignment = HorizontalAlignment.Center;
        txtNaam.Margin = new Thickness(0, 0, 0, 5);
        stack.Children.Add(txtNaam);

        // Email
        TextBlock txtEmail = new TextBlock();
        txtEmail.Text = patient.Email;
        txtEmail.FontSize = 12;
        txtEmail.Foreground = new SolidColorBrush(Color.FromRgb(0x4A, 0x55, 0x68));
        txtEmail.HorizontalAlignment = HorizontalAlignment.Center;
        txtEmail.Margin = new Thickness(0, 0, 0, 3);
        stack.Children.Add(txtEmail);

        // GSM
        TextBlock txtGsm = new TextBlock();
        txtGsm.Text = patient.Gsm ?? "(geen gsm)";
        txtGsm.FontSize = 12;
        txtGsm.Foreground = new SolidColorBrush(Color.FromRgb(0x4A, 0x55, 0x68));
        txtGsm.HorizontalAlignment = HorizontalAlignment.Center;
        txtGsm.Margin = new Thickness(0, 0, 0, 15);
        stack.Children.Add(txtGsm);

        // Knoppen onderaan
        StackPanel knoppen = new StackPanel();
        knoppen.Orientation = Orientation.Horizontal;
        knoppen.HorizontalAlignment = HorizontalAlignment.Center;

        Button btnDetails = MaakKnop("Details", Color.FromRgb(0x2D, 0x2D, 0x2D));
        // Closure: bewaar de patient referentie in de click handler
        btnDetails.Click += delegate (object s, RoutedEventArgs e) { GaNaarDetails(patient); };
        knoppen.Children.Add(btnDetails);

        Button btnWijzig = MaakKnop("Wijzig", Color.FromRgb(0x4B, 0x55, 0x63));
        btnWijzig.Click += delegate (object s, RoutedEventArgs e) { GaNaarWijzig(patient); };
        knoppen.Children.Add(btnWijzig);

        Button btnVerwijder = MaakKnop("Verwijder", Color.FromRgb(0xB9, 0x1C, 0x1C));
        btnVerwijder.Click += delegate (object s, RoutedEventArgs e) { VerwijderPatient(patient); };
        knoppen.Children.Add(btnVerwijder);

        stack.Children.Add(knoppen);

        border.Child = stack;
        return border;
    }

    // Helper voor het maken van een gestyleerde knop
    private Button MaakKnop(string tekst, Color kleur)
    {
        Button btn = new Button();
        btn.Content = tekst;
        btn.Background = new SolidColorBrush(kleur);
        btn.Foreground = Brushes.White;
        btn.BorderThickness = new Thickness(0);
        btn.Padding = new Thickness(8, 4, 8, 4);
        btn.Margin = new Thickness(3);
        btn.FontSize = 11;
        return btn;
    }

    // Filter de cards op basis van wat in de zoekbalk staat
    private void txtZoek_TextChanged(object sender, TextChangedEventArgs e)
    {
        string zoekterm = txtZoek.Text.Trim().ToLower();
        if (string.IsNullOrEmpty(zoekterm))
        {
            TekenCards(allePatienten);
            return;
        }

        // Filter zonder LINQ (verboden in deze cursus)
        List<Patient> gefilterd = new List<Patient>();
        for (int i = 0; i < allePatienten.Count; i++)
        {
            Patient p = allePatienten[i];
            if (p.Voornaam.ToLower().Contains(zoekterm) ||
                p.Achternaam.ToLower().Contains(zoekterm))
            {
                gefilterd.Add(p);
            }
        }
        TekenCards(gefilterd);
    }

    // Knop voor nieuwe patiënt
    private void btnNieuw_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new PatientBewerkenPage(null));
        }
    }

    // Navigatie naar details
    private void GaNaarDetails(Patient patient)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new PatientDetailsPage(patient));
        }
    }

    // Navigatie naar wijzigen
    private void GaNaarWijzig(Patient patient)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new PatientBewerkenPage(patient));
        }
    }

    // Verwijdert een patiënt na bevestiging
    private void VerwijderPatient(Patient patient)
    {
        MessageBoxResult resultaat = MessageBox.Show(
            "Wil u patiënt '" + patient.VolledigeNaam + "' definitief verwijderen?\n" +
            "Alle gekoppelde afspraken worden ook verwijderd.",
            "Bevestiging",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (resultaat != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            patient.Verwijderen();
            LaadPatienten();
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij verwijderen: " + ex.Message;
        }
    }
}
