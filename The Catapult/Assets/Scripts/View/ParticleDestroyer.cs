using Logic;
using UnityEngine;

namespace View
{
	internal class ParticleDestroyer : MonoBehaviour
	{
		private float liveTime = 2f;

		private void Start()
		{
			if (base.gameObject.GetComponent<CharacterPart>() != null)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void OnDisable()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void Update()
		{
			if (liveTime > 0f)
			{
				liveTime -= Time.deltaTime;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
