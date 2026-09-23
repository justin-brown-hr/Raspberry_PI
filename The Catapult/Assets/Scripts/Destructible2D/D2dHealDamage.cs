using UnityEngine;

namespace Destructible2D
{
	[RequireComponent(typeof(D2dDestructible))]
	[AddComponentMenu("Destructible 2D/D2D Heal Damage")]
	public class D2dHealDamage : MonoBehaviour
	{
		[Tooltip("How many seconds it takes for the Destructible to get healed another step")]
		public float DelayPerHeal = 0.1f;

		[Tooltip("How much alpha gets healed per second (Alpha is 0-255)")]
		public int HealAmount = 10;

		private D2dSnapshot snapshot;

		private D2dDestructible destructible;

		[SerializeField]
		private float cooldown;

		protected virtual void Awake()
		{
			if (destructible == null)
			{
				destructible = GetComponent<D2dDestructible>();
			}
			snapshot = destructible.GetSnapshot();
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
			if (!(cooldown <= 0f))
			{
				return;
			}
			cooldown = DelayPerHeal;
			if (destructible == null)
			{
				destructible = GetComponent<D2dDestructible>();
			}
			if (snapshot.AlphaWidth != destructible.AlphaWidth || snapshot.AlphaHeight != destructible.AlphaHeight)
			{
				return;
			}
			destructible.BeginAlphaModifications();
			for (int num = snapshot.AlphaHeight - 1; num >= 0; num--)
			{
				for (int num2 = snapshot.AlphaWidth - 1; num2 >= 0; num2--)
				{
					int num3 = num2 + num * snapshot.AlphaWidth;
					byte b = destructible.AlphaData[num3];
					byte b2 = snapshot.AlphaData[num3];
					if (b != b2)
					{
						b2 = (byte)Mathf.MoveTowards((int)b, (int)b2, HealAmount);
						destructible.WriteAlpha(num2, num, b2);
					}
				}
			}
			destructible.EndAlphaModifications();
		}
	}
}
