using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using BattleLabCompanion.Globals;
using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BattleLabCompanion.Mods;

public sealed class BattleLabPlayer : ModPlayer
{
    private int? _preHitLife;
    private readonly Dictionary<int, int> _prevBuffs = new();

    private static readonly HashSet<int> ChanneledItems = new()
    {
        ItemID.LastPrism,
    };

    public string? CurrentCastId { get; private set; }
    private int _prevItemAnim;
    private bool _castBegan;

    public override void OnEnterWorld()
    {
        var id = EntityRegistry.Resolve(Player);
        if (id is null) return;

        Tracking.Emit(EventType.EntityDeclare, new EntityDeclareData
        {
            Id = id,
            Kind = EntityKind.Player,
            Name = Player.name,
            MaxHp = Player.statLifeMax2,
            Position = new Position
            {
                X = Player.position.X,
                Y = Player.position.Y,
                Facing = (sbyte)Player.direction,
            },
            X = new JsonObject
            {
                ["terraria"] = new JsonObject
                {
                    ["slot"] = Player.whoAmI,
                },
            },
        });
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        _preHitLife = Player.statLife;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        var targetId = EntityRegistry.Resolve(Player);
        if (targetId is null) return;

        var (actorId, via, kind) = ResolveSource(info);
        if (actorId is null) return;

        var preHit = _preHitLife ?? Player.statLifeMax2;
        int? overkill = info.Damage > preHit ? info.Damage - preHit : (int?)null;

        Tracking.Emit(EventType.Damage, new DamageData
        {
            Actor = actorId,
            Target = targetId,
            Amount = info.Damage,
            Crit = false,
            Kind = kind,
            Overkill = overkill,
            CastId = ExtractCastId(via),
            Via = via,
            Position = new Position
            {
                X = Player.position.X,
                Y = Player.position.Y,
                Facing = (sbyte)Player.direction,
            },
            X = new JsonObject
            {
                ["terraria"] = new JsonObject
                {
                    ["hitDirection"] = info.HitDirection,
                    ["knockback"] = SafeNumber.Json(info.Knockback),
                    ["cooldownCounter"] = info.CooldownCounter,
                    ["pvp"] = info.PvP,
                    ["sourceDamage"] = info.SourceDamage,
                },
            },
        });
    }

    private static (string? actorId, Via? via, DamageKind kind) ResolveSource(Player.HurtInfo info)
    {
        if (!info.DamageSource.TryGetCausingEntity(out var entity))
            return (EnvIds.Unknown, null, DamageKind.Environmental);

        return entity switch
        {
            NPC n => (EntityRegistry.Resolve(n), null, DamageKind.Hit),
            Projectile pj => ResolveProjectileSource(pj),
            Player p => (EntityRegistry.Resolve(p), null, DamageKind.Hit),
            _ => (EnvIds.Unknown, null, DamageKind.Environmental),
        };
    }

    public override void PostUpdateBuffs()
    {
        BuffDiff.EmitTransitions(_prevBuffs, Player.buffType, Player.buffTime, () => EntityRegistry.Resolve(Player));
    }

    public override void PostUpdate()
    {
        var anim = Player.itemAnimation;
        var heldItem = Player.HeldItem;
        var itemType = heldItem?.type ?? 0;

        var isStarting = _prevItemAnim == 0 && anim > 0;
        var isEnding = _prevItemAnim > 0 && anim == 0;

        if (isStarting)
        {
            CurrentCastId = $"cast:{Guid.NewGuid():N}";
            if (heldItem is not null && ChanneledItems.Contains(itemType))
            {
                EmitCastBegin(heldItem);
                _castBegan = true;
            }
        }

        if (isEnding)
        {
            if (_castBegan) EmitCastEnd(CastEndReason.Released);
            _castBegan = false;
            CurrentCastId = null;
        }

        _prevItemAnim = anim;
    }

    private void EmitCastBegin(Item item)
    {
        var actor = EntityRegistry.Resolve(Player);
        if (actor is null || CurrentCastId is null) return;

        Tracking.Emit(EventType.CastBegin, new CastBeginData
        {
            Id = CurrentCastId,
            Actor = actor,
            Ability = Lang.GetItemNameValue(item.type),
            Position = new Position { X = Player.position.X, Y = Player.position.Y },
        });
    }

    private void EmitCastEnd(CastEndReason reason)
    {
        if (CurrentCastId is null) return;

        Tracking.Emit(EventType.CastEnd, new CastEndData
        {
            Id = CurrentCastId,
            Reason = reason,
        });
    }

    private static (string? actorId, Via? via, DamageKind kind) ResolveProjectileSource(Projectile projectile)
    {
        var prov = projectile.GetGlobalProjectile<CombatProvenanceGlobalProjectile>();
        var via = new Via
        {
            Kind = ViaKind.Projectile,
            Name = Lang.GetProjectileName(projectile.type).Value,
            Id = prov.EntityLocalId,
            Owner = prov.SourceEntityId,
            Weapon = prov.SourceItemType is int it ? Lang.GetItemNameValue(it) : null,
            Intermediate = prov.ViaTail,
        };
        return (prov.SourceEntityId ?? EnvIds.Unknown, via, DamageKind.Hit);
    }

    private static string? ExtractCastId(Via? via)
    {
        // No castId on player-target side from raw Via; the source projectile's
        // prov.CastId is the source of truth and would need to be plumbed
        // through ResolveProjectileSource. Defer to follow-up.
        _ = via;
        return null;
    }
}
