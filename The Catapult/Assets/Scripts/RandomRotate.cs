using System;
using UnityEngine;

[Serializable]
public class RandomRotate : MonoBehaviour
{
	private Quaternion rotTarget;

	public float rotateEverySecond;

	public RandomRotate()
	{
		rotateEverySecond = 1f;
	}

	public void Start()
	{
		randomRot();
		InvokeRepeating("randomRot", 0f, rotateEverySecond);
	}

	public void Update()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, rotTarget, Time.time * Time.deltaTime);
	}

	public void randomRot()
	{
		rotTarget = UnityEngine.Random.rotation;
	}

	public void Main()
	{
	}
}
