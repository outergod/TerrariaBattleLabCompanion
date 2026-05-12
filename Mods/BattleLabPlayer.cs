using System.Collections.Generic;
using System.Text.Json.Nodes;
using BattleLabCompanion.Globals;
using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BattleLabCompanion.Mods;

public sealed class BattleLabPlayer : ModPlayer
{
    private int? _preHitLife;
    private readonly Dictionary<int, int> _prevBuffs = new();

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
                    ["knockback"] = info.Knockback,
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
        };
        return (prov.SourceEntityId ?? EnvIds.Unknown, via, DamageKind.Hit);
    }
}
