using UnityEngine;

public class Spawner : MonoBehaviour
{
	public float spawnTime = 5f;

	public float spawnDelay = 3f;

	public GameObject[] enemies;

	private void Start()
	{
		InvokeRepeating("Spawn", spawnDelay, spawnTime);
	}

	private void Spawn()
	{
		int num = UnityEngine.Random.Range(0, enemies.Length);
		Object.Instantiate(enemies[num], base.transform.position, base.transform.rotation);
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.Play();
		}
	}
}
