using Delaunay;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteExploder
{
	public static List<GameObject> GenerateTriangularPieces(GameObject source, int extraPoints = 0, int subshatterSteps = 0, Material mat = null)
	{
		List<GameObject> list = new List<GameObject>();
		if (mat == null)
		{
			mat = createFragmentMaterial(source);
		}
		Vector3 localScale = source.transform.localScale;
		source.transform.localScale = Vector3.one;
		Quaternion localRotation = source.transform.localRotation;
		source.transform.localRotation = Quaternion.identity;
		Vector2 velocity = source.GetComponent<Rigidbody2D>().velocity;
		PolygonCollider2D component = source.GetComponent<PolygonCollider2D>();
		BoxCollider2D component2 = source.GetComponent<BoxCollider2D>();
		List<Vector2> list2 = new List<Vector2>();
		List<Vector2> boundary = new List<Vector2>();
		if (component != null)
		{
			list2 = getPoints(component);
			boundary = getPoints(component);
		}
		else if (component2 != null)
		{
			list2 = getPoints(component2);
			boundary = getPoints(component2);
		}
		Rect rect = getRect(source);
		if (list2.Count == 3)
		{
			list2.Add((list2[0] + list2[1] + list2[2]) / 3f);
		}
		for (int i = 0; i < extraPoints; i++)
		{
			list2.Add(new Vector2(Random.Range(rect.width / -2f, rect.width / 2f), Random.Range(rect.height / -2f, rect.height / 2f)));
		}
		Voronoi voronoi = new Voronoi(list2, null, rect);
		List<List<Vector2>> list3 = new List<List<Vector2>>();
		foreach (Triangle item in voronoi.Triangles())
		{
			list3 = ClipperHelper.clip(boundary, item);
			foreach (List<Vector2> item2 in list3)
			{
				list.Add(generateTriangularPiece(source, item2, velocity, localScale, localRotation, mat));
			}
		}
		List<GameObject> list4 = new List<GameObject>();
		if (subshatterSteps > 0)
		{
			subshatterSteps--;
			foreach (GameObject item3 in list)
			{
				list4.AddRange(GenerateTriangularPieces(item3, extraPoints, subshatterSteps, mat));
				UnityEngine.Object.DestroyImmediate(item3);
			}
		}
		else
		{
			list4 = list;
		}
		source.transform.localScale = localScale;
		source.transform.localRotation = localRotation;
		Resources.UnloadUnusedAssets();
		return list4;
	}

	private static GameObject generateTriangularPiece(GameObject source, List<Vector2> tri, Vector2 origVelocity, Vector3 origScale, Quaternion origRotation, Material mat)
	{
		GameObject gameObject = new GameObject(source.name + " piece");
		gameObject.transform.position = source.transform.position;
		gameObject.transform.rotation = source.transform.rotation;
		gameObject.transform.localScale = source.transform.localScale;
		MeshFilter meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
		gameObject.AddComponent(typeof(MeshRenderer));
		Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
		if (sharedMesh == null)
		{
			meshFilter.mesh = new Mesh();
			sharedMesh = meshFilter.sharedMesh;
		}
		Vector3[] array = new Vector3[3];
		int[] array2 = new int[3];
		ref Vector3 reference = ref array[0];
		Vector2 vector = tri[0];
		float x = vector.x;
		Vector2 vector2 = tri[0];
		reference = new Vector3(x, vector2.y, 0f);
		ref Vector3 reference2 = ref array[1];
		Vector2 vector3 = tri[1];
		float x2 = vector3.x;
		Vector2 vector4 = tri[1];
		reference2 = new Vector3(x2, vector4.y, 0f);
		ref Vector3 reference3 = ref array[2];
		Vector2 vector5 = tri[2];
		float x3 = vector5.x;
		Vector2 vector6 = tri[2];
		reference3 = new Vector3(x3, vector6.y, 0f);
		array2[0] = 0;
		array2[1] = 1;
		array2[2] = 2;
		sharedMesh.vertices = array;
		sharedMesh.triangles = array2;
		if (source.GetComponent<SpriteRenderer>() != null)
		{
			sharedMesh.uv = calcUV(array, source.GetComponent<SpriteRenderer>(), source.transform);
		}
		else
		{
			sharedMesh.uv = calcUV(array, source.GetComponent<MeshRenderer>(), source.transform);
		}
		gameObject.transform.localScale = origScale;
		gameObject.transform.localRotation = origRotation;
		Vector3 diff = calcPivotCenterDiff(gameObject);
		centerMeshPivot(gameObject, diff);
		sharedMesh.RecalculateBounds();
		gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;
		meshFilter.mesh = sharedMesh;
		PolygonCollider2D polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
		polygonCollider2D.SetPath(0, new Vector2[3]
		{
			sharedMesh.vertices[0],
			sharedMesh.vertices[1],
			sharedMesh.vertices[2]
		});
		Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
		rigidbody2D.velocity = origVelocity;
		return gameObject;
	}

	public static List<GameObject> GenerateVoronoiPieces(GameObject source, int extraPoints = 0, int subshatterSteps = 0, Material mat = null)
	{
		List<GameObject> list = new List<GameObject>();
		if (mat == null)
		{
			mat = createFragmentMaterial(source);
		}
		Vector3 localScale = source.transform.localScale;
		source.transform.localScale = Vector3.one;
		Quaternion localRotation = source.transform.localRotation;
		source.transform.localRotation = Quaternion.identity;
		Vector2 velocity = source.GetComponent<Rigidbody2D>().velocity;
		PolygonCollider2D component = source.GetComponent<PolygonCollider2D>();
		BoxCollider2D component2 = source.GetComponent<BoxCollider2D>();
		List<Vector2> list2 = new List<Vector2>();
		List<Vector2> boundary = new List<Vector2>();
		if (component != null)
		{
			list2 = getPoints(component);
			boundary = getPoints(component);
		}
		else if (component2 != null)
		{
			list2 = getPoints(component2);
			boundary = getPoints(component2);
		}
		Rect rect = getRect(source);
		for (int i = 0; i < extraPoints; i++)
		{
			list2.Add(new Vector2(Random.Range(rect.width / -2f, rect.width / 2f), Random.Range(rect.height / -2f, rect.height / 2f)));
		}
		Voronoi voronoi = new Voronoi(list2, null, rect);
		List<List<Vector2>> list3 = new List<List<Vector2>>();
		foreach (List<Vector2> item in voronoi.Regions())
		{
			list3 = ClipperHelper.clip(boundary, item);
			foreach (List<Vector2> item2 in list3)
			{
				list.Add(generateVoronoiPiece(source, item2, velocity, localScale, localRotation, mat));
			}
		}
		List<GameObject> list4 = new List<GameObject>();
		if (subshatterSteps > 0)
		{
			subshatterSteps--;
			foreach (GameObject item3 in list)
			{
				list4.AddRange(GenerateVoronoiPieces(item3, extraPoints, subshatterSteps));
				UnityEngine.Object.DestroyImmediate(item3);
			}
		}
		else
		{
			list4 = list;
		}
		source.transform.localScale = localScale;
		source.transform.localRotation = localRotation;
		Resources.UnloadUnusedAssets();
		return list4;
	}

	private static GameObject generateVoronoiPiece(GameObject source, List<Vector2> region, Vector2 origVelocity, Vector3 origScale, Quaternion origRotation, Material mat)
	{
		GameObject gameObject = new GameObject(source.name + " piece");
		gameObject.transform.position = source.transform.position;
		gameObject.transform.rotation = source.transform.rotation;
		gameObject.transform.localScale = source.transform.localScale;
		MeshFilter meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
		gameObject.AddComponent(typeof(MeshRenderer));
		Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
		if (sharedMesh == null)
		{
			meshFilter.mesh = new Mesh();
			sharedMesh = meshFilter.sharedMesh;
		}
		Voronoi region2 = new Voronoi(region, null, getRect(region));
		Vector3[] vertices = calcVerts(region2);
		int[] triangles = calcTriangles(region2);
		sharedMesh.vertices = vertices;
		sharedMesh.triangles = triangles;
		if (source.GetComponent<SpriteRenderer>() != null)
		{
			sharedMesh.uv = calcUV(vertices, source.GetComponent<SpriteRenderer>(), source.transform);
		}
		else
		{
			sharedMesh.uv = calcUV(vertices, source.GetComponent<MeshRenderer>(), source.transform);
		}
		gameObject.transform.localScale = origScale;
		gameObject.transform.localRotation = origRotation;
		Vector3 vector = calcPivotCenterDiff(gameObject);
		centerMeshPivot(gameObject, vector);
		sharedMesh.RecalculateBounds();
		gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;
		meshFilter.mesh = sharedMesh;
		PolygonCollider2D polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
		polygonCollider2D.SetPath(0, calcPolyColliderPoints(region, vector));
		Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
		rigidbody2D.velocity = origVelocity;
		return gameObject;
	}

	private static List<Vector2> getPoints(BoxCollider2D collider)
	{
		List<Vector2> list = new List<Vector2>();
		Vector2 offset = collider.offset;
		Vector2 size = collider.size;
		list.Add(new Vector2(offset.x - size.x / 2f, offset.y - size.y / 2f));
		list.Add(new Vector2(offset.x - size.x / 2f, offset.y + size.y / 2f));
		list.Add(new Vector2(offset.x + size.x / 2f, offset.y + size.y / 2f));
		list.Add(new Vector2(offset.x + size.x / 2f, offset.y - size.y / 2f));
		return list;
	}

	private static List<Vector2> getPoints(PolygonCollider2D collider)
	{
		List<Vector2> list = new List<Vector2>();
		Vector2[] path = collider.GetPath(0);
		foreach (Vector2 item in path)
		{
			list.Add(item);
		}
		return list;
	}

	private static List<Vector2> getRendererPoints(GameObject source)
	{
		List<Vector2> list = new List<Vector2>();
		Bounds bounds = source.GetComponent<Renderer>().bounds;
		List<Vector2> list2 = list;
		Vector3 center = bounds.center;
		float x = center.x;
		Vector3 extents = bounds.extents;
		float x2 = x - extents.x;
		Vector3 center2 = bounds.center;
		float y = center2.y;
		Vector3 extents2 = bounds.extents;
		list2.Add(new Vector2(x2, y - extents2.y) - (Vector2)source.transform.position);
		List<Vector2> list3 = list;
		Vector3 center3 = bounds.center;
		float x3 = center3.x;
		Vector3 extents3 = bounds.extents;
		float x4 = x3 + extents3.x;
		Vector3 center4 = bounds.center;
		float y2 = center4.y;
		Vector3 extents4 = bounds.extents;
		list3.Add(new Vector2(x4, y2 - extents4.y) - (Vector2)source.transform.position);
		List<Vector2> list4 = list;
		Vector3 center5 = bounds.center;
		float x5 = center5.x;
		Vector3 extents5 = bounds.extents;
		float x6 = x5 + extents5.x;
		Vector3 center6 = bounds.center;
		float y3 = center6.y;
		Vector3 extents6 = bounds.extents;
		list4.Add(new Vector2(x6, y3 + extents6.y) - (Vector2)source.transform.position);
		List<Vector2> list5 = list;
		Vector3 center7 = bounds.center;
		float x7 = center7.x;
		Vector3 extents7 = bounds.extents;
		float x8 = x7 - extents7.x;
		Vector3 center8 = bounds.center;
		float y4 = center8.y;
		Vector3 extents8 = bounds.extents;
		list5.Add(new Vector2(x8, y4 + extents8.y) - (Vector2)source.transform.position);
		return list;
	}

	private static Rect getRect(GameObject source)
	{
		Bounds bounds = source.GetComponent<Renderer>().bounds;
		Vector3 extents = bounds.extents;
		float x = extents.x * -1f;
		Vector3 extents2 = bounds.extents;
		float y = extents2.y * -1f;
		Vector3 size = bounds.size;
		float x2 = size.x;
		Vector3 size2 = bounds.size;
		return new Rect(x, y, x2, size2.y);
	}

	private static Rect getRect(List<Vector2> region)
	{
		Vector2 a = default(Vector2);
		Vector2 vector = region[0];
		float x = vector.x;
		float num = x;
		Vector2 vector2 = region[0];
		float y = vector2.y;
		float num2 = y;
		foreach (Vector2 item in region)
		{
			Vector2 current = item;
			a += current;
			if (current.x < x)
			{
				x = current.x;
			}
			if (current.x > num)
			{
				num = current.x;
			}
			if (current.y < y)
			{
				y = current.y;
			}
			if (current.y > num2)
			{
				num2 = current.y;
			}
		}
		a /= region.Count;
		Vector2 size = new Vector2(num - x, num2 - y);
		return new Rect(a, size);
	}

	private static Vector2[] calcUV(Vector3[] vertices, SpriteRenderer sRend, Transform sTransform)
	{
		Vector3 extents = sRend.bounds.extents;
		float num = extents.y * 2f;
		Vector3 localScale = sTransform.localScale;
		float num2 = num / localScale.y;
		Vector3 extents2 = sRend.bounds.extents;
		float num3 = extents2.x * 2f;
		Vector3 localScale2 = sTransform.localScale;
		float num4 = num3 / localScale2.x;
		Vector3 center = sRend.bounds.center;
		float x = center.x;
		Vector3 extents3 = sRend.bounds.extents;
		float x2 = x - extents3.x;
		Vector3 center2 = sRend.bounds.center;
		float y = center2.y;
		Vector3 extents4 = sRend.bounds.extents;
		Vector3 vector = sTransform.InverseTransformPoint(new Vector3(x2, y - extents4.y, 0f));
		Vector2[] array = new Vector2[vertices.Length];
		Vector2[] uv = sRend.sprite.uv;
		getUVRange(out Vector2 min, out Vector2 max, uv);
		for (int i = 0; i < vertices.Length; i++)
		{
			float target = (vertices[i].x - vector.x) / num4;
			target = scaleRange(target, 0f, 1f, min.x, max.x);
			float target2 = (vertices[i].y - vector.y) / num2;
			target2 = scaleRange(target2, 0f, 1f, min.y, max.y);
			array[i] = new Vector2(target, target2);
		}
		return array;
	}

	private static Vector2[] calcUV(Vector3[] vertices, MeshRenderer mRend, Transform sTransform)
	{
		Vector3 extents = mRend.bounds.extents;
		float num = extents.y * 2f;
		Vector3 localScale = sTransform.localScale;
		float num2 = num / localScale.y;
		Vector3 extents2 = mRend.bounds.extents;
		float num3 = extents2.x * 2f;
		Vector3 localScale2 = sTransform.localScale;
		float num4 = num3 / localScale2.x;
		Vector3 center = mRend.bounds.center;
		float x = center.x;
		Vector3 extents3 = mRend.bounds.extents;
		float x2 = x - extents3.x;
		Vector3 center2 = mRend.bounds.center;
		float y = center2.y;
		Vector3 extents4 = mRend.bounds.extents;
		Vector3 vector = sTransform.InverseTransformPoint(new Vector3(x2, y - extents4.y, 0f));
		Vector2[] array = new Vector2[vertices.Length];
		Vector2[] uv = sTransform.GetComponent<MeshFilter>().sharedMesh.uv;
		getUVRange(out Vector2 min, out Vector2 max, uv);
		for (int i = 0; i < vertices.Length; i++)
		{
			float target = (vertices[i].x - vector.x) / num4;
			target = scaleRange(target, 0f, 1f, min.x, max.x);
			float target2 = (vertices[i].y - vector.y) / num2;
			target2 = scaleRange(target2, 0f, 1f, min.y, max.y);
			array[i] = new Vector2(target, target2);
		}
		return array;
	}

	private static void getUVRange(out Vector2 min, out Vector2 max, Vector2[] uv)
	{
		min = uv[0];
		max = uv[0];
		for (int i = 0; i < uv.Length; i++)
		{
			Vector2 vector = uv[i];
			if (vector.x < min.x)
			{
				min.x = vector.x;
			}
			if (vector.x > max.x)
			{
				max.x = vector.x;
			}
			if (vector.y < min.y)
			{
				min.y = vector.y;
			}
			if (vector.y > max.y)
			{
				max.y = vector.y;
			}
		}
	}

	private static float scaleRange(float target, float oldMin, float oldMax, float newMin, float newMax)
	{
		return target / ((oldMax - oldMin) / (newMax - newMin)) + newMin;
	}

	private static Vector3[] calcVerts(Voronoi region)
	{
		List<Site> sites = region.Sites()._sites;
		Vector3[] array = new Vector3[sites.Count];
		int num = 0;
		foreach (Site item in sites)
		{
			array[num++] = new Vector3(item.x, item.y, 0f);
		}
		return array;
	}

	private static int[] calcTriangles(Voronoi region)
	{
		int[] array = new int[region.Triangles().Count * 3];
		List<Site> sites = region.Sites()._sites;
		int num = 0;
		foreach (Triangle item in region.Triangles())
		{
			array[num++] = sites.IndexOf(item.sites[0]);
			array[num++] = sites.IndexOf(item.sites[1]);
			array[num++] = sites.IndexOf(item.sites[2]);
		}
		return array;
	}

	private static Vector2[] calcPolyColliderPoints(List<Vector2> points, Vector2 offset)
	{
		Vector2[] array = new Vector2[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			array[i] = points[i] + offset;
		}
		return array;
	}

	private static Vector3 calcPivotCenterDiff(GameObject target)
	{
		Mesh sharedMesh = target.GetComponent<MeshFilter>().sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		Vector3 a = default(Vector3);
		for (int i = 0; i < vertices.Length; i++)
		{
			a += vertices[i];
		}
		Vector3 b = a / vertices.Length;
		Vector3 a2 = target.transform.InverseTransformPoint(target.transform.position);
		return a2 - b;
	}

	private static void centerMeshPivot(GameObject target, Vector3 diff)
	{
		Mesh sharedMesh = target.GetComponent<MeshFilter>().sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] += diff;
		}
		sharedMesh.vertices = vertices;
		Vector3 a = target.transform.InverseTransformPoint(target.transform.position);
		target.transform.localPosition = target.transform.TransformPoint(a - diff);
	}

	private static void setFragmentMaterial(GameObject newSprite, GameObject source)
	{
		Material material = new Material(Shader.Find("Sprites/Default"));
		SpriteRenderer component = source.GetComponent<SpriteRenderer>();
		if (component != null)
		{
			material.SetTexture("_MainTex", component.sprite.texture);
			material.color = component.color;
		}
		else
		{
			material = source.GetComponent<MeshRenderer>().sharedMaterial;
		}
		newSprite.GetComponent<MeshRenderer>().sharedMaterial = material;
	}

	private static Material createFragmentMaterial(GameObject source)
	{
		SpriteRenderer component = source.GetComponent<SpriteRenderer>();
		if (component != null)
		{
			Material material = new Material(Shader.Find("Sprites/Default"));
			material.SetTexture("_MainTex", component.sprite.texture);
			material.color = component.color;
			return material;
		}
		return source.GetComponent<MeshRenderer>().sharedMaterial;
	}
}
