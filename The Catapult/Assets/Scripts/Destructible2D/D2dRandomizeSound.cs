using UnityEngine;

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("Destructible 2D/D2D Randomize Sound")]
	public class D2dRandomizeSound : MonoBehaviour
	{
		[Tooltip("The minimum pitch of this sound")]
		public float PitchMin = 0.9f;

		[Tooltip("The maximum pitch of this sound")]
		public float PitchMax = 1.1f;

		[Tooltip("The audio clips that can be given to this sound")]
		public AudioClip[] Clips;

		protected virtual void Awake()
		{
			AudioSource component = GetComponent<AudioSource>();
			component.pitch = UnityEngine.Random.Range(PitchMin, PitchMax);
			if (Clips != null && Clips.Length > 0)
			{
				component.clip = Clips[Random.Range(0, Clips.Length)];
			}
			component.Play();
		}
	}
}
