using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;

	private void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}

	private IEnumerator CheckIfAlive()
	{
		ParticleSystem ps = GetComponent<ParticleSystem>();
		do
		{
			if (true && ps != null)
			{
				yield return new WaitForSeconds(0.5f);
				continue;
			}
			yield break;
		}
		while (ps.IsAlive(withChildren: true));
		if (OnlyDeactivate)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
