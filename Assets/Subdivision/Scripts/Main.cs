using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using Subdivision.Core;
using System.Collections.Generic;

public class Main : MonoBehaviour
{

    GameObject nowObj;
    GameObject lastObj;

    void Start()
    {
    }

    bool first = true;
    private void Update()
    {
        selectedObject();
    }

    /// <summary>
    /// 按钮点击事件
    /// </summary>
    public void HandleOnSundive()
    {
        nowObj.GetComponent<SubGameObject>().RunSub();        
    }

    public void GenerateSubCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "SubCube";
        cube.AddComponent<GenerateCube>();
        cube.transform.position = new UnityEngine.Vector3(0, 0, 2);
    }

    void selectedObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject != lastObj)
            {
                first = true;
            }
        }
        else
            return;
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0) && first ) 
        {
            nowObj = hit.transform.gameObject;
            nowObj.AddComponent<SubGameObject>();
            nowObj.AddComponent<DeformerObject>();
            first = false;
        }
        if (Input.GetMouseButtonUp(0) && !false)
            first = true;
       
       if(lastObj != nowObj )
        {
            if (lastObj != null)
            {
                Destroy(lastObj.GetComponent<SubGameObject>());
                Destroy(lastObj.GetComponent<DeformerObject>());                
            }                
            lastObj = nowObj;
        }
        else  if (lastObj == nowObj && nowObj != null)
             first = false;
        
     }
 
}


