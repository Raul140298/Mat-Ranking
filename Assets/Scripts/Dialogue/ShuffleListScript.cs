using System.Collections.Generic;

public static class ShuffleListScript
{
	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		System.Random rng = new System.Random();

		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
