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

namespace TrophyIsBetter.ViewModels
{
  public class GameListViewModel : ObservableObject, IPageViewModel
  {
    #region Private Members

    private readonly IGameListModel _model;
    private string _name = "Games";

    private ObservableCollection<GameViewModel> _gameCollection = new ObservableCollection<GameViewModel>();
    private CollectionView _gameCollectionView = null;

    private bool _isOpen = false;

    #endregion Private Members
    #region Constructors

    public GameListViewModel(IGameListModel model)
    {
      _model = model;

      ImportCommand = new RelayCommand(Import);

      GameCollectionView.CurrentChanged += OnSelectedGameChanged;

      LoadGames();
    } // GameListViewModel

    #endregion Constructors
    #region Public Properties

    /// <summary>
    /// Import a single trophy folder or a directory containing multiple
    /// </summary>
    public RelayCommand ImportCommand { get; set; }

    /// <summary>
    /// Get/Set the list of games
    /// </summary>
    public ObservableCollection<GameViewModel> GameCollection
    {
      get => _gameCollection;
      private set => SetProperty(ref _gameCollection, value);
    }

    /// <summary>
    /// Get/Set the collection view, used for sorting
    /// </summary>
    public CollectionView GameCollectionView
    {
      get
      {
        if (_gameCollectionView == null)
        {
          _gameCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(GameCollection);
        }

        return _gameCollectionView;
      }
    }

    /// <summary>
    /// Get the selected game from the list
    /// </summary>
    /// 
    public GameViewModel SelectedGame => (GameViewModel)GameCollectionView.CurrentItem;

    /// <summary>
    /// Is a game selected ready to be edited
    /// </summary>
    public bool CanEdit
    {
      get => SelectedGame != null;
    }

    /// <summary>
    /// The name of the view model
    /// </summary>
    public string Name { get => _name; set => _name = value; }

    #endregion Public Properties
    #region Public Methods

    /// <summary>
    /// Show dialog to choose a directory to import then fire off importing process
    /// </summary>
    public void Import()
    {
      string path = ChoosePath();
      if (!string.IsNullOrEmpty(path))
      {
        ImportDirectory(path);
      }
    } // Import

    /// <summary>
    /// Save files and close the directory
    /// </summary>
    public void CloseDirectory()
    {
      if (_isOpen)
      {
        _model.CloseFiles();
      }
    } // CloseDirectory

    #endregion Public Methods
    #region Private Methods

    /// <summary>
    /// Notify that the selected game has changed
    /// </summary>
    private void OnSelectedGameChanged(object sender, EventArgs e)
    {
      OnPropertyChanged(nameof(CanEdit));
    } // OnSelectedGameChanged

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
      List<Game> games = _model.LoadGames();

      if (games != null)
      {
        GameCollection.Clear();

        foreach (Game entry in games)
        {
          GameCollection.Add(new GameViewModel(entry));
        }
      }

      _isOpen = true;
    } // LoadGames

    #endregion Private Methods
  } // GameListViewModel
} // TrophyIsBetter.ViewModels
