using BepInEx.Configuration;

namespace CrowdControl.UI;

/// <summary>
/// User-facing mod settings, persisted by BepInEx to
/// <c>BepInEx\config\WarpWorld.CrowdControl.cfg</c> so a streamer can turn the on-screen pieces off.
/// </summary>
/// <remarks>
/// Everything here defaults to ON. The overlay is genuinely useful - knowing at a glance whether
/// Crowd Control is connected saves a lot of "is it broken?" - but it is drawn over someone's
/// stream, so it must be possible to opt out without touching the DLL.
/// </remarks>
public static class ModSettings
{
    private const string SECTION = "Overlay";
    private const string SECTION_ADVANCED = "Advanced";

    private static ConfigEntry<bool> _showMessages;
    private static ConfigEntry<bool> _showIndicator;
    private static ConfigEntry<float> _messageSeconds;
    private static ConfigEntry<bool> _alwaysAttemptConnection;

    /// <summary>Show a line on screen when an effect fires.</summary>
    public static bool ShowMessages => _showMessages?.Value ?? true;

    /// <summary>Show the small connection dot.</summary>
    public static bool ShowIndicator => _showIndicator?.Value ?? true;

    /// <summary>How long each on-screen message stays up, in seconds.</summary>
    public static float MessageSeconds => _messageSeconds?.Value ?? 4f;

    /// <summary>Connect without first checking that the Crowd Control app is running.</summary>
    /// <remarks>
    /// On by default for Lethal Company: the mod always connected unconditionally before it moved to this
    /// client, and the presence check gets it wrong when the app runs elevated (or under Wine/Proton).
    /// The cost of connecting blind is one cheap failed connect every few seconds.
    /// </remarks>
    public static bool AlwaysAttemptConnection => _alwaysAttemptConnection?.Value ?? true;

    /// <summary>Binds the settings to the plugin's config file. Safe to call more than once.</summary>
    public static void Initialize(ConfigFile config)
    {
        try
        {
            _showMessages = config.Bind(
                SECTION, "ShowMessages", true,
                "Displays a short line when an effect fires. Turn off for a clean capture.");

            _showIndicator = config.Bind(
                SECTION, "ShowConnectionIndicator", true,
                "Small dot showing whether the mod is connected to the Crowd Control app. " +
                "Green connected, red not.");

            _messageSeconds = config.Bind(
                SECTION, "MessageSeconds", 4f,
                "How long an on-screen effect message stays visible, in seconds.");

            _alwaysAttemptConnection = config.Bind(
                SECTION_ADVANCED, "AlwaysAttemptConnection", true,
                "Connect to the Crowd Control app without first checking that it is running. Leave this on if the " +
                "app runs as administrator (the check cannot see it then). Turn it off to stop the mod trying to " +
                "connect every few seconds while the app is closed.");
        }
        catch (Exception e)
        {
            //settings are a convenience - never let them stop the mod loading
            CrowdControlMod.Instance?.Logger.LogWarning($"Could not create settings: {e.Message}");
        }
    }
}
