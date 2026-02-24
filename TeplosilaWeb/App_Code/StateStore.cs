using System;
using System.Web;

public static class StateStore
{
    private static string BuildKey(string token, string key)
    {
        return $"{token}:{key}";
    }

    public static void Set(string token, string key, object value, int minutes = 60)
    {
        string fullKey = BuildKey(token, key);

        HttpRuntime.Cache.Insert(
            fullKey,
            value,
            null,
            DateTime.Now.AddMinutes(minutes),
            System.Web.Caching.Cache.NoSlidingExpiration
        );
    }

    public static T Get<T>(string token, string key)
    {
        string fullKey = BuildKey(token, key);

        var value = HttpRuntime.Cache.Get(fullKey);
        if (value == null)
            return default(T);

        return (T)value;
    }

    public static void Remove(string token, string key)
    {
        string fullKey = BuildKey(token, key);
        HttpRuntime.Cache.Remove(fullKey);
    }
}
