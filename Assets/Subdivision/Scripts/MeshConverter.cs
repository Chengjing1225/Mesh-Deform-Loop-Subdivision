using UnityEngine;
using System.Collections.Generic;
using Subdivision.Core;

public class MeshConverter
{ 
    /// <summary>
    /// 将网格转换成形状
    /// </summary>
    /// <param name="mesh">原网格</param>
    /// <returns>新的模型</returns>
    public Shape OnConvert(Mesh mesh )
    {
        MeshNormalizer normalizer = new MeshNormalizer(mesh);
        List<UnityEngine.Vector3> vertices = normalizer.UniqueVertices;
        List<int> triangles = normalizer.UniqueTriangles;

        List<Subdivision.Core.Point> points = new List<Subdivision.Core.Point>();

        foreach(var v in normalizer.UniqueVertices)
        {
            points.Add(new Subdivision.Core.Point(v.x, v.y, v.z));
        }

        List<Subdivision.Core.Edge> edges = new List<Subdivision.Core.Edge>();

        Shape shape = new Shape();
        for (int i = 0; i < normalizer.UniqueTriangles.Count / 3; i++)
        {
            Subdivision.Core.Point p0 = points[triangles[3 * i]];
            Subdivision.Core.Point p1 = points[triangles[3 * i + 1]];
            Subdivision.Core.Point p2 = points[triangles[3 * i + 2]];

            Subdivision.Core.Edge e0 = new Subdivision.Core.Edge(p0, p1);
            Subdivision.Core.Edge e1 = new Subdivision.Core.Edge(p1, p2);
            Subdivision.Core.Edge e2 = new Subdivision.Core.Edge(p2, p0);

            bool b0 = true;
            bool b1 = true;
            bool b2 = true;
            foreach (Subdivision.Core.Edge ee in edges)
            {
                if (e0.IsMatchFor(ee)) { e0 = ee; b0 = false; }
                if (e1.IsMatchFor(ee)) { e1 = ee; b1 = false; }
                if (e2.IsMatchFor(ee)) { e2 = ee; b2 = false; }
            }

            if (b0) edges.Add(e0);
            if (b1) edges.Add(e1);
            if (b2) edges.Add(e2);

            shape.AddFace(new Face(e0, e1, e2));

        }

        return shape;

    }

    /// <summary>
    /// 将shape转换成新的网格信息
    /// </summary>
    /// <param name="shape"></param>
    /// <returns>网格</returns>
    public Mesh ConvertToMesh(Shape shape)
    {
        Mesh mesh = new Mesh();
        List<UnityEngine.Vector3> vertices = new List<UnityEngine.Vector3>();
        List<int> triangles = new List<int>();
        List<UnityEngine.Vector3> normals = new List<UnityEngine.Vector3>();

        int n = 0;
        UnityEngine.Vector3 normal;
        foreach (var face in shape.Faces)
        {
            //Debug.Log(face.AllPoints.Count);
            vertices.AddRange(ConvertToVectors(face.AllPoints.ToArray()));
            int length = vertices.Count;
            normal = calculateCorss(vertices[length - 2] - vertices[length - 3],vertices[length - 1] - vertices[length - 2]);

            if (face.AllPoints.Count == 3)
            {
                triangles.AddRange(new int[] { 3 * n, 3 * n + 1, 3 * n + 2 });
                normals.AddRange(new UnityEngine.Vector3[] { normal, normal, normal });
            }
            else if (face.AllPoints.Count == 4)
            {

                triangles.AddRange(new int[] { 4 * n, 4 * n + 1, 4 * n + 2, 4 * n, 4 * n + 2, 4 * n + 3 });
                normals.AddRange(new UnityEngine.Vector3[] { normal, normal, normal,normal });
            }
            n++;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();


        return mesh;
    }


    /// <summary>
    /// 将平面shape转换成新的网格信息
    /// </summary>
    /// <param name="shape">平面shape</param>
    /// <returns>平面网格信息</returns>
    public Mesh ConvertToPlaneMesh(Shape shape)
    {
        Mesh mesh = new Mesh();
        List<UnityEngine.Vector3> vertices = new List<UnityEngine.Vector3>();
        List<int> triangles = new List<int>();
        List<UnityEngine.Vector3> normals = new List<UnityEngine.Vector3>();

        vertices.AddRange(ConvertToVectors(shape.AllPoints.ToArray()));
        int n = 0;
        UnityEngine.Vector3 normal;
        foreach (var face in shape.Faces)
        {
            //Debug.Log(face.AllPoints.Count);
           
            //int length = vertices.Count;
            //normal = calculateCorss(vertices[length - 2] - vertices[length - 3], vertices[length - 1] - vertices[length - 2]);

            if (face.AllPoints.Count == 3)
            {
                UnityEngine.Vector3[] faceVertices = ConvertToVectors(face.AllPoints.ToArray());
                for(int u = 0; u < 3; u++)
                {
                    triangles.Add(vertices.FindIndex(v => v == faceVertices[u]));
                }
                
                //triangles.AddRange(new int[] { 3 * n, 3 * n + 1, 3 * n + 2 });
                //normals.AddRange(new UnityEngine.Vector3[] { normal, normal, normal });
            }
            else if (face.AllPoints.Count == 4)
            {

                //triangles.AddRange(new int[] { 4 * n, 4 * n + 1, 4 * n + 2, 4 * n, 4 * n + 2, 4 * n + 3 });
                //normals.AddRange(new UnityEngine.Vector3[] { normal, normal, normal, normal });
                UnityEngine.Vector3[] faceVertices = ConvertToVectors(shape.AllPoints.ToArray());
                for (int u = 0; u < 4; u++)
                {
                    triangles.Add(vertices.FindIndex(v => v == faceVertices[u]));
                }
            }
            n++;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();


        return mesh;
    }

    /// <summary>
    /// 顶点位置转化成三维坐标
    /// </summary>
    /// <param name="pt"></param>
    /// <returns>三维坐标值</returns>
    UnityEngine.Vector3 ConvertToVector(Subdivision.Core.Point pt)
    {
        return new UnityEngine.Vector3((float)pt.Position.X, (float)pt.Position.Y, (float)pt.Position.Z);
    }

    /// <summary>
    /// 顶点坐标值
    /// </summary>
    /// <param name="pts"></param>
    /// <returns>返回顶点坐标值</returns>
    UnityEngine.Vector3[] ConvertToVectors(params Subdivision.Core.Point[] pts)
    {
        List<UnityEngine.Vector3> v3s = new List<UnityEngine.Vector3>();
        foreach (var pt in pts)
        {
            v3s.Add(ConvertToVector(pt));
        }

        return v3s.ToArray();
    }
    UnityEngine.Vector3 calculateCorss(UnityEngine.Vector3 v0, UnityEngine.Vector3 v1)
    {
        UnityEngine.Vector3 crossMuti = new UnityEngine.Vector3();
        crossMuti.x = v0.y * v1.z - v0.z * v1.y;
        crossMuti.y = v0.z * v1.x - v0.x * v1.z;
        crossMuti.z = v0.x * v1.y - v0.y * v1.x;
        return crossMuti.normalized;
    }



    Subdivision.Core.Point ConvertToPoint(UnityEngine.Vector3 v3)
    {
        return new Subdivision.Core.Point(v3.x, v3.y, v3.z);
    }
}
