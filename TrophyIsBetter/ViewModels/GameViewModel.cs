using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TrophyIsBetter.Interfaces;
using TrophyIsBetter.Models;
using TrophyIsBetter.Views;

namespace TrophyIsBetter.ViewModels
{
  public class GameViewModel : ObservableObject, IPageViewModel
  {
    #region Private Members

    private Game _model;
    private ObservableCollection<TrophyViewModel> _trophyCollection = new ObservableCollection<TrophyViewModel>();
    private CollectionView _trophyCollectionView;

    #endregion Private Members
    #region Constructors

    public GameViewModel(Game entry)
    {
      _model = entry;
      ((ApplicationWindow)Application.Current.MainWindow).Closing += OnCloseSave; ;

      ChangeViewToHomeCommand = new RelayCommand(ChangeViewToHome);
      EditTrophyCommand = new RelayCommand(EditTrophy);

      _trophyCollectionView =
        (CollectionView)CollectionViewSource.GetDefaultView(_trophyCollection);

      LoadTrophies();
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public ObservableCollection<TrophyViewModel> TrophyCollection
    {
      get => _trophyCollection;
      private set => SetProperty(ref _trophyCollection, value);
    }

    public CollectionView TrophyCollectionView { get => _trophyCollectionView; }

    /// <summary>
    /// Change the view back to the game list
    /// </summary>
    public ICommand ChangeViewToHomeCommand { get; set; }

    /// <summary>
    /// Edit the timestamp of a single trophy
    /// </summary>
    public ICommand EditTrophyCommand { get; set; }

    public string Icon { get => _model.Icon; }

    public string Name { get => _model.Name; }

    public string NpCommunicationID { get => _model.NpCommID; }

    public string Platform { get => _model.Platform; }

    public bool HasPlatinum { get => _model.HasPlatinum; }

    public bool IsSynced { get => _model.IsSynced; }

    public string Progress { get => _model.Progress; }

    public DateTime? LastTimestamp { get => _model.LastTimestamp; }

    public DateTime? SyncTime { get => _model.SyncTime; }

    public string Path { get => _model.Path; }

    #endregion Public Properties
    #region Public Methods

    public void EditTrophy()
    {
      EditTimestampWindow window = new EditTimestampWindow();
      window.DataContext = new EditTimestampViewModel();

      bool? result = window.ShowDialog();

      if (result == true)
      {
        TrophyViewModel selectedTrophy = (TrophyViewModel)TrophyCollectionView.CurrentItem;
        DateTime timestamp = ((EditTimestampViewModel)window.DataContext).Timestamp;

        _model.UnlockTrophy(selectedTrophy.Model, timestamp);
        selectedTrophy.Timestamp = timestamp;
      }
    } // EditTrophy

    public void ChangeViewToHome()
    {
      _model.Save();

      // Go back to game list
      ((ApplicationViewModel)Application.Current.MainWindow.DataContext)
        .ChangePageToHomeCommand.Execute(null);
    } // ChangeViewToHome

    #endregion Public Methods
    #region Private Methods

    private void LoadTrophies()
    {
      List<Trophy> trophies = _model.Trophies;

      if (trophies.Count != 0)
      {
        TrophyCollection.Clear();

        foreach (Trophy trophy in trophies)
        {
          TrophyCollection.Add(new TrophyViewModel(trophy));
        }
      }
    } // LoadTrophies

    private void OnCloseSave(object sender, System.ComponentModel.CancelEventArgs e)
    {
      _model.Save();
    } // OnCloseSave

    #endregion Private Methods

  } // GameListViewModel
} // TrophyIsBetter.ViewModels
