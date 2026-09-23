using UnityEngine;

namespace Logic
{
	public class DestroyedCatapultParticle : MonoBehaviour
	{
		public int index;

		private bool _inactiveCollider;

		private bool fromCriticalPart;

		private bool isActive;

		private float activeTime;

		private float chanceToAvoidCollision = 0.5f;

		private bool biggerChance;

		private void Awake()
		{
			isActive = false;
			biggerChance = false;
		}

		public void ChangeSprite(int type, int level, bool isPlayer = false)
		{
			Sprite sprite = null;
			sprite = (isPlayer ? InitController.instance.AskPlayerDestroyerParticle(type, index, level) : InitController.instance.AskDestroyerParticle(type, index, level));
			if (sprite != null)
			{
				GetComponent<SpriteRenderer>().sprite = sprite;
			}
		}

		public void SetLayerDelayed(bool biggerChance)
		{
			activeTime = 1f;
			isActive = true;
			this.biggerChance = biggerChance;
			Invoke("SetLayer", 0.25f);
		}

		public void DeactivatePart()
		{
			ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			isActive = false;
		}

		private void SetLayer()
		{
			float num = (!biggerChance) ? 0.75f : 0.9f;
			if (UnityEngine.Random.Range(0f, 1f) < num)
			{
				base.gameObject.layer = 14;
			}
		}

		private void Update()
		{
			if (isActive)
			{
				if (activeTime > 0f)
				{
					activeTime -= Time.deltaTime;
				}
				else if (!GetComponent<Renderer>().isVisible)
				{
					base.gameObject.SetActive(value: false);
				}
			}
		}
	}
}
