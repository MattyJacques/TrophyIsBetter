using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TrophyIsBetter.Interfaces;
using TrophyIsBetter.Models;

namespace TrophyIsBetter.ViewModels
{
  internal class ApplicationViewModel : ObservableObject
  {
    #region Private Members

    private readonly List<IPageViewModel> _pageViewModels = new List<IPageViewModel>();

    private IPageViewModel _currentPageViewModel;

    #endregion Private Members
    #region Internal Constructors

    internal ApplicationViewModel()
    {
      WindowClosing = new RelayCommand<CancelEventArgs>(OnWindowClosing);

      ChangePageCommand = new RelayCommand<IPageViewModel>(p => ChangeViewModel(p));
      ChangePageToHomeCommand = new RelayCommand(ChangePageToGameList);

      // Add available pages
      PageViewModels.Add(new GameListViewModel(new GameList()));

      ChangePageToGameList();
    } // Constructor

    #endregion Internal Constructors
    #region Public Properties

    public IPageViewModel CurrentPageViewModel
    {
      get => _currentPageViewModel;
      set => SetProperty(ref _currentPageViewModel, value);
    }

    public ICommand WindowClosing { get; private set; }

    #endregion Public Properties
    #region Internal Properties

    internal List<IPageViewModel> PageViewModels { get => _pageViewModels; }

    internal ICommand ChangePageCommand { get; set; }

    internal ICommand ChangePageToHomeCommand { get; set; }

    #endregion Internal Properties
    #region Private Methods

    /// <summary>
    /// Change the ViewModel to the given type, changing the View
    /// </summary>
    /// <param name="viewModel">View model to change to</param>
    private void ChangeViewModel(IPageViewModel viewModel)
    {
      if (viewModel != null)
      {
        if (!PageViewModels.Contains(viewModel))
        {
          PageViewModels.Add(viewModel);
        }

        CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
      }
    } // ChangeViewModel

    /// <summary>
    /// Change the ViewModel to the Game List ViewModel
    /// </summary>
    private void ChangePageToGameList()
    {
      ChangePageCommand.Execute(PageViewModels[0]);
    } // ChangePageToGameList

    /// <summary>
    /// Confirm exit of the application
    /// </summary>
    /// <param name="viewModel">View model to change to</param>
    private void OnWindowClosing(CancelEventArgs args)
    {
      if (!ConfirmExit())
      {
        args.Cancel = true;
      }
    } // ChangeViewModel

    /// <summary>
    /// Confirm with the user that they want to close the window
    /// </summary>
    private bool ConfirmExit()
    {
      MessageBoxResult result = MessageBox.Show("Are you sure you want to close?",
                                                "Closing",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Warning);

      return result == MessageBoxResult.Yes;
    } // ChoosePath

    #endregion Private Methods
  }
}