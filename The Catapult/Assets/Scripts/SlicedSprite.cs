using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class SlicedSprite : MonoBehaviour
{
	private MeshRenderer m_MeshRenderer;

	private MeshFilter m_MeshFilter;

	private Transform m_Transform;

	private Vector2 m_MinCoords;

	private Vector2 m_MaxCoords;

	private Vector2 m_Centroid;

	private Vector2 m_UVOffset;

	private Bounds m_SpriteBounds;

	private int m_ParentInstanceID;

	private int m_CutsSinceParentObject;

	private bool m_Rotated;

	private bool m_VFlipped;

	private bool m_HFlipped;

	public MeshRenderer MeshRenderer => m_MeshRenderer;

	public Vector2 MinCoords => m_MinCoords;

	public Vector2 MaxCoords => m_MaxCoords;

	public Bounds SpriteBounds => m_SpriteBounds;

	public int ParentInstanceID => m_ParentInstanceID;

	public int CutsSinceParentObject => m_CutsSinceParentObject;

	public bool Rotated => m_Rotated;

	public bool HFlipped => m_HFlipped;

	public bool VFlipped => m_VFlipped;

	private void Awake()
	{
		m_Transform = base.transform;
		m_MeshFilter = GetComponent<MeshFilter>();
		m_MeshRenderer = GetComponent<MeshRenderer>();
		m_ParentInstanceID = base.gameObject.GetInstanceID();
		m_MinCoords = new Vector2(0f, 0f);
		m_MaxCoords = new Vector2(1f, 1f);
		if ((bool)m_MeshFilter.mesh)
		{
			m_SpriteBounds = m_MeshFilter.mesh.bounds;
		}
	}

	public void InitFromSlicedSprite(SlicedSprite slicedSprite, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, bool isConcave)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		slicedSprite.m_MeshRenderer.GetPropertyBlock(materialPropertyBlock);
		m_MeshRenderer.SetPropertyBlock(materialPropertyBlock);
		InitSprite(slicedSprite.gameObject, slicedSprite.MeshRenderer, ref polygon, ref polygonPoints, slicedSprite.MinCoords, slicedSprite.MaxCoords, slicedSprite.SpriteBounds, slicedSprite.m_MeshRenderer.sharedMaterial, slicedSprite.Rotated, slicedSprite.HFlipped, slicedSprite.VFlipped, slicedSprite.m_Centroid, slicedSprite.m_UVOffset, isConcave);
		m_ParentInstanceID = slicedSprite.GetInstanceID();
		m_CutsSinceParentObject = slicedSprite.CutsSinceParentObject + 1;
	}

	public void InitFromUnitySprite(SpriteRenderer unitySprite, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, bool isConcave)
	{
		Sprite sprite = unitySprite.sprite;
		Bounds bounds = unitySprite.bounds;
		Vector2 vector = unitySprite.transform.position;
		Vector2 vector2 = bounds.min;
		Vector2 vector3 = bounds.max;
		Vector2 vector4 = bounds.size;
		Vector2 zero = Vector2.zero;
		Vector3 lossyScale = unitySprite.transform.lossyScale;
		if (Mathf.Sign(lossyScale.x) < 0f)
		{
			zero.x = vector3.x - vector.x;
		}
		else
		{
			zero.x = vector.x - vector2.x;
		}
		if (Mathf.Sign(lossyScale.y) < 0f)
		{
			zero.y = vector3.y - vector.y;
		}
		else
		{
			zero.y = vector.y - vector2.y;
		}
		Vector2 a = new Vector2(zero.x / vector4.x, zero.y / vector4.y);
		a -= new Vector2(0.5f, 0.5f);
		Texture2D texture = sprite.texture;
		Vector2 vector5 = new Vector2(texture.width, texture.height);
		Material sharedMaterial = unitySprite.sharedMaterial;
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetTexture("_MainTex", texture);
		m_MeshRenderer.SetPropertyBlock(materialPropertyBlock);
		Rect textureRect = sprite.textureRect;
		Vector2 v = new Vector2(textureRect.xMin / vector5.x, textureRect.yMin / vector5.y);
		InitSprite(maxCoords: new Vector2(textureRect.xMax / vector5.x, textureRect.yMax / vector5.y), parentObject: unitySprite.gameObject, parentRenderer: unitySprite.GetComponent<Renderer>(), polygon: ref polygon, polygonPoints: ref polygonPoints, minCoords: v, spriteBounds: unitySprite.sprite.bounds, material: sharedMaterial, rotated: false, hFlipped: false, vFlipped: false, parentCentroid: Vector2.zero, uvOffset: a, isConcave: isConcave);
		m_ParentInstanceID = unitySprite.gameObject.GetInstanceID();
	}

	private void InitSprite(GameObject parentObject, Renderer parentRenderer, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, Vector3 minCoords, Vector3 maxCoords, Bounds spriteBounds, Material material, bool rotated, bool hFlipped, bool vFlipped, Vector2 parentCentroid, Vector2 uvOffset, bool isConcave)
	{
		m_MinCoords = minCoords;
		m_MaxCoords = maxCoords;
		m_SpriteBounds = spriteBounds;
		m_VFlipped = vFlipped;
		m_HFlipped = hFlipped;
		m_Rotated = rotated;
		m_SpriteBounds = spriteBounds;
		m_UVOffset = uvOffset;
		base.gameObject.tag = parentObject.tag;
		base.gameObject.layer = parentObject.layer;
		Mesh mesh = new Mesh();
		mesh.name = "SlicedSpriteMesh";
		m_MeshFilter.mesh = mesh;
		int num = polygonPoints.Length;
		Vector3[] array = new Vector3[num];
		Color[] array2 = new Color[num];
		Vector2[] array3 = new Vector2[num];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = polygonPoints[i];
			array2[i] = Color.white;
		}
		Vector2 vector = maxCoords - minCoords;
		Vector3 size = spriteBounds.size;
		Vector2 vector2 = new Vector2(1f / size.x, 1f / size.y);
		for (int j = 0; j < num; j++)
		{
			Vector2 vector3 = polygonPoints[j] + parentCentroid;
			float num2 = 0.5f + (vector3.x * vector2.x + uvOffset.x);
			float num3 = 0.5f + (vector3.y * vector2.y + uvOffset.y);
			if (hFlipped)
			{
				num2 = 1f - num2;
			}
			if (vFlipped)
			{
				num3 = 1f - num3;
			}
			Vector2 vector4 = default(Vector2);
			if (rotated)
			{
				vector4.y = maxCoords.y - vector.y * (1f - num2);
				vector4.x = minCoords.x + vector.x * num3;
			}
			else
			{
				vector4.x = minCoords.x + vector.x * num2;
				vector4.y = minCoords.y + vector.y * num3;
			}
			array3[j] = vector4;
		}
		int[] array4;
		if (isConcave)
		{
			List<Vector2> points = new List<Vector2>(polygonPoints);
			array4 = SpriteSlicer2D.Triangulate(ref points);
		}
		else
		{
			int num4 = 0;
			array4 = new int[num * 3];
			for (int k = 1; k < num - 1; k++)
			{
				array4[num4++] = 0;
				array4[num4++] = k + 1;
				array4[num4++] = k;
			}
		}
		mesh.Clear();
		mesh.vertices = array;
		mesh.uv = array3;
		mesh.triangles = array4;
		mesh.colors = array2;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		Vector2 vector5 = Vector3.zero;
		if (SpriteSlicer2D.s_CentreChildSprites)
		{
			vector5 = mesh.bounds.center;
			for (int l = 0; l < num; l++)
			{
				array[l] -= (Vector3)vector5;
			}
			for (int m = 0; m < num; m++)
			{
				polygonPoints[m] -= vector5;
			}
			m_Centroid = vector5 + parentCentroid;
			polygon.SetPath(0, polygonPoints);
			mesh.vertices = array;
			mesh.RecalculateBounds();
		}
		Transform transform = parentObject.transform;
		m_Transform.parent = transform.parent;
		m_Transform.position = transform.position + transform.rotation * vector5;
		m_Transform.rotation = transform.rotation;
		m_Transform.localScale = transform.localScale;
		m_MeshRenderer.material = material;
		m_MeshRenderer.sortingLayerID = parentRenderer.sortingLayerID;
		m_MeshRenderer.sortingOrder = parentRenderer.sortingOrder;
	}
}
