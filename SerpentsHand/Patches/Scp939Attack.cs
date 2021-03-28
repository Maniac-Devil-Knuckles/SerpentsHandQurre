using System;
using CustomPlayerEffects;
using Qurre.API;
using HarmonyLib;
using UnityEngine;

namespace SerpentsHand.Patches
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(Scp939PlayerScript), "CallCmdShoot")]
	internal class Scp939Attack
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00003374 File Offset: 0x00001574
		public static void Postfix(Scp939PlayerScript __instance, GameObject target)
		{
			Player player = Player.Get(target);
			if (player.Role == RoleType.Tutorial && !SerpentsHand.instance.cfg.FriendlyFire)
			{
				player.ReferenceHub.playerEffectsController.DisableEffect<Amnesia>();
			}
		}
	}
}
