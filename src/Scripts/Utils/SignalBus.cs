namespace CookieDungeon.Scripts.Utils;

public static class SignalBus
{
    public static Action<string, string>? StateChanged;
    public static void BroadcastStateChanged(string oldState, string newState) => StateChanged?.Invoke(oldState, newState);
}
