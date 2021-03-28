using System;
using System.Collections.Generic;
using System.ComponentModel;
using MEC;

namespace SerpentsHand
{
	public class Config 
	{
		public Config()
        {
			Timing.CallDelayed(2f, () => Load());
        }

		[Description("The items Serpents Hand spawn with.")]
		public List<int> SpawnItems { get; set; } = new List<int>
		{
			21,
			26,
			12,
			14,
			10
		};

		[Description("The change for Serpents Hand to spawn instead of Chaos.")]
		public int SpawnChance { get; set; } = 50;

		[Description("The amount of health Serpents Hand has.")]
		public int Health { get; set; } = 120;

		[Description("The maximum size of a Serpents Hand squad.")]
		public int MaxSquad { get; set; } = 8;

		[Description("How many respawn waves must occur before considering Serpents Hand to spawn.")]
		public int RespawnDelay { get; set; } = 1;

		[Description("The message announced by CASSIE when Serpents hand spawn.")]
		public string EntryAnnouncement { get; set; } = "";

		[Description("The message announced by CASSIE when Chaos spawn.")]
		public string CiEntryAnnouncement { get; set; } = "";

		[Description("The broadcast sent to Serpents Hand when they spawn.")]
		public string SpawnBroadcast { get; set; } = "<size=60>Вы <color=#03F555><b>Длань Змея</b></color></size>\n<i>Помогай <color=\"red\">SCPs</color> убивая других классов!</i>";

		[Description("Determines if friendly fire between Serpents Hand and SCPs is enabled.")]
		public bool FriendlyFire { get; set; } = false;

		[Description("Determines if Serpents Hand should teleport to SCP-106 after exiting his pocket dimension.")]
		public bool TeleportTo106 { get; set; } = false;

		
		[Description("Determines if Serpents Hand should be able to hurt SCPs after the round ends.")]
		public bool EndRoundFriendlyFire { get; set; } = false;

		[Description("[IMPORTANT] Set this config to false if Chaos and SCPs CANNOT win together on your server.")]
		public bool ScpsWinWithChaos { get; set; } = true;

		[Description("Displayed Custom Player Info")]
		public string DisplayedCustomPlayerInfo { get; set; } = "Serpent's Hand";
		private static Config GetOriginal() => new Config();
		public void Load()
        {
			SpawnItems = SerpentsHand.Config.GetIntList("sh_spawnitems");
			if (SpawnItems.Count == 0) SpawnItems=GetOriginal().SpawnItems;
			SpawnChance = SerpentsHand.Config.GetInt("sh_spawnchance", SpawnChance);
			Health= SerpentsHand.Config.GetInt("sh_health", Health);
			MaxSquad= SerpentsHand.Config.GetInt("sh_maxsquad", MaxSquad);
			RespawnDelay= SerpentsHand.Config.GetInt("sh_respawndelay", RespawnDelay);
			EntryAnnouncement = SerpentsHand.Config.GetString("sh_entryannouncement", EntryAnnouncement);
			CiEntryAnnouncement = SerpentsHand.Config.GetString("sh_cientryannouncement", CiEntryAnnouncement);
			SpawnBroadcast = SerpentsHand.Config.GetString("sh_spawnbroadcast", SpawnBroadcast);
			FriendlyFire = SerpentsHand.Config.GetBool("sh_friendlyfire", FriendlyFire);
			TeleportTo106 = SerpentsHand.Config.GetBool("sh_teleportto106", TeleportTo106);
			EndRoundFriendlyFire = SerpentsHand.Config.GetBool("sh_endroundfriendlyfire", EndRoundFriendlyFire);
			ScpsWinWithChaos = SerpentsHand.Config.GetBool("sh_scpswinwithchaos", ScpsWinWithChaos);
			DisplayedCustomPlayerInfo = SerpentsHand.Config.GetString("sh_displayedcustomplayerinfo", DisplayedCustomPlayerInfo);
		}
	}
}
