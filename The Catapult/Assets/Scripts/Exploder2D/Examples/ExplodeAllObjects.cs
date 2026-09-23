using Exploder2D.Utils;
using UnityEngine;

namespace Exploder2D.Examples
{
	public class ExplodeAllObjects : MonoBehaviour
	{
		private GameObject[] DestroyableObjects;

		private void Start()
		{
			DestroyableObjects = GameObject.FindGameObjectsWithTag("Exploder2D");
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
			{
				GameObject[] destroyableObjects = DestroyableObjects;
				foreach (GameObject gameObject in destroyableObjects)
				{
					ExplodeObject(gameObject);
				}
			}
		}

		private void ExplodeObject(GameObject gameObject)
		{
			Exploder2DObject exploder2DInstance = Exploder2DSingleton.Exploder2DInstance;
			exploder2DInstance.transform.position = Exploder2DUtils.GetCentroid(gameObject);
			exploder2DInstance.Radius = 1f;
			exploder2DInstance.Explode();
		}

		private void OnGUI()
		{
			GUI.Label(new Rect(200f, 10f, 300f, 30f), "Hit enter to explode everything!");
		}
	}
}
