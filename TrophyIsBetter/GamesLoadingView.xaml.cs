using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TrophyIsBetter.Models;

namespace TrophyIsBetter
{
  /// <summary>
  /// Interaction logic for GamesLoadingView.xaml
  /// </summary>
  public partial class GamesLoadingView : Window, INotifyPropertyChanged
  {
    #region Properties

    public string BarText { get; set; }
    public int GameCount { get; set; }

    #endregion
    #region Public Members

    public bool IsFinished = false;
    public string LoadedPath;
    internal ObservableCollection<Game> GameCollection = new ObservableCollection<Game>();

    #endregion
    #region Private Members

    string _pathToLoad;

    #endregion
    #region Public Methods

    public GamesLoadingView(string pathToLoad)
    {
      InitializeComponent();

      BarText = "Copying Data";
      this.DataContext = this;
      _pathToLoad = pathToLoad;
    } // GamesLoadingView

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
    #region Private Methods

    private void LoadGames(object sender, DoWorkEventArgs e)
    {
      Debug.WriteLine("Loading games");

      LoadedPath = Utility.File.CopyDirToTemp(_pathToLoad);
      string[] trophyFolders = Directory.GetDirectories(LoadedPath);

      for (int i = 0; i < trophyFolders.Length; i++)
      {
        string folder = trophyFolders[i];

        Game list = new Game(folder);

        GameCollection.Add(list);

        BarText = $"Loaded {list.Name}";
        OnPropertyChanged("BarText");
        (sender as BackgroundWorker).ReportProgress((int)Math.Ceiling((double)(100 * i) / trophyFolders.Length));
      }

      IsFinished = true;
      e.Result = GameCollection;
    } // LoadGames

    private void UpdateProgress(object sender, ProgressChangedEventArgs e)
    {
      loadingStatus.Value = e.ProgressPercentage;
    } // UpdateProgress

    private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      //((GameListView)Application.Current.MainWindow).GameCollection = (ObservableCollection<GameListEntry>)e.Result;
      OnPropertyChanged("GameCollection");
    } // WorkerCompleted

    #endregion
    #region Events

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      BackgroundWorker worker = new BackgroundWorker();
      worker.WorkerReportsProgress = true;
      worker.DoWork += LoadGames;
      worker.ProgressChanged += UpdateProgress;
      worker.RunWorkerCompleted += WorkerCompleted;

      worker.RunWorkerAsync();
    } // Window_ContentRendered

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    } // OnPropertyChanged

    #endregion
  }
}
