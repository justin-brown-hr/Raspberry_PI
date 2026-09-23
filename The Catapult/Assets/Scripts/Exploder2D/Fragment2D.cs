using Exploder2D.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Exploder2D
{
	public class Fragment2D : MonoBehaviour
	{
		public bool explodable;

		public DeactivateOptions deactivateOptions;

		public float deactivateTimeout = 10f;

		public FadeoutOptions fadeoutOptions;

		public float maxVelocity = 1000f;

		public bool disableColliders;

		public float disableCollidersTimeout;

		public bool visible;

		public bool activeObj;

		public float minSizeToExplode = 0.5f;

		public AudioSource audioSource;

		public AudioClip audioClip;

		private ParticleSystem[] particleSystems;

		private SpriteRenderer spriteRenderer;

		private GameObject particleChild;

		public PolygonCollider2D polygonCollider2D;

		public Rigidbody2D rigid2D;

		private Mesh _meshForCollider;

		public Exploder2DOption options;

		public Rigidbody rigidBody;

		private Vector2 originalScale;

		private float visibilityCheckTimer;

		private float deactivateTimer;

		public bool IsSleeping()
		{
			return rigid2D.IsSleeping();
		}

		public void Sleep()
		{
			rigid2D.Sleep();
		}

		public void WakeUp()
		{
			rigid2D.WakeUp();
		}

		public void SetConstraints(RigidbodyConstraints constraints)
		{
		}

		public void SetSFX(Exploder2DObject.SFXOption sfx, bool allowParticle)
		{
			audioClip = sfx.FragmentSoundClip;
			if ((bool)audioClip && !audioSource)
			{
				audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			if ((bool)sfx.FragmentEmitter && allowParticle)
			{
				if (!particleChild)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(sfx.FragmentEmitter);
					if ((bool)gameObject)
					{
						gameObject.transform.position = Vector3.zero;
						particleChild = new GameObject("Particles");
						particleChild.transform.parent = base.gameObject.transform;
						gameObject.transform.parent = particleChild.transform;
					}
				}
				if ((bool)particleChild)
				{
					particleSystems = particleChild.GetComponentsInChildren<ParticleSystem>();
				}
			}
			else if ((bool)particleChild)
			{
				UnityEngine.Object.Destroy(particleChild);
			}
		}

		private void OnCollisionEnter2D()
		{
		}

		public void SetFragmentPhysicsOptions(Exploder2DObject.FragmentOption opt)
		{
			if ((bool)base.gameObject)
			{
				base.gameObject.layer = LayerMask.NameToLayer(opt.Layer);
				DisableColliders(opt.DisableColliders);
			}
		}

		public void DisableColliders(bool disable)
		{
			if (disable)
			{
				UnityEngine.Object.Destroy(polygonCollider2D);
			}
			else if (!polygonCollider2D)
			{
				polygonCollider2D = base.gameObject.AddComponent<PolygonCollider2D>();
			}
		}

		public void CreateSprite(SpriteMesh spriteMesh, Sprite parentSprite, Transform parentTransform, string layer, int order, Color color)
		{
			Sprite sprite = UnityEngine.Object.Instantiate(parentSprite, parentTransform.position, parentTransform.rotation);
			Vector3 min = parentSprite.bounds.min;
			Vector3 max = parentSprite.bounds.max;
			Vector2[] array = new Vector2[spriteMesh.vertices.Length];
			for (int i = 0; i < spriteMesh.vertices.Length; i++)
			{
				Vector2 vector = spriteMesh.vertices[i];
				float value = (vector.x - min.x) / (max.x - min.x);
				float value2 = (vector.y - min.y) / (max.y - min.y);
				value = Mathf.Clamp01(value);
				value2 = Mathf.Clamp01(value2);
				array[i] = new Vector2(value * parentSprite.rect.width, value2 * parentSprite.rect.height);
			}
			sprite.OverrideGeometry(array, spriteMesh.uTriangles);
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingLayerName = layer;
			spriteRenderer.sortingOrder = order;
			spriteRenderer.color = color;
		}

		private void MeshToPolygone()
		{
			int[] triangles = _meshForCollider.triangles;
			Vector3[] vertices = _meshForCollider.vertices;
			Dictionary<string, KeyValuePair<int, int>> dictionary = new Dictionary<string, KeyValuePair<int, int>>();
			for (int i = 0; i < triangles.Length; i += 3)
			{
				for (int j = 0; j < 3; j++)
				{
					int num = triangles[i + j];
					int num2 = triangles[(i + j + 1 <= i + 2) ? (i + j + 1) : i];
					string key = Mathf.Min(num, num2) + ":" + Mathf.Max(num, num2);
					if (dictionary.ContainsKey(key))
					{
						dictionary.Remove(key);
					}
					else
					{
						dictionary.Add(key, new KeyValuePair<int, int>(num, num2));
					}
				}
			}
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
			foreach (KeyValuePair<int, int> value in dictionary.Values)
			{
				if (!dictionary2.ContainsKey(value.Key))
				{
					dictionary2.Add(value.Key, value.Value);
				}
			}
			PolygonCollider2D component = base.gameObject.GetComponent<PolygonCollider2D>();
			component.pathCount = 0;
			int num3 = 0;
			int num4 = num3;
			int num5 = num3;
			List<Vector2> list = new List<Vector2>();
			while (true)
			{
				list.Add(vertices[num4]);
				if (!dictionary2.ContainsKey(num4))
				{
					break;
				}
				num4 = dictionary2[num4];
				if (num4 > num5)
				{
					num5 = num4;
				}
				if (num4 == num3)
				{
					component.pathCount++;
					component.SetPath(component.pathCount - 1, list.ToArray());
					list.Clear();
					if (!dictionary2.ContainsKey(num5 + 1))
					{
						break;
					}
					num3 = num5 + 1;
					num4 = num3;
				}
			}
		}

		public void ApplyExplosion2D(Transform meshTransform, Vector2 centroid, Vector2 mainCentroid, Exploder2DObject.FragmentOption fragmentOption, bool useForceVector, Vector2 ForceVector, float force, GameObject original, int targetFragments)
		{
			Rigidbody2D rigidbody2D = rigid2D;
			Vector2 b = Vector2.zero;
			float num = 0f;
			float mass = fragmentOption.Mass;
			float gravityScale = fragmentOption.GravityScale;
			if (fragmentOption.InheritParentPhysicsProperty && (bool)original && (bool)original.GetComponent<Rigidbody2D>())
			{
				Rigidbody2D component = original.GetComponent<Rigidbody2D>();
				b = component.velocity;
				num = component.angularVelocity;
				mass = component.mass / (float)targetFragments;
				gravityScale = component.gravityScale;
			}
			Vector3 vector = meshTransform.TransformPoint(centroid);
			Vector2 a = (new Vector2(vector.x, vector.y) - mainCentroid).normalized;
			float angularVelocity = fragmentOption.AngularVelocity;
			float num2;
			if (fragmentOption.RandomAngularVelocityVector)
			{
				Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				num2 = insideUnitCircle.x;
			}
			else
			{
				num2 = fragmentOption.AngularVelocityVector.y;
			}
			float num3 = angularVelocity * num2;
			if (useForceVector)
			{
				a = ForceVector;
			}
			rigidbody2D.velocity = a * force + b;
			rigidbody2D.angularVelocity = num3 + num;
			rigidbody2D.mass = mass;
			maxVelocity = fragmentOption.MaxVelocity;
			rigidbody2D.gravityScale = gravityScale;
		}

		public void RefreshComponentsCache()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			options = GetComponent<Exploder2DOption>();
			rigid2D = GetComponent<Rigidbody2D>();
			polygonCollider2D = GetComponent<PolygonCollider2D>();
		}

		public void Explode()
		{
			activeObj = true;
			Exploder2DUtils.SetActiveRecursively(base.gameObject, status: true);
			visibilityCheckTimer = 0.1f;
			visible = true;
			deactivateTimer = deactivateTimeout;
			originalScale = base.transform.localScale;
			if (explodable)
			{
				base.tag = Exploder2DObject.Tag;
			}
			Emit(centerToBound: true);
		}

		public void Emit(bool centerToBound)
		{
			if (particleSystems != null)
			{
				if (centerToBound && (bool)particleChild && (bool)spriteRenderer)
				{
					particleChild.transform.position = spriteRenderer.bounds.center;
				}
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Clear();
					particleSystem.Play();
				}
			}
		}

		public void Deactivate()
		{
			Exploder2DUtils.SetActive(base.gameObject, status: false);
			visible = false;
			activeObj = false;
			if (particleSystems != null)
			{
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Clear();
				}
			}
		}

		private void Start()
		{
			visibilityCheckTimer = 1f;
			RefreshComponentsCache();
			visible = false;
		}

		private void FixedUpdate()
		{
		}

		private void OnDestroy()
		{
		}

		private void Update()
		{
			if (!activeObj)
			{
				return;
			}
			if ((bool)rigidBody)
			{
				if (rigidBody.velocity.sqrMagnitude > maxVelocity * maxVelocity)
				{
					Vector3 normalized = rigidBody.velocity.normalized;
					rigidBody.velocity = normalized * maxVelocity;
				}
			}
			else if ((bool)rigid2D && rigid2D.velocity.sqrMagnitude > maxVelocity * maxVelocity)
			{
				Vector2 normalized2 = rigid2D.velocity.normalized;
				rigid2D.velocity = normalized2 * maxVelocity;
			}
			if (deactivateOptions == DeactivateOptions.Timeout)
			{
				deactivateTimer -= Time.deltaTime;
				if (deactivateTimer < 0f)
				{
					Sleep();
					activeObj = false;
					Exploder2DUtils.SetActiveRecursively(base.gameObject, status: false);
					FadeoutOptions fadeoutOptions = this.fadeoutOptions;
					if (fadeoutOptions == FadeoutOptions.FadeoutAlpha)
					{
					}
				}
				else
				{
					float num = deactivateTimer / deactivateTimeout;
					switch (this.fadeoutOptions)
					{
					case FadeoutOptions.FadeoutAlpha:
					{
						Color color = spriteRenderer.color;
						color.a = num;
						spriteRenderer.color = color;
						break;
					}
					case FadeoutOptions.ScaleDown:
						base.gameObject.transform.localScale = originalScale * num;
						break;
					}
				}
			}
			visibilityCheckTimer -= Time.deltaTime;
			if (!(visibilityCheckTimer < 0f) || !Camera.main)
			{
				return;
			}
			Vector3 vector = Camera.main.WorldToViewportPoint(base.transform.position);
			if (vector.z < 0f || vector.x < 0f || vector.y < 0f || vector.x > 1f || vector.y > 1f)
			{
				if (deactivateOptions == DeactivateOptions.OutsideOfCamera)
				{
					Sleep();
					activeObj = false;
					Exploder2DUtils.SetActiveRecursively(base.gameObject, status: false);
				}
				visible = false;
			}
			else
			{
				visible = true;
			}
			visibilityCheckTimer = UnityEngine.Random.Range(0.1f, 0.3f);
			if (explodable)
			{
				Vector3 size = GetComponent<Collider>().bounds.size;
				if (Mathf.Max(size.x, size.y, size.z) < minSizeToExplode)
				{
					base.tag = string.Empty;
				}
			}
		}
	}
}
