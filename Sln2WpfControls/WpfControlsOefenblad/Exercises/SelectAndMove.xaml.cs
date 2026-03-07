using System;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Select And Move", Description = "Items verplaatsen tussen twee ListBoxes.", Order = 9, IsVisible = true)]
    public partial class SelectAndMove : Page
    {
        public SelectAndMove()
        {
            InitializeComponent();
        }
        private void btnToSelected_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = lstAvailable.SelectedItem as ListBoxItem;

            if (item == null) return;

            lstAvailable.Items.Remove(item);
            lstSelected.Items.Add(item);

            btnToSelected.IsEnabled = false;
        }

        private void btnToAvailable_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = lstSelected.SelectedItem as ListBoxItem;

            if (item == null) return;

            lstSelected.Items.Remove(item);
            lstAvailable.Items.Add(item);

            btnToAvailable.IsEnabled = false;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnToSelected.IsEnabled = lstAvailable.SelectedItem != null;
            btnToAvailable.IsEnabled = lstSelected.SelectedItem != null;
        }
    }
}
