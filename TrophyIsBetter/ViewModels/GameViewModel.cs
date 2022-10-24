using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

      EditTrophyCommand = new RelayCommand(EditTrophy);

      _trophyCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(_trophyCollection);

      LoadTrophies();
    } // Constructor(Game)

    #endregion Constructors
    #region Public Properties

    public Game Model { get => _model; set => SetProperty(ref _model, value); }

    public ObservableCollection<TrophyViewModel> TrophyCollection
    {
      get => _trophyCollection;
      private set => SetProperty(ref _trophyCollection, value);
    }

    public CollectionView TrophyCollectionView { get => _trophyCollectionView; }

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
        ((TrophyViewModel)TrophyCollectionView.CurrentItem).Timestamp =
          ((EditTimestampViewModel)window.DataContext).Timestamp;
      }
    } // EditTrophy

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

    #endregion Private Methods

  } // GameListViewModel
} // TrophyIsBetter.ViewModels
