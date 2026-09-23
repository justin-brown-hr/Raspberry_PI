using UnityEngine;

namespace Logic
{
	public class BossCharacterPart : MonoBehaviour
	{
		public BossCharacter _character;

		public bool _canBePoisoned;

		private float _barrelHealth;

		public void InitPart(float health)
		{
			if (!_canBePoisoned)
			{
				_barrelHealth = health;
			}
		}

		public void PoisonCharacter(GameObject acidParticle)
		{
			_character.PoisonCharacter();
			Object.Instantiate(acidParticle, _character.characterAnimator.transform.position, Quaternion.identity);
		}

		public void ComponentHit(float hitForce)
		{
			if (!base.gameObject.GetComponent<SpriteRenderer>().isVisible)
			{
				return;
			}
			if (_canBePoisoned)
			{
				_character.CharacterHit(hitForce);
				return;
			}
			_barrelHealth -= hitForce;
			if (_barrelHealth <= 0f)
			{
				_character.CannonDestroyed();
			}
		}
	}
}
