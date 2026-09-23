using System.Collections.Generic;
using UnityEngine;

namespace Exploder2D
{
	public class ExploderQueue
	{
		private readonly Queue<Exploder2DSettings> queue;

		private readonly Exploder2DObject _exploder2D;

		public ExploderQueue(Exploder2DObject _exploder2D)
		{
			this._exploder2D = _exploder2D;
			queue = new Queue<Exploder2DSettings>();
		}

		public bool IsProcessing()
		{
			return queue.Count > 0;
		}

		public void Explode(Exploder2DObject.OnExplosion callback, GameObject obj = null)
		{
			Exploder2DSettings exploder2DSettings = new Exploder2DSettings();
			exploder2DSettings.Position = Exploder2DUtils.GetCentroid(_exploder2D.gameObject);
			exploder2DSettings.DontUseTag = _exploder2D.DontUseTag;
			exploder2DSettings.Radius = _exploder2D.Radius;
			exploder2DSettings.ForceVector = _exploder2D.ForceVector;
			exploder2DSettings.UseForceVector = _exploder2D.UseForceVector;
			exploder2DSettings.Force = _exploder2D.Force;
			exploder2DSettings.FrameBudget = _exploder2D.FrameBudget;
			exploder2DSettings.TargetFragments = _exploder2D.TargetFragments;
			exploder2DSettings.DeactivateOptions = _exploder2D.DeactivateOptions;
			exploder2DSettings.DeactivateTimeout = _exploder2D.DeactivateTimeout;
			exploder2DSettings.ExplodeSelf = _exploder2D.ExplodeSelf;
			exploder2DSettings.HideSelf = _exploder2D.HideSelf;
			exploder2DSettings.DestroyOriginalObject = _exploder2D.DestroyOriginalObject;
			exploder2DSettings.ExplodeFragments = _exploder2D.ExplodeFragments;
			exploder2DSettings.SplitMeshIslands = _exploder2D.SplitMeshIslands;
			exploder2DSettings.FragmentOptions = _exploder2D.FragmentOptions.Clone();
			exploder2DSettings.SfxOptions = _exploder2D.SFXOptions.Clone();
			exploder2DSettings.Callback = callback;
			exploder2DSettings.processing = false;
			exploder2DSettings.RybakExplodeObject = obj;
			Exploder2DSettings item = exploder2DSettings;
			queue.Enqueue(item);
			ProcessQueue();
		}

		private void ProcessQueue()
		{
			if (queue.Count > 0)
			{
				Exploder2DSettings exploder2DSettings = queue.Peek();
				if (!exploder2DSettings.processing)
				{
					_exploder2D.DontUseTag = exploder2DSettings.DontUseTag;
					_exploder2D.Radius = exploder2DSettings.Radius;
					_exploder2D.ForceVector = exploder2DSettings.ForceVector;
					_exploder2D.UseForceVector = exploder2DSettings.UseForceVector;
					_exploder2D.Force = exploder2DSettings.Force;
					_exploder2D.FrameBudget = exploder2DSettings.FrameBudget;
					_exploder2D.TargetFragments = exploder2DSettings.TargetFragments;
					_exploder2D.DeactivateOptions = exploder2DSettings.DeactivateOptions;
					_exploder2D.DeactivateTimeout = exploder2DSettings.DeactivateTimeout;
					_exploder2D.ExplodeSelf = exploder2DSettings.ExplodeSelf;
					_exploder2D.HideSelf = exploder2DSettings.HideSelf;
					_exploder2D.DestroyOriginalObject = exploder2DSettings.DestroyOriginalObject;
					_exploder2D.ExplodeFragments = exploder2DSettings.ExplodeFragments;
					_exploder2D.SplitMeshIslands = exploder2DSettings.SplitMeshIslands;
					_exploder2D.FragmentOptions = exploder2DSettings.FragmentOptions;
					_exploder2D.SFXOptions = exploder2DSettings.SfxOptions;
					_exploder2D.RybakExplodeObject = exploder2DSettings.RybakExplodeObject;
					exploder2DSettings.id = Random.Range(int.MinValue, int.MaxValue);
					exploder2DSettings.processing = true;
					_exploder2D.StartExplosionFromQueue(exploder2DSettings.Position, exploder2DSettings.id, exploder2DSettings.Callback, exploder2DSettings.RybakExplodeObject);
				}
			}
		}

		public void OnExplosionFinished(int id)
		{
			Exploder2DSettings exploder2DSettings = queue.Dequeue();
			ProcessQueue();
		}
	}
}
