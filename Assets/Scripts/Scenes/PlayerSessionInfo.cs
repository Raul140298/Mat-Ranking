using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

public static class PlayerSessionInfo
{
	public static  int knowledgePoints{ get; set; }
	public static  Vector3 playerPosition{ get; set; }
	
	public static TimeSpan timePlayed { get; set; }
	
	public static  bool tutorial{ get; set; }
	public static  int adventureWorldLevel{ get; set; }
	public static  int arithmeticChallenges{ get; set; }
	public static  int algebraChallenges{ get; set; }
	public static  int geometryChallenges{ get; set; }
	public static  int statisticsChallenges{ get; set; }
	
	public static  float soundtracksVolume{ get; set; }
	public static  float soundsVolume{ get; set; }
	public static  string language{ get; set; }
	
	// Method to convert session information to a byte array
    public static byte[] Serialize(bool firstTime = false)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, new PlayerSessionInfoSerializable(firstTime));
            return stream.ToArray();
        }
    }

    // Method to deserialize a byte array and update session information
    public static void Deserialize(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(data))
        {
            PlayerSessionInfoSerializable info = (PlayerSessionInfoSerializable)formatter.Deserialize(stream);
            // Update session values with deserialized values
            knowledgePoints = info.knowledgePoints;
            timePlayed = info.timePlayed;
            playerPosition = info.playerPosition;
            adventureWorldLevel = info.adventureWorldLevel;
            arithmeticChallenges = info.arithmeticChallenges;
            algebraChallenges = info.algebraChallenges;
            geometryChallenges = info.geometryChallenges;
            statisticsChallenges = info.statisticsChallenges;
            tutorial = info.tutorial;
            soundtracksVolume = info.soundtracksVolume;
            soundsVolume = info.soundsVolume;
            language = info.language;
        }
    }

    [System.Serializable]
    private class PlayerSessionInfoSerializable
    {
        public int knowledgePoints;
        public TimeSpan timePlayed;
        public Vector3 playerPosition;
        public int adventureWorldLevel;
        public int arithmeticChallenges;
        public int algebraChallenges;
        public int geometryChallenges;
        public int statisticsChallenges;
        public bool tutorial;
        public float soundtracksVolume;
        public float soundsVolume;
        public string language;

        public PlayerSessionInfoSerializable(bool firstTime)
        {
			if (firstTime)
			{
				knowledgePoints = 0;
				timePlayed = TimeSpan.Zero;
				playerPosition = new Vector3(300, -30, 0);
				adventureWorldLevel = 0;
				arithmeticChallenges = 0;
				algebraChallenges = 0;
				geometryChallenges = 0;
				statisticsChallenges = 0;
				tutorial = false;
				soundtracksVolume = 1;
				soundsVolume = 1;
				
				switch (Application.systemLanguage)
				{
					case SystemLanguage.Spanish:
						language = "es";
						break;
					case SystemLanguage.English:
						language = "en";
						break;
					default:
						language = "es";
						break;
				}
			}
			else
			{
				// Copy values from the static class to the serializable class
				knowledgePoints = PlayerSessionInfo.knowledgePoints;
				timePlayed = PlayerSessionInfo.timePlayed;
				playerPosition = PlayerSessionInfo.playerPosition;
				adventureWorldLevel = PlayerSessionInfo.adventureWorldLevel;
				arithmeticChallenges = PlayerSessionInfo.arithmeticChallenges;
				algebraChallenges = PlayerSessionInfo.algebraChallenges;
				geometryChallenges = PlayerSessionInfo.geometryChallenges;
				statisticsChallenges = PlayerSessionInfo.statisticsChallenges;
				tutorial = PlayerSessionInfo.tutorial;
				soundtracksVolume = PlayerSessionInfo.soundtracksVolume;
				soundsVolume = PlayerSessionInfo.soundsVolume;
				language = PlayerSessionInfo.language;
			}
        }
    }
}