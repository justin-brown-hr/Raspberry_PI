using UnityEngine;

namespace Exploder2D
{
	internal class Exploder2DSettings
	{
		public Vector2 Position;

		public Vector2 ForceVector;

		public float Force;

		public float FrameBudget;

		public float Radius;

		public float DeactivateTimeout;

		public int id;

		public int TargetFragments;

		public DeactivateOptions DeactivateOptions;

		public Exploder2DObject.FragmentOption FragmentOptions;

		public Exploder2DObject.SFXOption SfxOptions;

		public Exploder2DObject.OnExplosion Callback;

		public bool DontUseTag;

		public bool UseForceVector;

		public bool ExplodeSelf;

		public bool HideSelf;

		public bool DestroyOriginalObject;

		public bool ExplodeFragments;

		public bool SplitMeshIslands;

		public bool processing;

		public GameObject RybakExplodeObject;
	}
}
