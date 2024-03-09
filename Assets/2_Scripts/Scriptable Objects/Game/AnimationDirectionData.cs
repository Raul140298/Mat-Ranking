using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Animation Direction Data", menuName = "Create Animation Direction Data")]
public class AnimationDirectionData : SerializedScriptableObject
{
	[Header("VALUES")]
	public Dictionary<eDirection, AnimationClip> animations;
}
