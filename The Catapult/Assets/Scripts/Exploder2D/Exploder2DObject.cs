using Exploder2D.Core;
using Exploder2D.Core.Math;
using Exploder2D.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Grid = Exploder2D.Core.Grid;

namespace Exploder2D
{
	public class Exploder2DObject : MonoBehaviour
	{
		public enum CutStrategy
		{
			Randomized
		}

		[Serializable]
		public class SFXOption
		{
			public AudioClip ExplosionSoundClip;

			public AudioClip FragmentSoundClip;

			public GameObject FragmentEmitter;

			public float HitSoundTimeout;

			public int EmitersMax;

			public SFXOption Clone()
			{
				SFXOption sFXOption = new SFXOption();
				sFXOption.ExplosionSoundClip = ExplosionSoundClip;
				sFXOption.FragmentSoundClip = FragmentSoundClip;
				sFXOption.FragmentEmitter = FragmentEmitter;
				sFXOption.HitSoundTimeout = HitSoundTimeout;
				sFXOption.EmitersMax = EmitersMax;
				return sFXOption;
			}
		}

		[Serializable]
		public class FragmentOption
		{
			public string Layer;

			public bool InheritSortingLayer;

			public string SortingLayer;

			public bool InheritOrderInLayer;

			public int OrderInLayer;

			public bool InheritSpriteRendererColor;

			public Color SpriteRendererColor;

			public float MaxVelocity;

			public bool InheritParentPhysicsProperty;

			public float Mass;

			public float GravityScale;

			public bool DisableColliders;

			public float AngularVelocity;

			public Vector2 AngularVelocityVector;

			public bool RandomAngularVelocityVector;

			public FragmentOption Clone()
			{
				FragmentOption fragmentOption = new FragmentOption();
				fragmentOption.Layer = Layer;
				fragmentOption.InheritSortingLayer = InheritSortingLayer;
				fragmentOption.SortingLayer = SortingLayer;
				fragmentOption.OrderInLayer = OrderInLayer;
				fragmentOption.InheritSpriteRendererColor = InheritSpriteRendererColor;
				fragmentOption.SpriteRendererColor = SpriteRendererColor;
				fragmentOption.InheritOrderInLayer = InheritOrderInLayer;
				fragmentOption.Mass = Mass;
				fragmentOption.DisableColliders = DisableColliders;
				fragmentOption.GravityScale = GravityScale;
				fragmentOption.MaxVelocity = MaxVelocity;
				fragmentOption.InheritParentPhysicsProperty = InheritParentPhysicsProperty;
				fragmentOption.AngularVelocity = AngularVelocity;
				fragmentOption.AngularVelocityVector = AngularVelocityVector;
				fragmentOption.RandomAngularVelocityVector = RandomAngularVelocityVector;
				return fragmentOption;
			}
		}

		public delegate void OnExplosion(float timeMS, ExplosionState state);

		public enum ExplosionState
		{
			ExplosionStarted,
			ExplosionFinished
		}

		public delegate void OnCracked();

		private enum State
		{
			None,
			Preprocess,
			ProcessCutter,
			IsolateMeshIslands,
			PostprocessInit,
			Postprocess,
			DryRun
		}

		public struct CutMesh
		{
			public SpriteMesh spriteMesh;

			public Transform transform;

			public Transform parent;

			public Vector2 position;

			public Quaternion rotation;

			public Vector2 localScale;

			public Sprite sprite;

			public GameObject original;

			public Vector2 centroidLocal;

			public float distance;

			public int vertices;

			public int level;

			public int fragments;

			public string sortingLayer;

			public int orderInLayer;

			public Color color;

			public Exploder2DOption option;
		}

		private struct MeshData
		{
			public SpriteMesh spriteMesh;

			public Sprite sprite;

			public SpriteRenderer spriteRenderer;

			public GameObject gameObject;

			public GameObject parentObject;

			public Vector2 centroid;
		}

		public static string Tag = "Exploder2D";

		public bool DontUseTag;

		public float Radius = 10f;

		public Vector2 ForceVector = Vector2.up;

		public bool UseForceVector;

		public float Force = 30f;

		public float FrameBudget = 15f;

		public int TargetFragments = 30;

		public DeactivateOptions DeactivateOptions;

		public float DeactivateTimeout = 10f;

		public FadeoutOptions FadeoutOptions;

		public bool ExplodeSelf = true;

		public bool DisableRadiusScan;

		public bool HideSelf = true;

		public bool DestroyOriginalObject;

		public bool ExplodeFragments = true;

		public bool UniformFragmentDistribution;

		public bool SplitMeshIslands;

		public bool DisableQueue;

		public GameObject RybakExplodeObject;

		public CutStrategy CuttingStrategy;

		public int FragmentPoolSize = 200;

		public SFXOption SFXOptions = new SFXOption
		{
			ExplosionSoundClip = null,
			FragmentSoundClip = null,
			FragmentEmitter = null,
			HitSoundTimeout = 0.3f,
			EmitersMax = 1000
		};

		public FragmentOption FragmentOptions = new FragmentOption
		{
			Layer = "Default",
			SortingLayer = "Default",
			InheritSortingLayer = true,
			InheritOrderInLayer = true,
			OrderInLayer = 0,
			InheritSpriteRendererColor = true,
			SpriteRendererColor = Color.white,
			Mass = 20f,
			MaxVelocity = 1000f,
			DisableColliders = false,
			GravityScale = 1f,
			InheritParentPhysicsProperty = true,
			AngularVelocity = 1f,
			AngularVelocityVector = Vector2.up,
			RandomAngularVelocityVector = true
		};

		private OnExplosion ExplosionCallback;

		private OnCracked CrackedCallback;

		private bool crack;

		private bool cracked;

		private GameObject exploderObject;

		private State state;

		private ExploderQueue queue;

		private MeshCutter2D cutter;

		private Stopwatch timer;

		private HashSet<CutMesh> newFragments;

		private HashSet<CutMesh> meshToRemove;

		private HashSet<CutMesh> meshSet;

		private int[] levelCount;

		private HashSet<CutMesh> meshToCut;

		private int poolIdx;

		private List<CutMesh> postList;

		private List<Fragment2D> pool;

		private Vector2 mainCentroid;

		private bool splitMeshIslands;

		private List<CutMesh> islands;

		private int explosionID;

		private AudioSource audioSource;

		private bool ProcessCutterGrid(out long cuttingTime)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			foreach (CutMesh item in meshSet)
			{
				CutMesh current = item;
				if ((bool)current.transform)
				{
					Vector2[] path = Hull2D.ChainHull2D(current.spriteMesh.vertices);
					Grid grid = new Grid(current.spriteMesh.min, current.spriteMesh.max, 0.5f);
					grid.Intersect(path);
					grid.Triangulate();
					grid.DebugDraw(current.transform);
					break;
				}
			}
			stopwatch.Stop();
			cuttingTime = stopwatch.ElapsedMilliseconds;
			return true;
		}

		private bool ProcessCutterRandomized(out long cuttingTime)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = true;
			bool flag2 = false;
			int num = 0;
			cuttingTime = 0L;
			while (flag)
			{
				num++;
				if (num > TargetFragments)
				{
					break;
				}
				newFragments.Clear();
				meshToRemove.Clear();
				flag = false;
				int count = meshSet.Count;
				foreach (CutMesh item in meshSet)
				{
					CutMesh current = item;
					if (levelCount[current.level] > 0)
					{
						Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
						if ((bool)current.transform)
						{
							Line2D line2D = Line2D.CreateNormalPoint(insideUnitCircle, current.centroidLocal);
							if ((bool)current.option)
							{
								splitMeshIslands |= current.option.SplitMeshIslands;
							}
							List<CutterMesh> meshes = null;
							cutter.Cut(current.spriteMesh, current.transform, line2D, ref meshes);
							flag = true;
							if (meshes != null)
							{
								foreach (CutterMesh item2 in meshes)
								{
									CutterMesh current2 = item2;
									newFragments.Add(new CutMesh
									{
										spriteMesh = current2.mesh,
										centroidLocal = current2.centroidLocal,
										sprite = current.sprite,
										vertices = current.vertices,
										transform = current.transform,
										distance = current.distance,
										level = current.level,
										fragments = current.fragments,
										original = current.original,
										parent = current.transform.parent,
										position = current.transform.position,
										rotation = current.transform.rotation,
										localScale = current.transform.localScale,
										sortingLayer = current.sortingLayer,
										orderInLayer = current.orderInLayer,
										color = current.color,
										option = current.option
									});
								}
								meshToRemove.Add(current);
								levelCount[current.level]--;
								if (count + newFragments.Count - meshToRemove.Count >= TargetFragments)
								{
									cuttingTime = stopwatch.ElapsedMilliseconds;
									meshSet.ExceptWith(meshToRemove);
									meshSet.UnionWith(newFragments);
									return true;
								}
								if ((float)stopwatch.ElapsedMilliseconds > FrameBudget)
								{
									flag2 = true;
									break;
								}
							}
						}
					}
				}
				meshSet.ExceptWith(meshToRemove);
				meshSet.UnionWith(newFragments);
				if (flag2)
				{
					break;
				}
			}
			cuttingTime = stopwatch.ElapsedMilliseconds;
			if (!flag2)
			{
				return true;
			}
			return false;
		}

		public void Explode()
		{
			Explode(null);
		}

		public void Explode(OnExplosion callback, GameObject obj = null)
		{
			if (!DisableQueue || !queue.IsProcessing())
			{
				queue.Explode(callback, obj);
			}
		}

		public void StartExplosionFromQueue(Vector2 pos, int id, OnExplosion callback, GameObject obj = null)
		{
			mainCentroid = pos;
			explosionID = id;
			state = State.Preprocess;
			ExplosionCallback = callback;
			exploderObject = obj;
		}

		public void Crack()
		{
			Crack(null);
		}

		public void Crack(OnCracked callback)
		{
			if (!crack)
			{
				CrackedCallback = callback;
				crack = true;
				cracked = false;
				Explode(null);
			}
		}

		public void ExplodeCracked(OnExplosion callback)
		{
			if (cracked)
			{
				PostCrackExplode(callback);
				crack = false;
			}
		}

		public void ExplodeCracked()
		{
			ExplodeCracked(null);
		}

		private void Awake()
		{
			cutter = new MeshCutter2D();
			cutter.Init(512, 512);
			UnityEngine.Random.InitState(DateTime.Now.Millisecond);
			FragmentPool2D.Instance.Allocate(FragmentPoolSize);
			FragmentPool2D.Instance.SetDeactivateOptions(DeactivateOptions, FadeoutOptions, DeactivateTimeout);
			FragmentPool2D.Instance.SetExplodableFragments(ExplodeFragments, DontUseTag);
			FragmentPool2D.Instance.SetFragmentPhysicsOptions(FragmentOptions);
			FragmentPool2D.Instance.SetSFXOptions(SFXOptions);
			timer = new Stopwatch();
			queue = new ExploderQueue(this);
			if (DontUseTag)
			{
				base.gameObject.AddComponent<Explodable2D>();
			}
			else
			{
				base.gameObject.tag = Tag;
			}
			state = State.DryRun;
			PreAllocateBuffers();
			state = State.None;
			if ((bool)SFXOptions.ExplosionSoundClip)
			{
				audioSource = base.gameObject.GetComponent<AudioSource>();
				if (!audioSource)
				{
					audioSource = base.gameObject.AddComponent<AudioSource>();
				}
			}
		}

		private void PreAllocateBuffers()
		{
			newFragments = new HashSet<CutMesh>();
			meshToRemove = new HashSet<CutMesh>();
			meshSet = new HashSet<CutMesh>();
			meshToCut = new HashSet<CutMesh>();
			for (int i = 0; i < 64; i++)
			{
				meshSet.Add(default(CutMesh));
			}
			levelCount = new int[64];
			Preprocess();
			ProcessCutterRandomized(out long _);
		}

		private void OnDrawGizmos()
		{
			if (base.enabled && (!ExplodeSelf || !DisableRadiusScan))
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(Exploder2DUtils.GetCentroid(base.gameObject), Radius);
			}
		}

		private int GetLevelFragments(int level, int fragmentsMax)
		{
			return fragmentsMax * 2 / (level * level + level) + 1;
		}

		private int GetLevel(float distance, float radius)
		{
			float num = distance / radius * 6f;
			int value = (int)num / 2 + 1;
			return Mathf.Clamp(value, 0, 10);
		}

		private List<MeshData> GetMeshData(GameObject obj)
		{
			SpriteRenderer[] componentsInChildren = obj.GetComponentsInChildren<SpriteRenderer>();
			List<MeshData> list = new List<MeshData>(componentsInChildren.Length);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				SpriteMesh spriteMesh = new SpriteMesh(componentsInChildren[i].sprite);
				Vector2 centroid = obj.transform.TransformPoint(spriteMesh.GetCentroidLocal());
				list.Add(new MeshData
				{
					spriteMesh = spriteMesh,
					spriteRenderer = componentsInChildren[i],
					centroid = centroid,
					sprite = componentsInChildren[i].sprite,
					gameObject = componentsInChildren[i].gameObject,
					parentObject = obj
				});
			}
			return list;
		}

		private bool IsExplodable(GameObject obj)
		{
			if (DontUseTag)
			{
				return obj.GetComponent<Explodable2D>() != null;
			}
			return obj.CompareTag(Tag);
		}

		private List<CutMesh> GetMeshList()
		{
			GameObject[] array;
			if (exploderObject == null)
			{
				array = null;
				if (DontUseTag)
				{
					UnityEngine.Object[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(Explodable2D));
					List<GameObject> list = new List<GameObject>(array2.Length);
					UnityEngine.Object[] array3 = array2;
					foreach (UnityEngine.Object @object in array3)
					{
						Explodable2D explodable2D = (Explodable2D)@object;
						if ((bool)explodable2D)
						{
							list.Add(explodable2D.gameObject);
						}
					}
					array = list.ToArray();
				}
				else
				{
					array = GameObject.FindGameObjectsWithTag(Tag);
				}
			}
			else
			{
				array = new GameObject[1]
				{
					exploderObject
				};
			}
			List<CutMesh> list2 = new List<CutMesh>(array.Length);
			GameObject[] array4 = array;
			foreach (GameObject gameObject in array4)
			{
				if ((!ExplodeSelf && gameObject == base.gameObject) || (gameObject != base.gameObject && ExplodeSelf && DisableRadiusScan))
				{
					continue;
				}
				float sqrMagnitude = (!(exploderObject == null)) ? 0f : (sqrMagnitude = (Exploder2DUtils.GetCentroid(gameObject) - mainCentroid).sqrMagnitude);
				if (!(sqrMagnitude < Radius * Radius))
				{
					continue;
				}
				List<MeshData> meshData = GetMeshData(gameObject);
				int count = meshData.Count;
				for (int k = 0; k < count; k++)
				{
					MeshData meshData2 = meshData[k];
					Vector2 centroid = meshData2.centroid;
					float magnitude = (centroid - mainCentroid).magnitude;
					object text;
					if (FragmentOptions.InheritSortingLayer)
					{
						MeshData meshData3 = meshData[k];
						text = meshData3.spriteRenderer.sortingLayerName;
					}
					else
					{
						text = FragmentOptions.SortingLayer;
					}
					string sortingLayer = (string)text;
					int num;
					if (FragmentOptions.InheritOrderInLayer)
					{
						MeshData meshData4 = meshData[k];
						num = meshData4.spriteRenderer.sortingOrder;
					}
					else
					{
						num = FragmentOptions.OrderInLayer;
					}
					int orderInLayer = num;
					Color color;
					if (FragmentOptions.InheritSpriteRendererColor)
					{
						MeshData meshData5 = meshData[k];
						color = meshData5.spriteRenderer.color;
					}
					else
					{
						color = FragmentOptions.SpriteRendererColor;
					}
					Color color2 = color;
					List<CutMesh> list3 = list2;
					CutMesh item = default(CutMesh);
					MeshData meshData6 = meshData[k];
					item.spriteMesh = meshData6.spriteMesh;
					MeshData meshData7 = meshData[k];
					item.centroidLocal = meshData7.gameObject.transform.InverseTransformPoint(centroid);
					MeshData meshData8 = meshData[k];
					item.transform = meshData8.gameObject.transform;
					MeshData meshData9 = meshData[k];
					item.sprite = meshData9.sprite;
					MeshData meshData10 = meshData[k];
					item.parent = meshData10.gameObject.transform.parent;
					MeshData meshData11 = meshData[k];
					item.position = meshData11.gameObject.transform.position;
					MeshData meshData12 = meshData[k];
					item.rotation = meshData12.gameObject.transform.rotation;
					MeshData meshData13 = meshData[k];
					item.localScale = meshData13.gameObject.transform.localScale;
					item.distance = magnitude;
					item.level = GetLevel(magnitude, Radius);
					MeshData meshData14 = meshData[k];
					item.original = meshData14.parentObject;
					item.sortingLayer = sortingLayer;
					item.orderInLayer = orderInLayer;
					item.color = color2;
					item.option = gameObject.GetComponent<Exploder2DOption>();
					list3.Add(item);
				}
			}
			if (list2.Count == 0)
			{
				return list2;
			}
			list2.Sort((CutMesh m0, CutMesh m1) => m0.level.CompareTo(m1.level));
			if (list2.Count > TargetFragments)
			{
				list2.RemoveRange(TargetFragments - 1, list2.Count - TargetFragments);
			}
			CutMesh cutMesh = list2[list2.Count - 1];
			int level = cutMesh.level;
			int levelFragments = GetLevelFragments(level, TargetFragments);
			int num2 = 0;
			int count2 = list2.Count;
			int[] array5 = new int[level + 1];
			foreach (CutMesh item2 in list2)
			{
				CutMesh current = item2;
				array5[current.level]++;
			}
			for (int l = 0; l < count2; l++)
			{
				CutMesh value = list2[l];
				int num3 = level + 1 - value.level;
				num2 += (value.fragments = num3 * levelFragments / array5[value.level]);
				list2[l] = value;
				if (num2 >= TargetFragments)
				{
					value.fragments -= num2 - TargetFragments;
					num2 -= num2 - TargetFragments;
					list2[l] = value;
					break;
				}
			}
			return list2;
		}

		private void Update()
		{
			long cuttingTime = 0L;
			switch (state)
			{
			default:
				return;
			case State.None:
				return;
			case State.Preprocess:
				timer.Reset();
				timer.Start();
				if (!Preprocess())
				{
					OnExplosionFinished(success: false);
					return;
				}
				state = State.ProcessCutter;
				goto case State.ProcessCutter;
			case State.ProcessCutter:
			{
				bool flag = false;
				if (CuttingStrategy == CutStrategy.Randomized)
				{
					flag = ProcessCutterRandomized(out cuttingTime);
				}
				if (flag)
				{
					poolIdx = 0;
					postList = new List<CutMesh>(meshSet);
					if (splitMeshIslands)
					{
						islands = new List<CutMesh>(meshSet.Count);
						state = State.IsolateMeshIslands;
						goto case State.IsolateMeshIslands;
					}
					state = State.PostprocessInit;
					goto case State.PostprocessInit;
				}
				return;
			}
			case State.IsolateMeshIslands:
				if (IsolateMeshIslands(ref cuttingTime))
				{
					state = State.PostprocessInit;
					goto case State.PostprocessInit;
				}
				return;
			case State.PostprocessInit:
				InitPostprocess();
				state = State.Postprocess;
				break;
			case State.Postprocess:
				break;
			}
			if (Postprocess(cuttingTime))
			{
				timer.Stop();
			}
		}

		private bool Preprocess()
		{
			List<CutMesh> meshList = GetMeshList();
			if (meshList.Count == 0)
			{
				return false;
			}
			newFragments.Clear();
			meshToRemove.Clear();
			meshToCut.Clear();
			meshSet = new HashSet<CutMesh>(meshList);
			splitMeshIslands = SplitMeshIslands;
			CutMesh cutMesh = meshList[meshList.Count - 1];
			int level = cutMesh.level;
			levelCount = new int[level + 1];
			foreach (CutMesh item in meshSet)
			{
				CutMesh current = item;
				levelCount[current.level] += current.fragments;
			}
			if (UniformFragmentDistribution)
			{
				int[] array = new int[64];
				foreach (CutMesh item2 in meshSet)
				{
					CutMesh current2 = item2;
					array[current2.level]++;
				}
				int num = TargetFragments / meshSet.Count;
				foreach (CutMesh item3 in meshSet)
				{
					CutMesh current3 = item3;
					levelCount[current3.level] = num * array[current3.level];
				}
			}
			return true;
		}

		private bool IsolateMeshIslands(ref long timeOffset)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int count = postList.Count;
			while (poolIdx < count)
			{
				CutMesh item = postList[poolIdx];
				poolIdx++;
				bool flag = false;
				if (SplitMeshIslands || ((bool)item.option && item.option.SplitMeshIslands))
				{
					List<CutterMesh> list = MeshUtils.IsolateMeshIslands(item.spriteMesh);
					if (list != null)
					{
						flag = true;
						foreach (CutterMesh item2 in list)
						{
							CutterMesh current = item2;
							islands.Add(new CutMesh
							{
								spriteMesh = current.mesh,
								centroidLocal = current.centroidLocal,
								sprite = item.sprite,
								vertices = item.vertices,
								transform = item.transform,
								distance = item.distance,
								level = item.level,
								fragments = item.fragments,
								original = item.original,
								parent = item.transform.parent,
								position = item.transform.position,
								rotation = item.transform.rotation,
								localScale = item.transform.localScale,
								sortingLayer = item.sortingLayer,
								orderInLayer = item.orderInLayer,
								color = item.color,
								option = item.option
							});
						}
					}
				}
				if (!flag)
				{
					islands.Add(item);
				}
				if ((float)(stopwatch.ElapsedMilliseconds + timeOffset) > FrameBudget)
				{
					return false;
				}
			}
			postList = islands;
			return true;
		}

		private void InitPostprocess()
		{
			int count = postList.Count;
			FragmentPool2D.Instance.Allocate(count);
			FragmentPool2D.Instance.SetDeactivateOptions(DeactivateOptions, FadeoutOptions, DeactivateTimeout);
			FragmentPool2D.Instance.SetExplodableFragments(ExplodeFragments, DontUseTag);
			FragmentPool2D.Instance.SetFragmentPhysicsOptions(FragmentOptions);
			FragmentPool2D.Instance.SetSFXOptions(SFXOptions);
			poolIdx = 0;
			pool = FragmentPool2D.Instance.GetAvailableFragments(count);
			if (ExplosionCallback != null)
			{
				ExplosionCallback(timer.ElapsedMilliseconds, ExplosionState.ExplosionStarted);
			}
			if ((bool)SFXOptions.ExplosionSoundClip)
			{
				if (!audioSource)
				{
					audioSource = base.gameObject.AddComponent<AudioSource>();
				}
				audioSource.PlayOneShot(SFXOptions.ExplosionSoundClip);
			}
		}

		private void PostCrackExplode(OnExplosion callback)
		{
			callback?.Invoke(0f, ExplosionState.ExplosionStarted);
			int count = postList.Count;
			poolIdx = 0;
			while (poolIdx < count)
			{
				Fragment2D fragment2D = pool[poolIdx];
				CutMesh cutMesh = postList[poolIdx];
				poolIdx++;
				if (cutMesh.original != base.gameObject)
				{
					Exploder2DUtils.SetActiveRecursively(cutMesh.original, status: false);
				}
				else
				{
					Exploder2DUtils.EnableCollider(cutMesh.original, status: false);
					Exploder2DUtils.SetVisible(cutMesh.original, status: false);
				}
				fragment2D.Explode();
			}
			if (DestroyOriginalObject)
			{
				foreach (CutMesh post in postList)
				{
					CutMesh current = post;
					if ((bool)current.original && !current.original.GetComponent<Fragment2D>())
					{
						UnityEngine.Object.Destroy(current.original);
					}
				}
			}
			if (ExplodeSelf && !DestroyOriginalObject)
			{
				Exploder2DUtils.SetActiveRecursively(base.gameObject, status: false);
			}
			if (HideSelf)
			{
				Exploder2DUtils.SetActiveRecursively(base.gameObject, status: false);
			}
			ExplosionCallback = callback;
			OnExplosionFinished(success: true);
		}

		private bool Postprocess(long timeOffset)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int count = postList.Count;
			while (poolIdx < count)
			{
				Fragment2D fragment2D = pool[poolIdx];
				CutMesh cutMesh = postList[poolIdx];
				poolIdx++;
				if (!cutMesh.original)
				{
					continue;
				}
				if (crack)
				{
					Exploder2DUtils.SetActiveRecursively(fragment2D.gameObject, status: false);
				}
				fragment2D.CreateSprite(cutMesh.spriteMesh, cutMesh.sprite, cutMesh.original.transform, cutMesh.sortingLayer, cutMesh.orderInLayer, cutMesh.color);
				Transform parent = fragment2D.transform.parent;
				fragment2D.transform.parent = cutMesh.parent;
				Transform transform = fragment2D.transform;
				float x = cutMesh.position.x;
				float y = cutMesh.position.y;
				Vector3 position = cutMesh.original.transform.position;
				transform.position = new Vector3(x, y, position.z);
				fragment2D.transform.rotation = cutMesh.rotation;
				fragment2D.transform.localScale = cutMesh.localScale;
				fragment2D.transform.parent = null;
				fragment2D.transform.parent = parent;
				if (!crack)
				{
					if (cutMesh.original != base.gameObject)
					{
						Exploder2DUtils.SetActiveRecursively(cutMesh.original, status: false);
					}
					else
					{
						Exploder2DUtils.EnableCollider(cutMesh.original, status: false);
						Exploder2DUtils.SetVisible(cutMesh.original, status: false);
					}
				}
				if (!FragmentOptions.DisableColliders)
				{
					MeshUtils.GeneratePolygonCollider(fragment2D.polygonCollider2D, cutMesh.spriteMesh);
				}
				if ((bool)cutMesh.option)
				{
					cutMesh.option.DuplicateSettings(fragment2D.options);
				}
				if (!crack)
				{
					fragment2D.Explode();
				}
				float force = Force;
				if ((bool)cutMesh.option && cutMesh.option.UseLocalForce)
				{
					force = cutMesh.option.Force;
				}
				fragment2D.ApplyExplosion2D(cutMesh.transform, cutMesh.centroidLocal, mainCentroid, FragmentOptions, UseForceVector, ForceVector, force, cutMesh.original, TargetFragments);
				if (!((float)(stopwatch.ElapsedMilliseconds + timeOffset) > FrameBudget))
				{
					continue;
				}
				return false;
			}
			if (!crack)
			{
				if (DestroyOriginalObject)
				{
					foreach (CutMesh post in postList)
					{
						CutMesh current = post;
						if ((bool)current.original && !current.original.GetComponent<Fragment2D>())
						{
							UnityEngine.Object.Destroy(current.original);
						}
					}
				}
				if (ExplodeSelf && !DestroyOriginalObject)
				{
					Exploder2DUtils.SetActiveRecursively(base.gameObject, status: false);
				}
				if (HideSelf)
				{
					Exploder2DUtils.SetActiveRecursively(base.gameObject, status: false);
				}
				OnExplosionFinished(success: true);
			}
			else
			{
				cracked = true;
				if (CrackedCallback != null)
				{
					CrackedCallback();
				}
			}
			return true;
		}

		private void OnExplosionFinished(bool success)
		{
			if (ExplosionCallback != null)
			{
				if (!success)
				{
					ExplosionCallback(timer.ElapsedMilliseconds, ExplosionState.ExplosionStarted);
					OnExplosionStarted();
				}
				ExplosionCallback(timer.ElapsedMilliseconds, ExplosionState.ExplosionFinished);
			}
			state = State.None;
			queue.OnExplosionFinished(explosionID);
		}

		private void OnExplosionStarted()
		{
		}
	}
}
