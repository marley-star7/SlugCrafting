using HUD;

namespace SlugCrafting.Menus;

public class ShelterCraftScreen : Menu.Menu, IOwnAHUD
{
    public class ShelterCraftScreenDataPackage
    {
        public MenuInventory playerShelterInventory;

        public SleepAndDeathScreen.SleepDeathScreenDataPackage sleepAndDeathScreenDataPackage;

        public ShelterCraftScreenDataPackage(SleepAndDeathScreen.SleepDeathScreenDataPackage sleepDeathScreenDataPackage)
        {
            sleepAndDeathScreenDataPackage = sleepDeathScreenDataPackage;
        }
    }

    public global::HUD.HUD hud { get; private set; }

    public ShelterCraftScreenDataPackage fromGameDataPackage;

    public ShelterCraftSelector shelterCraftSelector;

    public MenuInventoryDisplay inventoryDisplay;

    public Player.InputPackage MapInput => RWInput.PlayerInput(0);

    public int CurrentFood => 0;

    public Vector2 MapOwnerInRoomPosition => new Vector2(0f, 0f);
    public int MapOwnerRoom => -1;
    public bool RevealMap => false;
    public bool MapDiscoveryActive => false;

    public SimpleButton continueButton;
    public virtual bool ButtonsGreyedOut => false;

    public float ContinueAndExitButtonsXPos => manager.rainWorld.options.ScreenSize.x + (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;

    public ShelterCraftScreen(ProcessManager manager, ProcessManager.ProcessID ID) : base(manager, ID)
    {
        this.ID = Enums.ProcessID.ShelterCraft;

        //-- Ms7: Mmmmmmm the rainnnnnn, mmmmmmm my asmr rain world rain mmmmmmm.
        this.mySoundLoopID = SoundID.MENU_Sleep_Screen_LOOP;
        PlaySound(mySoundLoopID);

        pages.Add(new Page(this, null, "main", 0));
        selectedObject = null;

        AddContinueButton(black: true);

        shelterCraftSelector = new ShelterCraftSelector(this, this.pages[0], new Vector2(400f, manager.rainWorld.options.ScreenSize.y / 2), ShelterCraftSelector.GetDefaultSize); // Same position as karma ladder, ui pos is relative to center of the ui element.
        inventoryDisplay = new MenuInventoryDisplay(this, this.pages[0], new Vector2(800f, manager.rainWorld.options.ScreenSize.y - 32));
    }

    public override void Update()
    {
        base.Update();
        shelterCraftSelector.Update();
        inventoryDisplay.Update();

        if (continueButton != null)
        {
            continueButton.buttonBehav.greyedOut = ButtonsGreyedOut;
            continueButton.black = Mathf.Max(0f, continueButton.black - 0.025f);
        }
        if (hud != null)
        {
            hud.Update();
        }
    }

    public override void GrafUpdate(float timeStacker)
    {
        base.GrafUpdate(timeStacker);
        shelterCraftSelector.GrafUpdate(timeStacker);
        inventoryDisplay.GrafUpdate(timeStacker);
    }

    private void AddContinueButton(bool black)
    {
        continueButton = new SimpleButton(this, pages[0], Translate("CONTINUE"), "CONTINUE", new Vector2(ContinueAndExitButtonsXPos - 180f - manager.rainWorld.options.SafeScreenOffset.x, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
        pages[0].subObjects.Add(continueButton);
        continueButton.black = (black ? 1f : 0f);
        pages[0].lastSelectedObject = continueButton;
    }

    public HUD.HUD.OwnerType GetOwnerType()
    {
        return global::HUD.HUD.OwnerType.SleepScreen;
    }

    public override void Singal(MenuObject sender, string message)
    {
        switch (message)
        {
            case "CONTINUE":
                manager.rainWorld.progression.SaveWorldStateAndProgression(fromGameDataPackage.sleepAndDeathScreenDataPackage.goalMalnourished); // Goal malnourished is if malnourished this cycle.
                manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SleepScreen);
                break;
        }
    }

    public void GetDataFromGame(ShelterCraftScreenDataPackage package)
    {
        Plugin.LogDebug($"Got the data from the game for the Shelter Craft Screen!");

        fromGameDataPackage = package;
        shelterCraftSelector.SetShelterCraftScreenDataPackage(fromGameDataPackage);
        inventoryDisplay.SetInventory(fromGameDataPackage.playerShelterInventory);
    }

    public override void CommunicateWithUpcomingProcess(MainLoopProcess nextProcess)
    {
        // Ms7: Have to communicate to send the data package to sleep screen.

        base.CommunicateWithUpcomingProcess(nextProcess);
        if (nextProcess is SleepAndDeathScreen sleepAndDeathScreen)
        {
            sleepAndDeathScreen.GetDataFromGame(fromGameDataPackage.sleepAndDeathScreenDataPackage);
        }
    }


    public void FoodCountDownDone()
    {

    }

    public void PlayHUDSound(SoundID soundID)
    {
        PlaySound(soundID);
    }
}
