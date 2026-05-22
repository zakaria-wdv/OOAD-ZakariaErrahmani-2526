using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DokterspraktijkLib;

namespace WPFDokter.Pages;

// Pagina die alle gegevens van een geselecteerde patiënt toont
public partial class PatientDetailsPage : Page
{
    public PatientDetailsPage(Patient patient)
    {
        InitializeComponent();
        VulIn(patient);
    }

    // Vult alle velden in met de gegevens van de patiënt
    private void VulIn(Patient patient)
    {
        txtNaam.Text = patient.VolledigeNaam;
        txtEmail.Text = patient.Email;
        txtGsm.Text = patient.Gsm ?? "(niet ingevuld)";
        txtGeslacht.Text = patient.Geslacht.ToString();
        txtGeboortedatum.Text = patient.Geboortedatum.ToString("dd MMMM yyyy");
        txtNotificaties.Text = patient.Notificaties.ToString();

        // Toon foto indien beschikbaar
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

    // Terug-knop volgens de opgave (op elke frame een terug-knop)
    private void btnTerug_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new PatientenOverzichtPage());
        }
    }
}
