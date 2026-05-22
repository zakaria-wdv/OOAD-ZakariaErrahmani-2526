using System.IO;
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

namespace DoktersPraktijk
{
    /// hoofdpagina van de dokter app
    /// heeft navigatie knoppen aan linkerkant + frame voor de pagina's
    public partial class MainWindow : Window
    {
        //referentie zodat pagigin's kunnen navigeren via MainWindow.Instanc

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            mainFrame.Navigate(new StartPage());
        }

        // gaat verder nadat je succesvol bent ingelogd
        // update de UI om de naam, foto te tonen, activeert de knoppen
        public void UpdateNaLogin()
        {
            Patient? patient = Sessie.IngelogdePatient;
            if (patient == null)
            {
                return;
            }
            
            // toont de naam in header
            txtIngelogdeNaam.Text = patient.VolledigeNaam;

            // toon foto indien beschikbaar
            if (patient.ProfielFotoData != null)
            {
                try
                {
                    // maak een BitmapImage van de byte array
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = new MemoryStream(patient.ProfielFotoData);
                    bitmap.EndInit();
                    imgIngelogdeFoto.Source = bitmap;
                }
                catch
                {
                    // negeer foto-fouten - geen verder impact
                    imgIngelogdeFoto.Source = null;
                }
            }

            btnAfspraken.IsEnabled = true;
            btnProfiel.IsEnabled = true;
            btnUitloggen.IsEnabled = true;
        }

        // header wordt verniewt na profiel verandering  
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
            // Reset de sessie en UI
            Sessie.Logout();
            txtIngelogdeNaam.Text = "";
            imgIngelogdeFoto.Source = null;
            btnAfspraken.IsEnabled = false;
            btnProfiel.IsEnabled = false;
            btnUitloggen.IsEnabled = false;
            mainFrame.Navigate(new StartPage());
        }
    }
}