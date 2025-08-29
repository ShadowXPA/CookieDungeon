namespace CookieDungeon.Scripts.Utils;

public static class SignalBus
{
    public static Action<int>? LevelUpdated;
    public static void BroadcastLevelUpdated(int level) => LevelUpdated?.Invoke(level);
    public static Action<int, int>? HealthUpdated;
    public static void BroadcastHealthUpdated(int health, int maxHealth) => HealthUpdated?.Invoke(health, maxHealth);
    public static Action<int, int>? ManaUpdated;
    public static void BroadcastManaUpdated(int mana, int maxMana) => ManaUpdated?.Invoke(mana, maxMana);
    public static Action<float, float>? DashCooldownUpdated;
    public static void BroadcastDashCooldownUpdated(float cooldown, float maxCooldown) => DashCooldownUpdated?.Invoke(cooldown, maxCooldown);
}
