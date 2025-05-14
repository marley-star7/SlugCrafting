using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImprovedInput;
using UnityEngine;

namespace SlugCrafting.Inputs;

public static class Input
{
    public static PlayerKeybind? Scavenge = null;
    public static PlayerKeybind? Craft = null;

    public static void OnPluginEnable()
    {
        var inputScavengeId = Plugin.ID + ":scavenge";
        var inputCraftId = Plugin.ID + ":craft";

        if (Scavenge == null)
            Scavenge = PlayerKeybind.Get(inputScavengeId);

        if (Scavenge == null)
            Scavenge = PlayerKeybind.Register(inputScavengeId, Plugin.NAME, "Scavenge", KeyCode.LeftShift, KeyCode.JoystickButton2);

        if (Craft == null)
            Craft = PlayerKeybind.Get(inputCraftId);

        if (Craft == null)
            Craft = PlayerKeybind.Register(inputCraftId, Plugin.NAME, "Craft", KeyCode.LeftShift, KeyCode.JoystickButton2);

        // Conflicts
        Craft.HideConflict = (keyBind) => (keyBind == PlayerKeybind.Grab || keyBind == Scavenge);
        Scavenge.HideConflict = (keyBind) => (keyBind == PlayerKeybind.Grab || keyBind == Craft);
    }
}