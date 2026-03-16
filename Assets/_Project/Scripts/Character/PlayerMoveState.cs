namespace GreedIsland.Character
{
    public enum PlayerMoveState
    {
        Idle = 0,
        Move = 1,
        Sprint = 2,
        JumpStart = 3,
        InAir = 4,
        Land = 5,
        Dash = 6,
        AbilityLocked = 7,
        Stunned = 8
    }
}
