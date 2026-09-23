using System.Collections;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
	public GameObject[] pickups;

	public float pickupDeliveryTime = 5f;

	public float dropRangeLeft;

	public float dropRangeRight;

	public float highHealthThreshold = 75f;

	public float lowHealthThreshold = 25f;

	private PlayerHealth playerHealth;

	private void Awake()
	{
		playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
	}

	private void Start()
	{
		StartCoroutine(DeliverPickup());
	}

	public IEnumerator DeliverPickup()
	{
		yield return new WaitForSeconds(pickupDeliveryTime);
		float dropPosX = UnityEngine.Random.Range(dropRangeLeft, dropRangeRight);
		Vector3 dropPos = new Vector3(dropPosX, 15f, 1f);
		if (playerHealth.health >= highHealthThreshold)
		{
			Object.Instantiate(pickups[0], dropPos, Quaternion.identity);
			yield break;
		}
		if (playerHealth.health <= lowHealthThreshold)
		{
			Object.Instantiate(pickups[1], dropPos, Quaternion.identity);
			yield break;
		}
		int num = UnityEngine.Random.Range(0, pickups.Length);
		Object.Instantiate(pickups[num], dropPos, Quaternion.identity);
	}
}
