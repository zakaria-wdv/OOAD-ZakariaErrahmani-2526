using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DokterspraktijkLib;

namespace WPFDokter.Pages;

// Pagina voor het toevoegen of bewerken van een patiënt
// Werkt voor beide scenario's: 
//  - null = nieuwe patiënt
//  - bestaande Patient = wijzigen
public partial class PatientBewerkenPage : Page
{
    // Houdt de huidige patiënt bij (null = nieuwe)
    private Patient? bestaandePatient;

    // Tijdelijke buffer voor de nieuwe (of bestaande) foto-data
    private byte[]? huidigeFotoData;

    public PatientBewerkenPage(Patient? patient)
    {
        InitializeComponent();
        bestaandePatient = patient;

        if (patient == null)
        {
            // Nieuwe patiënt - lege velden, default datum
            txtTitel.Text = "Nieuwe Patiënt";
            dpGeboortedatum.SelectedDate = new DateTime(2000, 1, 1);
            cboGeslacht.SelectedIndex = 0;
            cboNotificaties.SelectedIndex = 0;
        }
        else
        {
            // Bestaande patiënt - velden invullen
            txtTitel.Text = "Patiënt Bewerken";
            txtVoornaam.Text = patient.Voornaam;
            txtAchternaam.Text = patient.Achternaam;
            cboGeslacht.SelectedIndex = (int)patient.Geslacht;
            dpGeboortedatum.SelectedDate = patient.Geboortedatum;
            txtGsm.Text = patient.Gsm ?? "";
            txtEmail.Text = patient.Email;
            cboNotificaties.SelectedIndex = (int)patient.Notificaties;
            // Bij wijzigen: wachtwoord-hint tonen en wachtwoord-veld leeg laten
            txtPaswoordHint.Visibility = Visibility.Visible;
            huidigeFotoData = patient.ProfielFotoData;

            // Toon de huidige foto
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
    }

    // Open een bestand-dialoog om een nieuwe foto te kiezen
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
            // Lees het bestand in als bytes
            huidigeFotoData = File.ReadAllBytes(dialog.FileName);

            // Toon de foto
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

    // Slaat de patiënt op (insert of update)
    private void btnOpslaan_Click(object sender, RoutedEventArgs e)
    {
        txtFoutmelding.Text = "";

        // === Form checking ===
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
        if (string.IsNullOrEmpty(email))
        {
            txtFoutmelding.Text = "Email is verplicht.";
            return;
        }
        if (!email.Contains("@") || !email.Contains("."))
        {
            txtFoutmelding.Text = "Geef een geldig email-adres in.";
            return;
        }
        // Bij nieuwe patiënt: wachtwoord is verplicht
        // Bij wijziging: leeg = behouden, anders nieuw wachtwoord
        if (bestaandePatient == null && string.IsNullOrEmpty(paswoord))
        {
            txtFoutmelding.Text = "Wachtwoord is verplicht voor nieuwe patiënt.";
            return;
        }
        if (cboNotificaties.SelectedIndex < 0)
        {
            txtFoutmelding.Text = "Notificaties is verplicht.";
            return;
        }
        // Optionele check voor gsm: enkel cijfers, max 10 tekens
        if (!string.IsNullOrEmpty(gsm) && gsm.Length > 10)
        {
            txtFoutmelding.Text = "GSM mag maximaal 10 tekens lang zijn.";
            return;
        }

        // === Opslaan ===
        try
        {
            if (bestaandePatient == null)
            {
                // Nieuwe patiënt aanmaken
                Patient nieuwe = new Patient();
                nieuwe.Voornaam = voornaam;
                nieuwe.Achternaam = achternaam;
                nieuwe.Geslacht = (Geslacht)cboGeslacht.SelectedIndex;
                nieuwe.Geboortedatum = dpGeboortedatum.SelectedDate.Value;
                nieuwe.Gsm = string.IsNullOrEmpty(gsm) ? null : gsm;
                nieuwe.Email = email;
                nieuwe.Paswoord = Hasher.Hash(paswoord);
                nieuwe.Notificaties = (Notificaties)cboNotificaties.SelectedIndex;
                nieuwe.ProfielFotoData = huidigeFotoData;
                nieuwe.Toevoegen();
            }
            else
            {
                // Bestaande patiënt bijwerken
                bestaandePatient.Voornaam = voornaam;
                bestaandePatient.Achternaam = achternaam;
                bestaandePatient.Geslacht = (Geslacht)cboGeslacht.SelectedIndex;
                bestaandePatient.Geboortedatum = dpGeboortedatum.SelectedDate.Value;
                bestaandePatient.Gsm = string.IsNullOrEmpty(gsm) ? null : gsm;
                bestaandePatient.Email = email;
                // Indien wachtwoord ingevuld: hash en update; anders behoud bestaand
                if (!string.IsNullOrEmpty(paswoord))
                {
                    bestaandePatient.Paswoord = Hasher.Hash(paswoord);
                }
                bestaandePatient.Notificaties = (Notificaties)cboNotificaties.SelectedIndex;
                bestaandePatient.ProfielFotoData = huidigeFotoData;
                bestaandePatient.Bijwerken();
            }

            // Terug naar overzicht
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.mainFrame.Navigate(new PatientenOverzichtPage());
            }
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij opslaan: " + ex.Message;
        }
    }

    // Terug-knop naar overzicht
    private void btnTerug_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new PatientenOverzichtPage());
        }
    }
}
