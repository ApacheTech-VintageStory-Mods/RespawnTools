#nullable enable
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet.Patches;

public static class Helpers
{
    public static bool TryGetItemWithBehaviour<T>(IPlayer player, out ItemSlot? slot) where T : CollectibleBehavior
    {
        slot = null;

        if (player.Entity.RightHandItemSlot.Itemstack?.Item?.GetBehavior<T>() is not null)
        {
            slot = player.Entity.RightHandItemSlot;
        }
        if (slot is not null) return true;

        if (player.Entity.LeftHandItemSlot.Itemstack?.Item?.GetBehavior<T>() is not null)
        {
            slot = player.Entity.LeftHandItemSlot;
        }
        if (slot is not null) return true;

        foreach (var backpack in player.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName))
        {
            if (backpack.Itemstack?.Item?.GetBehavior<T>() is not null)
            {
                slot = backpack;
            }
            if (slot is not null) return true;
        }
        return slot is not null;
    }

    public static bool TryGetItemInUsableSlot<T>(IPlayer player, out ItemSlot? slot, out T? item) where T : Item
    {
        slot = null;
        item = null;

        if (player.Entity.RightHandItemSlot.Itemstack?.Item is T rhItem)
        {
            slot = player.Entity.RightHandItemSlot;
            item = rhItem;
        }
        if (slot is not null) return true;

        if (player.Entity.LeftHandItemSlot.Itemstack?.Item is T lhItem)
        {
            slot = player.Entity.LeftHandItemSlot;
            item = lhItem;
        }
        if (slot is not null) return true;

        foreach (var backpack in player.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName))
        {
            if (backpack.Itemstack?.Item is T bpItem)
            {
                slot = backpack;
                item = bpItem;
            }
            if (slot is not null) return true;
        }
        return slot is not null;
    }
}