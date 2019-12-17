using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using Subdivision.Core;
using System.Collections.Generic;

public class Main : MonoBehaviour
{

    public Text text;
    public MeshFilter meshFilter;
    MeshConverter converter;
    LoopSubdivider subdivider;

    public Camera camera;
    private float radius = 0.3f;
    private float force = 0.3f;
    private float distance = 100;

    //struct defPoint
    //{
    //    public UnityEngine.Vector3 defPosition;
    //    public UnityEngine.Vector3 defNormal;
    //};

    public Shape shape;

    // Use this for initialization
    //void Start()
    void Awake()
    {
        converter = new MeshConverter();
        subdivider = new LoopSubdivider();
        shape = converter.OnConvert(meshFilter.mesh);
        text.text = shape.AllPoints.Count.ToString();
    }

    /// <summary>
    /// 按钮点击事件
    /// </summary>
    public void HandleOnSundive()
    {
        shape = subdivider.Subdivide(shape);
        text.text = shape.AllPoints.Count.ToString();
        meshFilter.mesh = converter.ConvertToMesh(shape);
    }
 
}


