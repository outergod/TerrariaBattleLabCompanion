namespace BattleLabCompanion.Wire;

public enum EntityKind
{
    Player,
    Npc,
    Proj,
    Minion,
    Sentry,
    Pet,
    Env
}

public enum DamageKind
{
    Hit,
    Tick,
    Splash,
    Reflect,
    Environmental
}

public enum HealKind
{
    Potion,
    Regen,
    Lifesteal,
    Ability,
    SetBonus,
    BuffTick,
    Natural
}

public enum StatusKind
{
    Buff,
    Debuff,
    Neutral
}

public enum StatusRemoveReason
{
    Expired,
    Removed,
    Dispelled
}

public enum MitigationKind
{
    Block,
    Dodge,
    Parry,
    Resist,
    Absorb
}

public enum ViaKind
{
    Projectile,
    ArmorSet,
    Accessory,
    Trap,
    Other
}

public enum EncounterKind
{
    Boss,
    Event,
    Invasion
}

public enum EncounterOutcome
{
    Victory,
    Defeat,
    Abandoned
}

public enum EncounterEndCause
{
    AllMembersDead,
    AllPlayersDead,
    Despawned,
    WorldExit,
    Manual
}

public enum CastEndReason
{
    Released,
    Interrupted,
    OutOfMana,
    OutOfAmmo,
    Completed,
    Death
}

public enum ChatChannel
{
    Say,
    Team,
    Whisper,
    Broadcast
}

public enum Severity
{
    Info,
    Warn,
    Error
}

public enum SessionEndReason
{
    WorldExit,
    Crash,
    Manual,
    ProcessExit
}
