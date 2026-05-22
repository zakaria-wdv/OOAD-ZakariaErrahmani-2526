using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFPatient.Pages;

// Pagina waarmee een patiënt een nieuwe afspraak kan boeken bij een dokter
public partial class NieuweAfspraakPage : Page
{
    // Lokale lijst van dokters om de geselecteerde te kunnen ophalen
    private List<Dokter> alleDokters = new List<Dokter>();

    public NieuweAfspraakPage()
    {
        InitializeComponent();
        LaadDokters();
        VulTijdsblokken();
        // Default datum = morgen
        dpDatum.SelectedDate = DateTime.Today.AddDays(1);
        // Vandaag is de minimum datum (geen afspraken in het verleden)
        dpDatum.DisplayDateStart = DateTime.Today;
    }

    // Laad alle dokters in de listbox
    private void LaadDokters()
    {
        txtFoutmelding.Text = "";
        try
        {
            alleDokters = Dokter.GeefAlleDokters();
            lstDokters.Items.Clear();
            for (int i = 0; i < alleDokters.Count; i++)
            {
                Dokter d = alleDokters[i];
                lstDokters.Items.Add("Dr. " + d.VolledigeNaam);
            }
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij laden dokters: " + ex.Message;
        }
    }

    // Vult de tijdsblokken (08:00 t/m 17:30, elk half uur)
    private void VulTijdsblokken()
    {
        cboTijd.Items.Clear();
        for (int uur = 8; uur < 18; uur++)
        {
            // Twee tijdsblokken per uur: hh:00 en hh:30
            string blok1 = uur.ToString("D2") + ":00";
            string blok2 = uur.ToString("D2") + ":30";
            cboTijd.Items.Add(blok1);
            cboTijd.Items.Add(blok2);
        }
    }

    // Wanneer een dokter geselecteerd wordt: toon zijn basisinformatie
    private void lstDokters_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int idx = lstDokters.SelectedIndex;
        if (idx < 0 || idx >= alleDokters.Count)
        {
            txtDokterNaam.Text = "";
            txtDokterEmail.Text = "";
            txtDokterGsm.Text = "";
            txtDokterRiziv.Text = "";
            txtDokterGeconv.Text = "";
            return;
        }

        Dokter d = alleDokters[idx];
        txtDokterNaam.Text = "Dr. " + d.VolledigeNaam;
        txtDokterEmail.Text = "Email: " + d.Email;
        txtDokterGsm.Text = "GSM: " + (d.Gsm ?? "(geen)");
        txtDokterRiziv.Text = "RIZIV: " + d.Rizivnummer;
        txtDokterGeconv.Text = "Geconventioneerd: " + (d.IsGeconventioneerd ? "Ja" : "Nee");
    }

    // Bevestig de afspraak
    private void btnBevestigen_Click(object sender, RoutedEventArgs e)
    {
        txtFoutmelding.Text = "";

        // Form checking
        int idx = lstDokters.SelectedIndex;
        if (idx < 0)
        {
            txtFoutmelding.Text = "Selecteer een dokter.";
            return;
        }
        if (dpDatum.SelectedDate == null)
        {
            txtFoutmelding.Text = "Selecteer een datum.";
            return;
        }
        if (cboTijd.SelectedItem == null)
        {
            txtFoutmelding.Text = "Selecteer een tijdsblok.";
            return;
        }
        string reden = txtReden.Text.Trim();
        if (string.IsNullOrEmpty(reden))
        {
            txtFoutmelding.Text = "Geef een reden op voor de consultatie.";
            return;
        }

        Patient? patient = Sessie.IngelogdePatient;
        if (patient == null)
        {
            txtFoutmelding.Text = "Geen patiënt ingelogd.";
            return;
        }

        try
        {
            // Bouw het tijdstip op uit datum + uur:minuut
            DateTime datum = dpDatum.SelectedDate.Value;
            string tijdString = (string)cboTijd.SelectedItem;
            // Format: "HH:mm" - splitsen op ':'
            string[] delen = tijdString.Split(':');
            int uur = int.Parse(delen[0]);
            int minuut = int.Parse(delen[1]);
            DateTime moment = new DateTime(datum.Year, datum.Month, datum.Day, uur, minuut, 0);

            // Mag niet in het verleden zijn
            if (moment < DateTime.Now)
            {
                txtFoutmelding.Text = "U kan geen afspraak in het verleden boeken.";
                return;
            }

            // Maak en sla de afspraak op
            Afspraak nieuwe = new Afspraak();
            nieuwe.Moment = moment;
            nieuwe.Klacht = reden;
            nieuwe.PatientId = patient.Id;
            nieuwe.DokterId = alleDokters[idx].Id;
            nieuwe.Toevoegen();

            // Navigeer terug naar overzicht
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.mainFrame.Navigate(new AfsprakenOverzichtPage());
            }
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij boeken afspraak: " + ex.Message;
        }
    }

    private void btnTerug_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new AfsprakenOverzichtPage());
        }
    }
}
