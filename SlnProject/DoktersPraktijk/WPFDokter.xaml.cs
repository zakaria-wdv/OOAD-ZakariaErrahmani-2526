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
        public static MainWindow? Instance {  get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            //begint direct op start pagina
            mainFrame.Navigate(new StartPage());
        }
        // gaat verder nadat je succesvol bent ingelogd
        // update de UI om de naam, foto te tonen, activeert de knoppen
        public void UpdateNaLogin()
        {
            Dokter? dokter = Sessie.IngelogdeDokter;
            if (dokter == null)
            {
                return;
            }

            // toont de naam in header
            txtIngelogdeNaam.Text = "Dr. " + dokter.VolledigeNaam;

            // toon foto indien beschikbaar
            if (dokter.ProfielFotoData != null)
            {
                try
                {
                    // maak een BitmapImage van de byte array
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = new MemoryStream(dokter.ProfielFotoData);
                    bitmap.EndInit();
                    imgIngelogdeFoto.Source = bitmap;
                }
                catch
                {
                    // negeer foto-fouten - geen verder impact
                    imgIngelogdeFoto.Source = null;
                }
            }

            // activeert de andere knoppen
            btnAfspraken.IsEnabled = true;
            btnPatienten.IsEnabled = true;
            btnUitloggen.IsEnabled = true;
        }

        // navigatie voor de knoppen
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
}