using UnityEngine;

namespace Logic
{
	public class Shield : MonoBehaviour
	{
		internal PvPCatapultLogic _protectedCatapult;

		private float liveTime;

		private float liveTick;

		public void Init(PvPCatapultLogic catapult)
		{
			liveTick = 0f;
			liveTime = 3f;
			_protectedCatapult = catapult;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (!collision.gameObject.GetComponent<Collider2D>().isTrigger)
			{
				if (collision.gameObject.tag != "Catapult" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Tower" && collision.gameObject.tag != "BirdBonus")
				{
					Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
				}
				else if (collision.gameObject.tag == "Projectile" && collision.gameObject.GetComponent<Projectile>().launchFrom == _protectedCatapult.playerSide)
				{
					Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
				}
			}
		}

		private void Update()
		{
			if (liveTick <= liveTime)
			{
				liveTick += Time.deltaTime;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
