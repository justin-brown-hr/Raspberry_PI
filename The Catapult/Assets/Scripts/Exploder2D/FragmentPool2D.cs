using System.Collections.Generic;
using UnityEngine;

namespace Exploder2D
{
	public class FragmentPool2D : MonoBehaviour
	{
		private static FragmentPool2D instance;

		private Fragment2D[] pool;

		private float fragmentSoundTimeout;

		public float HitSoundTimeout = 1f;

		public int MaxEmitters = 1000;

		public static FragmentPool2D Instance
		{
			get
			{
				if (instance == null)
				{
					GameObject gameObject = new GameObject("FragmentRoot");
					instance = gameObject.AddComponent<FragmentPool2D>();
				}
				return instance;
			}
		}

		public int PoolSize => pool.Length;

		public Fragment2D[] Pool => pool;

		private void Awake()
		{
			instance = this;
		}

		private void OnDestroy()
		{
			DestroyFragments();
			instance = null;
		}

		public List<Fragment2D> GetAvailableFragments(int size)
		{
			if (size > pool.Length)
			{
				UnityEngine.Debug.LogError("Requesting pool size higher than allocated! Please call Allocate first! " + size);
				return null;
			}
			if (size == pool.Length)
			{
				return new List<Fragment2D>(pool);
			}
			List<Fragment2D> list = new List<Fragment2D>();
			int num = 0;
			Fragment2D[] array = pool;
			foreach (Fragment2D fragment2D in array)
			{
				if (fragment2D != null)
				{
					if (!fragment2D.activeObj)
					{
						list.Add(fragment2D);
						num++;
					}
					if (num == size)
					{
						return list;
					}
				}
			}
			Fragment2D[] array2 = pool;
			foreach (Fragment2D fragment2D2 in array2)
			{
				if (fragment2D2 != null)
				{
					if (!fragment2D2.visible)
					{
						list.Add(fragment2D2);
						num++;
					}
					if (num == size)
					{
						return list;
					}
				}
			}
			if (num < size)
			{
				Fragment2D[] array3 = pool;
				foreach (Fragment2D fragment2D3 in array3)
				{
					if (fragment2D3 != null)
					{
						if (fragment2D3.IsSleeping() && fragment2D3.visible)
						{
							list.Add(fragment2D3);
							num++;
						}
						if (num == size)
						{
							return list;
						}
					}
				}
			}
			if (num < size)
			{
				Fragment2D[] array4 = pool;
				foreach (Fragment2D fragment2D4 in array4)
				{
					if (fragment2D4 != null)
					{
						if (!fragment2D4.IsSleeping() && fragment2D4.visible)
						{
							list.Add(fragment2D4);
							num++;
						}
						if (num == size)
						{
							return list;
						}
					}
				}
			}
			return null;
		}

		public void Allocate(int poolSize)
		{
			if (pool == null || pool.Length < poolSize)
			{
				DestroyFragments();
				pool = new Fragment2D[poolSize];
				for (int i = 0; i < poolSize; i++)
				{
					GameObject gameObject = new GameObject("fragment_" + i);
					gameObject.AddComponent<SpriteRenderer>();
					gameObject.AddComponent<PolygonCollider2D>();
					gameObject.AddComponent<Rigidbody2D>();
					gameObject.AddComponent<Exploder2DOption>();
					Fragment2D fragment2D = gameObject.AddComponent<Fragment2D>();
					gameObject.transform.parent = base.gameObject.transform;
					pool[i] = fragment2D;
					Exploder2DUtils.SetActiveRecursively(gameObject.gameObject, status: false);
					fragment2D.RefreshComponentsCache();
					fragment2D.Sleep();
				}
			}
		}

		public void WakeUp()
		{
			Fragment2D[] array = pool;
			foreach (Fragment2D fragment2D in array)
			{
				if (fragment2D != null)
				{
					fragment2D.WakeUp();
				}
			}
		}

		public void Sleep()
		{
			Fragment2D[] array = pool;
			foreach (Fragment2D fragment2D in array)
			{
				if (fragment2D != null)
				{
					fragment2D.Sleep();
				}
			}
		}

		public void DestroyFragments()
		{
			if (pool == null)
			{
				return;
			}
			Fragment2D[] array = pool;
			foreach (Fragment2D fragment2D in array)
			{
				if (fragment2D != null)
				{
					UnityEngine.Object.Destroy(fragment2D.gameObject);
				}
			}
			pool = null;
		}

		public void DeactivateFragments()
		{
			if (pool == null)
			{
				return;
			}
			Fragment2D[] array = pool;
			foreach (Fragment2D fragment2D in array)
			{
				if ((bool)fragment2D)
				{
					fragment2D.Deactivate();
				}
			}
		}

		public void SetDeactivateOptions(DeactivateOptions options, FadeoutOptions fadeoutOptions, float timeout)
		{
			if (pool == null)
			{
				return;
			}
			Fragment2D[] array = pool;
			foreach (Fragment2D fragment2D in array)
			{
				if (fragment2D != null)
				{
					fragment2D.deactivateOptions = options;
					fragment2D.deactivateTimeout = timeout;
					fragment2D.fadeoutOptions = fadeoutOptions;
				}
			}
		}

		public void SetExplodableFragments(bool explodable, bool dontUseTag)
		{
			if (pool == null)
			{
				return;
			}
			if (dontUseTag)
			{
				Fragment2D[] array = pool;
				foreach (Fragment2D fragment2D in array)
				{
					if (fragment2D != null && fragment2D.gameObject != null && !fragment2D.gameObject.GetComponent<Explodable2D>())
					{
						fragment2D.gameObject.AddComponent<Explodable2D>();
					}
				}
			}
			else
			{
				if (!explodable)
				{
					return;
				}
				Fragment2D[] array2 = pool;
				foreach (Fragment2D fragment2D2 in array2)
				{
					if (fragment2D2 != null)
					{
						fragment2D2.tag = Exploder2DObject.Tag;
					}
				}
			}
		}

		public void SetFragmentPhysicsOptions(Exploder2DObject.FragmentOption options)
		{
			if (pool == null)
			{
				return;
			}
			Fragment2D[] array = pool;
			foreach (Fragment2D fragment2D in array)
			{
				if (fragment2D != null)
				{
					fragment2D.SetFragmentPhysicsOptions(options);
				}
			}
		}

		public void SetSFXOptions(Exploder2DObject.SFXOption sfx)
		{
			if (pool == null)
			{
				return;
			}
			HitSoundTimeout = sfx.HitSoundTimeout;
			MaxEmitters = sfx.EmitersMax;
			for (int i = 0; i < pool.Length; i++)
			{
				if (pool[i] != null)
				{
					pool[i].SetSFX(sfx, i < MaxEmitters);
				}
			}
		}

		public List<Fragment2D> GetActiveFragments()
		{
			if (pool != null)
			{
				List<Fragment2D> list = new List<Fragment2D>(pool.Length);
				Fragment2D[] array = pool;
				foreach (Fragment2D fragment2D in array)
				{
					if (fragment2D != null && fragment2D.gameObject != null && Exploder2DUtils.IsActive(fragment2D.gameObject))
					{
						list.Add(fragment2D);
					}
				}
				return list;
			}
			return null;
		}

		private void Update()
		{
			fragmentSoundTimeout -= Time.deltaTime;
		}

		public void OnFragmentHit()
		{
			fragmentSoundTimeout = HitSoundTimeout;
		}

		public bool CanPlayHitSound()
		{
			return fragmentSoundTimeout <= 0f;
		}
	}
}
