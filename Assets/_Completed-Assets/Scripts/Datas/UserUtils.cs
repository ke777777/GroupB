using UnityEngine;
using System;

public static class UserUtils
{
    public const string UserIdKey = "user_id";
    public const string UserNameKey = "user_name";
    public static string UniqueKeySuffix { get; private set; }

    // 起動時に一度だけ呼び出してGUIDを生成
    public static void Initialize()
    {
        if (string.IsNullOrEmpty(UniqueKeySuffix))
        {
            // GUIDで一意なサフィックスを生成する
            UniqueKeySuffix = Guid.NewGuid().ToString();
        }
    }

    public static int GetUserId()
    {
        return PlayerPrefs.GetInt(UserIdKey + UniqueKeySuffix, 0);
    }

    public static void SetUserIdAndName(int userId, string userName)
    {
        PlayerPrefs.SetInt(UserIdKey + UniqueKeySuffix, userId);
        PlayerPrefs.SetString(UserNameKey + UniqueKeySuffix, userName);
        PlayerPrefs.Save();
    }

    public static string GetUserName()
    {
        return PlayerPrefs.GetString(UserNameKey + UniqueKeySuffix, "NoName");
    }
}
