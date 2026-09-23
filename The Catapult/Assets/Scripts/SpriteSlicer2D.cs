using System.Collections.Generic;
using UnityEngine;

public static class SpriteSlicer2D
{
	private enum LineSide
	{
		Left,
		Right,
		On
	}

	private struct SpriteSlicerLine
	{
		public Vector2 p1;

		public Vector2 p2;

		public SpriteSlicerLine(Vector2 a, Vector2 b)
		{
			p1 = a;
			p2 = b;
		}
	}

	public class Polygon
	{
		public List<Vector2> points = new List<Vector2>();
	}

	private class LinkedPolygonPoint
	{
		public Vector2 Position;

		public LineSide SliceSide;

		public LinkedPolygonPoint Next;

		public LinkedPolygonPoint Prev;

		public float DistOnLine;

		public bool Visited;

		public LinkedPolygonPoint(Vector2 startPos, LineSide side)
		{
			Position = startPos;
			SliceSide = side;
		}
	}

	public class VectorComparer : IComparer<Vector2>
	{
		public int Compare(Vector2 vectorA, Vector2 vectorB)
		{
			if (vectorA.x > vectorB.x)
			{
				return 1;
			}
			if (vectorA.x < vectorB.x)
			{
				return -1;
			}
			return 0;
		}
	}

	private class EdgeComparer : IComparer<LinkedPolygonPoint>
	{
		private SpriteSlicerLine line;

		public EdgeComparer(SpriteSlicerLine ln)
		{
			line = ln;
		}

		public int Compare(LinkedPolygonPoint edgeA, LinkedPolygonPoint edgeB)
		{
			float num = DotProduct(line.p1, line.p2, edgeA.Position);
			float num2 = DotProduct(line.p1, line.p2, edgeB.Position);
			if (num < num2)
			{
				return -1;
			}
			return 1;
		}
	}

	private static bool s_DebugLoggingEnabled = false;

	public static bool s_CentreChildSprites = false;

	private static List<SpriteSlicer2DSliceInfo> s_SubSlicesCont = new List<SpriteSlicer2DSliceInfo>();

	private static List<LinkedPolygonPoint> s_ConcavePolygonPoints = new List<LinkedPolygonPoint>();

	private static List<LinkedPolygonPoint> s_ConcavePolygonIntersectionPoints = new List<LinkedPolygonPoint>();

	private static List<Polygon> s_ConcaveSlicePolygonResults = new List<Polygon>();

	private static VectorComparer s_VectorComparer = new VectorComparer();

	public static bool DebugLoggingEnabled
	{
		get
		{
			return s_DebugLoggingEnabled;
		}
		set
		{
			s_DebugLoggingEnabled = value;
		}
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint)
	{
		LayerMask layerMask = -1;
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects: true, -1, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, LayerMask layerMask)
	{
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects: true, -1, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, string tag)
	{
		LayerMask layerMask = -1;
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects: true, -1, ref slicedObjectInfo, layerMask, tag);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, string tag)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, tag);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask)
	{
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask)
	{
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite)
	{
		if ((bool)sprite)
		{
			LayerMask layerMask = -1;
			List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, destroySlicedObjects: true, -1, ref slicedObjectInfo, layerMask, null);
		}
	}

	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		if ((bool)sprite)
		{
			LayerMask layerMask = -1;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
		}
	}

	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		if ((bool)sprite)
		{
			LayerMask layerMask = -1;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
		}
	}

	public static void ExplodeSprite(GameObject sprite, int numCuts, float explosionForce)
	{
		if ((bool)sprite)
		{
			List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
			if (explosionForce != 0f)
			{
				slicedObjectInfo = new List<SpriteSlicer2DSliceInfo>();
			}
			ExplodeSprite(sprite, numCuts, explosionForce, destroySlicedObjects: true, ref slicedObjectInfo);
		}
	}

	public static void ExplodeSprite(GameObject sprite, int numCuts, float explosionForce, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		if (!sprite || !GetSpriteBounds(sprite, out Bounds spriteBounds))
		{
			return;
		}
		LayerMask layerMask = -1;
		int instanceID = sprite.GetInstanceID();
		Vector3 position = sprite.transform.position;
		Vector3 size = spriteBounds.size;
		float x = size.x;
		Vector3 size2 = spriteBounds.size;
		float d = x + size2.y;
		Vector3 b2 = default(Vector3);
		for (int i = 0; i < numCuts; i++)
		{
			float f = Random.Range(0f, 6.28318548f);
			Vector3 b = new Vector3(Mathf.Sin(f), Mathf.Cos(f), 0f) * d;
			f = Random.Range(0f, 6.28318548f);
			float num = Mathf.Sin(f);
			Vector3 size3 = spriteBounds.size;
			float x2 = num * (size3.x * 0.25f);
			float num2 = Mathf.Cos(f);
			Vector3 size4 = spriteBounds.size;
			b2 = new Vector3(x2, num2 * (size4.y * 0.25f), 0f);
			Vector3 worldStartPoint = position + b + b2;
			Vector3 worldEndPoint = position - b + b2;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, null, instanceID, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
			s_SubSlicesCont.Clear();
			for (int j = 0; j < slicedObjectInfo.Count; j++)
			{
				for (int k = 0; k < slicedObjectInfo[j].ChildObjects.Count; k++)
				{
					SliceSpritesInternal(worldStartPoint, worldEndPoint, slicedObjectInfo[j].ChildObjects[k], 0, destroySlicedObjects, -1, ref s_SubSlicesCont, layerMask, null);
				}
			}
			slicedObjectInfo.AddRange(s_SubSlicesCont);
		}
		if (slicedObjectInfo == null)
		{
			return;
		}
		for (int l = 0; l < slicedObjectInfo.Count; l++)
		{
			for (int m = 0; m < slicedObjectInfo[l].ChildObjects.Count; m++)
			{
				Rigidbody2D component = slicedObjectInfo[l].ChildObjects[m].GetComponent<Rigidbody2D>();
				if ((bool)component)
				{
					component.AddForceAtPosition(new Vector2(0f, 1f) * explosionForce, position);
				}
			}
		}
	}

	public static void ShatterSprite(GameObject spriteObject, float explosionForce)
	{
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		ShatterSprite(spriteObject, explosionForce, destroySlicedObjects: true, ref slicedObjectInfo);
	}

	public static void ShatterSprite(GameObject spriteObject, float explosionForce, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		Rigidbody2D component = spriteObject.GetComponent<Rigidbody2D>();
		if (!component)
		{
			UnityEngine.Debug.LogWarning("Could not shatter sprite - no attached rigidbody");
			return;
		}
		SlicedSprite slicedSprite = null;
		SpriteRenderer spriteRenderer = null;
		spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
		{
			slicedSprite = spriteObject.GetComponent<SlicedSprite>();
			if (slicedSprite == null)
			{
				return;
			}
		}
		List<Vector2> points = null;
		PolygonCollider2D component2 = spriteObject.GetComponent<PolygonCollider2D>();
		PhysicsMaterial2D physicsMaterial = null;
		if ((bool)component2)
		{
			points = new List<Vector2>(component2.points);
			physicsMaterial = component2.sharedMaterial;
		}
		else
		{
			BoxCollider2D component3 = spriteObject.GetComponent<BoxCollider2D>();
			if ((bool)component3)
			{
				points = new List<Vector2>(4);
				List<Vector2> list = points;
				Vector2 size = component3.size;
				float x = (0f - size.x) * 0.5f;
				Vector2 size2 = component3.size;
				list.Add(new Vector2(x, (0f - size2.y) * 0.5f));
				List<Vector2> list2 = points;
				Vector2 size3 = component3.size;
				float x2 = size3.x * 0.5f;
				Vector2 size4 = component3.size;
				list2.Add(new Vector2(x2, (0f - size4.y) * 0.5f));
				List<Vector2> list3 = points;
				Vector2 size5 = component3.size;
				float x3 = size5.x * 0.5f;
				Vector2 size6 = component3.size;
				list3.Add(new Vector2(x3, size6.y * 0.5f));
				List<Vector2> list4 = points;
				Vector2 size7 = component3.size;
				float x4 = (0f - size7.x) * 0.5f;
				Vector2 size8 = component3.size;
				list4.Add(new Vector2(x4, size8.y * 0.5f));
				physicsMaterial = component3.sharedMaterial;
			}
			else
			{
				CircleCollider2D component4 = spriteObject.GetComponent<CircleCollider2D>();
				if ((bool)component4)
				{
					int num = 32;
					float num2 = 6.28318548f / (float)num;
					points = new List<Vector2>(num);
					for (int i = 0; i < num; i++)
					{
						float f = num2 * (float)i;
						points.Add(new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * component4.radius);
					}
					physicsMaterial = component4.sharedMaterial;
				}
			}
		}
		if (points == null || points.Count <= 3)
		{
			return;
		}
		SpriteSlicer2DSliceInfo spriteSlicer2DSliceInfo = null;
		if (slicedObjectInfo != null)
		{
			spriteSlicer2DSliceInfo = new SpriteSlicer2DSliceInfo();
			slicedObjectInfo.Add(spriteSlicer2DSliceInfo);
			if (!destroySlicedObjects)
			{
				spriteSlicer2DSliceInfo.SlicedObject = spriteObject;
			}
		}
		Vector2 vector = spriteObject.transform.position;
		int[] array = Triangulate(ref points);
		List<Vector2> list5 = new List<Vector2>(3);
		list5.Add(Vector2.zero);
		list5.Add(Vector2.zero);
		list5.Add(Vector2.zero);
		float parentArea = Mathf.Abs(Area(ref points));
		for (int j = 0; j < array.Length; j += 3)
		{
			list5[0] = points[array[j]];
			list5[1] = points[array[j + 1]];
			list5[2] = points[array[j + 2]];
			Vector2[] vertices = list5.ToArray();
			float area = GetArea(ref vertices);
			CreateChildSprite(component, physicsMaterial, ref vertices, parentArea, area, out SlicedSprite slicedSprite2, out PolygonCollider2D polygonCollider);
			slicedSprite2.gameObject.name = spriteObject.name + "_child";
			if ((bool)slicedSprite)
			{
				slicedSprite2.InitFromSlicedSprite(slicedSprite, ref polygonCollider, ref vertices, isConcave: false);
			}
			else if ((bool)spriteRenderer)
			{
				slicedSprite2.InitFromUnitySprite(spriteRenderer, ref polygonCollider, ref vertices, isConcave: false);
			}
			Vector2 a = vector + (vertices[0] + vertices[1] + vertices[2]) * 0.33f;
			Vector2 a2 = a - vector;
			a2.Normalize();
			slicedSprite2.GetComponent<Rigidbody2D>().AddForceAtPosition(a2 * explosionForce, vector);
			spriteSlicer2DSliceInfo?.ChildObjects.Add(slicedSprite2.gameObject);
		}
		if (destroySlicedObjects)
		{
			UnityEngine.Object.Destroy(component.gameObject);
		}
		else
		{
			component.gameObject.SetActive(value: false);
		}
	}

	private static bool GetSpriteBounds(GameObject sprite, out Bounds spriteBounds)
	{
		spriteBounds = default(Bounds);
		bool result = false;
		SpriteRenderer component = sprite.GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			spriteBounds = component.sprite.bounds;
			result = true;
		}
		else
		{
			SlicedSprite component2 = sprite.GetComponent<SlicedSprite>();
			if (component2 != null)
			{
				spriteBounds = component2.SpriteBounds;
				result = true;
			}
		}
		return result;
	}

	private static void SliceSpritesInternal(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject spriteObject, int spriteInstanceID, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask, string tag)
	{
		Vector3 vector = Vector3.Normalize(worldEndPoint - worldStartPoint);
		float distance = Vector3.Distance(worldStartPoint, worldEndPoint);
		RaycastHit2D[] array = Physics2D.RaycastAll(worldStartPoint, vector, distance, layerMask.value);
		RaycastHit2D[] array2 = Physics2D.RaycastAll(worldEndPoint, -vector, distance, layerMask.value);
		if (array.Length != array2.Length)
		{
			return;
		}
		for (int i = 0; i < array.Length && i < array2.Length; i++)
		{
			RaycastHit2D raycastHit2D = array[i];
			int num = -1;
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].collider == raycastHit2D.collider)
				{
					num = j;
					break;
				}
			}
			if (num == -1)
			{
				continue;
			}
			RaycastHit2D raycastHit2D2 = array2[num];
			if (!(raycastHit2D.rigidbody == raycastHit2D2.rigidbody))
			{
				continue;
			}
			Rigidbody2D rigidbody = raycastHit2D.rigidbody;
			Transform transform = raycastHit2D.transform;
			if (!rigidbody || rigidbody.gameObject.isStatic || (spriteObject != null && rigidbody.gameObject != spriteObject) || (tag != null && rigidbody.tag != tag))
			{
				continue;
			}
			SlicedSprite slicedSprite = null;
			SpriteRenderer spriteRenderer = null;
			spriteRenderer = rigidbody.GetComponent<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				slicedSprite = rigidbody.GetComponent<SlicedSprite>();
				if (slicedSprite == null || (maxCutDepth >= 0 && slicedSprite.CutsSinceParentObject >= maxCutDepth))
				{
					continue;
				}
			}
			if (spriteInstanceID != 0 && rigidbody.gameObject.GetInstanceID() != spriteInstanceID && (slicedSprite == null || slicedSprite.ParentInstanceID != spriteInstanceID))
			{
				continue;
			}
			Vector3 v = transform.InverseTransformPoint(worldStartPoint);
			Vector3 v2 = transform.InverseTransformPoint(worldEndPoint);
			Vector3 v3 = transform.InverseTransformPoint(raycastHit2D.point);
			Vector3 v4 = transform.InverseTransformPoint(raycastHit2D2.point);
			List<Vector2> polygonPoints = null;
			PolygonCollider2D component = rigidbody.GetComponent<PolygonCollider2D>();
			PhysicsMaterial2D physicsMaterial = null;
			if ((bool)component)
			{
				polygonPoints = new List<Vector2>(component.points);
				physicsMaterial = component.sharedMaterial;
			}
			else
			{
				BoxCollider2D component2 = rigidbody.GetComponent<BoxCollider2D>();
				if ((bool)component2)
				{
					polygonPoints = new List<Vector2>(4);
					Vector2 offset = component2.offset;
					List<Vector2> list = polygonPoints;
					Vector2 a = offset;
					Vector2 size = component2.size;
					float x = (0f - size.x) * 0.5f;
					Vector2 size2 = component2.size;
					list.Add(a + new Vector2(x, (0f - size2.y) * 0.5f));
					List<Vector2> list2 = polygonPoints;
					Vector2 a2 = offset;
					Vector2 size3 = component2.size;
					float x2 = size3.x * 0.5f;
					Vector2 size4 = component2.size;
					list2.Add(a2 + new Vector2(x2, (0f - size4.y) * 0.5f));
					List<Vector2> list3 = polygonPoints;
					Vector2 a3 = offset;
					Vector2 size5 = component2.size;
					float x3 = size5.x * 0.5f;
					Vector2 size6 = component2.size;
					list3.Add(a3 + new Vector2(x3, size6.y * 0.5f));
					List<Vector2> list4 = polygonPoints;
					Vector2 a4 = offset;
					Vector2 size7 = component2.size;
					float x4 = (0f - size7.x) * 0.5f;
					Vector2 size8 = component2.size;
					list4.Add(a4 + new Vector2(x4, size8.y * 0.5f));
					physicsMaterial = component2.sharedMaterial;
				}
				else
				{
					CircleCollider2D component3 = rigidbody.GetComponent<CircleCollider2D>();
					if ((bool)component3)
					{
						int num2 = 32;
						float num3 = 6.28318548f / (float)num2;
						polygonPoints = new List<Vector2>(num2);
						Vector2 offset2 = component3.offset;
						for (int k = 0; k < num2; k++)
						{
							float f = num3 * (float)k;
							polygonPoints.Add(offset2 + new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * component3.radius);
						}
						physicsMaterial = component3.sharedMaterial;
					}
				}
			}
			if (polygonPoints == null)
			{
				continue;
			}
			if (IsPointInsidePolygon(v, ref polygonPoints) || IsPointInsidePolygon(v2, ref polygonPoints))
			{
				if (s_DebugLoggingEnabled && spriteObject != null)
				{
					UnityEngine.Debug.LogWarning("Failed to slice " + rigidbody.gameObject.name + " - start or end cut point is inside the collision mesh");
				}
				continue;
			}
			SpriteSlicer2DSliceInfo spriteSlicer2DSliceInfo = new SpriteSlicer2DSliceInfo();
			spriteSlicer2DSliceInfo.SlicedObject = rigidbody.gameObject;
			spriteSlicer2DSliceInfo.SliceEnterWorldPosition = raycastHit2D.point;
			spriteSlicer2DSliceInfo.SliceExitWorldPosition = raycastHit2D2.point;
			float parentArea = Mathf.Abs(Area(ref polygonPoints));
			if (IsConvex(ref polygonPoints))
			{
				int count = polygonPoints.Count;
				List<Vector2> vertices = new List<Vector2>(count);
				List<Vector2> vertices2 = new List<Vector2>(count);
				vertices.Add(v3);
				vertices.Add(v4);
				vertices2.Add(v3);
				vertices2.Add(v4);
				for (int l = 0; l < count; l++)
				{
					Vector2 vector2 = polygonPoints[l];
					float num4 = CalculateDeterminant2x3(v, v2, vector2);
					if (num4 > 0f)
					{
						vertices.Add(vector2);
					}
					else
					{
						vertices2.Add(vector2);
					}
				}
				Vector2[] vertices3 = ArrangeVertices(ref vertices);
				Vector2[] vertices4 = ArrangeVertices(ref vertices2);
				float area = GetArea(ref vertices3);
				float area2 = GetArea(ref vertices4);
				if (!AreVerticesAcceptable(ref vertices3, area, failOnConcave: true) || !AreVerticesAcceptable(ref vertices4, area2, failOnConcave: true))
				{
					continue;
				}
				CreateChildSprite(rigidbody, physicsMaterial, ref vertices3, parentArea, area, out SlicedSprite slicedSprite2, out PolygonCollider2D polygonCollider);
				slicedSprite2.gameObject.name = rigidbody.gameObject.name + "_child1";
				CreateChildSprite(rigidbody, physicsMaterial, ref vertices4, parentArea, area2, out SlicedSprite slicedSprite3, out PolygonCollider2D polygonCollider2);
				slicedSprite3.gameObject.name = rigidbody.gameObject.name + "_child2";
				if ((bool)slicedSprite)
				{
					slicedSprite2.InitFromSlicedSprite(slicedSprite, ref polygonCollider, ref vertices3, isConcave: false);
					slicedSprite3.InitFromSlicedSprite(slicedSprite, ref polygonCollider2, ref vertices4, isConcave: false);
				}
				else if ((bool)spriteRenderer)
				{
					slicedSprite2.InitFromUnitySprite(spriteRenderer, ref polygonCollider, ref vertices3, isConcave: false);
					slicedSprite3.InitFromUnitySprite(spriteRenderer, ref polygonCollider2, ref vertices4, isConcave: false);
				}
				spriteSlicer2DSliceInfo.ChildObjects.Add(slicedSprite2.gameObject);
				spriteSlicer2DSliceInfo.ChildObjects.Add(slicedSprite3.gameObject);
			}
			else
			{
				Polygon polygon = new Polygon();
				SpriteSlicerLine line = new SpriteSlicerLine(v, v2);
				polygon.points = polygonPoints;
				SliceConcave(polygon, line);
				for (int m = 0; m < s_ConcaveSlicePolygonResults.Count; m++)
				{
					Vector2[] vertices5 = s_ConcaveSlicePolygonResults[m].points.ToArray();
					float area3 = GetArea(ref vertices5);
					if (AreVerticesAcceptable(ref vertices5, area3, failOnConcave: false))
					{
						CreateChildSprite(rigidbody, physicsMaterial, ref vertices5, parentArea, area3, out SlicedSprite slicedSprite4, out PolygonCollider2D polygonCollider3);
						slicedSprite4.gameObject.name = rigidbody.gameObject.name + "_child" + m;
						if ((bool)slicedSprite)
						{
							slicedSprite4.InitFromSlicedSprite(slicedSprite, ref polygonCollider3, ref vertices5, isConcave: true);
						}
						else if ((bool)spriteRenderer)
						{
							slicedSprite4.InitFromUnitySprite(spriteRenderer, ref polygonCollider3, ref vertices5, isConcave: true);
						}
						spriteSlicer2DSliceInfo.ChildObjects.Add(slicedSprite4.gameObject);
					}
				}
			}
			if (spriteSlicer2DSliceInfo.ChildObjects.Count <= 0)
			{
				continue;
			}
			rigidbody.gameObject.SendMessage("OnSpriteSliced", spriteSlicer2DSliceInfo, SendMessageOptions.DontRequireReceiver);
			if (slicedObjectInfo != null)
			{
				if (destroySlicedObjects)
				{
					spriteSlicer2DSliceInfo.SlicedObject = null;
				}
				slicedObjectInfo.Add(spriteSlicer2DSliceInfo);
			}
			if (destroySlicedObjects)
			{
				UnityEngine.Object.Destroy(rigidbody.gameObject);
			}
			else
			{
				rigidbody.gameObject.SetActive(value: false);
			}
		}
	}

	private static void CreateChildSprite(Rigidbody2D parentRigidBody, PhysicsMaterial2D physicsMaterial, ref Vector2[] spriteVertices, float parentArea, float childArea, out SlicedSprite slicedSprite, out PolygonCollider2D polygonCollider)
	{
		GameObject gameObject = new GameObject();
		slicedSprite = gameObject.AddComponent<SlicedSprite>();
		Rigidbody2D component = slicedSprite.GetComponent<Rigidbody2D>();
		component.mass = parentRigidBody.mass * (childArea / parentArea);
		component.drag = parentRigidBody.drag;
		component.angularDrag = parentRigidBody.angularDrag;
		component.gravityScale = parentRigidBody.gravityScale;
		component.fixedAngle = parentRigidBody.fixedAngle;
		component.isKinematic = parentRigidBody.isKinematic;
		component.interpolation = parentRigidBody.interpolation;
		component.sleepMode = parentRigidBody.sleepMode;
		component.collisionDetectionMode = parentRigidBody.collisionDetectionMode;
		component.velocity = parentRigidBody.velocity;
		component.angularVelocity = parentRigidBody.angularVelocity;
		polygonCollider = slicedSprite.GetComponent<PolygonCollider2D>();
		polygonCollider.SetPath(0, spriteVertices);
		polygonCollider.sharedMaterial = physicsMaterial;
	}

	private static LineSide GetSideOfLine(SpriteSlicerLine line, Vector2 pt)
	{
		float num = (pt.x - line.p1.x) * (line.p2.y - line.p1.y) - (pt.y - line.p1.y) * (line.p2.x - line.p1.x);
		return (num > float.Epsilon) ? LineSide.Right : ((!(num < -1.401298E-45f)) ? LineSide.On : LineSide.Left);
	}

	private static float CalculateDeterminant2x3(Vector2 start, Vector2 end, Vector2 point)
	{
		return start.x * end.y + end.x * point.y + point.x * start.y - start.y * end.x - end.y * point.x - point.y * start.x;
	}

	public static float CalculateDeterminant2x2(Vector2 vectorA, Vector2 vectorB)
	{
		return vectorA.x * vectorB.y - vectorA.y * vectorB.x;
	}

	public static float DotProduct(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
	{
		return (point.x - lineStart.x) * (lineEnd.x - lineStart.x) + (point.y - lineStart.y) * (lineEnd.y - lineStart.y);
	}

	public static int[] Triangulate(ref List<Vector2> points)
	{
		List<int> list = new List<int>();
		int count = points.Count;
		if (count < 3)
		{
			return list.ToArray();
		}
		int[] array = new int[count];
		if (Area(ref points) > 0f)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = i;
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				array[j] = count - 1 - j;
			}
		}
		int num = count;
		int num2 = 2 * num;
		int num3 = 0;
		int num4 = num - 1;
		while (num > 2)
		{
			if (num2-- <= 0)
			{
				return list.ToArray();
			}
			int num6 = num4;
			if (num <= num6)
			{
				num6 = 0;
			}
			num4 = num6 + 1;
			if (num <= num4)
			{
				num4 = 0;
			}
			int num7 = num4 + 1;
			if (num <= num7)
			{
				num7 = 0;
			}
			if (Snip(ref points, num6, num4, num7, num, array))
			{
				int item = array[num6];
				int item2 = array[num4];
				int item3 = array[num7];
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				num3++;
				int num8 = num4;
				for (int k = num4 + 1; k < num; k++)
				{
					array[num8] = array[k];
					num8++;
				}
				num--;
				num2 = 2 * num;
			}
		}
		list.Reverse();
		return list.ToArray();
	}

	private static float Area(ref List<Vector2> points)
	{
		int count = points.Count;
		float num = 0f;
		int index = count - 1;
		int num2 = 0;
		while (num2 < count)
		{
			Vector2 vector = points[index];
			Vector2 vector2 = points[num2];
			num += vector.x * vector2.y - vector2.x * vector.y;
			index = num2++;
		}
		return num * 0.5f;
	}

	private static bool Snip(ref List<Vector2> points, int u, int v, int w, int n, int[] V)
	{
		Vector2 a = points[V[u]];
		Vector2 b = points[V[v]];
		Vector2 c = points[V[w]];
		if (Mathf.Epsilon > (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x))
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w)
			{
				Vector2 p = points[V[i]];
				if (InsideTriangle(a, b, c, p))
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		float num = C.x - B.x;
		float num2 = C.y - B.y;
		float num3 = A.x - C.x;
		float num4 = A.y - C.y;
		float num5 = B.x - A.x;
		float num6 = B.y - A.y;
		float num7 = P.x - A.x;
		float num8 = P.y - A.y;
		float num9 = P.x - B.x;
		float num10 = P.y - B.y;
		float num11 = P.x - C.x;
		float num12 = P.y - C.y;
		float num13 = num * num10 - num2 * num9;
		float num14 = num5 * num8 - num6 * num7;
		float num15 = num3 * num12 - num4 * num11;
		return num13 >= 0f && num15 >= 0f && num14 >= 0f;
	}

	private static Vector2[] ArrangeVertices(ref List<Vector2> vertices)
	{
		int count = vertices.Count;
		int num = 1;
		int num2 = count - 1;
		vertices.Sort(s_VectorComparer);
		List<Vector2> list = new List<Vector2>(count);
		for (int i = 0; i < count; i++)
		{
			list.Add(Vector2.zero);
		}
		Vector2 vector = vertices[0];
		Vector2 vector2 = vertices[count - 1];
		list[0] = vector;
		for (int j = 1; j < count - 1; j++)
		{
			float num3 = CalculateDeterminant2x3(vector, vector2, vertices[j]);
			if (num3 < 0f)
			{
				list[num++] = vertices[j];
			}
			else
			{
				list[num2--] = vertices[j];
			}
		}
		list[num] = vector2;
		return list.ToArray();
	}

	private static float GetArea(ref Vector2[] vertices)
	{
		int num = vertices.Length;
		float num2 = vertices[0].y * (vertices[num - 1].x - vertices[1].x);
		for (int i = 1; i < num; i++)
		{
			num2 += vertices[i].y * (vertices[i - 1].x - vertices[(i + 1) % num].x);
		}
		return Mathf.Abs(num2 * 0.5f);
	}

	public static bool IsConvex(ref Vector2[] vertices)
	{
		int num = vertices.Length;
		Vector3 v = vertices[0] - vertices[num - 1];
		Vector3 vector = vertices[1] - vertices[0];
		float num2 = CalculateDeterminant2x2(v, vector);
		float num3;
		for (int i = 1; i < num - 1; i++)
		{
			v = vector;
			vector = vertices[i + 1] - vertices[i];
			num3 = CalculateDeterminant2x2(v, vector);
			if (num2 * num3 < 0f)
			{
				return false;
			}
		}
		v = vector;
		vector = vertices[0] - vertices[num - 1];
		num3 = CalculateDeterminant2x2(v, vector);
		if (num2 * num3 < 0f)
		{
			return false;
		}
		return true;
	}

	public static bool IsConvex(ref List<Vector2> vertices)
	{
		int count = vertices.Count;
		Vector3 v = vertices[0] - vertices[count - 1];
		Vector3 vector = vertices[1] - vertices[0];
		float num = CalculateDeterminant2x2(v, vector);
		float num2;
		for (int i = 1; i < count - 1; i++)
		{
			v = vector;
			vector = vertices[i + 1] - vertices[i];
			num2 = CalculateDeterminant2x2(v, vector);
			if (num * num2 < 0f)
			{
				return false;
			}
		}
		v = vector;
		vector = vertices[0] - vertices[count - 1];
		num2 = CalculateDeterminant2x2(v, vector);
		if (num * num2 < 0f)
		{
			return false;
		}
		return true;
	}

	private static bool AreVerticesAcceptable(ref Vector2[] vertices, float area, bool failOnConcave)
	{
		if (vertices.Length < 3)
		{
			if (s_DebugLoggingEnabled)
			{
				UnityEngine.Debug.LogWarning("Vertices rejected - insufficient vertices");
			}
			return false;
		}
		if (area < 0.0001f)
		{
			if (s_DebugLoggingEnabled)
			{
				UnityEngine.Debug.LogWarning("Vertices rejected - below minimum area");
			}
			return false;
		}
		if (failOnConcave && !IsConvex(ref vertices))
		{
			if (s_DebugLoggingEnabled)
			{
				UnityEngine.Debug.LogWarning("Vertices rejected - shape is not convex");
			}
			return false;
		}
		return true;
	}

	private static void SliceConcave(Polygon poly, SpriteSlicerLine line)
	{
		SplitEdges(poly, line);
		SortEdges(line);
		SplitPolygon();
		CollectPolys();
	}

	private static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
	{
		float num = pe1.y - ps1.y;
		float num2 = ps1.x - pe1.x;
		float num3 = num * ps1.x + num2 * ps1.y;
		float num4 = pe2.y - ps2.y;
		float num5 = ps2.x - pe2.x;
		float num6 = num4 * ps2.x + num5 * ps2.y;
		float num7 = num * num5 - num4 * num2;
		if (num7 == 0f)
		{
			UnityEngine.Debug.LogError("Lines are parallel!");
		}
		float num8 = 1f / num7;
		return new Vector2((num5 * num3 - num2 * num6) * num8, (num * num6 - num4 * num3) * num8);
	}

	private static void SplitEdges(Polygon poly, SpriteSlicerLine line)
	{
		s_ConcavePolygonPoints.Clear();
		s_ConcavePolygonIntersectionPoints.Clear();
		for (int i = 0; i < poly.points.Count; i++)
		{
			SpriteSlicerLine spriteSlicerLine = new SpriteSlicerLine(poly.points[i], poly.points[(i + 1) % poly.points.Count]);
			LineSide sideOfLine = GetSideOfLine(line, spriteSlicerLine.p1);
			LineSide sideOfLine2 = GetSideOfLine(line, spriteSlicerLine.p2);
			s_ConcavePolygonPoints.Add(new LinkedPolygonPoint(poly.points[i], sideOfLine));
			if (sideOfLine == LineSide.On)
			{
				s_ConcavePolygonIntersectionPoints.Add(s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1]);
			}
			else if (sideOfLine != sideOfLine2 && sideOfLine2 != LineSide.On)
			{
				Vector2 startPos = LineIntersectionPoint(spriteSlicerLine.p1, spriteSlicerLine.p2, line.p1, line.p2);
				s_ConcavePolygonPoints.Add(new LinkedPolygonPoint(startPos, LineSide.On));
				s_ConcavePolygonIntersectionPoints.Add(s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1]);
			}
		}
		for (int j = 0; j < s_ConcavePolygonPoints.Count - 1; j++)
		{
			int index = (j + 1) % s_ConcavePolygonPoints.Count;
			LinkedPolygonPoint linkedPolygonPoint = s_ConcavePolygonPoints[j];
			(linkedPolygonPoint.Next = s_ConcavePolygonPoints[index]).Prev = linkedPolygonPoint;
		}
		s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1].Next = s_ConcavePolygonPoints[0];
		s_ConcavePolygonPoints[0].Prev = s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1];
	}

	private static void SortEdges(SpriteSlicerLine line)
	{
		EdgeComparer comparer = new EdgeComparer(line);
		s_ConcavePolygonIntersectionPoints.Sort(comparer);
		for (int i = 1; i < s_ConcavePolygonIntersectionPoints.Count; i++)
		{
			s_ConcavePolygonIntersectionPoints[i].DistOnLine = (s_ConcavePolygonIntersectionPoints[i].Position - s_ConcavePolygonIntersectionPoints[0].Position).magnitude;
		}
	}

	private static void SplitPolygon()
	{
		LinkedPolygonPoint linkedPolygonPoint = null;
		for (int i = 0; i < s_ConcavePolygonIntersectionPoints.Count; i++)
		{
			LinkedPolygonPoint linkedPolygonPoint2 = linkedPolygonPoint;
			linkedPolygonPoint = null;
			while (linkedPolygonPoint2 == null && i < s_ConcavePolygonIntersectionPoints.Count)
			{
				LinkedPolygonPoint linkedPolygonPoint3 = s_ConcavePolygonIntersectionPoints[i];
				LineSide sliceSide = linkedPolygonPoint3.SliceSide;
				LineSide sliceSide2 = linkedPolygonPoint3.Prev.SliceSide;
				LineSide sliceSide3 = linkedPolygonPoint3.Next.SliceSide;
				if (sliceSide != LineSide.On)
				{
					UnityEngine.Debug.LogError("Current side should be ON");
				}
				if ((sliceSide2 == LineSide.Left && sliceSide3 == LineSide.Right) || (sliceSide2 == LineSide.Left && sliceSide3 == LineSide.On && linkedPolygonPoint3.Next.DistOnLine < linkedPolygonPoint3.DistOnLine) || (sliceSide2 == LineSide.On && sliceSide3 == LineSide.Right && linkedPolygonPoint3.Prev.DistOnLine < linkedPolygonPoint3.DistOnLine))
				{
					linkedPolygonPoint2 = linkedPolygonPoint3;
				}
				i++;
			}
			LinkedPolygonPoint linkedPolygonPoint4 = null;
			while (linkedPolygonPoint4 == null && i < s_ConcavePolygonIntersectionPoints.Count)
			{
				LinkedPolygonPoint linkedPolygonPoint5 = s_ConcavePolygonIntersectionPoints[i];
				LineSide sliceSide4 = linkedPolygonPoint5.SliceSide;
				LineSide sliceSide5 = linkedPolygonPoint5.Prev.SliceSide;
				LineSide sliceSide6 = linkedPolygonPoint5.Next.SliceSide;
				if (sliceSide4 != LineSide.On)
				{
					UnityEngine.Debug.LogError("Current side should be ON");
				}
				if ((sliceSide5 == LineSide.Right && sliceSide6 == LineSide.Left) || (sliceSide5 == LineSide.On && sliceSide6 == LineSide.Left) || (sliceSide5 == LineSide.Right && sliceSide6 == LineSide.On) || (sliceSide5 == LineSide.Right && sliceSide6 == LineSide.Right) || (sliceSide5 == LineSide.Left && sliceSide6 == LineSide.Left))
				{
					linkedPolygonPoint4 = linkedPolygonPoint5;
				}
				else
				{
					i++;
				}
			}
			if (linkedPolygonPoint2 != null && linkedPolygonPoint4 != null)
			{
				LinkPoints(linkedPolygonPoint2, linkedPolygonPoint4);
				VerifyPolygons();
				if (linkedPolygonPoint2.Prev.Prev.SliceSide == LineSide.Left)
				{
					linkedPolygonPoint = linkedPolygonPoint2.Prev;
				}
				else if (linkedPolygonPoint4.Next.SliceSide == LineSide.Right)
				{
					linkedPolygonPoint = linkedPolygonPoint4;
				}
			}
		}
	}

	private static void CollectPolys()
	{
		s_ConcaveSlicePolygonResults.Clear();
		foreach (LinkedPolygonPoint s_ConcavePolygonPoint in s_ConcavePolygonPoints)
		{
			if (!s_ConcavePolygonPoint.Visited)
			{
				Polygon polygon = new Polygon();
				LinkedPolygonPoint linkedPolygonPoint = s_ConcavePolygonPoint;
				do
				{
					linkedPolygonPoint.Visited = true;
					polygon.points.Add(linkedPolygonPoint.Position);
					linkedPolygonPoint = linkedPolygonPoint.Next;
				}
				while (linkedPolygonPoint != s_ConcavePolygonPoint);
				s_ConcaveSlicePolygonResults.Add(polygon);
			}
		}
	}

	private static void VerifyPolygons()
	{
		foreach (LinkedPolygonPoint s_ConcavePolygonPoint in s_ConcavePolygonPoints)
		{
			LinkedPolygonPoint linkedPolygonPoint = s_ConcavePolygonPoint;
			int num = 0;
			do
			{
				if (num >= s_ConcavePolygonPoints.Count)
				{
					UnityEngine.Debug.LogError("Invalid polygon cycle detected");
					break;
				}
				linkedPolygonPoint = linkedPolygonPoint.Next;
				num++;
			}
			while (linkedPolygonPoint != s_ConcavePolygonPoint);
		}
	}

	private static void LinkPoints(LinkedPolygonPoint srcEdge, LinkedPolygonPoint dstEdge)
	{
		s_ConcavePolygonPoints.Add(new LinkedPolygonPoint(srcEdge.Position, srcEdge.SliceSide));
		LinkedPolygonPoint linkedPolygonPoint = s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1];
		s_ConcavePolygonPoints.Add(new LinkedPolygonPoint(dstEdge.Position, dstEdge.SliceSide));
		LinkedPolygonPoint linkedPolygonPoint2 = s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1];
		linkedPolygonPoint.Next = dstEdge;
		linkedPolygonPoint.Prev = srcEdge.Prev;
		linkedPolygonPoint2.Next = srcEdge;
		linkedPolygonPoint2.Prev = dstEdge.Prev;
		srcEdge.Prev.Next = linkedPolygonPoint;
		srcEdge.Prev = linkedPolygonPoint2;
		dstEdge.Prev.Next = linkedPolygonPoint2;
		dstEdge.Prev = linkedPolygonPoint;
	}

	private static bool IsPointInsidePolygon(Vector2 pos, ref List<Vector2> polygonPoints)
	{
		int num = 0;
		int count = polygonPoints.Count;
		for (int i = 0; i < count; i++)
		{
			int num2 = i + 1;
			if (num2 >= count)
			{
				num2 = 0;
			}
			Vector2 vector = polygonPoints[i];
			Vector2 vector2 = polygonPoints[num2];
			if (vector.y <= pos.y)
			{
				if (vector2.y > pos.y)
				{
					float num3 = (vector2.x - vector.x) * (pos.y - vector.y) - (pos.x - vector.x) * (vector2.y - vector.y);
					if (num3 > 0f)
					{
						num++;
					}
				}
			}
			else if (vector2.y <= pos.y)
			{
				float num4 = (vector2.x - vector.x) * (pos.y - vector.y) - (pos.x - vector.x) * (vector2.y - vector.y);
				if (num4 < 0f)
				{
					num--;
				}
			}
		}
		return num != 0;
	}
}
