using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	public Vector3 offset;

	private Transform player;

	private void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	private void Update()
	{
		base.transform.position = player.position + offset;
	}
}
