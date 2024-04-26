public enum Role
{
    Server,
    Client
}

public enum PlayerState
{
    ReadyGame,
    Normal,
    Stun,
    Death,
    EndGame
}

public enum HavingWeaponState
{
    HaveLongRangeWeapon,
    HaveMeleeWeapon,
    NotHave
}

public enum UsingWeaponState
{
    ReadyUsing,
    Using,
    NotUsing
}

public enum EffectUI
{
    Freeze,
    Boom
}

public enum ArmorRingState
{
    UnderGround,
    OnGround,
    WaitPlayerStayPlace,
    CreateArmor,
}

public enum DataType
{
    Skin,
    Expression
}

public enum StartGameButtonState
{
    Play,
    UnPlay,
    Ready,
    UnReady,
    None
}