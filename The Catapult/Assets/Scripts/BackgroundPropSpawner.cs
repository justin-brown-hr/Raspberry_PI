using System;
using System.Collections;
using UnityEngine;

public class BackgroundPropSpawner : MonoBehaviour
{
	public Rigidbody2D backgroundProp;

	public float leftSpawnPosX;

	public float rightSpawnPosX;

	public float minSpawnPosY;

	public float maxSpawnPosY;

	public float minTimeBetweenSpawns;

	public float maxTimeBetweenSpawns;

	public float minSpeed;

	public float maxSpeed;

	private void Start()
	{
		UnityEngine.Random.InitState(DateTime.Today.Millisecond);
		StartCoroutine("Spawn");
	}

	private IEnumerator Spawn()
	{
		float waitTime = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
		yield return new WaitForSeconds(waitTime);
		bool facingLeft = UnityEngine.Random.Range(0, 2) == 0;
		float posX = (!facingLeft) ? leftSpawnPosX : rightSpawnPosX;
		float posY = UnityEngine.Random.Range(minSpawnPosY, maxSpawnPosY);
		float x = posX;
		float y = posY;
		Vector3 position = base.transform.position;
		Rigidbody2D propInstance = UnityEngine.Object.Instantiate<Rigidbody2D>(position: new Vector3(x, y, position.z), original: backgroundProp, rotation: Quaternion.identity);
		if (!facingLeft)
		{
			Vector3 localScale = propInstance.transform.localScale;
			localScale.x *= -1f;
			propInstance.transform.localScale = localScale;
		}
		float speed2 = UnityEngine.Random.Range(minSpeed, maxSpeed);
		speed2 *= ((!facingLeft) ? 1f : (-1f));
		propInstance.velocity = new Vector2(speed2, 0f);
		StartCoroutine(Spawn());
		while (propInstance != null)
		{
			if (facingLeft)
			{
				Vector3 position3 = propInstance.transform.position;
				if (position3.x < leftSpawnPosX - 0.5f)
				{
					UnityEngine.Object.Destroy(propInstance.gameObject);
				}
			}
			else
			{
				Vector3 position4 = propInstance.transform.position;
				if (position4.x > rightSpawnPosX + 0.5f)
				{
					UnityEngine.Object.Destroy(propInstance.gameObject);
				}
			}
			yield return null;
		}
	}
}
