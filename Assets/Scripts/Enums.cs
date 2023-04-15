//using System;

// -------------------------------------------------
// GOOGLE PLAY 
// -------------------------------------------------

public enum eAchievements
{
    Complete1Challenge = 0, //CgkIlve8wrUJEAIQAg
    Complete10Challenge = 1,
    Complete100Challenge = 2,
    ApprenticeArithmetic = 3,
    ApprenticeAlgebra = 4,
    ApprenticeGeometry = 5,
    ApprenticeStatistics = 6,
    MathFree = 7,
}

// -------------------------------------------------
// GAME
// -------------------------------------------------


public enum eGameStates
{
    MainMenu,
    Adventure,
    Level,
    OnConversation,
    Pause,
}

public enum ePoolableObjectType
{
    Bullet,
}

// -------------------------------------------------
// SOUND
// -------------------------------------------------


public enum eSounds
{
    Select,
    ChangeSelect,
    LevelStart,
    Exclamation,
    Lock,
    PopPositive,
    PopNegative,
    KeyUnlocking,
    LosingHeart,
    WinHeart,
    WinPoints,
    Hit,
    Laser
}

public enum eSoundtracks
{
    GardenOfMath,
    Adventure,
    Level0,
    Level1,
    Level2,
    Level3
}

public enum eBattleSoundtracks
{
    BattleLayerLevelOne,
    BattleLayer
}