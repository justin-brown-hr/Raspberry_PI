using System;
using UnityEngine;

namespace Destructible2D
{
	public static class D2dHelper
	{
		public const string ComponentMenuPrefix = "Destructible 2D/D2D ";

		public static byte[] AlphaData;

		private static float reciprocalOf255 = 0.003921569f;

		public static float ConvertAlpha(byte a)
		{
			return reciprocalOf255 * (float)(int)a;
		}

		public static byte ConvertAlpha(float a)
		{
			return (byte)(255f * a);
		}

		public static T Destroy<T>(T o) where T : UnityEngine.Object
		{
			if ((UnityEngine.Object)o != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Destroy(o);
			}
			return (T)null;
		}

		public static Matrix4x4 TranslationMatrix(Vector3 xyz)
		{
			return TranslationMatrix(xyz.x, xyz.y, xyz.z);
		}

		public static Matrix4x4 TranslationMatrix(float x, float y, float z)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m03 = x;
			identity.m13 = y;
			identity.m23 = z;
			return identity;
		}

		public static Matrix4x4 RotationMatrix(Quaternion q)
		{
			return Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
		}

		public static Matrix4x4 ScalingMatrix(float xyz)
		{
			return ScalingMatrix(xyz, xyz, xyz);
		}

		public static Matrix4x4 ScalingMatrix(Vector3 xyz)
		{
			return ScalingMatrix(xyz.x, xyz.y, xyz.z);
		}

		public static Matrix4x4 ScalingMatrix(float x, float y, float z)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m00 = x;
			identity.m11 = y;
			identity.m22 = z;
			return identity;
		}

		public static Vector3 ScreenToWorldPosition(Vector2 screenPosition, float intercept, Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (camera == null)
			{
				return screenPosition;
			}
			Ray ray = camera.ScreenPointToRay(screenPosition);
			Vector3 origin = ray.origin;
			float a = origin.z - intercept;
			Vector3 direction = ray.direction;
			float d = Divide(a, direction.z);
			return ray.origin - ray.direction * d;
		}

		public static bool IsValidUV(Vector2 uv)
		{
			return uv.x >= 0f && uv.y >= 0f && uv.x < 1f && uv.y < 1f;
		}

		public static bool IsValidUV(float u, float v)
		{
			return u >= 0f && v >= 0f && u < 1f && v < 1f;
		}

		public static bool Zero(float v)
		{
			return v == 0f;
		}

		public static float Reciprocal(float v)
		{
			return Zero(v) ? 0f : (1f / v);
		}

		public static float Divide(float a, float b)
		{
			return Zero(b) ? 0f : (a / b);
		}

		public static float Atan2(Vector2 xy)
		{
			return Mathf.Atan2(xy.x, xy.y);
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T val = b;
			b = a;
			a = val;
		}

		public static bool AlphaIsValid(byte[] data, int width, int height)
		{
			return data != null && width > 0 && height > 0 && data.Length >= width * height;
		}

		public static bool AlphaIsValid(byte[] data, D2dRect rect)
		{
			return data != null && rect.IsSet && data.Length >= rect.Area;
		}

		public static float DampenFactor(float dampening, float elapsed)
		{
			return 1f - Mathf.Pow(2.71828175f, (0f - dampening) * elapsed);
		}

		public static float Dampen(float current, float target, float dampening, float elapsed, float minStep = 0f)
		{
			float num = DampenFactor(dampening, elapsed);
			float maxDelta = Mathf.Abs(target - current) * num + minStep * elapsed;
			return Mathf.MoveTowards(current, target, maxDelta);
		}

		public static Vector2 Dampen2(Vector2 current, Vector2 target, float dampening, float elapsed, float minStep = 0f)
		{
			float num = DampenFactor(dampening, elapsed);
			float maxDistanceDelta = (target - current).magnitude * num + minStep * elapsed;
			return Vector2.MoveTowards(current, target, maxDistanceDelta);
		}

		public static void ClearAlpha(int width, int height)
		{
			if (width <= 0 || height <= 0)
			{
				throw new ArgumentOutOfRangeException("Invalid width or height");
			}
			int num = width * height;
			if (AlphaData == null || AlphaData.Length != num)
			{
				AlphaData = new byte[num];
				return;
			}
			for (int i = 0; i < num; i++)
			{
				AlphaData[i] = 0;
			}
		}

		public static void PasteAlpha(byte[] src, int srcWidth, int srcXMin, int srcXMax, int srcYMin, int srcYMax, int dstXMin, int dstYMin, int dstWidth)
		{
			for (int i = srcYMin; i < srcYMax; i++)
			{
				int num = (i - srcYMin + dstYMin) * dstWidth - srcXMin + dstXMin;
				int num2 = i * srcWidth;
				for (int j = srcXMin; j < srcXMax; j++)
				{
					int num3 = num + j;
					int num4 = num2 + j;
					AlphaData[num3] = src[num4];
				}
			}
		}

		public static bool ExtractAlpha(Texture2D texture2D, int x, int y, int width, int height)
		{
			if (texture2D != null && x >= 0 && y >= 0 && x + width <= texture2D.width && y + height <= texture2D.height)
			{
				Color32[] pixels = texture2D.GetPixels32();
				int num = width * height;
				int width2 = texture2D.width;
				if (AlphaData == null || AlphaData.Length != num)
				{
					AlphaData = new byte[num];
				}
				for (int num2 = height - 1; num2 >= 0; num2--)
				{
					int num3 = num2 * width;
					int num4 = (y + num2) * width2 + x;
					for (int num5 = width - 1; num5 >= 0; num5--)
					{
						AlphaData[num3 + num5] = pixels[num4 + num5].a;
					}
				}
				return true;
			}
			return false;
		}

		public static void Halve(ref byte[] data, ref int width, ref int height)
		{
			if (data == null || data.Length < width * height || width <= 2 || height <= 2)
			{
				return;
			}
			int num = width / 2;
			int num2 = height / 2;
			byte[] array = new byte[num * num2];
			float num3 = 1f / (float)width;
			float num4 = 1f / (float)height;
			float num5 = (1f - num3 * 2f) / (float)(num - 1);
			float num6 = (1f - num4 * 2f) / (float)(num2 - 1);
			for (int i = 0; i < num2; i++)
			{
				int num7 = i * num;
				for (int j = 0; j < num; j++)
				{
					array[j + num7] = GetBilinearFast(data, (float)j * num5 + num3, (float)i * num6 + num4, width, height);
				}
			}
			data = array;
			width = num;
			height = num2;
		}

		public static void Blur(byte[] alphaData, int alphaWidth, int alphaHeight)
		{
			if (alphaData == null)
			{
				return;
			}
			int num = alphaWidth * alphaHeight;
			if (alphaData.Length >= num)
			{
				if (AlphaData == null || AlphaData.Length < num)
				{
					AlphaData = new byte[num];
				}
				BlurHorizontally(alphaData, AlphaData, alphaWidth, alphaHeight);
				BlurVertically(AlphaData, alphaData, alphaWidth, alphaHeight);
			}
		}

		private static void BlurHorizontally(byte[] src, byte[] dst, int width, int height)
		{
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					byte @default = GetDefault(src, j - 1, i, width, height);
					byte default2 = GetDefault(src, j, i, width);
					byte default3 = GetDefault(src, j + 1, i, width, height);
					int num = @default + default2 + default3;
					dst[j + i * width] = (byte)(num / 3);
				}
			}
		}

		private static void BlurVertically(byte[] src, byte[] dst, int width, int height)
		{
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					byte @default = GetDefault(src, j, i - 1, width, height);
					byte default2 = GetDefault(src, j, i, width);
					byte default3 = GetDefault(src, j, i + 1, width, height);
					int num = @default + default2 + default3;
					dst[j + i * width] = (byte)(num / 3);
				}
			}
		}

		public static byte GetDefault(byte[] data, int x, int y, int width)
		{
			return data[x + y * width];
		}

		public static byte GetDefault(byte[] data, int x, int y, int width, int height)
		{
			if (x >= 0 && x < width && y >= 0 && y < height)
			{
				return data[x + y * width];
			}
			return 0;
		}

		public static byte GetClamp(byte[] data, int x, int y, int width, int height)
		{
			if (x < 0)
			{
				x = 0;
			}
			else if (x >= width)
			{
				x = width - 1;
			}
			if (y < 0)
			{
				y = 0;
			}
			else if (y >= height)
			{
				y = height - 1;
			}
			return data[x + y * width];
		}

		public static byte GetBilinear(byte[] data, float u, float v, int width, int height)
		{
			u *= (float)(width - 1);
			v *= (float)(height - 1);
			int num = Mathf.FloorToInt(u);
			int num2 = Mathf.FloorToInt(v);
			float t = u - (float)num;
			float t2 = v - (float)num2;
			byte clamp = GetClamp(data, num, num2, width, height);
			byte clamp2 = GetClamp(data, num + 1, num2, width, height);
			byte clamp3 = GetClamp(data, num, num2 + 1, width, height);
			byte clamp4 = GetClamp(data, num + 1, num2 + 1, width, height);
			byte a = Lerp(clamp, clamp2, t);
			byte b = Lerp(clamp3, clamp4, t);
			return Lerp(a, b, t2);
		}

		public static byte GetBilinearFast(byte[] data, float u, float v, int width, int height)
		{
			u *= (float)(width - 1);
			v *= (float)(height - 1);
			int num = Mathf.FloorToInt(u);
			int num2 = Mathf.FloorToInt(v);
			float t = u - (float)num;
			float t2 = v - (float)num2;
			byte @default = GetDefault(data, num, num2, width);
			byte default2 = GetDefault(data, num + 1, num2, width);
			byte default3 = GetDefault(data, num, num2 + 1, width);
			byte default4 = GetDefault(data, num + 1, num2 + 1, width);
			byte a = Lerp(@default, default2, t);
			byte b = Lerp(default3, default4, t);
			return Lerp(a, b, t2);
		}

		public static byte Lerp(byte a, byte b, float t)
		{
			float num = 1f - t;
			return (byte)((float)(int)a * num + (float)(int)b * t);
		}

		public static float Lerp(float a, float b, float t)
		{
			float num = 1f - t;
			return a * num + b * t;
		}

		public static float InverseLerp(float a, float b, float value)
		{
			if (a != b)
			{
				return (value - a) / (b - a);
			}
			return 0f;
		}

		public static bool CalculateRect(Matrix4x4 matrix, ref D2dRect rect, int sizeX, int sizeY)
		{
			Vector3 vector = matrix.MultiplyPoint(new Vector3(0f, 0f, 0f));
			Vector3 vector2 = matrix.MultiplyPoint(new Vector3(1f, 0f, 0f));
			Vector3 vector3 = matrix.MultiplyPoint(new Vector3(0f, 1f, 0f));
			Vector3 vector4 = matrix.MultiplyPoint(new Vector3(1f, 1f, 0f));
			float num = Mathf.Min(Mathf.Min(vector.x, vector2.x), Mathf.Min(vector3.x, vector4.x));
			float num2 = Mathf.Max(Mathf.Max(vector.x, vector2.x), Mathf.Max(vector3.x, vector4.x));
			float num3 = Mathf.Min(Mathf.Min(vector.y, vector2.y), Mathf.Min(vector3.y, vector4.y));
			float num4 = Mathf.Max(Mathf.Max(vector.y, vector2.y), Mathf.Max(vector3.y, vector4.y));
			if (num < num2 && num3 < num4)
			{
				rect.MinX = Mathf.FloorToInt(num * (float)sizeX);
				rect.MaxX = Mathf.CeilToInt(num2 * (float)sizeX);
				rect.MinY = Mathf.FloorToInt(num3 * (float)sizeY);
				rect.MaxY = Mathf.CeilToInt(num4 * (float)sizeY);
				return true;
			}
			return false;
		}
	}
}
