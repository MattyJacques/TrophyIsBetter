using System.Windows;

namespace TrophyIsBetter.Views
{
  /// <summary>
  /// Interaction logic for EditTimestampWindow.xaml
  /// </summary>
  public partial class EditTimestampWindow : Window
  {
    #region Constructors

    public EditTimestampWindow()
    {
      InitializeComponent();
    } // Constructor

    #endregion Constructors
    #region Event Handlers

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    } // Confirm_Click

    #endregion Event Handlers
  } // EditTimestampWindow
} // TrophyIsBetter.Views
