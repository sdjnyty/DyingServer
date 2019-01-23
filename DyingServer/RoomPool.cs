using System;
using System.Collections.Generic;
using System.Linq;
using POCO;

namespace DyingServer
{
  public static class RoomPool
  {
    private const int NumRooms = 200;
    private static List<RoomInfo> _roomList =
      Enumerable.Range(0, NumRooms)
        .Select(i => new RoomInfo { Id = i })
        .ToList();

    public static RoomInfo GetById(int rid)
    {
      return _roomList[rid];
    }

    public static IReadOnlyList<RoomInfo> Enumerate()
    {
      return _roomList;
    }
  }
}