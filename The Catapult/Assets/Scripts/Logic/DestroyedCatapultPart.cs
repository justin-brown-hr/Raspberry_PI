using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using View;

namespace Logic
{
	public class DestroyedCatapultPart : MonoBehaviour
	{
		private bool initialized;

		private Vector2[] initialPartPositions;

		private Quaternion[] initialPartRotations;

		public GameObject[] allParts;

		public bool generateMoreActiveFragments;

		private Transform initialParent;

		private new bool enabled;

		private float hideTime;

		public GameObject woodPartDestroy;

		public GameObject steelPartDestroy;

		private int catapultLevel;

		private bool isFromPlayer;

		public void InitPart()
		{
			if (initialized)
			{
				return;
			}
			enabled = false;
			initialized = true;
			initialPartPositions = new Vector2[allParts.Length];
			initialPartRotations = new Quaternion[allParts.Length];
			initialParent = base.transform.parent;
			for (int i = 0; i < allParts.Length; i++)
			{
				if (allParts[i] != null)
				{
					initialPartPositions[i] = allParts[i].transform.localPosition;
				}
				initialPartRotations[i] = allParts[i].transform.localRotation;
			}
		}

		public void SetDestroyedSprites(int type, int level)
		{
			catapultLevel = level;
			isFromPlayer = false;
			for (int i = 0; i < allParts.Length; i++)
			{
				if (allParts[i] != null && allParts[i].GetComponent<DestroyedCatapultParticle>() != null)
				{
					allParts[i].GetComponent<DestroyedCatapultParticle>().ChangeSprite(type, level);
				}
			}
		}

		public void SetPlayerDestroyedSprites(int type, int level)
		{
			catapultLevel = level;
			isFromPlayer = true;
			for (int i = 0; i < allParts.Length; i++)
			{
				if (allParts[i] != null && allParts[i].GetComponent<DestroyedCatapultParticle>() != null)
				{
					allParts[i].GetComponent<DestroyedCatapultParticle>().ChangeSprite(type, level, isPlayer: true);
				}
			}
		}

		public bool ActivateDestroyedPart(Vector3 position, Quaternion rotation, int layer, Vector3 projectilePosition, Vector3 ProjectileVelocity)
		{
			base.transform.position = position;
			base.transform.rotation = rotation;
			base.gameObject.SetActive(value: true);
			GameObject gameObject = null;
			Vector3 vector = projectilePosition;
			if (projectilePosition != Vector3.zero)
			{
				vector += (base.transform.position - projectilePosition) / 2f;
				gameObject = (isFromPlayer ? ((catapultLevel <= 5) ? UnityEngine.Object.Instantiate(woodPartDestroy, vector, Quaternion.identity) : UnityEngine.Object.Instantiate(steelPartDestroy, vector, Quaternion.identity)) : ((catapultLevel <= 3) ? UnityEngine.Object.Instantiate(woodPartDestroy, vector, Quaternion.identity) : UnityEngine.Object.Instantiate(steelPartDestroy, vector, Quaternion.identity)));
				if (gameObject != null)
				{
					gameObject.AddComponent<ParticleDestroyer>();
					if (!isFromPlayer)
					{
						gameObject.transform.rotation = Quaternion.LookRotation(ProjectileVelocity);
					}
				}
			}
			float num = 1000000f;
			GameObject gameObject2 = null;
			Dictionary<GameObject, float> dictionary = new Dictionary<GameObject, float>();
			for (int i = 0; i < allParts.Length; i++)
			{
				if (!(allParts[i] == null))
				{
					allParts[i].transform.localPosition = initialPartPositions[i];
					allParts[i].transform.localRotation = initialPartRotations[i];
					allParts[i].layer = 15;
					allParts[i].gameObject.tag = "CatapultParticle";
					allParts[i].GetComponent<DestroyedCatapultParticle>().SetLayerDelayed(generateMoreActiveFragments);
					allParts[i].GetComponent<Rigidbody2D>().gravityScale = 2f;
					dictionary.Add(allParts[i], allParts[i].GetComponent<SpriteRenderer>().bounds.size.magnitude);
					allParts[i].GetComponent<SpriteRenderer>().sortingOrder = layer;
					allParts[i].SetActive(value: true);
					if (num > Vector2.Distance(allParts[i].transform.position, vector))
					{
						gameObject2 = allParts[i];
					}
				}
			}
			if (gameObject2 != null && gameObject != null)
			{
				gameObject.transform.parent = gameObject2.transform;
			}
			List<KeyValuePair<GameObject, float>> list = dictionary.ToList();
			list.Sort((KeyValuePair<GameObject, float> pair1, KeyValuePair<GameObject, float> pair2) => pair1.Value.CompareTo(pair2.Value));
			for (int j = 0; j < list.Count; j++)
			{
				float num2 = 1.5f + (float)j * 0.2f;
				if (num2 < 0f)
				{
					num2 *= -1f;
				}
				list[j].Key.GetComponent<Rigidbody2D>().mass = num2;
			}
			base.transform.parent = null;
			hideTime = 6f;
			enabled = true;
			if (gameObject != null)
			{
				return true;
			}
			return false;
		}

		public void DeactivateDestroyedPart()
		{
			hideTime = 3f;
			enabled = true;
		}

		public void DeatcivateItself()
		{
			try
			{
				enabled = false;
				base.gameObject.SetActive(value: false);
				for (int i = 0; i < allParts.Length; i++)
				{
					if (allParts[i] != null)
					{
						allParts[i].transform.localPosition = initialPartPositions[i];
						allParts[i].transform.localRotation = initialPartRotations[i];
						allParts[i].transform.parent = base.transform;
						ProjectileShootPart[] componentsInChildren = allParts[i].transform.GetComponentsInChildren<ProjectileShootPart>();
						for (int j = 0; j < componentsInChildren.Length; j++)
						{
							UnityEngine.Object.Destroy(componentsInChildren[j].gameObject);
						}
						allParts[i].GetComponent<DestroyedCatapultParticle>().DeactivatePart();
						base.gameObject.SetActive(value: false);
					}
				}
				base.transform.parent = initialParent;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex.ToString());
			}
		}

		private void Update()
		{
			if (enabled)
			{
				if (hideTime > 0f)
				{
					hideTime -= Time.deltaTime;
				}
				else
				{
					DeatcivateItself();
				}
			}
		}
	}
}
