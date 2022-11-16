using System.Windows;

namespace TrophyIsBetter.Views
{
  /// <summary>
  /// Interaction logic for CopyFromWindow.xaml
  /// </summary>
  public partial class CopyFromWindow : Window
  {
    public CopyFromWindow()
    {
      InitializeComponent();
    } // Constructor

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    } // Confirm_Click
  } // CopyFromWindow
} // TrophyIsBetter.Views
