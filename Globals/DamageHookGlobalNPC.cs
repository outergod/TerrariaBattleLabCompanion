using System.Text.Json.Nodes;
using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class DamageHookGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public int? PreHitLife { get; private set; }

    public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
    {
        PreHitLife = npc.life;
    }

    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        PreHitLife = npc.life;
    }

    public int? ComputeOverkill(int damageDone)
    {
        if (PreHitLife is not int pre) return null;
        var excess = damageDone - pre;
        return excess > 0 ? excess : (int?)null;
    }

    public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        var actorId = EntityRegistry.Resolve(player);
        var targetId = EntityRegistry.Resolve(npc);
        if (actorId is null || targetId is null) return;

        var overkill = ComputeOverkill(damageDone);
        var castId = player.GetModPlayer<Mods.BattleLabPlayer>().CurrentCastId;

        Tracking.Emit(EventType.Damage, new DamageData
        {
            Actor = actorId,
            Target = targetId,
            Amount = damageDone,
            Crit = hit.Crit,
            Kind = DamageKind.Hit,
            Overkill = overkill,
            CastId = castId,
            Position = new Position
            {
                X = player.position.X,
                Y = player.position.Y,
                Facing = (sbyte)player.direction,
            },
            X = new JsonObject
            {
                ["terraria"] = new JsonObject
                {
                    ["itemType"] = item.type,
                    ["itemName"] = Lang.GetItemNameValue(item.type),
                    ["damageClass"] = item.DamageType.DisplayName.Value,
                    ["hitDirection"] = hit.HitDirection,
                    ["knockback"] = SafeNumber.Json(hit.Knockback),
                    ["instantKill"] = hit.InstantKill,
                },
            },
        });
    }
}
