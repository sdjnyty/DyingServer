using MySql.Data.MySqlClient;
using Dapper;

namespace DyingServer
{
  public static class DAL
  {
    private static readonly MySqlConnectionStringBuilder _csb = new MySqlConnectionStringBuilder
    {
      Server = "hawkaoe.net",
      Port = 3306,
      UserID = "DyingServer",
      Password = "yty@HawkAoe",
      Database = "a1103184010541",
    };

    private static MySqlConnection GetConnection()
    {
      var ret = new MySqlConnection(_csb.ConnectionString);
      ret.Open();
      return ret;
    }

    public static bool Login(string userId, string passwordMd5)
    {
      using (var c = GetConnection())
      {
        return c.QueryFirstOrDefault<bool>("SELECT `password`=md5(concat(md5(@passwordMd5),`salt`)) FROM `x15_ucenter_members` WHERE `username`=@userId",new{userId,passwordMd5});
      }
    }
  }
}