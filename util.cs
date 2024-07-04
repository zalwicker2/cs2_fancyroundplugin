using System;
using System.Collections.Generic;
using System.Drawing;

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

public class Util
{
    public static void RemovePlayerWeapons(CCSPlayerController plr)
    {
        Console.WriteLine("has " + plr.PlayerPawn.Value!.WeaponServices!.ActiveWeapon.Value!.Globalname);
    }

    public static List<CCSPlayerController> GetTeamPlayers(CsTeam team)
    {
        List<CCSPlayerController> plrs = new List<CCSPlayerController>();
        foreach (CCSPlayerController plr in Utilities.GetPlayers())
        {

            if (plr.Team == team)
            {
                plrs.Add(plr);
            }
        }

        return plrs;
    }

    public static void HidePlayer(CCSPlayerPawn playerPawn)
    {
        int transparency = 175;
        playerPawn.RenderMode = RenderMode_t.kRenderTransColor;
        playerPawn.Render = Color.FromArgb(255 - transparency, 255, 255, 255);
        Utilities.SetStateChanged(playerPawn, "CBaseModelEntity", "m_clrRender");

        var activeWeapon = playerPawn!.WeaponServices?.ActiveWeapon.Value;
        if (activeWeapon != null && activeWeapon.IsValid)
        {
            activeWeapon.Render = Color.FromArgb(0, 0, 0, 0);
            activeWeapon.ShadowStrength = 0.0f;
            Utilities.SetStateChanged(activeWeapon, "CBaseModelEntity", "m_clrRender");
        }

        var myWeapons = playerPawn.WeaponServices?.MyWeapons;
        if (myWeapons != null)
        {
            foreach (var gun in myWeapons)
            {
                var weapon = gun.Value;
                if (weapon != null)
                {
                    weapon.Render = Color.FromArgb(0, 0, 0, 0);
                    weapon.ShadowStrength = 0.0f;
                    Utilities.SetStateChanged(weapon, "CBaseModelEntity", "m_clrRender");
                }
            }
        }
    }

    public static void ShowPlayer(CCSPlayerPawn playerPawn)
    {
        playerPawn.RenderMode = RenderMode_t.kRenderTransColor;
        playerPawn.Render = Color.FromArgb(255, 255, 255, 255);
        Utilities.SetStateChanged(playerPawn, "CBaseModelEntity", "m_clrRender");

        var activeWeapon = playerPawn!.WeaponServices?.ActiveWeapon.Value;
        if (activeWeapon != null && activeWeapon.IsValid)
        {
            activeWeapon.Render = Color.FromArgb(255, 255, 255, 255);
            activeWeapon.ShadowStrength = 1.0f;
            Utilities.SetStateChanged(activeWeapon, "CBaseModelEntity", "m_clrRender");
        }

        var myWeapons = playerPawn.WeaponServices?.MyWeapons;
        if (myWeapons != null)
        {
            foreach (var gun in myWeapons)
            {
                var weapon = gun.Value;
                if (weapon != null)
                {
                    weapon.Render = Color.FromArgb(255, 255, 255, 255);
                    weapon.ShadowStrength = 1.0f;
                    Utilities.SetStateChanged(weapon, "CBaseModelEntity", "m_clrRender");
                }
            }
        }
    }

    public static HookResult InfiniteGrenades(EventGrenadeThrown @event, GameEventInfo info)
    {
        Console.WriteLine("thrown " + @event.Weapon);
        @event.Userid!.GiveNamedItem("weapon_" + @event.Weapon);
        return HookResult.Continue;
    }

    static Random rng = new Random();

    public static List<CCSPlayerController> ShuffleList(List<CCSPlayerController> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CCSPlayerController value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    public static void SetHealth(CCSPlayerPawn playerPawn, int health)
    {
        playerPawn.Health = health;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_iHealth");
    }

    public static void SetArmor(CCSPlayerPawn playerPawn, int amount, bool helmet)
    {
        playerPawn.ArmorValue = 250;
        if (helmet)
        {
            CCSPlayer_ItemServices itemServices = new CCSPlayer_ItemServices(playerPawn.ItemServices.Handle);
            itemServices.HasHelmet = true;
            Utilities.SetStateChanged(playerPawn, "CBasePlayerPawn", "m_pItemServices");
        }
        Utilities.SetStateChanged(playerPawn, "CCSPlayerPawnBase", "m_ArmorValue");
    }

    public static void SetInventory(CCSPlayerController plr, string[] items, bool keepKnife = true)
    {
        var playerPawn = plr.PlayerPawn.Value!;
        var weaponServices = playerPawn.WeaponServices!;
        foreach (CHandle<CBasePlayerWeapon> weapon in weaponServices.MyWeapons)
        {
            
            if (weapon.Value != null && weapon.Value.DesignerName != null && (weapon.Value!.DesignerName != "weapon_c4"))
            {
                if (keepKnife && weapon.Value!.DesignerName == "weapon_knife")
                {
                    continue;
                }
                foreach (string filter in items)
                {
                    if (weapon.Value!.DesignerName == filter)
                    {
                        continue;
                    }
                }
                plr.RemoveItemByDesignerName(weapon.Value!.ToString());
                weapon.Value!.Remove();
            }
        }

        foreach (string item in items)
        {
            plr.GiveNamedItem(item);
        }

        var knife = weaponServices.MyWeapons.First();
        if (knife != null)
        {
            weaponServices.ActiveWeapon!.Raw = knife.Raw;
            Utilities.SetStateChanged(plr.PlayerPawn.Value, "CPlayer_WeaponServices", "m_hActiveWeapon");
            Utilities.SetStateChanged(plr.PlayerPawn.Value, "CCSPlayer_ViewModelServices", "m_hViewModel");
        }
    }

    public static void RemoveAllWeaponsWithFilter(CCSPlayerController plr, string[] filters)
    {
        var playerPawn = plr.PlayerPawn.Value!;
        var weaponServices = playerPawn.WeaponServices!;
        foreach (CHandle<CBasePlayerWeapon> weapon in weaponServices.MyWeapons)
        {
            if (weapon.Value != null && weapon.Value.DesignerName != null && (weapon.Value!.DesignerName != "weapon_c4"))
            {
                foreach (string filter in filters)
                {
                    if (weapon.Value!.DesignerName == filter)
                    {
                        continue;
                    }
                }
                plr.RemoveItemByDesignerName(weapon.Value.ToString()!);
                weapon.Value.Remove();
            }
        }
        Server.NextFrame(() =>
        {
            var knife = weaponServices.MyWeapons.First();
            if (knife != null)
            {
                weaponServices.ActiveWeapon!.Raw = knife.Raw;
                Utilities.SetStateChanged(plr.PlayerPawn.Value, "CPlayer_WeaponServices", "m_hActiveWeapon");
                Utilities.SetStateChanged(plr.PlayerPawn.Value, "CCSPlayer_ViewModelServices", "m_hViewModel");
            }
        });
    }

    public static void CleanupPlayer(CCSPlayerController plr)
    {
        var playerPawn = plr.PlayerPawn.Value!;

        if (playerPawn.ArmorValue > 100)
        {
            Util.SetArmor(playerPawn, 100, false);
        }

        if (playerPawn.Health > 100)
        {
            Util.SetHealth(playerPawn, 100);
        }
        Util.ShowPlayer(plr.PlayerPawn.Value!);
    }
}