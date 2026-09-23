using System.Collections.Generic;
using Gameplay.Objects;
using UnityEngine;

namespace Gameplay
{
	public class BoundingPlane
	{
		private Plane _plane;

		// Cabinet: the plane above faces sideways, so a ray aimed to the side of the
		// field never crosses it and the shot would be refused. This one faces the
		// camera at the depth of the objects, so any forward aim resolves to a point.
		private Plane _depthPlane;

		private Bounds _bounds;

		public BoundingPlane(List<BaseObject> objects, List<Table> tables)
		{
			float maxZ = float.NegativeInfinity;
			float minZ = float.PositiveInfinity;
			float minY = float.PositiveInfinity;
			if (tables != null)
			{
				foreach (Table table in tables)
				{
					if (table == null || table.meshRenderers == null)
					{
						continue;
					}
					foreach (MeshRenderer renderer in table.meshRenderers)
					{
						if (renderer == null)
						{
							continue;
						}
						Bounds bounds = renderer.bounds;
						maxZ = Mathf.Max(maxZ, bounds.max.z);
						minZ = Mathf.Min(minZ, bounds.min.z);
						minY = Mathf.Min(minY, bounds.min.y);
					}
				}
			}
			if (float.IsNegativeInfinity(maxZ) || float.IsPositiveInfinity(minZ) || float.IsPositiveInfinity(minY))
			{
				maxZ = 0f;
				minZ = 0f;
				minY = 0f;
			}

			float centerZ = 0f;
			if (objects != null)
			{
				foreach (BaseObject obj in objects)
				{
					if (obj == null)
					{
						continue;
					}
					centerZ = Mathf.Max(centerZ, obj.transform.position.z);
				}
			}

			float centerY = minY + (1000f - minY) * 0.5f;
			_bounds = new Bounds(new Vector3(0f, centerY, centerZ), new Vector3(2000f, 1000f - minY, maxZ - minZ));
			_plane = new Plane(Vector3.left, Vector3.zero);
			_depthPlane = new Plane(Vector3.back, new Vector3(0f, centerY, centerZ));
		}

		public bool TryCast(Ray ray, out Vector3 hitPosition)
		{
			if (_plane.Raycast(ray, out float enter) && enter > 0f)
			{
				hitPosition = ray.GetPoint(enter);
				return true;
			}
			if (_depthPlane.Raycast(ray, out float depthEnter) && depthEnter > 0f)
			{
				hitPosition = ray.GetPoint(depthEnter);
				return true;
			}
			hitPosition = Vector3.zero;
			return false;
		}
	}
}
