using MRCustom.Json;

namespace SlugCrafting.Items;

public static class LizardShellArmorFisob
{
    /// <summary>
    /// Parse the shared data between LizardShellArmors
    /// </summary>
    /// <param name="abstractLizardShellArmor"></param>
    /// <param name="saveData"></param>
    public static void ParseArmorData(AbstractLizardShellArmor abstractLizardShellArmor, EntitySaveData saveData)
    {
        // Data is just floats separated by ; characters.
        string[] parsedData = saveData.CustomData.Split(';');

        if (parsedData.Length < 2)
        {
            parsedData = new string[2];
        }

        if (MRJson.TryParseColor(parsedData[0], out var shellColorParsed))
            abstractLizardShellArmor.shellColor = shellColorParsed;

        if (float.TryParse(parsedData[0], out var healthParsed))
            abstractLizardShellArmor.health = healthParsed;
    }
}