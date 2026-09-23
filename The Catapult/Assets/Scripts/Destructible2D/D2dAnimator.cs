using System;
using UnityEngine;

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	[AddComponentMenu("Destructible 2D/D2D Animator")]
	public class D2dAnimator : MonoBehaviour
	{
		[Tooltip("If you set this, then the attached D2dDestructible will have ReplaceTextureWith caled with this Sprite")]
		public Sprite ReplaceTextureWithSprite;

		[Tooltip("If you set this, then the attached D2dDestructible will have ReplaceTextureWith caled with this Texture2D")]
		public Texture2D ReplaceTextureWithTexture;

		[NonSerialized]
		private D2dDestructible destructible;

		protected virtual void Update()
		{
			if (ReplaceTextureWithSprite != null)
			{
				if (destructible == null)
				{
					destructible = GetComponent<D2dDestructible>();
				}
				destructible.ReplaceTextureWith(ReplaceTextureWithSprite);
				ReplaceTextureWithSprite = null;
			}
			if (ReplaceTextureWithTexture != null)
			{
				if (destructible == null)
				{
					destructible = GetComponent<D2dDestructible>();
				}
				destructible.ReplaceTextureWith(ReplaceTextureWithTexture);
				ReplaceTextureWithTexture = null;
			}
		}
	}
}
