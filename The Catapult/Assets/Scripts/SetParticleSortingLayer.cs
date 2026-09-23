using UnityEngine;

public class SetParticleSortingLayer : MonoBehaviour
{
	public string sortingLayerName;

	private void Start()
	{
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortingLayerName;
	}
}
