using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFPatient.Pages;

// Overzicht van alle afspraken van de ingelogde patiënt
// Bij selectie wordt de reden getoond; toekomstige afspraken kunnen geannuleerd worden
public partial class AfsprakenOverzichtPage : Page
{
    // Lokale lijst van afspraken (om bij selectie de juiste op te halen)
    private List<Afspraak> huidigeAfspraken = new List<Afspraak>();

    public AfsprakenOverzichtPage()
    {
        InitializeComponent();
        LaadAfspraken();
    }

    private void LaadAfspraken()
    {
        txtFoutmelding.Text = "";
        try
        {
            Patient? patient = Sessie.IngelogdePatient;
            if (patient == null)
            {
                txtFoutmelding.Text = "Geen patiënt ingelogd.";
                return;
            }

            // Haal alle afspraken op via de class library
            huidigeAfspraken = Afspraak.GeefAfsprakenVoorPatient(patient.Id);

            lstAfspraken.Items.Clear();
            if (huidigeAfspraken.Count == 0)
            {
                lstAfspraken.Items.Add("U heeft nog geen afspraken ingepland.");
                lstAfspraken.IsEnabled = false;
            }
            else
            {
                lstAfspraken.IsEnabled = true;
                for (int i = 0; i < huidigeAfspraken.Count; i++)
                {
                    Afspraak a = huidigeAfspraken[i];
                    string dokterNaam = a.Dokter != null ? "Dr. " + a.Dokter.VolledigeNaam : "Onbekende dokter";
                    string regel = a.Moment.ToString("dd MMM yyyy HH:mm") + " - " + dokterNaam;
                    // Markeer afspraken in het verleden met een [VERLOPEN] tag
                    if (a.Moment < DateTime.Now)
                    {
                        regel = "[VERLOPEN] " + regel;
                    }
                    lstAfspraken.Items.Add(regel);
                }
            }

            // Reset onderaan
            txtReden.Text = "";
            btnAnnuleren.IsEnabled = false;
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij laden afspraken: " + ex.Message;
        }
    }

    // Bij selectie van een afspraak: toon de reden + (de)activeer annuleren knop
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

        // Enkel toekomstige afspraken mogen geannuleerd worden
        btnAnnuleren.IsEnabled = gekozen.Moment > DateTime.Now;
    }

    // Annuleer een toekomstige afspraak
    private void btnAnnuleren_Click(object sender, RoutedEventArgs e)
    {
        txtFoutmelding.Text = "";
        int idx = lstAfspraken.SelectedIndex;
        if (idx < 0 || idx >= huidigeAfspraken.Count)
        {
            return;
        }

        // Bevestiging vragen (MessageBox is enkel verboden voor foutmeldingen)
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
            huidigeAfspraken[idx].Verwijderen();
            // Herlaad de lijst
            LaadAfspraken();
        }
        catch (Exception ex)
        {
            txtFoutmelding.Text = "Fout bij annuleren: " + ex.Message;
        }
    }

    // Knop voor het maken van een nieuwe afspraak
    private void btnNieuw_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow.Instance != null)
        {
            MainWindow.Instance.mainFrame.Navigate(new NieuweAfspraakPage());
        }
    }
}
