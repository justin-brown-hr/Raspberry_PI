using Delaunay.Geo;
using Delaunay.LR;
using Delaunay.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
	public sealed class Voronoi : IDisposable
	{
		private SiteList _sites;

		private Dictionary<Vector2, Site> _sitesIndexedByLocation;

		private List<Triangle> _triangles;

		private List<Edge> _edges;

		private Rect _plotBounds;

		private Site fortunesAlgorithm_bottomMostSite;

		public Rect plotBounds => _plotBounds;

		public Voronoi(List<Vector2> points, List<uint> colors, Rect plotBounds)
		{
			_sites = new SiteList();
			_sitesIndexedByLocation = new Dictionary<Vector2, Site>();
			AddSites(points, colors);
			_plotBounds = plotBounds;
			_triangles = new List<Triangle>();
			_edges = new List<Edge>();
			FortunesAlgorithm();
		}

		public void Dispose()
		{
			if (_sites != null)
			{
				_sites.Dispose();
				_sites = null;
			}
			if (_triangles != null)
			{
				int count = _triangles.Count;
				for (int i = 0; i < count; i++)
				{
					_triangles[i].Dispose();
				}
				_triangles.Clear();
				_triangles = null;
			}
			if (_edges != null)
			{
				int count = _edges.Count;
				for (int i = 0; i < count; i++)
				{
					_edges[i].Dispose();
				}
				_edges.Clear();
				_edges = null;
			}
			_sitesIndexedByLocation = null;
		}

		private void AddSites(List<Vector2> points, List<uint> colors)
		{
			int count = points.Count;
			for (int i = 0; i < count; i++)
			{
				AddSite(points[i], (colors != null) ? colors[i] : 0u, i);
			}
		}

		private void AddSite(Vector2 p, uint color, int index)
		{
			if (!_sitesIndexedByLocation.ContainsKey(p))
			{
				float weight = Random.value * 100f;
				Site site = Site.Create(p, (uint)index, weight, color);
				_sites.Add(site);
				_sitesIndexedByLocation[p] = site;
			}
		}

		public List<Edge> Edges()
		{
			return _edges;
		}

		public List<Triangle> Triangles()
		{
			return _triangles;
		}

		public List<Vector2> Region(Vector2 p)
		{
			Site site = _sitesIndexedByLocation[p];
			if (site == null)
			{
				return new List<Vector2>();
			}
			return site.Region(_plotBounds);
		}

		public SiteList Sites()
		{
			return _sites;
		}

		public List<Vector2> NeighborSitesForSite(Vector2 coord)
		{
			List<Vector2> list = new List<Vector2>();
			Site site = _sitesIndexedByLocation[coord];
			if (site == null)
			{
				return list;
			}
			List<Site> list2 = site.NeighborSites();
			for (int i = 0; i < list2.Count; i++)
			{
				Site site2 = list2[i];
				list.Add(site2.Coord);
			}
			return list;
		}

		public List<Circle> Circles()
		{
			return _sites.Circles();
		}

		public List<LineSegment> VoronoiBoundaryForSite(Vector2 coord)
		{
			return DelaunayHelpers.VisibleLineSegments(DelaunayHelpers.SelectEdgesForSitePoint(coord, _edges));
		}

		public List<LineSegment> DelaunayLinesForSite(Vector2 coord)
		{
			return DelaunayHelpers.DelaunayLinesForEdges(DelaunayHelpers.SelectEdgesForSitePoint(coord, _edges));
		}

		public List<LineSegment> VoronoiDiagram()
		{
			return DelaunayHelpers.VisibleLineSegments(_edges);
		}

		public List<LineSegment> DelaunayTriangulation()
		{
			return DelaunayHelpers.DelaunayLinesForEdges(DelaunayHelpers.SelectNonIntersectingEdges(_edges));
		}

		public List<LineSegment> Hull()
		{
			return DelaunayHelpers.DelaunayLinesForEdges(HullEdges());
		}

		private List<Edge> HullEdges()
		{
			return _edges.FindAll((Edge edge) => edge.IsPartOfConvexHull());
		}

		public List<Vector2> HullPointsInOrder()
		{
			List<Edge> list = HullEdges();
			List<Vector2> list2 = new List<Vector2>();
			if (list.Count == 0)
			{
				return list2;
			}
			EdgeReorderer edgeReorderer = new EdgeReorderer(list, VertexOrSite.SITE);
			list = edgeReorderer.edges;
			List<Side> edgeOrientations = edgeReorderer.edgeOrientations;
			edgeReorderer.Dispose();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				Edge edge = list[i];
				Side leftRight = edgeOrientations[i];
				list2.Add(edge.Site(leftRight).Coord);
			}
			return list2;
		}

		public List<LineSegment> SpanningTree(KruskalType type = KruskalType.MINIMUM)
		{
			List<Edge> edges = DelaunayHelpers.SelectNonIntersectingEdges(_edges);
			List<LineSegment> lineSegments = DelaunayHelpers.DelaunayLinesForEdges(edges);
			return DelaunayHelpers.Kruskal(lineSegments, type);
		}

		public List<List<Vector2>> Regions()
		{
			return _sites.Regions(_plotBounds);
		}

		public List<uint> SiteColors()
		{
			return _sites.SiteColors();
		}

		public Vector2? NearestSitePoint(float x, float y)
		{
			return _sites.NearestSitePoint(x, y);
		}

		public List<Vector2> SiteCoords()
		{
			return _sites.SiteCoords();
		}

		private void FortunesAlgorithm()
		{
			Vector2 s = Vector2.zero;
			Rect sitesBounds = _sites.GetSitesBounds();
			int sqrt_nsites = (int)Mathf.Sqrt(_sites.Count + 4);
			HalfedgePriorityQueue halfedgePriorityQueue = new HalfedgePriorityQueue(sitesBounds.y, sitesBounds.height, sqrt_nsites);
			EdgeList edgeList = new EdgeList(sitesBounds.x, sitesBounds.width, sqrt_nsites);
			List<Halfedge> list = new List<Halfedge>();
			List<Vertex> list2 = new List<Vertex>();
			fortunesAlgorithm_bottomMostSite = _sites.Next();
			Site site = _sites.Next();
			while (true)
			{
				if (!halfedgePriorityQueue.Empty())
				{
					s = halfedgePriorityQueue.Min();
				}
				if (site != null && (halfedgePriorityQueue.Empty() || CompareByYThenX(site, s) < 0))
				{
					Halfedge halfedge = edgeList.EdgeListLeftNeighbor(site.Coord);
					Halfedge edgeListRightNeighbor = halfedge.edgeListRightNeighbor;
					Site site2 = FortunesAlgorithm_rightRegion(halfedge);
					Edge edge = Edge.CreateBisectingEdge(site2, site);
					_edges.Add(edge);
					Halfedge halfedge2 = Halfedge.Create(edge, Side.LEFT);
					list.Add(halfedge2);
					edgeList.Insert(halfedge, halfedge2);
					Vertex vertex;
					if ((vertex = Vertex.Intersect(halfedge, halfedge2)) != null)
					{
						list2.Add(vertex);
						halfedgePriorityQueue.Remove(halfedge);
						halfedge.vertex = vertex;
						halfedge.ystar = vertex.y + site.Dist(vertex);
						halfedgePriorityQueue.Insert(halfedge);
					}
					halfedge = halfedge2;
					halfedge2 = Halfedge.Create(edge, Side.RIGHT);
					list.Add(halfedge2);
					edgeList.Insert(halfedge, halfedge2);
					if ((vertex = Vertex.Intersect(halfedge2, edgeListRightNeighbor)) != null)
					{
						list2.Add(vertex);
						halfedge2.vertex = vertex;
						halfedge2.ystar = vertex.y + site.Dist(vertex);
						halfedgePriorityQueue.Insert(halfedge2);
					}
					site = _sites.Next();
					continue;
				}
				if (!halfedgePriorityQueue.Empty())
				{
					Halfedge halfedge = halfedgePriorityQueue.ExtractMin();
					Halfedge edgeListLeftNeighbor = halfedge.edgeListLeftNeighbor;
					Halfedge edgeListRightNeighbor = halfedge.edgeListRightNeighbor;
					Halfedge edgeListRightNeighbor2 = edgeListRightNeighbor.edgeListRightNeighbor;
					Site site2 = FortunesAlgorithm_leftRegion(halfedge);
					Site site3 = FortunesAlgorithm_rightRegion(edgeListRightNeighbor);
					_triangles.Add(new Triangle(site2, site3, FortunesAlgorithm_rightRegion(halfedge)));
					Vertex vertex2 = halfedge.vertex;
					vertex2.SetIndex();
					Edge edge2 = halfedge.edge;
					Side? leftRight = halfedge.leftRight;
					edge2.SetVertex(leftRight.Value, vertex2);
					Edge edge3 = edgeListRightNeighbor.edge;
					Side? leftRight2 = edgeListRightNeighbor.leftRight;
					edge3.SetVertex(leftRight2.Value, vertex2);
					edgeList.Remove(halfedge);
					halfedgePriorityQueue.Remove(edgeListRightNeighbor);
					edgeList.Remove(edgeListRightNeighbor);
					Side side = Side.LEFT;
					if (site2.y > site3.y)
					{
						Site site4 = site2;
						site2 = site3;
						site3 = site4;
						side = Side.RIGHT;
					}
					Edge edge = Edge.CreateBisectingEdge(site2, site3);
					_edges.Add(edge);
					Halfedge halfedge2 = Halfedge.Create(edge, side);
					list.Add(halfedge2);
					edgeList.Insert(edgeListLeftNeighbor, halfedge2);
					edge.SetVertex(SideHelper.Other(side), vertex2);
					Vertex vertex;
					if ((vertex = Vertex.Intersect(edgeListLeftNeighbor, halfedge2)) != null)
					{
						list2.Add(vertex);
						halfedgePriorityQueue.Remove(edgeListLeftNeighbor);
						edgeListLeftNeighbor.vertex = vertex;
						edgeListLeftNeighbor.ystar = vertex.y + site2.Dist(vertex);
						halfedgePriorityQueue.Insert(edgeListLeftNeighbor);
					}
					if ((vertex = Vertex.Intersect(halfedge2, edgeListRightNeighbor2)) != null)
					{
						list2.Add(vertex);
						halfedge2.vertex = vertex;
						halfedge2.ystar = vertex.y + site2.Dist(vertex);
						halfedgePriorityQueue.Insert(halfedge2);
					}
					continue;
				}
				break;
			}
			halfedgePriorityQueue.Dispose();
			edgeList.Dispose();
			for (int i = 0; i < list.Count; i++)
			{
				Halfedge halfedge3 = list[i];
				halfedge3.ReallyDispose();
			}
			list.Clear();
			for (int j = 0; j < _edges.Count; j++)
			{
				Edge edge = _edges[j];
				edge.ClipVertices(_plotBounds);
			}
			for (int k = 0; k < list2.Count; k++)
			{
				Vertex vertex = list2[k];
				vertex.Dispose();
			}
			list2.Clear();
		}

		private Site FortunesAlgorithm_leftRegion(Halfedge he)
		{
			Edge edge = he.edge;
			if (edge == null)
			{
				return fortunesAlgorithm_bottomMostSite;
			}
			Edge edge2 = edge;
			Side? leftRight = he.leftRight;
			return edge2.Site(leftRight.Value);
		}

		private Site FortunesAlgorithm_rightRegion(Halfedge he)
		{
			Edge edge = he.edge;
			if (edge == null)
			{
				return fortunesAlgorithm_bottomMostSite;
			}
			Edge edge2 = edge;
			Side? leftRight = he.leftRight;
			return edge2.Site(SideHelper.Other(leftRight.Value));
		}

		public static int CompareByYThenX(Site s1, Site s2)
		{
			if (s1.y < s2.y)
			{
				return -1;
			}
			if (s1.y > s2.y)
			{
				return 1;
			}
			if (s1.x < s2.x)
			{
				return -1;
			}
			if (s1.x > s2.x)
			{
				return 1;
			}
			return 0;
		}

		public static int CompareByYThenX(Site s1, Vector2 s2)
		{
			if (s1.y < s2.y)
			{
				return -1;
			}
			if (s1.y > s2.y)
			{
				return 1;
			}
			if (s1.x < s2.x)
			{
				return -1;
			}
			if (s1.x > s2.x)
			{
				return 1;
			}
			return 0;
		}
	}
}
