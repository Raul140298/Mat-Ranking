using System;

// -------------------------------------------------
// GOOGLE PLAY 
// -------------------------------------------------

public enum eAchievements
{
    Complete1Challenge = 0, //CgkIlve8wrUJEAIQAg
    Complete10Challenges = 1,
    Complete100Challenges = 2,
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

public enum eAnimation
{
    None,
    Idle,
    Walk,
    Attack,
    Death,
    Hit,
}

[Serializable]
public enum eFeedbackType
{
    Menu,
    Adventure,
    Level0,
    Level1,
    Level2,
    Level3,
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

public enum eScreen
{
    Splash,
    MainMenu,
    Adventure,
    Level,
    Loading
}