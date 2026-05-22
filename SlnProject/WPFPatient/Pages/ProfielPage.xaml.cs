using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFPatient.Pages;

// Pagina die de profielgegevens van de ingelogde patiënt toont
public partial class ProfielPage : Page
{
    public ProfielPage()
    {
        InitializeComponent();
        VulIn();
    }

    private void VulIn()
    {
        // Werk altijd met de laatste versie uit de databank
        Patient? sessiePatient = Sessie.IngelogdePatient;
        if (sessiePatient == null)
        {
            return;
        }
        // Herlaad om wijzigingen na bewerken te zien
        Patient? patient = Patient.GeefPatientPerId(sessiePatient.Id);
        if (patient == null)
        {
            return;
        }
        // Update sessie ook
        Sessie.IngelogdePatient = patient;

        txtNaam.Text = patient.VolledigeNaam;
        txtEmail.Text = patient.Email;
        txtGsm.Text = patient.Gsm ?? "(niet ingevuld)";
        txtGeslacht.Text = patient.Geslacht.ToString();
        txtGeboortedatum.Text = patient.Geboortedatum.ToString("dd MMMM yyyy");
        txtNotificaties.Text = patient.Notificaties.ToString();

        if (patient.ProfielFotoData != null)
        {
            try
            {
                BitmapImage bm = new BitmapImage();
                bm.BeginInit();
                bm.CacheOption = BitmapCacheOption.OnLoad;
                bm.StreamSource = new MemoryStream(patient.ProfielFotoData);
                bm.EndInit();
                imgFoto.Source = bm;
            }
            catch
            {
                // Foto-fout negeren
            }
        }
    }

    private void btnBewerken_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new ProfielBewerkenPage());
        }
    }
}
