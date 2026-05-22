using System;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFPatient.Pages;

// Login pagina voor de patiënt
public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        // Reset eventuele vorige foutmelding
        txtFoutmelding.Text = "";

        // Form checking
        string email = txtEmail.Text.Trim();
        string paswoord = txtPaswoord.Password;

        if (string.IsNullOrEmpty(email))
        {
            txtFoutmelding.Text = "Email is verplicht.";
            return;
        }
        if (string.IsNullOrEmpty(paswoord))
        {
            txtFoutmelding.Text = "Wachtwoord is verplicht.";
            return;
        }
        if (!email.Contains("@"))
        {
            txtFoutmelding.Text = "Geef een geldig email-adres in.";
            return;
        }

        try
        {
            // Zoek de patiënt op basis van email
            Patient? patient = Patient.GeefPatientPerEmail(email);
            if (patient == null)
            {
                txtFoutmelding.Text = "Email of wachtwoord is onjuist.";
                return;
            }

            // Vergelijk de SHA256 hash van het ingevoerde wachtwoord met de db
            string hash = Hasher.Hash(paswoord);
            if (hash != patient.Paswoord)
            {
                txtFoutmelding.Text = "Email of wachtwoord is onjuist.";
                return;
            }

            // Login succesvol - sla op in de sessie
            Sessie.IngelogdePatient = patient;

            // Update de UI van de MainWindow
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.UpdateNaLogin();
                // Navigeer naar de afspraken-pagina zoals gevraagd in de opgave
                MainWindow.Instance.mainFrame.Navigate(new AfsprakenOverzichtPage());
            }
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij inloggen: " + ex.Message;
        }
    }
}
