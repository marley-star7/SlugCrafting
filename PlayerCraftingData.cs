using RWCustom;
using UnityEngine;

namespace SlugCrafting;

public class PlayerCraftingData
{
	public int scavengeTimer;

	public WeakReference<Player> playerRef;

	public PlayerCraftingData(Player player)
	{
		playerRef = new WeakReference<Player>(player);
	}
}

public static class PlayerExtension
{
	private static readonly ConditionalWeakTable<Player, VinkiPlayerData> conditionalWeakTable = new();

	public static PlayerCraftingData CraftingData(this Player player) => cwt.GetValue(player, _ => new PlayerCraftingData(player));
}