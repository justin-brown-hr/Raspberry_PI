using System;
using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("Destructible 2D/D2D Destructible")]
	public class D2dDestructible : MonoBehaviour
	{
		public enum SplitType
		{
			None,
			Whole,
			Local
		}

		public static List<D2dDestructible> AllDestructibles = new List<D2dDestructible>();

		private static int[] indices = new int[6]
		{
			0,
			2,
			1,
			3,
			1,
			2
		};

		private static Vector3[] positions = new Vector3[4];

		private static Vector2[] coords = new Vector2[4];

		private static Color[] colors = new Color[4];

		private static MaterialPropertyBlock propertyBlock;

		private static List<D2dDestructible> clones = new List<D2dDestructible>();

		public D2dEvent OnAlphaDataReplaced;

		public D2dD2dRectEvent OnAlphaDataModified;

		public D2dD2dRectEvent OnAlphaDataSubset;

		public D2dFloatFloatEvent OnDamageChanged;

		public D2dEvent OnStartSplit;

		public D2dDestructibleListEvent OnEndSplit;

		[NonSerialized]
		public bool IsSplitting;

		[NonSerialized]
		public bool IsOnStartSplit;

		[Tooltip("The main texture applied to the Destructible")]
		public Texture MainTex;

		[Tooltip("If you set this then areas with a brighter alpha will be harder to destroy")]
		public Texture2D DensityTex;

		[Tooltip("If you set this then this destructible can be healed up to the maximum opacity in this heal texture")]
		public Texture2D HealTex;

		[Tooltip("If you enable this then this Destructible will be unable to take damage")]
		public bool Indestructible;

		[Tooltip("If you enable this then the OriginalAlphaCount variable will automatically be calculated")]
		public bool RecordAlphaCount = true;

		[Tooltip("If you enable this then the sharpness will automatically get modified by how detailed the Alpha Tex is relative to the Main Tex")]
		public bool AutoSharpen = true;

		[Tooltip("If you enable this then the destructible will automatically split pixel islands (areas in the Alpha Tex that aren't connected by solid pixels)")]
		public SplitType AutoSplit;

		[Tooltip("The minimum amount of pixels required in an island for it to be split")]
		public int MinSplitPixels = 5;

		[Tooltip("The amount of pixels around a stamp that will be checked for local splits")]
		public int LocalSplitExpand = 20;

		[Tooltip("Should the split islands get an extra border of pixels added to keep any original antialiasing?")]
		public bool FeatherSplit = true;

		[Tooltip("Allows you to set how sharp the edges of the Destructible image are")]
		[Range(1f, 5f)]
		public float Sharpness = 1f;

		[SerializeField]
		[Tooltip("The color tint of this destructible object")]
		private Color color = Color.white;

		[SerializeField]
		[Tooltip("The amount of numerical damage this destructible has taken (this value is separate from any visual damage)")]
		private float damage;

		[SerializeField]
		private Rect textureRect;

		[SerializeField]
		private Rect originalRect;

		[SerializeField]
		private Rect alphaRect;

		[SerializeField]
		private int alphaWidth;

		[SerializeField]
		private int alphaHeight;

		[SerializeField]
		private int alphaCount = -1;

		[SerializeField]
		private int originalAlphaCount = -1;

		[SerializeField]
		private byte[] alphaData;

		[NonSerialized]
		private Texture2D alphaTex;

		[NonSerialized]
		private D2dRect alphaDirty;

		[NonSerialized]
		private D2dRect alphaModified;

		[SerializeField]
		private Vector2 alphaScale;

		[SerializeField]
		private Vector2 alphaOffset;

		[NonSerialized]
		private float alphaRatio;

		[NonSerialized]
		private Mesh mesh;

		[NonSerialized]
		private MeshRenderer meshRenderer;

		[NonSerialized]
		private MeshFilter meshFilter;

		private static List<D2dSplitGroup> splitGroups = new List<D2dSplitGroup>();

		private static readonly Vector3[] quadNormals = new Vector3[4]
		{
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f)
		};

		private static readonly Vector4[] quadTangents = new Vector4[4]
		{
			new Vector4(1f, 0f, 0f, -1f),
			new Vector4(1f, 0f, 0f, -1f),
			new Vector4(1f, 0f, 0f, -1f),
			new Vector4(1f, 0f, 0f, -1f)
		};

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if (color != value)
				{
					color = value;
					if (mesh != null)
					{
						UpdateMeshColors();
					}
				}
			}
		}

		public float Damage
		{
			get
			{
				return damage;
			}
			set
			{
				if (damage != value && !Indestructible)
				{
					float arg = damage;
					damage = value;
					if (OnDamageChanged != null)
					{
						OnDamageChanged.Invoke(arg, value);
					}
				}
			}
		}

		public Texture2D AlphaTex
		{
			get
			{
				DeserializeAlphaTex();
				return alphaTex;
			}
		}

		public int AlphaWidth => alphaWidth;

		public int AlphaHeight => alphaHeight;

		public int AlphaCount
		{
			get
			{
				if (alphaCount == -1)
				{
					alphaCount = 0;
					int num = alphaWidth * alphaHeight;
					for (int i = 0; i < num; i++)
					{
						if (alphaData[i] > 127)
						{
							alphaCount++;
						}
					}
				}
				return alphaCount;
			}
		}

		public int OriginalAlphaCount
		{
			get
			{
				if (originalAlphaCount == -1)
				{
					originalAlphaCount = AlphaCount;
				}
				return originalAlphaCount;
			}
			set
			{
				originalAlphaCount = value;
			}
		}

		public float RemainingAlpha => D2dHelper.Divide(AlphaCount, OriginalAlphaCount);

		public byte[] AlphaData => alphaData;

		public bool AlphaIsValid => D2dHelper.AlphaIsValid(alphaData, alphaWidth, alphaHeight);

		public Rect AlphaRect => alphaRect;

		public Rect TextureRect => textureRect;

		public Rect OriginalRect => originalRect;

		public Matrix4x4 AlphaToWorldMatrix
		{
			get
			{
				Matrix4x4 rhs = D2dHelper.ScalingMatrix(alphaRect.width, alphaRect.height, 1f);
				Matrix4x4 rhs2 = D2dHelper.TranslationMatrix(alphaRect.x, alphaRect.y, 0f);
				return base.transform.localToWorldMatrix * rhs2 * rhs;
			}
		}

		public Matrix4x4 WorldToAlphaMatrix
		{
			get
			{
				Matrix4x4 identity = Matrix4x4.identity;
				Matrix4x4 identity2 = Matrix4x4.identity;
				identity.m00 = D2dHelper.Reciprocal(alphaRect.width);
				identity.m11 = D2dHelper.Reciprocal(alphaRect.height);
				identity2.m03 = 0f - alphaRect.x;
				identity2.m13 = 0f - alphaRect.y;
				return identity * identity2 * base.transform.worldToLocalMatrix;
			}
		}

		public D2dRect OriginalRectRelative
		{
			get
			{
				float num = D2dHelper.Divide(originalRect.width, alphaRect.width) * (float)alphaWidth;
				float num2 = D2dHelper.Divide(originalRect.height, alphaRect.height) * (float)alphaHeight;
				int minX = Mathf.CeilToInt((originalRect.x - alphaRect.x) * num);
				int minY = Mathf.CeilToInt((originalRect.y - alphaRect.y) * num2);
				int sizeX = Mathf.FloorToInt(num);
				int sizeY = Mathf.FloorToInt(num2);
				return D2dRect.CreateFromMinSize(minX, minY, sizeX, sizeY);
			}
		}

		public void SetIndestructible(bool newIndestructible)
		{
			Indestructible = newIndestructible;
		}

		public void Clear()
		{
			ClearAlpha();
		}

		public void ClearAlpha()
		{
			alphaData = null;
			alphaWidth = 0;
			alphaHeight = 0;
			alphaCount = 0;
			alphaTex = D2dHelper.Destroy(alphaTex);
		}

		public float SampleAlpha(Vector3 worldPosition)
		{
			Vector2 uv = WorldToAlphaMatrix.MultiplyPoint(worldPosition);
			if (D2dHelper.IsValidUV(uv))
			{
				int num = Mathf.FloorToInt(uv.x * (float)alphaWidth);
				int num2 = Mathf.FloorToInt(uv.y * (float)alphaHeight);
				return SampleAlpha(num + num2 * alphaWidth);
			}
			return 0f;
		}

		public static D2dHit RaycastAlphaFirst(Vector3 startPosition, Vector3 endPosition)
		{
			float num = float.PositiveInfinity;
			D2dHit result = null;
			for (int num2 = AllDestructibles.Count - 1; num2 >= 0; num2--)
			{
				D2dDestructible d2dDestructible = AllDestructibles[num2];
				D2dHit d2dHit = d2dDestructible.RaycastAlpha(startPosition, endPosition);
				if (d2dHit != null && d2dHit.Distance < num)
				{
					num = d2dHit.Distance;
					result = d2dHit;
				}
			}
			return result;
		}

		public D2dHit RaycastAlpha(Vector3 positionStart, Vector3 positionEnd)
		{
			Vector3 vector = WorldToAlphaMatrix.MultiplyPoint(positionStart);
			Vector3 vector2 = WorldToAlphaMatrix.MultiplyPoint(positionEnd);
			D2dVector2 start = default(D2dVector2);
			D2dVector2 end = default(D2dVector2);
			start.X = Mathf.RoundToInt(vector.x * (float)alphaWidth);
			start.Y = Mathf.RoundToInt(vector.y * (float)alphaHeight);
			end.X = Mathf.RoundToInt(vector2.x * (float)alphaWidth);
			end.Y = Mathf.RoundToInt(vector2.y * (float)alphaHeight);
			return RaycastAlpha(start, end);
		}

		public D2dHit RaycastAlpha(D2dVector2 start, D2dVector2 end)
		{
			Bounds bounds = default(Bounds);
			float distance = 0f;
			Vector3 normalized = (end - start).V.normalized;
			bounds.SetMinMax(new Vector3(0f, 0f, -1f), new Vector3(alphaWidth, alphaHeight, 1f));
			if (start != end && bounds.IntersectRay(new Ray(start.V, normalized), out distance))
			{
				if (distance > 0f)
				{
					start.X = Mathf.RoundToInt((float)start.X + normalized.x * distance);
					start.Y = Mathf.RoundToInt((float)start.Y + normalized.y * distance);
				}
				if (start != end && bounds.IntersectRay(new Ray(end.V, -normalized), out distance))
				{
					if (distance > 0f)
					{
						end.X = Mathf.RoundToInt((float)end.X - normalized.x * distance);
						end.Y = Mathf.RoundToInt((float)end.Y - normalized.y * distance);
					}
					int num = end.X - start.X;
					int num2 = end.Y - start.Y;
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					int num6 = 0;
					if (num < 0)
					{
						num3 = -1;
					}
					else if (num > 0)
					{
						num3 = 1;
					}
					if (num2 < 0)
					{
						num4 = -1;
					}
					else if (num2 > 0)
					{
						num4 = 1;
					}
					if (num < 0)
					{
						num5 = -1;
					}
					else if (num > 0)
					{
						num5 = 1;
					}
					int num7 = Mathf.Abs(num);
					int num8 = Mathf.Abs(num2);
					if (num7 <= num8)
					{
						num7 = Mathf.Abs(num2);
						num8 = Mathf.Abs(num);
						num5 = 0;
						if (num2 < 0)
						{
							num6 = -1;
						}
						else if (num2 > 0)
						{
							num6 = 1;
						}
					}
					int num9 = start.X;
					int num10 = start.Y;
					int num11 = num7 >> 1;
					for (int i = 0; i <= num7; i++)
					{
						if (SampleAlpha(num9, num10) >= 0.5f)
						{
							D2dHit d2dHit = new D2dHit();
							d2dHit.Pixel.X = num9;
							d2dHit.Pixel.Y = num10;
							d2dHit.Point.x = (float)num9 / (float)alphaWidth;
							d2dHit.Point.y = (float)num10 / (float)alphaHeight;
							d2dHit.Position = AlphaToWorldMatrix.MultiplyPoint(d2dHit.Point);
							d2dHit.Distance = Vector2.Distance(new Vector2(num9, num10), new Vector2(start.X, start.Y));
							return d2dHit;
						}
						num11 += num8;
						if (num11 >= num7)
						{
							num11 -= num7;
							num9 += num3;
							num10 += num4;
						}
						else
						{
							num9 += num5;
							num10 += num6;
						}
					}
				}
			}
			return null;
		}

		public static Matrix4x4 CalculateSliceMatrix(Vector2 startPos, Vector2 endPos, float thickness)
		{
			Vector2 position = (startPos + endPos) / 2f;
			Vector2 xy = endPos - startPos;
			Vector2 size = new Vector2(thickness, xy.magnitude);
			float angle = D2dHelper.Atan2(xy) * -57.29578f;
			return CalculateStampMatrix(position, size, angle);
		}

		public static Matrix4x4 CalculateStampMatrix(Vector2 position, Vector2 size, float angle)
		{
			Matrix4x4 lhs = D2dHelper.TranslationMatrix(position.x, position.y, 0f);
			Matrix4x4 rhs = D2dHelper.RotationMatrix(Quaternion.Euler(0f, 0f, angle));
			Matrix4x4 rhs2 = D2dHelper.ScalingMatrix(size.x, size.y, 1f);
			Matrix4x4 rhs3 = D2dHelper.TranslationMatrix(-0.5f, -0.5f, 0f);
			return lhs * rhs * rhs2 * rhs3;
		}

		public static void SliceAll(Vector2 startPos, Vector2 endPos, float thickness, Texture2D stampTex, float hardness, int layerMask = -1)
		{
			StampAll(CalculateSliceMatrix(startPos, endPos, thickness), stampTex, hardness, layerMask);
		}

		public static void StampAll(Vector2 position, Vector2 size, float angle, Texture2D stampTex, float hardness, int layerMask = -1)
		{
			StampAll(CalculateStampMatrix(position, size, angle), stampTex, hardness, layerMask);
		}

		public static void StampAll(Matrix4x4 matrix, Texture2D stampTex, float hardness, int layerMask = -1)
		{
			for (int num = AllDestructibles.Count - 1; num >= 0; num--)
			{
				D2dDestructible d2dDestructible = AllDestructibles[num];
				if (d2dDestructible != null && !d2dDestructible.Indestructible)
				{
					int num2 = 1 << d2dDestructible.gameObject.layer;
					if ((layerMask & num2) != 0)
					{
						d2dDestructible.BeginAlphaModifications();
						d2dDestructible.Stamp(matrix, stampTex, hardness);
						d2dDestructible.EndAlphaModifications();
					}
				}
			}
		}

		public void Slice(Vector2 startPos, Vector2 endPos, float thickness, Texture2D stampTex, float hardness)
		{
			Stamp(CalculateSliceMatrix(startPos, endPos, thickness), stampTex, hardness);
		}

		public void Stamp(Vector2 position, Vector2 size, float angle, Texture2D stampTex, float hardness)
		{
			Stamp(CalculateStampMatrix(position, size, angle), stampTex, hardness);
		}

		public void AddDamage(int amount)
		{
			Damage += amount;
		}

		public void RemoveDamage(int amount)
		{
			Damage -= amount;
		}

		public void Stamp(Matrix4x4 stampMatrix, Texture2D stampTex, float hardness)
		{
			if (!AlphaIsValid)
			{
				throw new InvalidOperationException("Invalid alpha");
			}
			if (stampTex == null || hardness == 0f || (hardness < 0f && HealTex == null))
			{
				return;
			}
			Matrix4x4 matrix = WorldToAlphaMatrix * stampMatrix;
			D2dRect rect = default(D2dRect);
			if (!D2dHelper.CalculateRect(matrix, ref rect, alphaWidth, alphaHeight))
			{
				return;
			}
			rect.MinX = Mathf.Clamp(rect.MinX, 0, alphaWidth);
			rect.MaxX = Mathf.Clamp(rect.MaxX, 0, alphaWidth);
			rect.MinY = Mathf.Clamp(rect.MinY, 0, alphaHeight);
			rect.MaxY = Mathf.Clamp(rect.MaxY, 0, alphaHeight);
			Matrix4x4 inverse = matrix.inverse;
			float num = D2dHelper.Reciprocal(alphaWidth);
			float num2 = D2dHelper.Reciprocal(alphaHeight);
			float num3 = num * 0.5f;
			float num4 = num2 * 0.5f;
			if (hardness < 0f)
			{
				if (HealTex != null)
				{
					float num5 = (alphaRect.x - originalRect.x) / originalRect.width;
					float num6 = (alphaRect.y - originalRect.y) / originalRect.height;
					float num7 = alphaRect.width / originalRect.width;
					float num8 = alphaRect.height / originalRect.height;
					for (int i = rect.MinY; i < rect.MaxY; i++)
					{
						float num9 = (float)i * num2 + num4;
						for (int j = rect.MinX; j < rect.MaxX; j++)
						{
							float num10 = (float)j * num + num3;
							Vector3 v = inverse.MultiplyPoint(new Vector3(num10, num9, 0f));
							if (D2dHelper.IsValidUV(v))
							{
								float u = num10 * num7 + num5;
								float v2 = num9 * num8 + num6;
								if (D2dHelper.IsValidUV(u, v2))
								{
									int i2 = j + i * alphaWidth;
									float num11 = SampleAlpha(i2);
									float num12 = SampleStamp(stampTex, v) * hardness;
									float b = SampleHeal(u, v2);
									WriteAlpha(i2, Mathf.Min(num11 - num12, b));
								}
							}
						}
					}
				}
				else
				{
					for (int k = rect.MinY; k < rect.MaxY; k++)
					{
						float y = (float)k * num2 + num4;
						for (int l = rect.MinX; l < rect.MaxX; l++)
						{
							float x = (float)l * num + num3;
							Vector3 v3 = inverse.MultiplyPoint(new Vector3(x, y, 0f));
							if (D2dHelper.IsValidUV(v3))
							{
								int i3 = l + k * alphaWidth;
								float num13 = SampleAlpha(i3);
								float num14 = SampleStamp(stampTex, v3) * hardness;
								WriteAlpha(i3, num13 - num14);
							}
						}
					}
				}
			}
			else if (DensityTex != null)
			{
				float num15 = (alphaRect.x - originalRect.x) / originalRect.width;
				float num16 = (alphaRect.y - originalRect.y) / originalRect.height;
				float num17 = alphaRect.width / originalRect.width;
				float num18 = alphaRect.height / originalRect.height;
				for (int m = rect.MinY; m < rect.MaxY; m++)
				{
					float num19 = (float)m * num2 + num4;
					for (int n = rect.MinX; n < rect.MaxX; n++)
					{
						float num20 = (float)n * num + num3;
						Vector3 v4 = inverse.MultiplyPoint(new Vector3(num20, num19, 0f));
						if (!D2dHelper.IsValidUV(v4))
						{
							continue;
						}
						float u2 = num20 * num17 + num15;
						float v5 = num19 * num18 + num16;
						if (D2dHelper.IsValidUV(u2, v5))
						{
							int i4 = n + m * alphaWidth;
							float num21 = SampleAlpha(i4);
							float num22 = SampleStamp(stampTex, v4) * hardness;
							float num23 = SampleDensity(u2, v5);
							if (num22 > num23)
							{
								WriteAlpha(i4, num21 - (num22 - num23));
							}
						}
					}
				}
			}
			else
			{
				for (int num24 = rect.MinY; num24 < rect.MaxY; num24++)
				{
					float y2 = (float)num24 * num2 + num4;
					for (int num25 = rect.MinX; num25 < rect.MaxX; num25++)
					{
						float x2 = (float)num25 * num + num3;
						Vector3 v6 = inverse.MultiplyPoint(new Vector3(x2, y2, 0f));
						if (D2dHelper.IsValidUV(v6))
						{
							int i5 = num25 + num24 * alphaWidth;
							float num26 = SampleAlpha(i5);
							float num27 = SampleStamp(stampTex, v6) * hardness;
							WriteAlpha(i5, num26 - num27);
						}
					}
				}
			}
			alphaModified.Add(rect);
		}

		public void WriteAlpha(int x, int y, byte alpha)
		{
			if (x >= 0 && y >= 0 && x < alphaWidth && y < alphaHeight)
			{
				alphaModified.Add(x, y);
				alphaData[x + y * alphaWidth] = alpha;
			}
		}

		public void BeginAlphaModifications()
		{
			alphaModified.Clear();
		}

		public void EndAlphaModifications()
		{
			if (!alphaModified.IsSet)
			{
				return;
			}
			alphaDirty.Add(alphaModified);
			alphaCount = -1;
			switch (AutoSplit)
			{
			case SplitType.Whole:
				if (!TrySplit())
				{
					break;
				}
				return;
			case SplitType.Local:
				if (!TrySplit())
				{
					break;
				}
				return;
			}
			if (OnAlphaDataModified != null)
			{
				OnAlphaDataModified.Invoke(alphaModified);
			}
		}

		public float SampleAlpha(int x, int y)
		{
			if (x >= 0 && y >= 0 && x < alphaWidth && y < alphaHeight)
			{
				return SampleAlpha(x + y * alphaWidth);
			}
			return 0f;
		}

		private float SampleAlpha(int i)
		{
			return D2dHelper.ConvertAlpha(alphaData[i]);
		}

		private void WriteAlpha(int i, float alpha)
		{
			alphaData[i] = D2dHelper.ConvertAlpha(Mathf.Clamp01(alpha));
		}

		private float SampleStamp(Texture2D texture2D, Vector2 uv)
		{
			int x = (int)(uv.x * (float)texture2D.width);
			int y = (int)(uv.y * (float)texture2D.height);
			Color pixel = texture2D.GetPixel(x, y);
			return pixel.a;
		}

		private float SampleDensity(float u, float v)
		{
			int x = (int)(u * (float)DensityTex.width);
			int y = (int)(v * (float)DensityTex.height);
			Color pixel = DensityTex.GetPixel(x, y);
			return pixel.a;
		}

		private float SampleHeal(float u, float v)
		{
			int x = (int)(u * (float)HealTex.width);
			int y = (int)(v * (float)HealTex.height);
			Color pixel = HealTex.GetPixel(x, y);
			return pixel.a;
		}

		public void ReplaceWith(Sprite sprite)
		{
			if (sprite != null)
			{
				Texture2D texture = sprite.texture;
				if (texture != null)
				{
					float num = 1f / sprite.pixelsPerUnit;
					ref Rect reference = ref originalRect;
					Vector2 pivot = sprite.pivot;
					float num2 = 0f - pivot.x;
					Vector2 textureRectOffset = sprite.textureRectOffset;
					reference.x = (num2 + Mathf.Ceil(textureRectOffset.x)) * num;
					ref Rect reference2 = ref originalRect;
					Vector2 pivot2 = sprite.pivot;
					float num3 = 0f - pivot2.y;
					Vector2 textureRectOffset2 = sprite.textureRectOffset;
					reference2.y = (num3 + Mathf.Ceil(textureRectOffset2.y)) * num;
					originalRect.width = Mathf.Floor(sprite.textureRect.width) * num;
					originalRect.height = Mathf.Floor(sprite.textureRect.height) * num;
					MainTex = texture;
					alphaRect = originalRect;
					ReplaceAlphaWith(sprite);
					UpdateMesh();
				}
			}
		}

		public void ReplaceWith(Texture2D texture2D)
		{
			if (texture2D != null)
			{
				originalRect.x = (float)texture2D.width * -0.5f;
				originalRect.y = (float)texture2D.height * -0.5f;
				originalRect.width = texture2D.width;
				originalRect.height = texture2D.height;
				MainTex = texture2D;
				alphaRect = originalRect;
				ReplaceAlphaWith(texture2D);
				UpdateMesh();
			}
		}

		public void ReplaceTextureWith(Texture2D texture2D)
		{
			if (texture2D != null)
			{
				MainTex = texture2D;
			}
		}

		public void ReplaceTextureWith(Sprite sprite)
		{
			if (sprite != null)
			{
				Texture2D texture = sprite.texture;
				if (texture != null)
				{
					float num = 1f / sprite.pixelsPerUnit;
					ref Rect reference = ref originalRect;
					Vector2 pivot = sprite.pivot;
					float num2 = 0f - pivot.x;
					Vector2 textureRectOffset = sprite.textureRectOffset;
					reference.x = (num2 + Mathf.Ceil(textureRectOffset.x)) * num;
					ref Rect reference2 = ref originalRect;
					Vector2 pivot2 = sprite.pivot;
					float num3 = 0f - pivot2.y;
					Vector2 textureRectOffset2 = sprite.textureRectOffset;
					reference2.y = (num3 + Mathf.Ceil(textureRectOffset2.y)) * num;
					originalRect.width = Mathf.Floor(sprite.textureRect.width) * num;
					originalRect.height = Mathf.Floor(sprite.textureRect.height) * num;
					MainTex = texture;
					Rect rect = sprite.textureRect;
					int num4 = Mathf.CeilToInt(rect.x);
					int num5 = Mathf.CeilToInt(rect.y);
					int num6 = Mathf.FloorToInt(rect.width);
					int num7 = Mathf.FloorToInt(rect.height);
					textureRect.x = D2dHelper.Divide(num4, texture.width);
					textureRect.y = D2dHelper.Divide(num5, texture.height);
					textureRect.width = D2dHelper.Divide(num6, texture.width);
					textureRect.height = D2dHelper.Divide(num7, texture.height);
					UpdateMesh();
				}
			}
		}

		public void ReplaceAlphaWith(D2dSnapshot snapshot)
		{
			if (snapshot != null)
			{
				alphaRect = snapshot.AlphaRect;
				ReplaceAlphaWith(snapshot.AlphaData, snapshot.AlphaWidth, snapshot.AlphaHeight);
			}
		}

		public void ReplaceAlphaWith(Sprite sprite)
		{
			if (sprite != null)
			{
				Texture2D texture = sprite.texture;
				if (texture != null)
				{
					Rect rect = sprite.textureRect;
					int x = Mathf.CeilToInt(rect.x);
					int y = Mathf.CeilToInt(rect.y);
					int width = Mathf.FloorToInt(rect.width);
					int height = Mathf.FloorToInt(rect.height);
					ReplaceAlphaWith(texture, x, y, width, height);
				}
			}
		}

		public void ReplaceAlphaWith(Texture2D texture2D)
		{
			if (texture2D != null)
			{
				ReplaceAlphaWith(texture2D, 0, 0, texture2D.width, texture2D.height);
			}
		}

		public void ReplaceAlphaWith(Texture2D texture2D, int x, int y, int width, int height, int newAlphaCount = -1)
		{
			if (D2dHelper.ExtractAlpha(texture2D, x, y, width, height))
			{
				textureRect.x = D2dHelper.Divide(x, texture2D.width);
				textureRect.y = D2dHelper.Divide(y, texture2D.height);
				textureRect.width = D2dHelper.Divide(width, texture2D.width);
				textureRect.height = D2dHelper.Divide(height, texture2D.height);
				ReplaceAlphaWith(D2dHelper.AlphaData, width, height, newAlphaCount);
			}
		}

		public void ReplaceAlphaWith(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight, int newAlphaCount = -1)
		{
			if (newAlphaData == null || newAlphaWidth <= 0 || newAlphaHeight <= 0)
			{
				return;
			}
			int num = newAlphaWidth * newAlphaHeight;
			if (newAlphaData.Length >= num)
			{
				FastCopyAlphaData(newAlphaData, newAlphaWidth, newAlphaHeight, newAlphaCount);
				if (RecordAlphaCount)
				{
					originalAlphaCount = AlphaCount;
				}
				else
				{
					originalAlphaCount = -1;
				}
				alphaDirty.Set(0, newAlphaWidth, 0, newAlphaHeight);
				if (OnAlphaDataReplaced != null)
				{
					OnAlphaDataReplaced.Invoke();
				}
			}
		}

		public void UpdateAlpha(byte[] newAlphaData, D2dRect newAlphaRect)
		{
			if (AlphaIsValid)
			{
				if (D2dHelper.AlphaIsValid(newAlphaData, newAlphaRect))
				{
					D2dRect a = new D2dRect(0, alphaWidth, 0, alphaHeight);
					D2dRect d2dRect = D2dRect.CalculateOverlap(a, newAlphaRect);
					if (!d2dRect.IsSet)
					{
						return;
					}
					for (int i = d2dRect.MinY; i < d2dRect.MaxY; i++)
					{
						for (int j = d2dRect.MinX; j < d2dRect.MaxX; j++)
						{
							int num = j - newAlphaRect.MinX;
							int num2 = i - newAlphaRect.MinY;
							byte alpha = newAlphaData[num2 * newAlphaRect.SizeX + num];
							WriteAlpha(j, i, alpha);
						}
					}
					alphaModified.Set(d2dRect.MinX, d2dRect.MaxX, d2dRect.MinY, d2dRect.MaxY);
					alphaDirty.Add(alphaModified);
					if (OnAlphaDataModified != null)
					{
						OnAlphaDataModified.Invoke(alphaModified);
					}
				}
				else
				{
					UnityEngine.Debug.LogError("New alpha data is null or invalid size " + newAlphaData.Length + " - " + newAlphaRect.SizeX + " - " + newAlphaRect.SizeY);
				}
			}
			else
			{
				UnityEngine.Debug.LogError("Alpha is not valid");
			}
		}

		private void FastCopyAlphaData(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight, int newAlphaCount = -1)
		{
			int num = newAlphaWidth * newAlphaHeight;
			if (alphaData == null || alphaData.Length != num)
			{
				alphaData = new byte[num];
			}
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				alphaData[num2] = newAlphaData[num2];
			}
			alphaWidth = newAlphaWidth;
			alphaHeight = newAlphaHeight;
			alphaCount = newAlphaCount;
			alphaRatio = 0f;
		}

		public D2dSnapshot GetSnapshot(D2dSnapshot snapshot = null)
		{
			if (AlphaIsValid)
			{
				if (snapshot == null)
				{
					snapshot = new D2dSnapshot();
				}
				int num = alphaWidth * alphaHeight;
				if (snapshot.AlphaData == null || snapshot.AlphaData.Length <= num)
				{
					snapshot.AlphaData = new byte[num];
				}
				for (int i = 0; i < num; i++)
				{
					snapshot.AlphaData[i] = alphaData[i];
				}
				snapshot.AlphaWidth = alphaWidth;
				snapshot.AlphaHeight = alphaHeight;
				snapshot.AlphaRect = alphaRect;
				return snapshot;
			}
			return null;
		}

		[ContextMenu("Optimize Alpha")]
		public void OptimizeAlpha()
		{
			TrimAlpha();
			BlurAlpha(replace: false);
			HalveAlpha(replace: true);
			TrimAlpha();
		}

		[ContextMenu("Halve Alpha")]
		public void HalveAlpha()
		{
			HalveAlpha(replace: true);
		}

		public void HalveAlpha(bool replace)
		{
			int num = alphaWidth;
			int num2 = alphaHeight;
			D2dHelper.Halve(ref alphaData, ref alphaWidth, ref alphaHeight);
			float num3 = D2dHelper.Reciprocal(num) * alphaRect.width;
			float num4 = D2dHelper.Reciprocal(num2) * alphaRect.height;
			alphaRect.xMin += num3 * 0.5f;
			alphaRect.xMax -= num3 * 0.5f;
			alphaRect.yMin += num4 * 0.5f;
			alphaRect.yMax -= num4 * 0.5f;
			if (replace)
			{
				ReplaceAlphaWith(alphaData, alphaWidth, alphaHeight);
			}
		}

		[ContextMenu("Blur Alpha")]
		public void BlurAlpha()
		{
			BlurAlpha(replace: true);
		}

		public void BlurAlpha(bool replace)
		{
			D2dHelper.Blur(alphaData, alphaWidth, alphaHeight);
			if (replace)
			{
				ReplaceAlphaWith(alphaData, alphaWidth, alphaHeight);
			}
		}

		[ContextMenu("Trim Alpha")]
		public void TrimAlpha()
		{
			if (!AlphaIsValid)
			{
				throw new InvalidOperationException("Invalid alpha");
			}
			int num = 0;
			int num2 = alphaWidth;
			int num3 = 0;
			int num4 = alphaHeight;
			for (int i = num; i < num2 && !FastSolidAlphaVertical(num3, num4, i); i++)
			{
				num++;
			}
			int num5 = num2 - 1;
			while (num5 >= num && !FastSolidAlphaVertical(num3, num4, num5))
			{
				num2--;
				num5--;
			}
			for (int j = num3; j < num4 && !FastSolidAlphaHorizontal(num, num2, j); j++)
			{
				num3++;
			}
			int num6 = num4 - 1;
			while (num6 >= num3 && !FastSolidAlphaHorizontal(num, num2, num6))
			{
				num4--;
				num6--;
			}
			int num7 = num2 - num + 2;
			int num8 = num4 - num3 + 2;
			D2dRect subRect = D2dRect.CreateFromMinSize(num - 1, num3 - 1, num7, num8);
			D2dHelper.ClearAlpha(num7, num8);
			D2dHelper.PasteAlpha(alphaData, alphaWidth, num, num2, num3, num4, 1, 1, num7);
			SubsetAlphaWith(D2dHelper.AlphaData, subRect, alphaCount);
		}

		private bool FastSolidAlphaHorizontal(int xMin, int xMax, int y)
		{
			int num = y * alphaWidth;
			for (int i = xMin; i < xMax; i++)
			{
				if (alphaData[i + num] > 0)
				{
					return true;
				}
			}
			return false;
		}

		private bool FastSolidAlphaVertical(int yMin, int yMax, int x)
		{
			for (int i = yMin; i < yMax; i++)
			{
				if (alphaData[x + i * alphaWidth] > 0)
				{
					return true;
				}
			}
			return false;
		}

		[ContextMenu("Reset Alpha")]
		public void ResetAlpha()
		{
			Texture2D texture2D = MainTex as Texture2D;
			if (texture2D != null)
			{
				int x = Mathf.RoundToInt(textureRect.x * (float)MainTex.width);
				int y = Mathf.RoundToInt(textureRect.y * (float)MainTex.height);
				int width = Mathf.RoundToInt(textureRect.width * (float)MainTex.width);
				int height = Mathf.RoundToInt(textureRect.height * (float)MainTex.height);
				alphaRect = originalRect;
				UpdateMesh();
				ReplaceAlphaWith(texture2D, x, y, width, height);
			}
		}

		public bool TryLocalSplit(D2dRect splitRect)
		{
			if (IsSplitting)
			{
				return false;
			}
			if (!AlphaIsValid)
			{
				throw new InvalidOperationException("Invalid alpha");
			}
			splitRect.Expand(LocalSplitExpand);
			D2dRect a = new D2dRect(0, alphaWidth, 0, alphaHeight);
			D2dRect rect = D2dRect.CalculateOverlap(a, splitRect);
			if (!rect.IsSet)
			{
				throw new ArgumentOutOfRangeException("Split rect does not overlap alpha");
			}
			D2dFloodfill.FastFindLocal(alphaData, alphaWidth, alphaHeight, rect);
			splitGroups.Clear();
			if (D2dFloodfill.BorderIslands.Count + D2dFloodfill.Islands.Count > 1)
			{
				D2dSplitGroup.ClearAll();
				D2dSplitGroup splitGroup = D2dSplitGroup.GetSplitGroup();
				for (int num = D2dFloodfill.BorderIslands.Count - 1; num >= 0; num--)
				{
					splitGroup.AddIsland(D2dFloodfill.BorderIslands[num]);
				}
				for (int num2 = D2dFloodfill.Islands.Count - 1; num2 >= 0; num2--)
				{
					D2dFloodfill.Island island = D2dFloodfill.Islands[num2];
					D2dSplitGroup splitGroup2 = D2dSplitGroup.GetSplitGroup();
					splitGroup2.AddIsland(island);
					splitGroups.Add(splitGroup2);
				}
				Split(splitGroup, splitGroups);
				D2dSplitGroup.ClearAll();
				return true;
			}
			return false;
		}

		[ContextMenu("Try Split")]
		public bool TrySplit()
		{
			if (IsSplitting)
			{
				return false;
			}
			if (!AlphaIsValid)
			{
				throw new InvalidOperationException("Invalid alpha");
			}
			D2dFloodfill.FastFind(alphaData, alphaWidth, alphaHeight);
			if (D2dFloodfill.Islands.Count > 1)
			{
				D2dSplitGroup.ClearAll();
				for (int num = D2dFloodfill.Islands.Count - 1; num >= 0; num--)
				{
					D2dFloodfill.Island island = D2dFloodfill.Islands[num];
					if (island.Pixels.Count > MinSplitPixels)
					{
						D2dSplitGroup splitGroup = D2dSplitGroup.GetSplitGroup();
						splitGroup.AddIsland(island);
					}
				}
				Split(null, D2dSplitGroup.SplitGroups);
				D2dSplitGroup.ClearAll();
				return true;
			}
			return false;
		}

		public void Split(D2dSplitGroup borderGroup, List<D2dSplitGroup> groups)
		{
			UnityEngine.Debug.Log(" ---- Spawn in D2Destructeble 222 ---> " + base.gameObject.tag);
			if (groups == null || groups.Count == 0 || base.gameObject.tag == "New_DestClone")
			{
				return;
			}
			IsSplitting = true;
			clones.Clear();
			IsOnStartSplit = true;
			if (OnStartSplit != null)
			{
				OnStartSplit.Invoke();
			}
			IsOnStartSplit = false;
			groups.Sort((D2dSplitGroup a, D2dSplitGroup b) => b.Pixels.Count.CompareTo(a.Pixels.Count));
			byte[] prevData = alphaData;
			int prevWidth = alphaWidth;
			int prevHeight = alphaHeight;
			alphaData = null;
			for (int num = groups.Count - 1; num >= 0; num--)
			{
				D2dSplitGroup d2dSplitGroup = groups[num];
				D2dDestructible d2dDestructible = null;
				if (num == 0 && borderGroup == null)
				{
					d2dDestructible = this;
				}
				else
				{
					d2dDestructible = UnityEngine.Object.Instantiate(this);
					d2dDestructible.name = base.name;
					d2dDestructible.tag = base.tag;
					d2dDestructible.gameObject.layer = base.gameObject.layer;
					d2dDestructible.transform.SetParent(base.transform.parent, worldPositionStays: false);
					d2dDestructible.transform.localPosition = base.transform.localPosition;
					d2dDestructible.transform.localRotation = base.transform.localRotation;
					d2dDestructible.transform.localScale = base.transform.localScale;
					d2dDestructible.transform.parent = null;
				}
				d2dSplitGroup.GenerateData();
				d2dSplitGroup.CombineData(prevData, prevWidth, prevHeight);
				d2dDestructible.SubsetAlphaWith(d2dSplitGroup.Data, d2dSplitGroup.Rect);
				clones.Add(d2dDestructible);
			}
			if (borderGroup != null)
			{
				borderGroup.GenerateData();
				borderGroup.CombineData(prevData, prevWidth, prevHeight);
				alphaData = prevData;
				UpdateAlpha(borderGroup.Data, borderGroup.Rect);
				clones.Add(this);
			}
			if (OnEndSplit != null)
			{
				OnEndSplit.Invoke(clones);
			}
			IsSplitting = false;
			clones.Clear();
		}

		[ContextMenu("Update Mesh")]
		public void UpdateMesh()
		{
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (MainTex != null)
			{
				Rect rect = textureRect;
				FastPadRect(ref rect);
				if (mesh == null)
				{
					mesh = new Mesh();
					UpdateMeshData(rect);
					mesh.hideFlags = HideFlags.DontSave;
					mesh.name = "Destructible Mesh";
					mesh.triangles = indices;
				}
				else
				{
					UpdateMeshData(rect);
				}
				UpdateProperties(rect);
				meshFilter.sharedMesh = mesh;
			}
			else
			{
				meshFilter.sharedMesh = null;
			}
		}

		public void SubsetAlphaWith(byte[] subData, D2dRect subRect, int newAlphaCount = -1)
		{
			if (!D2dHelper.AlphaIsValid(subData, subRect))
			{
				throw new ArgumentException("Invalid subset data");
			}
			float num = D2dHelper.Divide(alphaRect.width, alphaWidth);
			float num2 = D2dHelper.Divide(alphaRect.height, alphaHeight);
			alphaRect.x += num * (float)subRect.MinX;
			alphaRect.y += num2 * (float)subRect.MinY;
			alphaRect.width += num * (float)(subRect.SizeX - alphaWidth);
			alphaRect.height += num2 * (float)(subRect.SizeY - alphaHeight);
			FastCopyAlphaData(subData, subRect.SizeX, subRect.SizeY, newAlphaCount);
			UpdateMesh();
			alphaDirty.Set(0, subRect.SizeX, 0, subRect.SizeY);
			if (OnAlphaDataSubset != null)
			{
				OnAlphaDataSubset.Invoke(subRect);
			}
		}

		protected virtual void Awake()
		{
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			if (component != null)
			{
				Sprite sprite = component.sprite;
				int sortingOrder = component.sortingOrder;
				int sortingLayerID = component.sortingLayerID;
				Material sharedMaterial = component.sharedMaterial;
				UnityEngine.Object.DestroyImmediate(component);
				ReplaceWith(sprite);
				UpdateMesh();
				UpdateRenderer(sharedMaterial);
				D2dSorter d2dSorter = base.gameObject.AddComponent<D2dSorter>();
				d2dSorter.SortingOrder = sortingOrder;
				d2dSorter.SortingLayerID = sortingLayerID;
			}
			else
			{
				UpdateMesh();
				UpdateRenderer(null);
			}
		}

		protected virtual void OnEnable()
		{
			AllDestructibles.Add(this);
		}

		protected virtual void OnDisable()
		{
			AllDestructibles.Remove(this);
		}

		protected virtual void OnWillRenderObject()
		{
			DeserializeAlphaTex();
			if (!(MainTex != null) || !(alphaTex != null))
			{
				return;
			}
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			if (propertyBlock == null)
			{
				propertyBlock = new MaterialPropertyBlock();
			}
			float num = Sharpness;
			if (AutoSharpen)
			{
				if (alphaRatio <= 0f)
				{
					float a = D2dHelper.Divide(alphaRect.width, originalRect.width) * (float)MainTex.width * textureRect.width;
					alphaRatio = D2dHelper.Divide(a, alphaWidth);
				}
				num *= alphaRatio;
			}
			propertyBlock.SetTexture("_MainTex", MainTex);
			propertyBlock.SetTexture("_AlphaTex", alphaTex);
			propertyBlock.SetVector("_AlphaScale", alphaScale);
			propertyBlock.SetVector("_AlphaOffset", alphaOffset);
			propertyBlock.SetFloat("_AlphaSharpness", num);
			meshRenderer.SetPropertyBlock(propertyBlock);
		}

		private void UpdateRenderer(Material oldMaterial)
		{
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			if (meshRenderer.sharedMaterial == null)
			{
				meshRenderer.sharedMaterial = Resources.Load<Material>("Destructible 2D/Default");
			}
		}

		private void FastPadRect(ref Rect rect)
		{
			if (SystemInfo.npotSupport == NPOTSupport.Full)
			{
				return;
			}
			if (SystemInfo.npotSupport == NPOTSupport.Restricted)
			{
				Texture2D texture2D = MainTex as Texture2D;
				if (texture2D != null && texture2D.mipmapCount <= 1)
				{
					return;
				}
			}
			int width = MainTex.width;
			int height = MainTex.height;
			int num = Mathf.NextPowerOfTwo(width);
			int num2 = Mathf.NextPowerOfTwo(height);
			if (width != num)
			{
				float num3 = (float)width / (float)num;
				rect.x *= num3;
				rect.width *= num3;
			}
			if (height != num2)
			{
				float num4 = (float)height / (float)num2;
				rect.y *= num4;
				rect.height *= num4;
			}
		}

		private void UpdateMeshData(Rect paddedTextureRect)
		{
			positions[0] = new Vector3(alphaRect.xMin, alphaRect.yMin, 0f);
			positions[1] = new Vector3(alphaRect.xMax, alphaRect.yMin, 0f);
			positions[2] = new Vector3(alphaRect.xMin, alphaRect.yMax, 0f);
			positions[3] = new Vector3(alphaRect.xMax, alphaRect.yMax, 0f);
			mesh.vertices = positions;
			UpdateMeshColors();
			UpdateMeshCoords(paddedTextureRect);
			mesh.normals = quadNormals;
			mesh.tangents = quadTangents;
			mesh.RecalculateBounds();
		}

		private void UpdateMeshColors()
		{
			colors[0] = Color;
			colors[1] = Color;
			colors[2] = Color;
			colors[3] = Color;
			mesh.colors = colors;
		}

		private void UpdateMeshCoords(Rect paddedTextureRect)
		{
			float num = D2dHelper.InverseLerp(originalRect.xMin, originalRect.xMax, alphaRect.xMin);
			float num2 = D2dHelper.InverseLerp(originalRect.xMin, originalRect.xMax, alphaRect.xMax);
			float num3 = D2dHelper.InverseLerp(originalRect.yMin, originalRect.yMax, alphaRect.yMin);
			float num4 = D2dHelper.InverseLerp(originalRect.yMin, originalRect.yMax, alphaRect.yMax);
			float x = paddedTextureRect.x + paddedTextureRect.width * num;
			float x2 = paddedTextureRect.x + paddedTextureRect.width * num2;
			float y = paddedTextureRect.y + paddedTextureRect.height * num3;
			float y2 = paddedTextureRect.y + paddedTextureRect.height * num4;
			coords[0] = new Vector2(x, y);
			coords[1] = new Vector2(x2, y);
			coords[2] = new Vector2(x, y2);
			coords[3] = new Vector2(x2, y2);
			mesh.uv = coords;
		}

		private void UpdateProperties(Rect paddedTextureRect)
		{
			alphaScale.x = D2dHelper.Divide(D2dHelper.Divide(originalRect.width, alphaRect.width), paddedTextureRect.width);
			alphaScale.y = D2dHelper.Divide(D2dHelper.Divide(originalRect.height, alphaRect.height), paddedTextureRect.height);
			alphaOffset.x = paddedTextureRect.x + paddedTextureRect.width * D2dHelper.Divide(alphaRect.x - originalRect.x, originalRect.width);
			alphaOffset.y = paddedTextureRect.y + paddedTextureRect.height * D2dHelper.Divide(alphaRect.y - originalRect.y, originalRect.height);
		}

		private void DeserializeAlphaTex()
		{
			if (AlphaIsValid)
			{
				if (alphaTex == null)
				{
					ConstructAlphaTex();
				}
				else if (alphaTex.width != alphaWidth || alphaTex.height != alphaHeight)
				{
					alphaTex = D2dHelper.Destroy(alphaTex);
					ConstructAlphaTex();
				}
				else if (alphaDirty.IsSet)
				{
					ReconstructAlphaTex();
				}
			}
			else
			{
				Clear();
			}
		}

		private void ConstructAlphaTex()
		{
			alphaTex = new Texture2D(alphaWidth, alphaHeight, TextureFormat.Alpha8, mipChain: false);
			alphaTex.hideFlags = HideFlags.DontSave;
			alphaTex.wrapMode = TextureWrapMode.Clamp;
			for (int i = 0; i < alphaHeight; i++)
			{
				for (int j = 0; j < alphaWidth; j++)
				{
					Color color = default(Color);
					byte a = alphaData[j + i * alphaWidth];
					color.a = D2dHelper.ConvertAlpha(a);
					alphaTex.SetPixel(j, i, color);
				}
			}
			alphaTex.Apply();
			alphaDirty.Clear();
		}

		private void ReconstructAlphaTex()
		{
			int minX = alphaDirty.MinX;
			int maxX = alphaDirty.MaxX;
			int minY = alphaDirty.MinY;
			int maxY = alphaDirty.MaxY;
			for (int i = minY; i < maxY; i++)
			{
				for (int j = minX; j < maxX; j++)
				{
					Color color = default(Color);
					byte a = alphaData[j + i * alphaWidth];
					color.a = D2dHelper.ConvertAlpha(a);
					alphaTex.SetPixel(j, i, color);
				}
			}
			alphaTex.Apply();
			alphaDirty.Clear();
		}
	}
}
