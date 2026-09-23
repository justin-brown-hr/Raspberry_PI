using UnityEngine;

namespace Logic
{
	public class BossClearZone : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Projectile")
			{
				if (collision.gameObject.GetComponent<GroundedDetail>() == null)
				{
					collision.gameObject.AddComponent<GroundedDetail>().Init(1, catapultShieldCalled: true);
				}
			}
			else if (collision.gameObject.tag == "CatapultParticle" || collision.gameObject.tag == "ProjectileParticle" || collision.gameObject.tag == "ExploderFragment")
			{
				collision.gameObject.AddComponent<GroundedDetail>().Init(2, catapultShieldCalled: true);
			}
		}
	}
}
