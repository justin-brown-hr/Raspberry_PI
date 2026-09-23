using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Flipper")]
	public class D2dFlipper : MonoBehaviour
	{
		[Tooltip("Currently flipped?")]
		public bool Flipped;

		[Tooltip("The delay between flipping in seconds")]
		public float FlipDelay = 1f;

		[Tooltip("Called when Flipped = 1")]
		public D2dEvent OnFlip;

		[Tooltip("Called when Flipped = 0")]
		public D2dEvent OnUnflip;

		[SerializeField]
		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
			if (!(cooldown <= 0f))
			{
				return;
			}
			cooldown = FlipDelay;
			if (Flipped)
			{
				Flipped = false;
				if (OnUnflip != null)
				{
					OnUnflip.Invoke();
				}
			}
			else
			{
				Flipped = true;
				if (OnFlip != null)
				{
					OnFlip.Invoke();
				}
			}
		}
	}
}
