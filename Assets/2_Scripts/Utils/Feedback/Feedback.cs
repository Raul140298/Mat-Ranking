public static class Feedback
{
	public static void Do(eFeedbackType type)
	{
		switch (type)
		{
			// BACKGROUND MUSICS
			case eFeedbackType.Menu:
			case eFeedbackType.Adventure:
			case eFeedbackType.Level0:
			case eFeedbackType.Level1:
			case eFeedbackType.Level2:
			case eFeedbackType.Level3:
				SFXPlayer.PlayBGM(type);
				break;
			
			// SFXs
			case eFeedbackType.Select:
			case eFeedbackType.ChangeSelect:
			case eFeedbackType.LevelStart:
				SFXPlayer.PlaySFX(type);
				break;
		}
	}
}