using System;
using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Destroyer")]
	public class D2dDestroyer : MonoBehaviour
	{
		[Tooltip("The amount of seconds it takes for this GameObject to get destroyed if it falls below the MinAlphaCount")]
		public float Life = 3f;

		[Tooltip("Should the destructible attached to this GameObject fade out?")]
		public bool Fade;

		[Tooltip("The amount of seconds it takes for the fade animation to complete")]
		public float FadeDuration = 1f;

		[Tooltip("Should this GameObject shrink to 0?")]
		public bool Shrink;

		[Tooltip("The amount of seconds it takes for the shrink animation to complete")]
		public float ShrinkDuration = 1f;

		[Tooltip("Should these settings get randomized when this component is enabled?")]
		public bool RandomizeOnEnable;

		[Tooltip("The minimum randomized Life value")]
		public float LifeMin = 3f;

		[Tooltip("The minimum randomized Life value")]
		public float LifeMax = 5f;

		[SerializeField]
		private Color startColor;

		[SerializeField]
		private Vector3 startLocalScale;

		[NonSerialized]
		private D2dDestructible destructible;

		protected virtual void OnEnable()
		{
			if (RandomizeOnEnable)
			{
				Life = UnityEngine.Random.Range(LifeMin, LifeMax);
			}
		}

		protected virtual void Update()
		{
			Life -= Time.deltaTime;
			if (Life > 0f)
			{
				if (Fade)
				{
					UpdateFade();
				}
				if (Shrink)
				{
					UpdateShrink();
				}
			}
			else
			{
				D2dHelper.Destroy(base.gameObject);
			}
		}

		private void UpdateFade()
		{
			if (!(FadeDuration > 0f))
			{
				return;
			}
			if (destructible == null)
			{
				destructible = GetComponent<D2dDestructible>();
			}
			if (destructible != null && FadeDuration > 0f && Life < FadeDuration)
			{
				if (startColor == default(Color))
				{
					startColor = destructible.Color;
				}
				Color color = startColor;
				color.a *= Life / FadeDuration;
				destructible.Color = color;
			}
		}

		private void UpdateShrink()
		{
			if (ShrinkDuration > 0f)
			{
				if (startLocalScale == default(Vector3))
				{
					startLocalScale = base.transform.localScale;
				}
				if (startLocalScale != Vector3.zero)
				{
					Vector3 a = startLocalScale;
					a *= Life / FadeDuration;
					base.transform.localScale = a;
				}
			}
		}
	}
}
