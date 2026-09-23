using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Repeat Stamp")]
	public class D2dRepeatStamp : MonoBehaviour
	{
		[Tooltip("The layers the stamp works on")]
		public LayerMask Layers = -1;

		[Tooltip("The shape of the stamp")]
		public Texture2D StampTex;

		[Tooltip("The size of the stamp in world space")]
		public Vector2 Size = Vector2.one;

		[Tooltip("How hard the stamp is")]
		public float Hardness = 1f;

		[Tooltip("The delay between each repeat stamp")]
		public float Delay = 0.25f;

		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
			if (cooldown <= 0f)
			{
				cooldown = Delay;
				float angle = UnityEngine.Random.Range(0f, 360f);
				D2dDestructible.StampAll(base.transform.position, Size, angle, StampTex, Hardness, Layers);
			}
		}
	}
}
