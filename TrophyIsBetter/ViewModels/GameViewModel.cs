using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using TrophyIsBetter.Interfaces;
using TrophyIsBetter.Views;
using static TrophyParser.Enums;

namespace TrophyIsBetter.ViewModels
{
  internal class GameViewModel : ObservableObject, IPageViewModel
  {
    #region Private Members

    private IGameModel _model;
    private ObservableCollection<TrophyViewModel> _trophyCollection = new ObservableCollection<TrophyViewModel>();
    private CollectionView _trophyCollectionView;
    private DateTime? _lastUsedTimestamp = null;

    #endregion Private Members
    #region Constructors

    internal GameViewModel(IGameModel entry)
    {
      _model = entry;
      ((ApplicationWindow)Application.Current.MainWindow).Closing += OnCloseSave; ;

      ChangeViewToHomeCommand = new RelayCommand(ChangeViewToHome);
      EditTrophyCommand = new RelayCommand(EditTrophy, () => CanEdit);
      LockTrophyCommand = new RelayCommand(LockTrophy, () => CanLock);
      LockUnsyncedCommand = new RelayCommand(LockUnsynced);
      CopyFromCommand = new RelayCommand(CopyFrom);

      _trophyCollectionView =
        (CollectionView)CollectionViewSource.GetDefaultView(_trophyCollection);

      TrophyCollectionView.CurrentChanged += OnSelectedTrophyChanged;

      LoadTrophies();
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public CollectionView TrophyCollectionView { get => _trophyCollectionView; }

    /// <summary>
    /// Change the view back to the game list
    /// </summary>
    public RelayCommand ChangeViewToHomeCommand { get; set; }

    /// <summary>
    /// Edit the timestamp of a single trophy
    /// </summary>
    public RelayCommand EditTrophyCommand { get; set; }

    /// <summary>
    /// Lock a single trophy
    /// </summary>
    public RelayCommand LockTrophyCommand { get; set; }

    /// <summary>
    /// Lock all unsyncronised trophies
    /// </summary>
    public RelayCommand LockUnsyncedCommand { get; set; }

    /// <summary>
    /// Lock all unsyncronised trophies
    /// </summary>
    public RelayCommand CopyFromCommand { get; set; }

    public Visibility IconVisibility
      => _model.Platform == PlatformEnum.PS3 ? Visibility.Visible : Visibility.Hidden;

    public string Icon { get => _model.Icon; }

    public string Name { get => _model.Name; }

    public string NpCommunicationID { get => _model.NpCommID; }

    public PlatformEnum Platform { get => _model.Platform; }

    public bool HasPlatinum { get => _model.HasPlatinum; }

    public bool IsSynced { get => _model.IsSynced; }

    public int EarnedCount { get => _model.EarnedCount; }

    public int TrophyCount { get => _model.TrophyCount; }

    public string ProgressString { get => $"{EarnedCount}/{TrophyCount} ({EarnedExp}/{TotalExp})"; }

    public int EarnedExp { get => _model.EarnedExp; }

    public int TotalExp { get => _model.TotalExp; } 

    public DateTime? LastTimestamp { get => _model.LastTimestamp; }

    public DateTime? SyncTime { get => _model.SyncTime; }

    #endregion Public Properties
    #region Internal Properties

    internal ObservableCollection<TrophyViewModel> TrophyCollection
    {
      get => _trophyCollection;
      private set => SetProperty(ref _trophyCollection, value);
    }

    internal TrophyViewModel SelectedTrophy
    {
      get => (TrophyViewModel)TrophyCollectionView.CurrentItem;
    }

    /// <summary>
    /// Is a game selected ready to be edited
    /// </summary>
    internal bool CanEdit
      => SelectedTrophy != null && SelectedTrophy.Type != 'P';

    /// <summary>
    /// Is a game selected ready to be locked
    /// </summary>
    internal bool CanLock => SelectedTrophy != null
                          && SelectedTrophy.Achieved
                          && !SelectedTrophy.Synced
                          && SelectedTrophy.Type != 'P';

    internal string Path { get => _model.Path; }

    internal IGameModel Model { get => _model; }

    #endregion Internal Properties
    #region Private Methods

    private void ChangeViewToHome()
    {
      Save();

      // Go back to game list
      ((ApplicationViewModel)Application.Current.MainWindow.DataContext)
        .ChangePageToHomeCommand.Execute(null);
    } // ChangeViewToHome

    private void LoadTrophies()
    {
      List<ITrophyModel> trophies = _model.Trophies;

      if (trophies.Count != 0)
      {
        TrophyCollection.Clear();

        foreach (ITrophyModel trophy in trophies)
        {
          TrophyCollection.Add(new TrophyViewModel(trophy));
        }
      }
    } // LoadTrophies

    /// <summary>
    /// Notify that the selected trophy has changed
    /// </summary>
    private void OnSelectedTrophyChanged(object sender, EventArgs e)
      => LockTrophyCommand.NotifyCanExecuteChanged();

    /// <summary>
    /// Ask the user if they want to save when closing
    /// </summary>
    private void OnCloseSave(object sender, System.ComponentModel.CancelEventArgs e) => Save();

    /// <summary>
    /// Ask the user if they want to save
    /// </summary>
    private void Save()
    {
      if (_model.HasUnsavedChanges)
      {
        if (MessageBox.Show($"There are unsaved changes in {Name}, would you like to save?", "Save?",
            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
          _model.Save();
        }
        else
        {
          _model.Reload();
          LoadTrophies();
        }
      }
    } // Save

    private void EditTrophy()
    {
      EditTimestampWindow window = new EditTimestampWindow("Edit Timestamp")
      {
        DataContext = new EditTimestampViewModel(SelectedTrophy.Timestamp
                                              ?? _lastUsedTimestamp
                                              ?? DateTime.Now)
      };

      bool? result = window.ShowDialog();

      if (result == true)
      {
        DateTime timestamp = ((EditTimestampViewModel)window.DataContext).Timestamp;

        _model.UnlockTrophy(SelectedTrophy.Model, timestamp);

        _lastUsedTimestamp = timestamp;
        SelectedTrophy.Timestamp = timestamp;
        SelectedTrophy.Achieved= true;

        CheckPlatinum();
        NotifyTrophyChanges();
      }
    } // EditTrophy

    private void CheckPlatinum()
    {
      if (TrophyCollection[0].Type == 'P')
      {
        if (!TrophyCollection.Where(x => x.Group == "Base Game"
                                      && x.Achieved == false
                                      && x.Type != 'P').Any())
        {
          DateTime timestamp = GetPlatinumTimestamp();

          _model.UnlockTrophy(TrophyCollection[0].Model, timestamp);
          TrophyCollection[0].Timestamp = timestamp;
          TrophyCollection[0].Achieved= true;
        }
        else if (TrophyCollection[0].Achieved
              && TrophyCollection.Where(x => x.Group == "Base Game"
                                          && x.Achieved == false
                                          && x.Type != 'P').Any())
        {
          _model.LockTrophy(TrophyCollection[0].Model);
          TrophyCollection[0].Timestamp = null;
          TrophyCollection[0].Achieved= false;
        }
      }
    } // CheckPlatinum

    private DateTime GetPlatinumTimestamp()
    {
      int offset = _model.Platform == PlatformEnum.PS3 ? 1 : 0;
      return _model.LastTimestamp.Value.AddSeconds(offset);
    } // GetPlatinumTimestamp

    private void CopyFrom()
    {
      CopyFromWindow window = new CopyFromWindow
      {
        DataContext = new CopyFromViewModel(TrophyCollection)
      };

      bool? result = window.ShowDialog();

      if (result == true)
      {
        CopyTimestamps();

        NotifyTrophyChanges();
      }
    } // CopyFrom

    /// <summary>
    /// Unlock trophies using the remote timestamps
    /// </summary>
    private void CopyTimestamps()
    {
      foreach(TrophyViewModel trophy in TrophyCollection)
      {
        if (trophy.ShouldCopy && trophy.RemoteTimestamp.HasValue)
        {
          _model.UnlockTrophy(trophy.Model, trophy.RemoteTimestamp.Value);

          trophy.Timestamp = trophy.RemoteTimestamp.Value;
          trophy.RemoteTimestamp = null;
          trophy.ShouldCopy = false;
          trophy.Synced = false;
        }
      }
    } // CopyTimestamps

    private void LockTrophy()
    {
      _model.LockTrophy(SelectedTrophy.Model);
      SelectedTrophy.Timestamp = null;
      SelectedTrophy.Achieved = false;

      CheckPlatinum();
      NotifyTrophyChanges();
    } // LockTrophy

    private void LockUnsynced()
    {
      foreach (TrophyViewModel trophy in
        TrophyCollection.Where(trophy => trophy.Achieved && !trophy.Synced))
      {
        _model.LockTrophy(trophy.Model);
        trophy.Timestamp = null;
        trophy.Achieved = false;
      }

      NotifyTrophyChanges();
    } // LockUnsynced

    private void NotifyTrophyChanges()
    {
      LockTrophyCommand.NotifyCanExecuteChanged();
      OnPropertyChanged(nameof(LastTimestamp));
      OnPropertyChanged(nameof(EarnedCount));
      OnPropertyChanged(nameof(ProgressString));
    } // NotifyTrophyChanges

    #endregion Private Methods

  } // GameListViewModel
} // TrophyIsBetter.ViewModels
