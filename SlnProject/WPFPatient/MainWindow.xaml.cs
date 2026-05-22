using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;
using WPFPatient.Pages;

namespace WPFPatient;

// Hoofdvenster van de patiënt-applicatie
// Bevat de navigatie aan de linkerkant en een Frame voor de pagina's
public partial class MainWindow : Window
{
    // Static referentie zodat pagina's kunnen navigeren
    public static MainWindow? Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        mainFrame.Navigate(new StartPage());
    }

    // Wordt door LoginPage aangeroepen na succesvol inloggen
    public void UpdateNaLogin()
    {
        Patient? patient = Sessie.IngelogdePatient;
        if (patient == null)
        {
            return;
        }

        txtIngelogdeNaam.Text = patient.VolledigeNaam;

        if (patient.ProfielFotoData != null)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(patient.ProfielFotoData);
                bitmap.EndInit();
                imgIngelogdeFoto.Source = bitmap;
            }
            catch
            {
                imgIngelogdeFoto.Source = null;
            }
        }

        btnAfspraken.IsEnabled = true;
        btnProfiel.IsEnabled = true;
        btnUitloggen.IsEnabled = true;
    }

    // Wordt aangeroepen na profiel wijziging om header bij te werken
    public void UpdateHeader()
    {
        UpdateNaLogin();
    }

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
        mainFrame.Navigate(new AfsprakenOverzichtPage());
    }

    private void btnProfiel_Click(object sender, RoutedEventArgs e)
    {
        mainFrame.Navigate(new ProfielPage());
    }

    private void btnUitloggen_Click(object sender, RoutedEventArgs e)
    {
        Sessie.Logout();
        txtIngelogdeNaam.Text = "";
        imgIngelogdeFoto.Source = null;
        btnAfspraken.IsEnabled = false;
        btnProfiel.IsEnabled = false;
        btnUitloggen.IsEnabled = false;
        mainFrame.Navigate(new StartPage());
    }
}
