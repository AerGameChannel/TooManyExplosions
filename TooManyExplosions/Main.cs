using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TooMuchExplosions
{
    [BepInPlugin("com.aer.toomanyexplosions", "TooMuchExplosions", "1.0.0")]
    public class TooManyExplosions : BaseUnityPlugin
    {
        public static System.Random Random = new();
        public Harmony Harmony;
        private void Awake()
        {
            Harmony = new Harmony($"com.aer.toomanyexplosions");
            Harmony.PatchAll();
            Logger.LogInfo($"{Info.Metadata.Name} successfully loaded!");
        }
    }

    [HarmonyPatch(typeof(WeaponSpawner), "Start")]
    public class WeaponSpawnerPatch
    {
        public static void Prefix(WeaponSpawner __instance)
        {
            __instance.respawnTime /= 2f;
        }
    }

    [HarmonyPatch(typeof(WaveMode), nameof(WaveMode.GetRandomWeapon))]
    public class WaveModePatch
    {
        public static bool Prefix(WaveMode __instance, ref GameObject __result)
        {
            var randInt = TooManyExplosions.Random.Next(0, 2);
            __result = Traverse.Create(__instance).Field("possibleWeapons").GetValue<List<SpawnItem>>().Where(x => x.item.TryGetComponent(out Grenade _) || x.item.TryGetComponent(out Mine _)).ToArray()[randInt].item;
			return false;
        }
    }
}
