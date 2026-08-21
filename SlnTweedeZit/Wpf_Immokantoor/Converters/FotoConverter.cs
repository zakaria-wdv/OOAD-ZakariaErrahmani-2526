using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WpfImmo.Converters
{
    // Zet de fotonaam uit de databank om naar een afbeelding.
    // Als er geen foto is, of het bestand ontbreekt op schijf, wordt de
    // placeholder-afbeelding gebruikt zodat de UI nooit vastloopt.
    public class FotoConverter : IValueConverter
    {
        private static readonly string AfbeeldingenMap =
            Path.Combine(AppContext.BaseDirectory, "Assets", "Images");

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bestandsnaam = value as string;
            string pad;

            if (!string.IsNullOrWhiteSpace(bestandsnaam))
            {
                var volledigPad = Path.Combine(AfbeeldingenMap, bestandsnaam);
                pad = File.Exists(volledigPad) ? volledigPad : Path.Combine(AfbeeldingenMap, "placeholder.png");
            }
            else
            {
                pad = Path.Combine(AfbeeldingenMap, "placeholder.png");
            }

            if (!File.Exists(pad))
                return null;

            return new BitmapImage(new Uri(pad, UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
