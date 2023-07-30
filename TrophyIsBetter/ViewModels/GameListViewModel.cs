using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TrophyIsBetter.Interfaces;
using TrophyIsBetter.Models;
using TrophyIsBetter.Views;
using TrophyParser.PS4;

namespace TrophyIsBetter.ViewModels
{
  internal class GameListViewModel : ObservableObject, IPageViewModel
  {
    #region Private Members

    private readonly IGameListModel _model;
    private string _name = "Games";

    private ObservableCollection<GameViewModel> _gameCollection = new ObservableCollection<GameViewModel>();
    private ListCollectionView _gameCollectionView = null;
    private readonly SynchronizationContext _uiContext = SynchronizationContext.Current;

    private bool _hasGames = false;

    #endregion Private Members
    #region Constructors

    internal GameListViewModel(IGameListModel model)
    {
      _model = model;

      ImportCommand = new AsyncRelayCommand(Import);
      EditGameCommand = new RelayCommand(EditGame);
      ExportGameCommand = new RelayCommand(ExportGame);
      ExportAllGamesCommand = new RelayCommand(ExportAllGames);
      RemoveGameCommand = new RelayCommand(RemoveGame);
      RemoveAllGamesCommand = new RelayCommand(RemoveAllGames, () => HasGames);
      TrophyListCommand = new RelayCommand(OpenTrophyList);
      OpenPS4DBCommand = new RelayCommand(OpenPS4DB);

      _gameCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(GameCollection);
      _gameCollectionView.SortDescriptions.Add(new SortDescription(nameof(GameViewModel.LastTimestamp),
                                                                   ListSortDirection.Descending));

      GameCollectionView.CurrentChanged += OnSelectedGameChanged;

      LoadGames();
    } // GameListViewModel

    #endregion Constructors
    #region Public Properties

    /// <summary>
    /// Import a single trophy folder or a directory containing multiple
    /// </summary>
    public AsyncRelayCommand ImportCommand { get; set; }

    /// <summary>
    /// Edit a games trophies
    /// </summary>
    public RelayCommand EditGameCommand { get; set; }

    /// <summary>
    /// Encrypt the files and export the game
    /// </summary>
    public RelayCommand ExportGameCommand { get; set; }

    /// <summary>
    /// Encrypt the files and export all of the games
    /// </summary>
    public RelayCommand ExportAllGamesCommand { get; set; }

    /// <summary>
    /// Remove a game from the game list
    /// </summary>
    public RelayCommand RemoveGameCommand { get; set; }

    /// <summary>
    /// Remove all games from the game list
    /// </summary>
    public RelayCommand RemoveAllGamesCommand { get; set; }

    /// <summary>
    /// List all trophies chronologically
    /// </summary>
    public RelayCommand TrophyListCommand { get; set; }

    /// <summary>
    /// Open a PS4 trophy_db
    /// </summary>
    public RelayCommand OpenPS4DBCommand { get; set; }

    /// <summary>
    /// Get/Set the collection view, used for sorting
    /// </summary>
    public ListCollectionView GameCollectionView
    {
      get
      {
        // Horrific hack to fix sorting not being applied when view switches
        // It keeps losing the SortDescriptions
        _gameCollectionView.SortDescriptions.Add(new SortDescription(nameof(GameViewModel.LastTimestamp),
                                                                     ListSortDirection.Descending));

        return _gameCollectionView;
      }
    }

    /// <summary>
    /// Is a game selected
    /// </summary>
    public bool HasSelected => SelectedGame != null;

    /// <summary>
    /// Is a game selected
    /// </summary>
    public bool HasGames { get => _hasGames; set => SetProperty(ref _hasGames, value); }

    /// <summary>
    /// The name of the view model
    /// </summary>
    public string Name { get => _name; set => _name = value; }

    /// <summary>
    /// String showing current trophy progress
    /// </summary>
    public string ProgressString =>
      $"{TotalEarnedCount}/{TotalCount} ({TotalEarnedExp}/{TotalExp})";

    /// <summary>
    /// Total earned trophies
    /// </summary>
    public int TotalEarnedCount => GameCollection.Sum(game => game.EarnedCount);

    /// <summary>
    /// Total amount of trophies in lists
    /// </summary>
    public int TotalCount => GameCollection.Sum(game => game.TrophyCount);

    #endregion Public Properties
    #region Internal Properties

    /// <summary>
    /// Get/Set the list of games
    /// </summary>
    internal ObservableCollection<GameViewModel> GameCollection
    {
      get => _gameCollection;
      private set => SetProperty(ref _gameCollection, value);
    }

    /// <summary>
    /// Get the selected game from the list
    /// </summary>
    internal GameViewModel SelectedGame => (GameViewModel)_gameCollectionView.CurrentItem;

    /// <summary>
    /// Total amount of earned exp
    /// </summary>
    internal int TotalEarnedExp => GameCollection.Sum(game => game.EarnedExp);

    /// <summary>
    /// Total possible exp
    /// </summary>
    internal int TotalExp => GameCollection.Sum(game => game.TotalExp);

    #endregion Internal Properties
    #region Private Methods

    /// <summary>
    /// Notify that the selected game has changed
    /// </summary>
    private void OnSelectedGameChanged(object sender, EventArgs e)
    {
      OnPropertyChanged(nameof(HasSelected));
    } // OnSelectedGameChanged

    /// <summary>
    /// Show dialog to choose a directory to import then fire off importing process
    /// </summary>
    private Task Import()
    {
      string path = ChoosePath();
      if (!string.IsNullOrEmpty(path))
      {
        return Task.Run(() => ImportDirectory(path));
      }
      return Task.CompletedTask;
    } // Import

    /// <summary>
    /// Choose path to import
    /// </summary>
    private string ChoosePath()
    {
      string path = "";

      System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
      if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        path = dialog.SelectedPath;
      }

      return path;
    } // ChoosePath

    /// <summary>
    /// Import trophy folder contained in the given directory
    /// </summary>
    private void ImportDirectory(string path)
    {
      try
      {
        _model.ImportGames(path);
        LoadGames();
      }
      catch (Exception ex)
      {
        GC.Collect();
        Console.WriteLine(ex.StackTrace);
        MessageBox.Show("Import Failed:" + ex.Message);
      }
    } // ImportDirectory

    /// <summary>
    /// Load the folders contained in the game directory
    /// </summary>
    private void LoadGames()
    {
      List<IGameModel> games = _model.LoadGames();

      if (games != null)
      {
        _uiContext.Send(x => GameCollection.Clear(), null);

        foreach (Game entry in games)
        {
          _uiContext.Send(x => GameCollection.Add(new GameViewModel(entry, this)), null);
        }

        _uiContext.Send(x => NotifyGameChanges(), null);
      }
    } // LoadGames

    /// <summary>
    /// Switch page to the single game view
    /// </summary>
    private void EditGame()
    {
      ((ApplicationViewModel)Application.Current.MainWindow.DataContext)
        .ChangePageCommand.Execute(SelectedGame);
    } // EditGame

    /// <summary>
    /// Encrypt the files and export
    /// </summary>
    private void ExportGame()
    {
      string path = ChoosePath();
      if (!string.IsNullOrEmpty(path))
      {
        try
        {
          SelectedGame.Export(path);
        }
        catch (Exception ex)
        {
          GC.Collect();
          Console.WriteLine(ex.StackTrace);
          MessageBox.Show("Export Failed:" + ex.Message);
        }
      }
    } // ExportGame

    /// <summary>
    /// Encrypt the files and export
    /// </summary>
    private void ExportAllGames()
    {
      string path = ChoosePath();
      if (!string.IsNullOrEmpty(path))
      {
        try
        {
          foreach (GameViewModel game in GameCollection)
          {
            game.Export(path);
          }
        }
        catch (Exception ex)
        {
          GC.Collect();
          Console.WriteLine(ex.StackTrace);
          MessageBox.Show("Export Failed:" + ex.Message);
        }
      }
    } // ExportGame

    /// <summary>
    /// Remove a single game from the game list
    /// </summary>
    private void RemoveGame()
    {
      if (CheckShouldRemove(SelectedGame.Name))
      {
        _model.RemoveGame(SelectedGame.Model);
        GameCollection.Remove(SelectedGame);

        NotifyGameChanges();
      }
    } // RemoveGame

    /// <summary>
    /// Remove all games from game list
    /// </summary>
    private void RemoveAllGames()
    {
      if (CheckShouldRemove("ALL GAMES"))
      {
        foreach (GameViewModel game in GameCollection)
        {
          _model.RemoveGame(game.Model);
        }

        GameCollection.Clear();

        NotifyGameChanges();
      }
    } // RemoveAll

    /// <summary>
    /// List all trophies chronologically
    /// </summary>
    private void OpenTrophyList()
    {
      ObservableCollection<TrophyViewModel> trophies =
        new ObservableCollection<TrophyViewModel>();

      foreach (GameViewModel game in GameCollection)
      {
        game.TrophyCollection.ToList().ForEach(trophies.Add);
      }

      TrophyListWindow window = new TrophyListWindow()
      {
        DataContext = new TrophyListViewModel(trophies)
      };

      window.Show();
    } // OpenTrophyList

    /// <summary>
    /// Open PS4 trophy_db
    /// </summary>
    private void OpenPS4DB()
    {
      string path = ChoosePS4DBPath();

      if (path.Length > 0)
      {
        PS4GameListViewModel viewModel = new PS4GameListViewModel(new PS4GameList(path));
        ((ApplicationViewModel)Application.Current.MainWindow.DataContext)
          .ChangePageCommand.Execute(viewModel);
      }
    } // OpenPS4DB

    /// <summary>
    /// Choose path to PS4 DB
    /// </summary>
    private string ChoosePS4DBPath()
    {
      string path = "";

      var dialog = new Microsoft.Win32.OpenFileDialog
      {
        FileName = "trophy_local",
        DefaultExt = ".db",
        Filter = "Database files (.db)|*.db"
      };

      if (dialog.ShowDialog() == true)
      {
        path = dialog.FileName;
      }

      return path;
    } // ChoosePS4DBPath

    private bool CheckShouldRemove(string gameName)
      => MessageBox.Show($"Are you sure you want to delete {gameName}?", "Delete?",
                         MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;

    private void NotifyGameChanges()
    {
      HasGames = GameCollection.Count > 0;
      OnPropertyChanged(nameof(ProgressString));
    } // NotifyChanges

    #endregion Private Methods
  } // GameListViewModel
} // TrophyIsBetter.ViewModels
