using System;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="passwordMd5"></param>
    /// <returns>登录成功返回userId；用户名不存在或密码错误返回0</returns>
    public static int Login(string userName, string passwordMd5)
    {
      passwordMd5 = passwordMd5.ToLowerInvariant();
      using (var c = GetConnection())
      {
        var result= c.QueryFirstOrDefault("SELECT `uid` AS `UserId`,`password`=md5(concat(@passwordMd5,`salt`)) AS `GoodPwd` FROM `x15_ucenter_members` WHERE `username`=@userId",new{userId = userName,passwordMd5});
        if (result == null)
        {
          return 0;
        }
        else if (result.GoodPwd==0)
        {
          return 0;
        }
        else
        {
          return (int)result.UserId;
        }
      }
    }
  }
}