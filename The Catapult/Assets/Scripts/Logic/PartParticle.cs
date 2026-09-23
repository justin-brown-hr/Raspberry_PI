using UnityEngine;
using View;

namespace Logic
{
	public class PartParticle : MonoBehaviour
	{
		internal bool isOnFire;

		private void Start()
		{
			isOnFire = false;
		}

		public void Initialize(bool fromCatapult, bool isPlayer = false)
		{
			if (fromCatapult)
			{
				base.gameObject.tag = "CatapultParticle";
				base.gameObject.layer = UnityEngine.Random.Range(14, 16);
				GetComponent<Rigidbody2D>().useAutoMass = true;
				GetComponent<Rigidbody2D>().gravityScale = 4f;
				if (GetComponent<MeshRenderer>().bounds.size.magnitude < 1f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				if (isPlayer)
				{
					base.gameObject.AddComponent<ParticleDestroyer>();
				}
			}
			else
			{
				GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				base.gameObject.tag = "ProjectileParticle";
				base.gameObject.layer = 14;
				GetComponent<Rigidbody2D>().useAutoMass = true;
				GetComponent<Rigidbody2D>().gravityScale = 4f;
				GetComponent<Rigidbody2D>().angularDrag = 0f;
				GetComponent<Rigidbody2D>().drag = 0f;
				if (GetComponent<MeshRenderer>().bounds.size.magnitude < 1.2f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		private void OnDisable()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void Update()
		{
		}
	}
}
