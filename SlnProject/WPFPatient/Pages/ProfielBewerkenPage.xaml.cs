using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DokterspraktijkLib;

namespace WPFPatient.Pages;

// Pagina waarmee de ingelogde patiënt zijn eigen profiel kan bewerken
public partial class ProfielBewerkenPage : Page
{
    // Tijdelijke buffer voor de (eventueel nieuwe) foto-data
    private byte[]? huidigeFotoData;

    public ProfielBewerkenPage()
    {
        InitializeComponent();
        VulIn();
    }

    private void VulIn()
    {
        Patient? patient = Sessie.IngelogdePatient;
        if (patient == null)
        {
            return;
        }

        txtVoornaam.Text = patient.Voornaam;
        txtAchternaam.Text = patient.Achternaam;
        cboGeslacht.SelectedIndex = (int)patient.Geslacht;
        dpGeboortedatum.SelectedDate = patient.Geboortedatum;
        txtGsm.Text = patient.Gsm ?? "";
        txtEmail.Text = patient.Email;
        cboNotificaties.SelectedIndex = (int)patient.Notificaties;
        huidigeFotoData = patient.ProfielFotoData;

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

    private void btnKiesFoto_Click(object sender, RoutedEventArgs e)
    {
        txtFoutmelding.Text = "";
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "Afbeeldingen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
        dialog.Title = "Kies een profielfoto";

        bool? resultaat = dialog.ShowDialog();
        if (resultaat != true)
        {
            return;
        }

        try
        {
            huidigeFotoData = File.ReadAllBytes(dialog.FileName);

            BitmapImage bm = new BitmapImage();
            bm.BeginInit();
            bm.CacheOption = BitmapCacheOption.OnLoad;
            bm.StreamSource = new MemoryStream(huidigeFotoData);
            bm.EndInit();
            imgFoto.Source = bm;
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij inladen foto: " + ex.Message;
        }
    }

    private void btnOpslaan_Click(object sender, RoutedEventArgs e)
    {
        txtFoutmelding.Text = "";

        Patient? patient = Sessie.IngelogdePatient;
        if (patient == null)
        {
            txtFoutmelding.Text = "Geen patiënt ingelogd.";
            return;
        }

        // Form checking
        string voornaam = txtVoornaam.Text.Trim();
        string achternaam = txtAchternaam.Text.Trim();
        string email = txtEmail.Text.Trim();
        string paswoord = txtPaswoord.Password;
        string gsm = txtGsm.Text.Trim();

        if (string.IsNullOrEmpty(voornaam))
        {
            txtFoutmelding.Text = "Voornaam is verplicht.";
            return;
        }
        if (string.IsNullOrEmpty(achternaam))
        {
            txtFoutmelding.Text = "Achternaam is verplicht.";
            return;
        }
        if (cboGeslacht.SelectedIndex < 0)
        {
            txtFoutmelding.Text = "Geslacht is verplicht.";
            return;
        }
        if (dpGeboortedatum.SelectedDate == null)
        {
            txtFoutmelding.Text = "Geboortedatum is verplicht.";
            return;
        }
        if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
        {
            txtFoutmelding.Text = "Geef een geldig email-adres in.";
            return;
        }
        if (cboNotificaties.SelectedIndex < 0)
        {
            txtFoutmelding.Text = "Notificaties is verplicht.";
            return;
        }
        if (!string.IsNullOrEmpty(gsm) && gsm.Length > 10)
        {
            txtFoutmelding.Text = "GSM mag maximaal 10 tekens lang zijn.";
            return;
        }

        try
        {
            // Werk de gegevens van de patiënt bij
            patient.Voornaam = voornaam;
            patient.Achternaam = achternaam;
            patient.Geslacht = (Geslacht)cboGeslacht.SelectedIndex;
            patient.Geboortedatum = dpGeboortedatum.SelectedDate.Value;
            patient.Gsm = string.IsNullOrEmpty(gsm) ? null : gsm;
            patient.Email = email;
            // Indien wachtwoord ingevuld: hash en update, anders behoud
            if (!string.IsNullOrEmpty(paswoord))
            {
                patient.Paswoord = Hasher.Hash(paswoord);
            }
            patient.Notificaties = (Notificaties)cboNotificaties.SelectedIndex;
            patient.ProfielFotoData = huidigeFotoData;

            patient.Bijwerken();

            // Update sessie en header
            Sessie.IngelogdePatient = patient;
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.UpdateHeader();
                MainWindow.Instance.mainFrame.Navigate(new ProfielPage());
            }
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij opslaan: " + ex.Message;
        }
    }

    private void btnTerug_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new ProfielPage());
        }
    }
}
