using System.Collections.Generic;
using UnityEngine;

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
    
    public static Color[] DEFAULT_BTN_COLORS = new Color[4] {
        new Color(0.91f, 0.36f, 0.31f),
        new Color(0.67f, 0.86f, 0.46f),
        new Color(0.27f, 0.78f, 0.99f),
        new Color(1.00f, 0.88f, 0.45f) };
    
    public static Color[] DEFAULT_ZONE_COLORS = new Color[4] {
        new Color(0.56f, 0.42f, 0.19f),
        new Color(0.30f, 0.62f, 0.45f),
        new Color(0.29f, 0.40f, 0.60f),
        new Color(0.41f, 0.26f, 0.52f) };
}