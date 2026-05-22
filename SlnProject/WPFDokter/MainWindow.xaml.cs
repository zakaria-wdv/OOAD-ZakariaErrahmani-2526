using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;
using WPFDokter.Pages;

namespace WPFDokter;

// Hoofdvenster van de dokter-applicatie
// Bevat de navigatie-knoppen aan de linkerkant en een Frame voor de pagina's
public partial class MainWindow : Window
{
    // Static referentie zodat pagina's kunnen navigeren via MainWindow.Instance
    public static MainWindow? Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        // Start meteen op de start-pagina
        mainFrame.Navigate(new StartPage());
    }

    // Wordt door pagina's aangeroepen na succesvol inloggen
    // Update de UI om de naam en foto te tonen, en activeert de knoppen
    public void UpdateNaLogin()
    {
        Dokter? dokter = Sessie.IngelogdeDokter;
        if (dokter == null)
        {
            return;
        }

        // Toon naam in de header
        txtIngelogdeNaam.Text = "Dr. " + dokter.VolledigeNaam;

        // Toon foto indien beschikbaar
        if (dokter.ProfielFotoData != null)
        {
            try
            {
                // Maak een BitmapImage van de byte array
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(dokter.ProfielFotoData);
                bitmap.EndInit();
                imgIngelogdeFoto.Source = bitmap;
            }
            catch
            {
                // Negeer foto-fouten - geen impact op functionaliteit
                imgIngelogdeFoto.Source = null;
            }
        }

        // Activeer de overige knoppen
        btnAfspraken.IsEnabled = true;
        btnPatienten.IsEnabled = true;
        btnUitloggen.IsEnabled = true;
    }

    // Navigatie-handlers voor de knoppen
    private void btnStart_Click(object sender, RoutedEventArgs e)
    {
        mainFrame.Navigate(new StartPage());
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        mainFrame.Navigate(new LoginPage());
    }

    private void btnAfspraken_Click(object sender, RoutedEventArgs e)
    {
        mainFrame.Navigate(new AfsprakenPage());
    }

    private void btnPatienten_Click(object sender, RoutedEventArgs e)
    {
        mainFrame.Navigate(new PatientenOverzichtPage());
    }

    private void btnUitloggen_Click(object sender, RoutedEventArgs e)
    {
        // Reset de sessie en UI
        Sessie.Logout();
        txtIngelogdeNaam.Text = "";
        imgIngelogdeFoto.Source = null;
        btnAfspraken.IsEnabled = false;
        btnPatienten.IsEnabled = false;
        btnUitloggen.IsEnabled = false;
        mainFrame.Navigate(new StartPage());
    }
}
