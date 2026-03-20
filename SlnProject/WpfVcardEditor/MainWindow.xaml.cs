using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfVcardEditor
{
    public partial class MainWindow : Window
    {
        private string currentFilePath = null;
        private bool hasUnsavedChanges = false;
        private string currentPhotoBase64 = null;

        public MainWindow()
        {
            InitializeComponent();
            radUnknown.IsChecked = true;
            UpdateStatusBar();
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Ben je zeker dat je de applicatie wil afsluiten?",
                "Toepassing sluiten",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "vCard files (*.vcf)|*.vcf|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    LoadVCard(dialog.FileName);
                    currentFilePath = dialog.FileName;
                    hasUnsavedChanges = false;
                    mnuSave.IsEnabled = true;
                    UpdateStatusBar();
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(
                        $"Kan bestand {dialog.FileName} niet vinden.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"Geen toegang tot bestand {dialog.FileName}.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (IOException)
                {
                    MessageBox.Show(
                        $"Kan bestand {dialog.FileName} niet lezen.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Onverwachte fout: {ex.Message}",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                mnuSaveAs_Click(sender, e);
                return;
            }

            try
            {
                SaveVCard(currentFilePath);

                MessageBox.Show(
                    "Bestand werd opgeslagen.",
                    "Opslaan",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                hasUnsavedChanges = false;
                mnuSave.IsEnabled = true;
                UpdateStatusBar();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    $"Geen toegang tot bestand {currentFilePath}.",
                    "FOUT",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    $"Kan bestand {currentFilePath} niet schrijven.",
                    "FOUT",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Onverwachte fout: {ex.Message}",
                    "FOUT",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "vCard files (*.vcf)|*.vcf|All files (*.*)|*.*";
            dialog.DefaultExt = "vcf";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    SaveVCard(dialog.FileName);
                    currentFilePath = dialog.FileName;
                    hasUnsavedChanges = false;
                    mnuSave.IsEnabled = true;
                    UpdateStatusBar();
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"Geen toegang tot bestand {dialog.FileName}.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (IOException)
                {
                    MessageBox.Show(
                        $"Kan bestand {dialog.FileName} niet schrijven.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Onverwachte fout: {ex.Message}",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Er zijn nog onopgeslagen wijzigingen. Wil je verdergaan?",
                    "Nieuwe kaart",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            ClearForm();
            currentFilePath = null;
            hasUnsavedChanges = false;
            mnuSave.IsEnabled = false;
            UpdateStatusBar();
        }

        private void btnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Afbeeldingen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(dialog.FileName);
                    currentPhotoBase64 = Convert.ToBase64String(imageBytes);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(dialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    imgPhoto.Source = bitmap;
                    txtPhotoFileName.Text = dialog.FileName;

                    hasUnsavedChanges = true;
                    UpdateStatusBar();
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(
                        $"Kan afbeelding {dialog.FileName} niet vinden.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"Geen toegang tot afbeelding {dialog.FileName}.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (IOException)
                {
                    MessageBox.Show(
                        $"Kan afbeelding {dialog.FileName} niet lezen.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (FormatException)
                {
                    MessageBox.Show(
                        "Ongeldig afbeeldingsformaat.",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Onverwachte fout: {ex.Message}",
                        "FOUT",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void Card_Changed(object sender, RoutedEventArgs e)
        {
            hasUnsavedChanges = true;
            UpdateStatusBar();
        }

        private void LoadVCard(string filePath)
        {
            ClearForm();

            string[] lines = File.ReadAllLines(filePath);

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (line.StartsWith("N:"))
                {
                    string value = line.Substring(2);
                    string[] parts = value.Split(';');

                    if (parts.Length > 0)
                        txtLastname.Text = parts[0];

                    if (parts.Length > 1)
                        txtFirstname.Text = parts[1];
                }
                else if (line.StartsWith("BDAY:"))
                {
                    string value = line.Substring(5);

                    if (DateTime.TryParse(value, out DateTime birthDate))
                    {
                        datBirthday.SelectedDate = birthDate;
                    }
                }
                else if (line.StartsWith("GENDER:"))
                {
                    string value = line.Substring(7).ToUpper();

                    if (value == "F")
                        radFemale.IsChecked = true;
                    else if (value == "M")
                        radMale.IsChecked = true;
                    else
                        radUnknown.IsChecked = true;
                }
                else if (line.StartsWith("EMAIL") && line.Contains("HOME"))
                {
                    int index = line.IndexOf(':');
                    if (index >= 0)
                        txtPrivateEmail.Text = line.Substring(index + 1);
                }
                else if (line.StartsWith("TEL") && line.Contains("HOME"))
                {
                    int index = line.IndexOf(':');
                    if (index >= 0)
                        txtPrivatePhone.Text = line.Substring(index + 1);
                }
                else if (line.StartsWith("ORG:"))
                {
                    txtCompany.Text = line.Substring(4);
                }
                else if (line.StartsWith("TITLE:"))
                {
                    txtJobTitle.Text = line.Substring(6);
                }
                else if (line.StartsWith("EMAIL") && line.Contains("WORK"))
                {
                    int index = line.IndexOf(':');
                    if (index >= 0)
                        txtWorkEmail.Text = line.Substring(index + 1);
                }
                else if (line.StartsWith("TEL") && line.Contains("WORK"))
                {
                    int index = line.IndexOf(':');
                    if (index >= 0)
                        txtWorkPhone.Text = line.Substring(index + 1);
                }
                else if (line.StartsWith("X-SOCIALPROFILE"))
                {
                    int index = line.IndexOf(':');
                    if (index >= 0)
                    {
                        string url = line.Substring(index + 1);

                        if (line.ToLower().Contains("facebook"))
                            txtFacebook.Text = url;
                        else if (line.ToLower().Contains("linkedin"))
                            txtLinkedIn.Text = url;
                        else if (line.ToLower().Contains("instagram"))
                            txtInstagram.Text = url;
                        else if (line.ToLower().Contains("youtube"))
                            txtYoutube.Text = url;
                    }
                }
                else if (line.StartsWith("PHOTO"))
                {
                    int index = line.IndexOf(':');
                    if (index >= 0)
                    {
                        currentPhotoBase64 = line.Substring(index + 1);
                        txtPhotoFileName.Text = "(geladen uit vcf)";
                        ShowPhotoFromBase64(currentPhotoBase64);
                    }
                }
            }
        }

        private void SaveVCard(string filePath)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("BEGIN:VCARD");
            builder.AppendLine("VERSION:3.0");

            if (!string.IsNullOrWhiteSpace(txtFirstname.Text) || !string.IsNullOrWhiteSpace(txtLastname.Text))
            {
                builder.AppendLine($"N:{txtLastname.Text};{txtFirstname.Text};;;");
                builder.AppendLine($"FN:{txtFirstname.Text} {txtLastname.Text}".Trim());
            }

            if (datBirthday.SelectedDate.HasValue)
                builder.AppendLine($"BDAY:{datBirthday.SelectedDate.Value:yyyyMMdd}");

            if (radFemale.IsChecked == true)
                builder.AppendLine("GENDER:F");
            else if (radMale.IsChecked == true)
                builder.AppendLine("GENDER:M");
            else if (radUnknown.IsChecked == true)
                builder.AppendLine("GENDER:O");

            if (!string.IsNullOrWhiteSpace(txtPrivateEmail.Text))
                builder.AppendLine($"EMAIL;TYPE=HOME:{txtPrivateEmail.Text}");

            if (!string.IsNullOrWhiteSpace(txtPrivatePhone.Text))
                builder.AppendLine($"TEL;TYPE=HOME:{txtPrivatePhone.Text}");

            if (!string.IsNullOrWhiteSpace(txtCompany.Text))
                builder.AppendLine($"ORG:{txtCompany.Text}");

            if (!string.IsNullOrWhiteSpace(txtJobTitle.Text))
                builder.AppendLine($"TITLE:{txtJobTitle.Text}");

            if (!string.IsNullOrWhiteSpace(txtWorkEmail.Text))
                builder.AppendLine($"EMAIL;TYPE=WORK:{txtWorkEmail.Text}");

            if (!string.IsNullOrWhiteSpace(txtWorkPhone.Text))
                builder.AppendLine($"TEL;TYPE=WORK:{txtWorkPhone.Text}");

            if (!string.IsNullOrWhiteSpace(txtFacebook.Text))
                builder.AppendLine($"X-SOCIALPROFILE;TYPE=facebook:{txtFacebook.Text}");

            if (!string.IsNullOrWhiteSpace(txtLinkedIn.Text))
                builder.AppendLine($"X-SOCIALPROFILE;TYPE=linkedin:{txtLinkedIn.Text}");

            if (!string.IsNullOrWhiteSpace(txtInstagram.Text))
                builder.AppendLine($"X-SOCIALPROFILE;TYPE=instagram:{txtInstagram.Text}");

            if (!string.IsNullOrWhiteSpace(txtYoutube.Text))
                builder.AppendLine($"X-SOCIALPROFILE;TYPE=youtube:{txtYoutube.Text}");

            if (!string.IsNullOrWhiteSpace(currentPhotoBase64))
                builder.AppendLine($"PHOTO;ENCODING=b;TYPE=JPEG:{currentPhotoBase64}");

            builder.AppendLine("END:VCARD");

            File.WriteAllText(filePath, builder.ToString());
        }

        private void ClearForm()
        {
            txtFirstname.Text = "";
            txtLastname.Text = "";
            datBirthday.SelectedDate = null;

            radFemale.IsChecked = false;
            radMale.IsChecked = false;
            radUnknown.IsChecked = true;

            txtPrivateEmail.Text = "";
            txtPrivatePhone.Text = "";

            txtCompany.Text = "";
            txtJobTitle.Text = "";
            txtWorkEmail.Text = "";
            txtWorkPhone.Text = "";

            txtLinkedIn.Text = "";
            txtFacebook.Text = "";
            txtInstagram.Text = "";
            txtYoutube.Text = "";

            txtPhotoFileName.Text = "(geen geselecteerd)";
            imgPhoto.Source = null;
            currentPhotoBase64 = null;
        }

        private void ShowPhotoFromBase64(string base64)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);

                using MemoryStream stream = new MemoryStream(bytes);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();

                imgPhoto.Source = image;
            }
            catch
            {
                imgPhoto.Source = null;
            }
        }

        private void UpdateStatusBar()
        {
            if (string.IsNullOrEmpty(currentFilePath))
                txtCurrentCardStatus.Text = "huidige kaart: (geen geopend)";
            else
                txtCurrentCardStatus.Text = $"huidige kaart: {currentFilePath}";

            int totalFields = 13;
            int filledFields = 0;

            if (!string.IsNullOrWhiteSpace(txtFirstname.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtLastname.Text)) filledFields++;
            if (datBirthday.SelectedDate.HasValue) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtPrivateEmail.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtPrivatePhone.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtCompany.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtJobTitle.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtWorkEmail.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtWorkPhone.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtLinkedIn.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtFacebook.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtInstagram.Text)) filledFields++;
            if (!string.IsNullOrWhiteSpace(txtYoutube.Text)) filledFields++;

            if (!string.IsNullOrWhiteSpace(currentPhotoBase64))
            {
                filledFields++;
                totalFields++;
            }

            int percentage = 0;

            if (totalFields > 0)
                percentage = (int)Math.Round((double)filledFields / totalFields * 100);

            txtPercentageStatus.Text = $"percentage ingevuld: {percentage}%";
        }
    }
}