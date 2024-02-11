using System;
using System.Collections.Generic;
using System.Linq;

using Comfort.Common;

using EFT;

namespace LootingBots.Patch.Util
{
    // Cached used to keep track of what lootable are currently being targeted by a bot so that multiple bots
    // dont try and path to the same lootable
    public static class ActiveLootCache
    {
        // Id of container/corpse that the player is currently looting
        public static string PlayerLootId;

        // Handle to the players intance for use in friendly checks
        public static Player MainPlayer;

        public static Dictionary<string, BotOwner> ActiveLoot = new Dictionary<string, BotOwner>();

        public static void Init() {
            if (!MainPlayer) {
                MainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
            }
        }

        public static void Reset()
        {
            ActiveLoot = new Dictionary<string, BotOwner>();
            PlayerLootId = "";
            MainPlayer = null;
        }

        public static void CacheActiveLootId(string containerId, BotOwner botOwner)
        {
            ActiveLoot.Add(containerId, botOwner);
        }

        public static bool IsLootInUse(string lootId, BotOwner botOwner)
        {
            bool isFriendly = !botOwner.BotsGroup.IsPlayerEnemy(MainPlayer);
            return (isFriendly && lootId == PlayerLootId) || ActiveLoot.TryGetValue(lootId, out BotOwner _);
        }

        public static void Cleanup(BotOwner botOwner)
        {
            try
            {
                // Look through the entries in the disctionary and remove any that match the specified bot owner
                foreach (
                    var item in ActiveLoot
                        .Where(keyValue => keyValue.Value.name == botOwner.name)
                        .ToList()
                )
                {
                    ActiveLoot.Remove(item.Key);
                }
            }
            catch (Exception e)
            {
                LootingBots.LootLog.LogError(e);
            }
        }
    }
}
