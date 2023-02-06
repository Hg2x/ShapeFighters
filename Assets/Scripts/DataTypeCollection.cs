public static class Const
{
    public const int MAX_WEAPON_SLOT = 4;
    public const int SURVIVAL_TIME_GOAL = 30;
    public const int ENEMY_BASE_DAMAGE = 5;
}

public enum DamageSource
{
    None,
    Friendly,
    Enemy,
    Environment
}

public enum WeaponSlot
{
    None = -1,
    Head = 0,
    UpperBody = 1,
    LowerBody = 2,
    Arm = 3
}
