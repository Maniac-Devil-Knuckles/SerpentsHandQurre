using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qurre;
using Qurre.API;
using HarmonyLib;
using Player = Qurre.Events.Player;
using Server = Qurre.Events.Server;
using Qurre.Events;
namespace SerpentsHand
{

	public class SerpentsHand : Plugin
	{

		public Config cfg { get; set; } = null;
		
		public override void Enable()
		{
			cfg = new Config();
			hInstance = new Harmony("cyanox.serpentshand");
			hInstance.PatchAll();
			instance = this;
			EventHandlers = new EventHandlers();
			//Check035();
			Qurre.Events.Round.Start += EventHandlers.OnRoundStart;
			Qurre.Events.Round.TeamRespawn += EventHandlers.OnTeamRespawn;
			SCPs.SCP106.PocketDimensionEnter += EventHandlers.OnPocketDimensionEnter;
			SCPs.SCP106.PocketDimensionFailEscape += EventHandlers.OnPocketDimensionDie;
			SCPs.SCP106.PocketDimensionEscape += EventHandlers.OnPocketDimensionExit;
			Player.Damage += EventHandlers.OnPlayerHurt;
			Qurre.Events.Round.Check += EventHandlers.OnCheckRoundEnd;
			Player.RoleChange += EventHandlers.OnSetRole;
			Player.Leave += EventHandlers.OnDisconnect;
			SCPs.SCP106.Contain += EventHandlers.OnContain106;
			Server.SendingRA += EventHandlers.OnRACommand;
			Player.InteractGenerator += EventHandlers.OnGeneratorInsert;
			SCPs.SCP106.FemurBreakerEnter += EventHandlers.OnFemurEnter;
			Player.Dies += EventHandlers.OnPlayerDeath;
			Player.Shooting += EventHandlers.OnShoot;
			Player.Spawn += EventHandlers.OnSpawn;
		}

		public override void Disable()
		{
			Qurre.Events.Round.Start -= EventHandlers.OnRoundStart;
			Qurre.Events.Round.TeamRespawn -= EventHandlers.OnTeamRespawn;
			SCPs.SCP106.PocketDimensionEnter -= EventHandlers.OnPocketDimensionEnter;
			SCPs.SCP106.PocketDimensionFailEscape -= EventHandlers.OnPocketDimensionDie;
			SCPs.SCP106.PocketDimensionEscape -= EventHandlers.OnPocketDimensionExit;
			Player.Damage -= EventHandlers.OnPlayerHurt;
			Qurre.Events.Round.Check -= EventHandlers.OnCheckRoundEnd;
			Player.RoleChange -= EventHandlers.OnSetRole;
			Player.Leave -= EventHandlers.OnDisconnect;
			SCPs.SCP106.Contain -= EventHandlers.OnContain106;
			Server.SendingRA -= EventHandlers.OnRACommand;
			Player.InteractGenerator -= EventHandlers.OnGeneratorInsert;
			SCPs.SCP106.FemurBreakerEnter -= EventHandlers.OnFemurEnter;
			Player.Dies -= EventHandlers.OnPlayerDeath;
			Player.Shooting -= EventHandlers.OnShoot;
			Player.Spawn -= EventHandlers.OnSpawn;
		}

		public override string Name
		{
			get
			{
				return "SerpentsHand";
			}
		}

        public override string Developer
		{
			get
			{
				return "Cyanox,Exported to Qurre by Maniac Devil Knuckles";
			}
		}

		

		public EventHandlers EventHandlers;

		public static SerpentsHand instance;

		private Harmony hInstance;
		internal List<int> shPocketPlayers = new List<int>();
    }
}
