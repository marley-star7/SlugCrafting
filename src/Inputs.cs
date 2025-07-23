namespace SlugCrafting;

public static class Inputs
{
    public static PlayerKeybind? AlternateUse = null;
    public static PlayerKeybind? Scavenge = null;
    public static PlayerKeybind? Craft = null;

    internal static void RegisterInputs()
    {
        var alternateUseId = Plugin.ID + ":alternate_use";
        var inputScavengeId = Plugin.ID + ":scavenge";
        var inputCraftId = Plugin.ID + ":craft";

        if (Inputs.AlternateUse == null)
            Inputs.AlternateUse = PlayerKeybind.Get(alternateUseId);

        if (Inputs.AlternateUse == null)
            Inputs.AlternateUse = PlayerKeybind.Register(alternateUseId, Plugin.NAME, "AlternateUse", KeyCode.C, KeyCode.Joystick1Button3); // TODO: might not be the right keybind... like at all lol, looking for same as special

        if (Inputs.Scavenge == null)
            Inputs.Scavenge = PlayerKeybind.Get(inputScavengeId);

        if (Inputs.Scavenge == null)
            Inputs.Scavenge = PlayerKeybind.Register(inputScavengeId, Plugin.NAME, "Scavenge", KeyCode.LeftShift, KeyCode.JoystickButton2);

        if (Inputs.Craft == null)
            Inputs.Craft = PlayerKeybind.Get(inputCraftId);

        if (Inputs.Craft == null)
            Inputs.Craft = PlayerKeybind.Register(inputCraftId, Plugin.NAME, "Craft", KeyCode.LeftShift, KeyCode.JoystickButton2);

        // Conflicts
        Inputs.Craft.HideConflict = (keyBind) => (keyBind == PlayerKeybind.Grab || keyBind == Inputs.Scavenge);
        Inputs.Scavenge.HideConflict = (keyBind) => (keyBind == PlayerKeybind.Grab || keyBind == Inputs.Craft);
    }
}