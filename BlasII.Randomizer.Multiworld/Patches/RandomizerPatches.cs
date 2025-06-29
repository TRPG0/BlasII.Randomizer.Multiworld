﻿using BlasII.ModdingAPI;
using BlasII.Randomizer.Handlers;
using BlasII.Randomizer.Models;
using BlasII.Randomizer.Multiworld.Models;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BlasII.Randomizer.Multiworld.Patches;

/// <summary>
/// Finds items from an internal storage instead of the Randomizer's
/// </summary>
[HarmonyPatch(typeof(ItemHandler), nameof(ItemHandler.GetItemAtLocation))]
class ItemHandler_GetItemAtLocation_Patch
{
    public static bool Prefix(string locationId, ref Item __result)
    {
        __result = new MultiworldItem(false, "Player name");
        return false;
    }
}

/// <summary>
/// Overrides the Randomizer's shuffle with a fake one
/// </summary>
[HarmonyPatch(typeof(ItemHandler), nameof(ItemHandler.ShuffleItems))]
class ItemHandler_ShuffleItems_Pstch
{
    public static bool Prefix(ItemHandler __instance)
    {
        ModLog.Info("Overriding ItemHandler shuffle");

        var mapping = new Dictionary<string, string>();
        foreach (var location in Main.Randomizer.ItemLocationStorage.AsSequence)
        {
            mapping.Add(location.Id, location.Id);
        }

        __instance.MappedItems = mapping;
        return false;
    }
}

/// <summary>
/// Get validity for mw items
/// </summary>
[HarmonyPatch(typeof(ItemExtensions), nameof(ItemExtensions.IsValid))]
class ItemExtensions_IsValid_Patch
{
    public static bool Prefix(Item item, ref bool __result)
    {
        if (item is not MultiworldItem)
            return true;

        __result = true;
        return false;
    }
}

/// <summary>
/// Get sprite for mw items
/// </summary>
[HarmonyPatch(typeof(ItemExtensions), nameof(ItemExtensions.GetSprite))]
class ItemExtensions_GetSprite_Patch
{
    public static bool Prefix(Item item, ref Sprite __result)
    {
        if (item is not MultiworldItem mwitem)
            return true;

        __result = Main.Multiworld.IconStorage.ItemSprite;
        return false;
    }
}

/// <summary>
/// Get name for mw items
/// </summary>
[HarmonyPatch(typeof(ItemExtensions), nameof(ItemExtensions.GetName))]
class ItemExtensions_GetName_Patch
{
    public static bool Prefix(Item item, ref string __result)
    {
        if (item is not MultiworldItem mwitem)
            return true;

        __result = $"{mwitem.Name} <color=#F8E4C6>{Main.Multiworld.LocalizationHandler.Localize("item/for")}</color> {mwitem.Player}";
        return false;
    }
}

/// <summary>
/// Get description for mw items
/// </summary>
[HarmonyPatch(typeof(ItemExtensions), nameof(ItemExtensions.GetDescription))]
class ItemExtensions_GetDescription_Patch
{
    public static bool Prefix(Item item, ref string __result)
    {
        if (item is not MultiworldItem mwitem)
            return true;

        string key = $"item/desc/{(mwitem.Progression ? "progression" : "filler")}";
        __result = Main.Multiworld.LocalizationHandler.Localize(key);
        return false;
    }
}

/// <summary>
/// Skip reward for mw items
/// </summary>
[HarmonyPatch(typeof(ItemExtensions), nameof(ItemExtensions.GiveReward))]
class ItemExtensions_GiveReward_Patch
{
    public static bool Prefix(Item item)
    {
        return item is not MultiworldItem;
    }
}
