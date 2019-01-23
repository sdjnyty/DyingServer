using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using POCO;

namespace DyingClientWpf
{
  public class RoomViewModel : INotifyPropertyChanged
  {
    private RoomInfo _ri;
    private int _maxPlayers;

    public event PropertyChangedEventHandler PropertyChanged;

    public int MaxPlayers
    {
      get => _maxPlayers;
      set
      {
        if (value != _maxPlayers)
        {
          _maxPlayers = value;
          OnPropertyChanged(nameof(MaxPlayers));
        }
      }
    }

    public string Number => $"房间 #{_ri.Id}";

    public RoomViewModel(RoomInfo ri)
    {
      _ri = ri;
    }

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
