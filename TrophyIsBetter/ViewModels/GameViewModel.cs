using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows.Documents;
using System.Windows.Forms;
using TrophyIsBetter.Interfaces;
using TrophyIsBetter.Models;

namespace TrophyIsBetter.ViewModels
{
  public class GameViewModel : ObservableObject, IPageViewModel
  {
    #region Private Members

    private Game _model;
    private ObservableCollection<TrophyViewModel> _trophyCollection = new ObservableCollection<TrophyViewModel>();

    #endregion Private Members
    #region Constructors

    public GameViewModel()
    {
    } // Constructor()

    public GameViewModel(Game entry)
    {
      _model = entry;

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

    #endregion
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
    }

    #endregion Private Methods

  } // GameListViewModel
} // TrophyIsBetter.ViewModels
