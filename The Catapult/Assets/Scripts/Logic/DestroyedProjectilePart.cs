using System;
using UnityEngine;

namespace Logic
{
	public class DestroyedProjectilePart : MonoBehaviour
	{
		private bool initialized;

		private Vector2[] initialPartPositions;

		public GameObject[] allParts;

		private float activeTime;

		private bool isActive;

		public void InitPart()
		{
			if (!initialized)
			{
				initialized = true;
				isActive = false;
				activeTime = 0f;
				initialPartPositions = new Vector2[allParts.Length];
				for (int i = 0; i < allParts.Length; i++)
				{
					initialPartPositions[i] = allParts[i].transform.localPosition;
				}
			}
		}

		public void ActivateDestroyedPart(Vector3 position, Vector3 velocity)
		{
			try
			{
				base.transform.position = position;
				base.gameObject.SetActive(value: true);
				activeTime = 0f;
				isActive = true;
				if (initialPartPositions == null)
				{
					UnityEngine.Debug.LogError("No initial positions!");
				}
				for (int i = 0; i < allParts.Length; i++)
				{
					if (allParts[i] != null)
					{
						allParts[i].transform.localPosition = initialPartPositions[i];
						allParts[i].SetActive(value: true);
						if (allParts[i].GetComponent<Rigidbody2D>() == null)
						{
							allParts[i].AddComponent<Rigidbody2D>();
						}
						allParts[i].GetComponent<Rigidbody2D>().velocity = velocity;
					}
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex.ToString());
			}
		}

		public void DeactivateDestroyedPart()
		{
			try
			{
				isActive = false;
				for (int i = 0; i < allParts.Length; i++)
				{
					if (allParts[i] != null && allParts[i].transform.childCount > 0)
					{
						ProjectileShootPart[] componentsInChildren = allParts[i].transform.GetComponentsInChildren<ProjectileShootPart>();
						for (int j = 0; j < componentsInChildren.Length; j++)
						{
							UnityEngine.Object.Destroy(componentsInChildren[j].gameObject);
						}
					}
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.ToString());
			}
			finally
			{
				base.gameObject.SetActive(value: false);
			}
		}

		private void Update()
		{
			if (isActive)
			{
				if (activeTime < 5f)
				{
					activeTime += Time.deltaTime;
				}
				else
				{
					DeactivateDestroyedPart();
				}
			}
		}
	}
}
