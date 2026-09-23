using Logic;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Explodable : MonoBehaviour
{
	public enum ShatterType
	{
		Triangle,
		Voronoi
	}

	public Action<List<GameObject>> OnFragmentsGenerated;

	public bool allowRuntimeFragmentation;

	public int extraPoints;

	public int subshatterSteps;

	public string fragmentLayer = "Default";

	public string sortingLayerName = "Default";

	public int orderInLayer;

	public ShatterType shatterType;

	public List<GameObject> fragments = new List<GameObject>();

	private List<List<Vector2>> polygons = new List<List<Vector2>>();

	public void explode(Transform newParent)
	{
		if (fragments.Count == 0 && allowRuntimeFragmentation)
		{
			generateFragments();
		}
		else
		{
			foreach (GameObject fragment in fragments)
			{
				fragment.transform.parent = null;
				fragment.AddComponent<PartParticle>();
				fragment.SetActive(value: true);
			}
		}
		if (fragments.Count > 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void fragmentInEditor()
	{
		if (fragments.Count > 0)
		{
			deleteFragments();
		}
		generateFragments();
		setPolygonsForDrawing();
		foreach (GameObject fragment in fragments)
		{
			fragment.transform.parent = base.transform;
			fragment.SetActive(value: false);
		}
	}

	public void deleteFragments()
	{
		foreach (GameObject fragment in fragments)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(fragment);
			}
			else
			{
				UnityEngine.Object.Destroy(fragment);
			}
		}
		fragments.Clear();
		polygons.Clear();
	}

	public void generateFragments()
	{
		fragments = new List<GameObject>();
		switch (shatterType)
		{
		case ShatterType.Triangle:
			fragments = SpriteExploder.GenerateTriangularPieces(base.gameObject, extraPoints, subshatterSteps);
			break;
		case ShatterType.Voronoi:
			fragments = SpriteExploder.GenerateVoronoiPieces(base.gameObject, extraPoints, subshatterSteps);
			break;
		default:
			UnityEngine.Debug.Log("invalid choice");
			break;
		}
		foreach (GameObject fragment in fragments)
		{
			if (fragment != null)
			{
				fragment.layer = LayerMask.NameToLayer(fragmentLayer);
				fragment.GetComponent<Renderer>().sortingLayerName = sortingLayerName;
				fragment.GetComponent<Renderer>().sortingOrder = orderInLayer;
				fragment.AddComponent<PartParticle>();
			}
		}
		ExplodableAddon[] components = GetComponents<ExplodableAddon>();
		foreach (ExplodableAddon explodableAddon in components)
		{
			if (explodableAddon.enabled)
			{
				explodableAddon.OnFragmentsGenerated(fragments);
			}
		}
	}

	private void setPolygonsForDrawing()
	{
		polygons.Clear();
		foreach (GameObject fragment in fragments)
		{
			List<Vector2> list = new List<Vector2>();
			Vector2[] points = fragment.GetComponent<PolygonCollider2D>().points;
			foreach (Vector2 a in points)
			{
				Vector2 b = rotateAroundPivot(fragment.transform.position, base.transform.position, Quaternion.Inverse(base.transform.rotation)) - (Vector2)base.transform.position;
				float x = b.x;
				Vector3 localScale = base.transform.localScale;
				b.x = x / localScale.x;
				float y = b.y;
				Vector3 localScale2 = base.transform.localScale;
				b.y = y / localScale2.y;
				list.Add(a + b);
			}
			polygons.Add(list);
		}
	}

	private Vector2 rotateAroundPivot(Vector2 point, Vector2 pivot, Quaternion angle)
	{
		Vector2 v = point - pivot;
		v = angle * v;
		point = v + pivot;
		return point;
	}

	private void OnDrawGizmos()
	{
		if (Application.isEditor)
		{
			if (polygons.Count == 0 && fragments.Count != 0)
			{
				setPolygonsForDrawing();
			}
			Gizmos.color = Color.blue;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Vector2 b = (Vector2)base.transform.position * 0f;
			foreach (List<Vector2> polygon in polygons)
			{
				for (int i = 0; i < polygon.Count; i++)
				{
					if (i + 1 == polygon.Count)
					{
						Gizmos.DrawLine(polygon[i] + b, polygon[0] + b);
					}
					else
					{
						Gizmos.DrawLine(polygon[i] + b, polygon[i + 1] + b);
					}
				}
			}
		}
	}
}
