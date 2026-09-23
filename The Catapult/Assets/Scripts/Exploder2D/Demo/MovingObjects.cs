using Exploder2D.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Exploder2D.Demo
{
	public class MovingObjects : MonoBehaviour
	{
		public List<GameObject> rows;

		public float Speed = 10f;

		public float yStart = 22.5f;

		public float yLimit = 0.3f;

		private List<GameObject> sprites;

		private int index;

		private void Start()
		{
			sprites = new List<GameObject>();
			foreach (GameObject row in rows)
			{
				SpriteRenderer[] componentsInChildren = row.GetComponentsInChildren<SpriteRenderer>();
				SpriteRenderer[] array = componentsInChildren;
				foreach (SpriteRenderer spriteRenderer in array)
				{
					sprites.Add(spriteRenderer.gameObject);
				}
			}
		}

		private void Update()
		{
			foreach (GameObject row in rows)
			{
				row.transform.position -= new Vector3(0f, Speed * Time.deltaTime, 0f);
				Vector3 position = row.transform.position;
				if (position.y < yLimit)
				{
					Transform transform = row.transform;
					Vector3 position2 = row.transform.position;
					transform.position = new Vector3(position2.x, yStart);
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
			{
				ExplodeList();
			}
		}

		private void ExplodeList()
		{
			if (index < sprites.Count)
			{
				Exploder2DObject exploder2DInstance = Exploder2DSingleton.Exploder2DInstance;
				exploder2DInstance.transform.position = Exploder2DUtils.GetCentroid(sprites[index]);
				exploder2DInstance.Radius = 1f;
				exploder2DInstance.Explode(OnExplosion);
			}
		}

		public void OnExplosion(float timeMS, Exploder2DObject.ExplosionState state)
		{
			if (state == Exploder2DObject.ExplosionState.ExplosionFinished)
			{
				index++;
				ExplodeList();
			}
		}
	}
}
