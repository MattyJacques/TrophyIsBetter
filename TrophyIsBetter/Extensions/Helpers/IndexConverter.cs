using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace TrophyIsBetter.Extensions.Helpers
{
  internal class IndexConverter : IValueConverter
  {
    public object Convert(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture)
    {
      ListViewItem item = (ListViewItem)value;
      ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
      return (listView.ItemContainerGenerator.IndexFromContainer(item) + 1).ToString();
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
