using System;
using System.Collections.Generic;
using System.Linq;

namespace Subdivision.Core
{
    public class Point 
    {
        private readonly Lazy<Vector3> _lazyNormal;
        private readonly Lazy<List<Face>> _lazyAllFaces;

        public Point(double x, double y, double z)
            : this(new Vector3(x, y, z))
        {
        }

        public Point(Vector3 position)                  
        {
            Position = position;
            Edges = new List<Edge>();
            _lazyNormal = new Lazy<Vector3>(ComputeNormal);
            _lazyAllFaces = new Lazy<List<Face>>(GetAllFaces);
        }
        /// <summary>
        /// 顶点位置
        /// </summary>
        public Vector3 Position { get; set; }       
        /// <summary>
        /// 旧顶点的细分后的位置
        /// </summary>
        public Point Successor { get; set; }        
        /// <summary>
        /// 顶点上的半边
        /// </summary>
        public List<Edge> Edges { get; set; }       

        public bool IsOnBorderOfHole
        {
            get
            {
                // a point is on the border of a hole if nfaces != nedges 
                //  with nfaces the number of faces the point belongs to, 
                //  and nedges the number of edges a point belongs to.  

                // A simpler method is if any of the edges the point belongs to is on the border of a hole, then the point is too
                return Edges.Any(e => e.IsOnBorderOfHole);
            }
        }

        public Vector3 Normal { get { return _lazyNormal.Value; } }

        public List<Face> AllFaces { get { return _lazyAllFaces.Value; } }

        public override string ToString()
        {
            return Position.ToString();
        }

        private Vector3 ComputeNormal()
        {
            List<Face> faces = Edges.SelectMany(e => e.Faces).Distinct().ToList();  //distince 是去重
            List<Vector3> normals = faces.Select(f => f.Normal).ToList();
            return Vector3.Average(normals).Normalized();
        }

        private List<Face> GetAllFaces()
        {
            return
                Edges.SelectMany(e => e.Faces).Distinct().ToList();
        }
    }
}