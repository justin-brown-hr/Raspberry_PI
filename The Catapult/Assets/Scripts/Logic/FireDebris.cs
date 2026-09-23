using System;
using UnityEngine;

namespace Logic
{
	internal class FireDebris : MonoBehaviour
	{
		public GameObject fireOnParticle;

		public GameObject fireOnCatapult;

		public GameObject fireOnStickman;

		public void Init(Vector3 velocity)
		{
			GetComponent<Rigidbody2D>().velocity = velocity + new Vector3(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(0f, 30f));
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			try
			{
				if (collision.gameObject.tag == "Catapult")
				{
					collision.gameObject.GetComponent<CatapultComponent>().ComponentHit(0.5f, fireOnCatapult);
				}
				else if (collision.gameObject.tag == "Player")
				{
					collision.gameObject.GetComponent<CharacterPart>()._character.SetStickmanOnFire(fireOnStickman);
				}
				else if (collision.gameObject.tag == "CatapultParticle")
				{
					if (collision.relativeVelocity.magnitude > 4f)
					{
						UnityEngine.Object.Instantiate(fireOnParticle, collision.transform.position, Quaternion.identity, collision.transform);
					}
				}
				else if (collision.gameObject.tag == "ProjectileParticle" && collision.gameObject.GetComponent<FireDebris>() != null)
				{
					Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
	}
}
