using Godot;

namespace CookieDungeon.Scripts.Utils;

public static class ResourceManager
{
    public static class Identifier
    {
        public const string PlayerStats = "uid://cuh40jnhd72j6";
        public const string Egg = "uid://ceunla3edid8o";
    }

    public static T Load<T>(string resourceId) where T : Resource => GD.Load<T>(resourceId);
}
