using System;
using UnityEngine;

[Serializable]
public class RotateY : MonoBehaviour
{
	public void Start()
	{
	}

	public void Update()
	{
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		float y = eulerAngles.y + 1f * Time.deltaTime;
		Quaternion rotation = transform.rotation;
		Vector3 eulerAngles2 = rotation.eulerAngles;
		eulerAngles2.y = y;
		Vector3 vector2 = rotation.eulerAngles = eulerAngles2;
		Quaternion quaternion2 = transform.rotation = rotation;
	}

	public void Main()
	{
	}
}
