using UnityEngine;

namespace Destructible2D
{
	[AddComponentMenu("Destructible 2D/D2D Auto Collider")]
	public class D2dAutoCollider : D2dCollider
	{
		[SerializeField]
		private PolygonCollider2D polygonCollider2D;

		public override void UpdateColliderSettings()
		{
			if (polygonCollider2D != null)
			{
				polygonCollider2D.isTrigger = IsTrigger;
				polygonCollider2D.sharedMaterial = Material;
			}
		}

		protected override void OnAlphaDataReplaced()
		{
			base.OnAlphaDataReplaced();
			Rebuild();
		}

		protected override void OnAlphaDataModified(D2dRect rect)
		{
			base.OnAlphaDataModified(rect);
			Rebuild();
		}

		protected override void OnAlphaDataSubset(D2dRect rect)
		{
			base.OnAlphaDataSubset(rect);
			Rebuild();
		}

		protected override void OnStartSplit()
		{
			base.OnStartSplit();
			polygonCollider2D = null;
		}

		private void Destroy()
		{
			polygonCollider2D = D2dHelper.Destroy(polygonCollider2D);
		}

		private void Rebuild()
		{
			Destroy();
			Texture2D alphaTex = destructible.AlphaTex;
			if (alphaTex != null)
			{
				SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
				Sprite sprite = Sprite.Create(alphaTex, new Rect(0f, 0f, alphaTex.width, alphaTex.height), Vector2.zero, 1f, 0u, SpriteMeshType.FullRect);
				if (spriteRenderer == null)
				{
					spriteRenderer = child.AddComponent<SpriteRenderer>();
				}
				spriteRenderer.sprite = sprite;
				polygonCollider2D = child.AddComponent<PolygonCollider2D>();
				polygonCollider2D.enabled = !IsDefaultPolygonCollider2D(polygonCollider2D);
				polygonCollider2D.isTrigger = IsTrigger;
				polygonCollider2D.sharedMaterial = Material;
				D2dHelper.Destroy(sprite);
				D2dHelper.Destroy(spriteRenderer);
			}
		}

		private static bool IsDefaultPolygonCollider2D(PolygonCollider2D polygonCollider2D)
		{
			if (polygonCollider2D == null)
			{
				return false;
			}
			if (polygonCollider2D.GetTotalPointCount() != 5)
			{
				return false;
			}
			Vector2[] points = polygonCollider2D.points;
			float a = Vector2.Distance(points[0], points[4]);
			for (int i = 0; i < 4; i++)
			{
				float b = Vector2.Distance(points[i], points[i + 1]);
				if (!Mathf.Approximately(a, b))
				{
					return false;
				}
			}
			Vector2 b2 = (points[0] + points[1] + points[2] + points[3] + points[4]) * 0.2f;
			float a2 = Vector2.Distance(points[0], b2);
			for (int j = 1; j < 5; j++)
			{
				float b3 = Vector2.Distance(points[j], b2);
				if (!Mathf.Approximately(a2, b3))
				{
					return false;
				}
			}
			return true;
		}
	}
}
