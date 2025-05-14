using RWCustom;
using UnityEngine;

using System.Runtime.CompilerServices;

using SlugCrafting.Scavenges;
using SlugCrafting.Animations;

namespace SlugCrafting;

public class PlayerCraftingData
{
    public int scavengeTimer = 0;
    public int craftTimer = 0;

    public ScavengeSpot? currentScavengeSpot = null;
    public AbstractPhysicalObjectScavenge? currentScavenge = null;

	public HandAnimation? currentHandAnimation = null;

    public WeakReference<Player> playerRef;

	public PlayerCraftingData(Player player)
	{
		playerRef = new WeakReference<Player>(player);
	}
}

public static class PlayerExtension
{
	private static readonly ConditionalWeakTable<Player, PlayerCraftingData> conditionalWeakTable = new();

	public static PlayerCraftingData GetCraftingData(this Player player) => conditionalWeakTable.GetValue(player, _ => new PlayerCraftingData(player));
}