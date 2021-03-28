using System;
using System.Collections.Generic;
using System.Linq;
using Qurre.API;

namespace SerpentsHand.API
{
	// Token: 0x02000006 RID: 6
	public static class SerpentsHand
	{
		// Token: 0x06000042 RID: 66 RVA: 0x000033BB File Offset: 0x000015BB
		public static void SpawnPlayer(Player player, bool full = true)
		{
			EventHandlers.SpawnPlayer(player, full);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000033C4 File Offset: 0x000015C4
		public static void SpawnSquad(List<Player> playerList)
		{
			EventHandlers.SpawnSquad(playerList);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000033CC File Offset: 0x000015CC
		public static void SpawnSquad(int size)
		{
			EventHandlers.CreateSquad(size);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000033D4 File Offset: 0x000015D4
		public static List<Player> GetSHPlayers()
		{
			return (from x in EventHandlers.shPlayers
			select Player.Get(x)).ToList<Player>();
		}
	}
}
