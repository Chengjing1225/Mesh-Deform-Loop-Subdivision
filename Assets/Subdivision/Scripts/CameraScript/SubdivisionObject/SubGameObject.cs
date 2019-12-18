using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Subdivision.Core;

public class SubGameObject : MonoBehaviour {

    Text subVerNumText;
    MeshFilter meshFilter;
    MeshConverter converter;
    LoopSubdivider subdivider;
    public Shape shape;

    void Start () {
        subVerNumText = GameObject.Find("SubVerNumText").GetComponent<Text>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        converter = new MeshConverter();
        subdivider = new LoopSubdivider();
        shape = converter.OnConvert(meshFilter.mesh);
        subVerNumText.text = shape.AllPoints.Count.ToString();
    }
	
	public void RunSub()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        converter = new MeshConverter();
        subdivider = new LoopSubdivider();
        shape = converter.OnConvert(meshFilter.mesh);

        shape = subdivider.Subdivide(shape);
        subVerNumText.text = shape.AllPoints.Count.ToString();
        meshFilter.mesh = converter.ConvertToMesh(shape);
        subVerNumText.text = shape.AllPoints.Count.ToString();
    }
}
