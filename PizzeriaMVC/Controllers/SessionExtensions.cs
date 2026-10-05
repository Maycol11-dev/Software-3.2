using System.Text.Json;

namespace PizzeriaMVC.Controllers;

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
        => session.SetString(key, JsonSerializer.Serialize(value));

    public static T? Get<T>(this ISession session, string key)
    {
        var data = session.GetString(key);
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }
}
