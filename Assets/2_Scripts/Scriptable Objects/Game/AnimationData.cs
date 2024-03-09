using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Animation Data", menuName = "Create Animation Data")]
public class AnimationData : SerializedScriptableObject
{
	[Header("VALUES")]
	public Dictionary<eAnimation, AnimationDirectionData> animations;
}
