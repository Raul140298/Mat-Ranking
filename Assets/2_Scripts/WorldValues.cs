using System.Collections.Generic;

public static class WorldValues
{
    public static int CELL_SIZE = 30;
    
    
    // GOOGLE PLAY
    public static string GOOGLE_LEADERBOARD_ID = "CgkIlve8wrUJEAIQAQ";
    
    public static Dictionary<eAchievements, string> GOOGLE_ACHIEVEMENTS = new Dictionary<eAchievements, string>()
    {
        { eAchievements.Complete1Challenge, "CgkIlve8wrUJEAIQAg" },
        { eAchievements.Complete10Challenges, "CgkIlve8wrUJEAIQAw" },
        { eAchievements.Complete100Challenges, "CgkIlve8wrUJEAIQBA" },
        { eAchievements.ApprenticeArithmetic, "CgkIlve8wrUJEAIQBQ" },
        { eAchievements.ApprenticeAlgebra, "CgkIlve8wrUJEAIQBg" },
        { eAchievements.ApprenticeGeometry, "CgkIlve8wrUJEAIQBw" },
        { eAchievements.ApprenticeStatistics, "CgkIlve8wrUJEAIQCA" },
        { eAchievements.MathFree, "CgkIlve8wrUJEAIQCQ" }
    };
}