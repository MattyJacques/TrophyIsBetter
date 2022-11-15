using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TrophyIsBetter.Extensions.Helpers
{
  internal class StringToImageConverter : IValueConverter
  {
    public object Convert(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture)
    {

      var path = (string)value;
      // load the image, specify CacheOption so the file is not locked
      var image = new BitmapImage();
      image.BeginInit();
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.UriSource = new Uri(path);
      image.EndInit();

      return image;

    }

    public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  } // StringtoImageConverter
} // TrophyIsBetter.Extensions.Helpers
