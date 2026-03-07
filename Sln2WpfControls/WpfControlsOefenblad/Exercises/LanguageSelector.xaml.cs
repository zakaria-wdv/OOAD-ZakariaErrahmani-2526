using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises;

[NavPage(Title = "Language Selector", Description = "ComboBox SelectionChanged event en ComboBoxItem", Order = 6, IsVisible = true)]
public partial class LanguageSelector : Page
{
    public LanguageSelector()
    {
        InitializeComponent();

        string[] languages = { "Nederlands", "English", "Français" };

        foreach (string lang in languages)
        {
            cmbLanguage.Items.Add(new ComboBoxItem { Content = lang });
        }

        cmbLanguage.SelectionChanged += cmbLanguage_SelectionChanged;
    }



    private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (object item in cmbLanguage.Items)
        {
            if (item is ComboBoxItem cbItem)
            {
                cbItem.FontWeight = FontWeights.Normal;
            }
        }

        if (cmbLanguage.SelectedIndex <= 0)
        {
            txtGreeting.Text = "";
            return;
        }

        if (cmbLanguage.SelectedItem is ComboBoxItem selected)
        {
            selected.FontWeight = FontWeights.Bold;

            string taal = selected.Content as string ?? "";

            switch (taal)
            {
                case "Nederlands":
                    txtGreeting.Text = "Hallo";
                    break;
                case "English":
                    txtGreeting.Text = "Hello";
                    break;
                case "Français":
                    txtGreeting.Text = "Bonjour";
                    break;
                default:
                    txtGreeting.Text = "";
                    break;
            }
        }
    }
}