using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Logic
{
	public class BossDestroyedPart : MonoBehaviour
	{
		public GameObject[] destroyedParts;

		private Transform initialParent;

		private Vector2[] initialPartPositions;

		private Quaternion[] initialPartRotations;

		public void PrepareDestroyedPart()
		{
			initialPartPositions = new Vector2[destroyedParts.Length];
			initialPartRotations = new Quaternion[destroyedParts.Length];
			initialParent = base.transform.parent;
			for (int i = 0; i < destroyedParts.Length; i++)
			{
				initialPartPositions[i] = destroyedParts[i].transform.localPosition;
				initialPartRotations[i] = destroyedParts[i].transform.localRotation;
			}
			base.gameObject.SetActive(value: false);
		}

		public void ActivatePart(bool fromBaseStage = false)
		{
			Dictionary<GameObject, float> dictionary = new Dictionary<GameObject, float>();
			for (int i = 0; i < destroyedParts.Length; i++)
			{
				destroyedParts[i].transform.localPosition = initialPartPositions[i];
				destroyedParts[i].transform.localRotation = initialPartRotations[i];
				if (!fromBaseStage)
				{
					destroyedParts[i].layer = 15;
				}
				else
				{
					destroyedParts[i].layer = UnityEngine.Random.Range(14, 16);
				}
				destroyedParts[i].GetComponent<Rigidbody2D>().gravityScale = 3f;
				dictionary.Add(destroyedParts[i], destroyedParts[i].GetComponent<SpriteRenderer>().bounds.size.magnitude);
				destroyedParts[i].gameObject.tag = "CatapultParticle";
				UnityEngine.Object.Destroy(destroyedParts[i].gameObject, UnityEngine.Random.Range(0.5f, 3.5f));
			}
			List<KeyValuePair<GameObject, float>> list = dictionary.ToList();
			list.Sort((KeyValuePair<GameObject, float> pair1, KeyValuePair<GameObject, float> pair2) => pair1.Value.CompareTo(pair2.Value));
			for (int j = 0; j < list.Count; j++)
			{
				float num = 2f + (float)j * 0.75f;
				if (num < 0f)
				{
					num *= -1f;
				}
				list[j].Key.GetComponent<Rigidbody2D>().mass = num;
			}
			base.transform.parent = null;
			base.gameObject.SetActive(value: true);
			UnityEngine.Object.Destroy(base.gameObject, 4f);
		}

		private void Update()
		{
		}
	}
}
