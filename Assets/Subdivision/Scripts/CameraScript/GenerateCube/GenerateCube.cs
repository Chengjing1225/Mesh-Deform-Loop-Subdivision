using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subdivision.Core;
using UnityEngine.UI;

enum FACEDIR:int
{
    UP,DOWN,FORWARD,BACK,LEFT,RIGHT
}
public struct FACEINFO
{
    public List<UnityEngine.Vector3> vertices;
    public List<UnityEngine.Vector3> normals;
    public List<int> triangleIndex;
    public bool hasFind;
}

public class GenerateCube : MonoBehaviour
{
    Text text;
    Mesh baseMesh;
    Shape shape;
    List<Mesh> faceMesh = new List<Mesh>();
    List<FACEINFO> faceInfos = new List<FACEINFO>();
    

    MeshConverter converter;
    LoopSubdivider subdivider;
    void Start()
    {
        text = GameObject.Find("cubeVerticeNumber").GetComponent<Text>();
        baseMesh = gameObject.GetComponent<MeshFilter>().mesh;
        InitInfo();
        FacePart();
        SubdivisionPart();
        gameObject.GetComponent<MeshFilter>().mesh = IntegralMesh();
        text.text = gameObject.GetComponent<MeshFilter>().mesh.vertexCount.ToString();
        gameObject.AddComponent<MeshCollider>();
        //gameObject.AddComponent<DeformerObject>();

        gameObject.transform.Rotate(new UnityEngine.Vector3(50, 30, 40));
    }





    void InitInfo()
    {
        
        FACEINFO upFace = new FACEINFO();
        FACEINFO downFace = new FACEINFO();
        FACEINFO forwardFace = new FACEINFO();
        FACEINFO backFace = new FACEINFO();
        FACEINFO leftFace = new FACEINFO();
        FACEINFO rightFace = new FACEINFO();

        faceInfos.Add(upFace);
        faceInfos.Add(downFace);
        faceInfos.Add(forwardFace);
        faceInfos.Add(backFace);
        faceInfos.Add(leftFace);
        faceInfos.Add(rightFace);

        
        Mesh upMesh = new Mesh();
        Mesh downMesh = new Mesh();
        Mesh forwardMesh = new Mesh();
        Mesh backMesh = new Mesh();
        Mesh leftMesh = new Mesh();
        Mesh rightMesh = new Mesh();


        faceMesh.Add(upMesh);
        faceMesh.Add(downMesh);
        faceMesh.Add(forwardMesh);
        faceMesh.Add(backMesh);
        faceMesh.Add(leftMesh);
        faceMesh.Add(rightMesh);

    }

    /// <summary>
    /// 分割立方体的面
    /// </summary>
    void FacePart()
    {
        List<int> oldTriangles = new List<int>(baseMesh.triangles);
        for(int i = 0; i < oldTriangles.Count; i = i+3)
        {
            UnityEngine.Vector3 vertice =baseMesh.vertices[oldTriangles[i]];  //顶点
            UnityEngine.Vector3 normal =baseMesh.normals[oldTriangles[i]];  //法线
            int faceDirIndex = 7;
            if (normal == UnityEngine.Vector3.up)
                faceDirIndex = (int)FACEDIR.UP;
            else if (normal == UnityEngine.Vector3.down)
                faceDirIndex = (int)FACEDIR.DOWN;
            else if (normal == UnityEngine.Vector3.forward)
                faceDirIndex = (int)FACEDIR.FORWARD;
            else if (normal == UnityEngine.Vector3.back)
                faceDirIndex = (int)FACEDIR.BACK;
            else if (normal == UnityEngine.Vector3.left)
                faceDirIndex = (int)FACEDIR.LEFT;
            else if (normal == UnityEngine.Vector3.right)
                faceDirIndex = (int)FACEDIR.RIGHT;
            FACEINFO f = faceInfos[faceDirIndex];
            if(f.vertices == null)
            {
                f.vertices = new List<UnityEngine.Vector3>();
                f.normals = new List<UnityEngine.Vector3>();
                f.triangleIndex = new List<int>();
            }
            for (int u = 0; u < 3; u++)
            {
                vertice = baseMesh.vertices[oldTriangles[i + u]];
                if (!f.vertices.Exists(v => v==vertice))
                {
                    f.vertices.Add(vertice);
                    f.normals.Add(normal);
                }
                f.triangleIndex.Add(f.vertices.FindIndex(v => v == vertice));
                
            }
           faceInfos[faceDirIndex] = f;
        }
        for(int i = 0; i < faceInfos.Count; i++)
        {

            faceMesh[i].vertices = faceInfos[i].vertices.ToArray();
            faceMesh[i].normals = faceInfos[i].normals.ToArray();
            faceMesh[i].triangles = faceInfos[i].triangleIndex.ToArray();
            faceMesh[i].RecalculateBounds();
        }

    }

    /// <summary>
    /// 细分各个面
    /// </summary>
    void SubdivisionPart()
    {
        int i = 0;
        foreach(var mesh in faceMesh)
        {
            converter = new MeshConverter();
            subdivider = new LoopSubdivider();
            shape = converter.OnConvert(mesh);
            shape = subdivider.SubdividePlane(shape);
            shape = subdivider.SubdividePlane(shape);
            shape = subdivider.SubdividePlane(shape);
            faceMesh[i] = converter.ConvertToPlaneMesh(shape);
            i++;
        }
    }

    /// <summary>
    /// 整合细分后的面
    /// </summary>
    /// <returns></returns>
    public Mesh IntegralMesh()
    {
        Mesh integralMesh = new Mesh();         //综合模型
        int k = 0;
        int t = 0;
        int VerLength = 6 * faceMesh[0].vertices.Length;           //模型的总顶点数
        int TriLength = 6 * faceMesh[0].triangles.Length;
        UnityEngine.Vector3[] inteV= new UnityEngine.Vector3[VerLength];
        UnityEngine.Vector3[] inteN= new UnityEngine.Vector3[VerLength];
        int[] inteT= new int[TriLength];

        for (int i = 0; i < faceMesh.Count; i++)
        {
            faceMesh[i].vertices.CopyTo(inteV, k);
            faceMesh[i].normals.CopyTo(inteN, k);
            for (int j = 0; j < faceMesh[i].triangles.Length; j++)
                inteT[t + j] = faceMesh[i].triangles[j] + k;
            k += faceMesh[i].vertices.Length;
            t += faceMesh[i].triangles.Length;
        }

        integralMesh.vertices = inteV;
        integralMesh.normals = inteN;
        integralMesh.triangles = inteT;
        integralMesh.RecalculateBounds();
        integralMesh.RecalculateNormals();
        integralMesh.RecalculateTangents();
        return integralMesh;
    }

}
