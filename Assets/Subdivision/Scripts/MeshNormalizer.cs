using UnityEngine;
using System.Collections.Generic;
using Subdivision.Core;
using System.Linq;

public class MeshNormalizer
{

    Mesh srcMesh;

    int[] srcTriangles;
    UnityEngine.Vector3[] srcVertices;
    UnityEngine.Vector2[] srcUVs;
    UnityEngine.Vector3[] srcNormals;

    Lazy<List<UnityEngine.Vector3>> uniqueVertices;
    public List<UnityEngine.Vector3> UniqueVertices
    {
        get
        {
            return uniqueVertices.Value;
        }
    }

    Lazy<List<int>> uniqueTriangles;
    public List<int> UniqueTriangles
    {
        get
        {
            return uniqueTriangles.Value;
        }
    }

    Lazy<List<UnityEngine.Vector3>> uniqueNormals;
    public List<UnityEngine.Vector3> UniqueNormals
    {
        get
        {
            return uniqueNormals.Value;
        }
    }

    /// <summary>
    /// 标准化网格信息
    /// </summary>
    /// <param name="_mesh">网格</param>
    public MeshNormalizer(Mesh _mesh)
    {
        srcMesh = _mesh;

        srcTriangles = srcMesh.triangles;
        srcVertices = srcMesh.vertices;
        srcUVs = srcMesh.uv;
        srcNormals = srcMesh.normals;

        uniqueVertices = new Lazy<List<UnityEngine.Vector3>>(OnUnipueVertices);
        uniqueTriangles = new Lazy<List<int>>(OnUniqueTriangles);
        uniqueNormals = new Lazy<List<UnityEngine.Vector3>>(OnUniqueNormals);
    }

   

/*
    //simplify
   public Mesh UnipueMesh()
    {

        / *int[] triangles = mesh.triangles;
        UnityEngine.Vector3[] vertices = mesh.vertices;
        UnityEngine.Vector2[] uvs = mesh.uv;
        UnityEngine.Vector3[] normals = mesh.normals;* /

        //List<UnityEngine.Vector3> uniqueVertices = UnipueVertices();

        //List<int> uniqueTriangles = UniqueTriangles();

        Mesh m = new Mesh();

        m.vertices = UniqueVertices.ToArray();
        m.triangles = UniqueTriangles.ToArray();
        m.normals = UniqueNormals.ToArray();
        return m;
    }*/

    /// <summary>
    /// 返回不重复的三角形
    /// </summary>
    /// <returns>返回三角形的索引</returns>
    List<int> OnUniqueTriangles()
    {
        List<int> result = new List<int>();
        //List<UnityEngine.Vector3> uniqueVertices = UnipueVertices();

        foreach (var n in srcTriangles)
        {

            UnityEngine.Vector3 pt = srcVertices[n];

            result.Add(UniqueVertices.IndexOf(pt));

        }

        return result;
    }

    /// <summary>
    /// 没有重复顶点的顶点集
    /// </summary>
    /// <returns>返回原顶点的顶点集</returns>
    List<UnityEngine.Vector3> OnUnipueVertices()
    {
        List<UnityEngine.Vector3> result = new List<UnityEngine.Vector3>();

        foreach (var pt in srcVertices)
        {
            if (!result.Contains(pt))
            {
                result.Add(pt);
            }
        }
        return result;
    }

    /// <summary>
    /// 不重复的法线
    /// </summary>
    /// <returns></returns>
    List<UnityEngine.Vector3> OnUniqueNormals()
    {
        List<UnityEngine.Vector3> result = new List<UnityEngine.Vector3>();

        foreach(var v in UniqueVertices)
        {

            List<UnityEngine.Vector3> tempNormals = new List<UnityEngine.Vector3>();

            for(int i = 0; i < srcVertices.Length; i++)
            {
                if (srcVertices[i] == v)
                    tempNormals.Add(srcNormals[i]);
            }

            UnityEngine.Vector3 normal = UnityVectorUtils.Average(tempNormals);

            result.Add(normal.normalized);

        }

        foreach (var n in result)
        {
            Debug.Log(n);
        }

        return result;
    }

}
