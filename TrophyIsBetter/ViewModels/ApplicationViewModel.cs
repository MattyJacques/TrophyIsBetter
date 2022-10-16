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
  public class ApplicationViewModel : ObservableObject
  {
    #region Private Members

    private readonly List<IPageViewModel> _pageViewModels = new List<IPageViewModel>();
    private ICommand _changePageCommand;

    private IPageViewModel _currentPageViewModel;

    #endregion Private Members

    #region Public Constructors

    public ApplicationViewModel()
    {
      WindowClosing = new RelayCommand<CancelEventArgs>(OnWindowClosing);

      GameList gameListModel = new GameList();

      // Add available pages
      PageViewModels.Add(new GameListViewModel(gameListModel));

      // Set starting page
      CurrentPageViewModel = PageViewModels[0];
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// Command to change to the view of the window
    /// </summary>
    public ICommand ChangePageCommand
    {
      get
      {
        if (_changePageCommand == null)
        {
          _changePageCommand = new RelayCommand<IPageViewModel>(p => ChangeViewModel(p),
                                                                p => p is IPageViewModel);
        }

        return _changePageCommand;
      }
    }

    public IPageViewModel CurrentPageViewModel
    {
      get => _currentPageViewModel;
      set => SetProperty(ref _currentPageViewModel, value);
    }

    public List<IPageViewModel> PageViewModels { get => _pageViewModels; }

    public ICommand WindowClosing { get; private set; }

    #endregion Public Properties

    #region Private Methods

    /// <summary>
    /// Change the ViewModel to the given type, changing the View
    /// </summary>
    /// <param name="viewModel">View model to change to</param>
    private void ChangeViewModel(IPageViewModel viewModel)
    {
      if (!PageViewModels.Contains(viewModel))
      {
        PageViewModels.Add(viewModel);
      }

      CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
    } // ChangeViewModel

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