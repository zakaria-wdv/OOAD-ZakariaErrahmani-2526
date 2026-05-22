using System;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter.Pages;

// Login pagina voor de dokter
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

        // Form checking: zijn beide velden ingevuld?
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
        // Eenvoudige email validatie - moet een @ bevatten
        if (!email.Contains("@"))
        {
            txtFoutmelding.Text = "Geef een geldig email-adres in.";
            return;
        }

        // Probeer in te loggen - exception handling in deze applicatielaag (niet in de lib)
        try
        {
            // Zoek de dokter op basis van email
            Dokter? dokter = Dokter.GeefDokterPerEmail(email);
            if (dokter == null)
            {
                txtFoutmelding.Text = "Email of wachtwoord is onjuist.";
                return;
            }

            // Hash het ingevoerde wachtwoord en vergelijk met de hash in de databank
            string hash = Hasher.Hash(paswoord);
            if (hash != dokter.Paswoord)
            {
                txtFoutmelding.Text = "Email of wachtwoord is onjuist.";
                return;
            }

            // Login succesvol - sla op in de sessie
            Sessie.IngelogdeDokter = dokter;

            // Update de UI van de MainWindow (naam + foto rechtsboven, knoppen activeren)
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.UpdateNaLogin();
                // Navigeer naar de patiënten-pagina zoals gevraagd in de opgave
                MainWindow.Instance.mainFrame.Navigate(new PatientenOverzichtPage());
            }
        }
        catch (Exception ex)
        {
            // Toon database/andere fouten in de TextBlock
            txtFoutmelding.Text = "Fout bij inloggen: " + ex.Message;
        }
    }
}
