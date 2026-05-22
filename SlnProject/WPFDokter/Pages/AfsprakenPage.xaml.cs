using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter.Pages;

// Pagina voor het bekijken van afspraken op een gekozen dag
// + mogelijkheid om een afspraak te annuleren
public partial class AfsprakenPage : Page
{
    // Lokale lijst van afspraken voor de gekozen dag
    private List<Afspraak> huidigeAfspraken = new List<Afspraak>();

    public AfsprakenPage()
    {
        InitializeComponent();
        // Selecteer vandaag als startpunt; dit triggert kalender_SelectedDatesChanged
        kalender.SelectedDate = DateTime.Today;
    }

    // Helper-methode om afspraken te laden voor een specifieke datum
    private void LaadAfsprakenVoorDatum(DateTime gekozenDatum)
    {
        txtFoutmelding.Text = "";
        txtGekozenDatum.Text = "Afspraken op " + gekozenDatum.ToString("dddd dd MMMM yyyy");

        // Reset details onderaan
        txtReden.Text = "";
        btnAnnuleren.IsEnabled = false;

        try
        {
            Dokter? dokter = Sessie.IngelogdeDokter;
            if (dokter == null)
            {
                txtFoutmelding.Text = "Geen dokter ingelogd.";
                return;
            }
            // Haal afspraken op via de class library (geen SQL hier!)
            huidigeAfspraken = Afspraak.GeefAfsprakenVoorDokterOpDag(dokter.Id, gekozenDatum);

            // Vul de listbox - geen databinding, items toevoegen via Items.Add()
            lstAfspraken.Items.Clear();
            if (huidigeAfspraken.Count == 0)
            {
                lstAfspraken.Items.Add("Geen afspraken op deze dag.");
                lstAfspraken.IsEnabled = false;
            }
            else
            {
                lstAfspraken.IsEnabled = true;
                for (int i = 0; i < huidigeAfspraken.Count; i++)
                {
                    Afspraak a = huidigeAfspraken[i];
                    string regel = a.Moment.ToString("HH:mm") + " - " +
                                   (a.Patient != null ? a.Patient.VolledigeNaam : "Onbekende patiënt");
                    lstAfspraken.Items.Add(regel);
                }
            }
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij laden afspraken: " + ex.Message;
        }
    }

    // Event handler voor de Calendar control
    private void kalender_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (kalender.SelectedDate == null)
        {
            return;
        }
        LaadAfsprakenVoorDatum(kalender.SelectedDate.Value);
    }

    // Wanneer een afspraak geselecteerd wordt: toon de reden + activeer annuleren-knop
    private void lstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int idx = lstAfspraken.SelectedIndex;
        if (idx < 0 || idx >= huidigeAfspraken.Count)
        {
            txtReden.Text = "";
            btnAnnuleren.IsEnabled = false;
            return;
        }
        Afspraak gekozen = huidigeAfspraken[idx];
        txtReden.Text = gekozen.Klacht;
        btnAnnuleren.IsEnabled = true;
    }

    // Annuleert (verwijdert) de geselecteerde afspraak
    private void btnAnnuleren_Click(object sender, RoutedEventArgs e)
    {
        txtFoutmelding.Text = "";
        int idx = lstAfspraken.SelectedIndex;
        if (idx < 0 || idx >= huidigeAfspraken.Count)
        {
            return;
        }

        // Bevestiging via MessageBox is acceptabel voor confirmaties
        // (de regel "geen MessageBox" geldt enkel voor foutmeldingen)
        MessageBoxResult resultaat = MessageBox.Show(
            "Wil u deze afspraak echt annuleren?",
            "Bevestiging",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultaat != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            Afspraak gekozen = huidigeAfspraken[idx];
            gekozen.Verwijderen();
            // Reload de lijst voor de huidige datum via de helper-methode
            if (kalender.SelectedDate != null)
            {
                LaadAfsprakenVoorDatum(kalender.SelectedDate.Value);
            }
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij annuleren: " + ex.Message;
        }
    }
}
