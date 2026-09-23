using UnityEngine;

namespace Logic
{
	public class GameBounds : MonoBehaviour
	{
		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.tag == "Projectile")
			{
				collision.gameObject.GetComponent<Projectile>().ReturnToPoolDeleayed();
			}
			else if (collision.gameObject.tag == "Player")
			{
				collision.gameObject.GetComponent<CharacterPart>()._character.OutOfBounds();
			}
			else if (collision.gameObject.tag == "BirdBonus")
			{
				UnityEngine.Object.Destroy(collision.gameObject);
			}
			else if (collision.gameObject.tag == "Catapult")
			{
				collision.gameObject.GetComponent<CatapultComponent>().ComponentHit(100000f);
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "BirdBonus")
			{
				UnityEngine.Object.Destroy(collision.gameObject);
			}
			else if (collision.gameObject.tag == "Projectile")
			{
				collision.gameObject.GetComponent<Projectile>().ReturnToPoolDeleayed();
			}
		}
	}
}
