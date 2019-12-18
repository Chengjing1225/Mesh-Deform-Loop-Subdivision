using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subdivision.Core;

public class DeformerObject : MonoBehaviour {

     Camera camera;
    MeshFilter meshFilter;
    MeshConverter converter;

    private float radius = 0.3f;
    private float force = 0.3f;
    private float distance = 100;

    Shape shape;
    /// <summary>
    /// 变形点
    /// </summary>
    Subdivision.Core.Vector3 defPoint;
    /// <summary>
    /// 变形拖拽的方向
    /// </summary>
    UnityEngine.Vector3 dragPos;
    UnityEngine.Vector3 clickDownPos;
    UnityEngine.Vector3 clickUpPos;


    void Start () {
        camera = Camera.main;
        meshFilter = gameObject.GetComponent<MeshFilter>();
        converter = new MeshConverter();
        shape = converter.OnConvert(meshFilter.mesh);
        
	}

    public void Update()
    {
        mouseClickModel();
    }

    public GameObject getCube(float x, float y, float z)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.SetParent(meshFilter.GetComponent<Transform>());
        go.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
        go.transform.localPosition = new UnityEngine.Vector3(x, y, z);
        return go;
    }



    bool first = true;
    /// <summary>
    /// 鼠标点击模型事件
    /// </summary>
    public void mouseClickModel()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Input.GetMouseButton(0))
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.green);
        }
        bool isHit = Physics.Raycast(ray, out hit, distance);
        if (isHit && hit.transform.gameObject == gameObject)
        {
            if (isHit && first && Input.GetMouseButtonDown(0))
            {
                first = false;
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                clickDownPos = getMousePosition();
                UnityEngine.Vector3 clickPos = meshFilter.transform.InverseTransformPoint(hit.point);
                defPoint = new Subdivision.Core.Vector3(clickPos.x, clickPos.y, clickPos.z);
            }
        }
        if (!first && Input.GetMouseButton(0))
            {
                clickUpPos = getMousePosition();
        }
        dragPos = clickUpPos - clickDownPos;
        if (!first && Input.GetMouseButtonUp(0))
        {
            print("dragPos" + dragPos);
            first = true;
            if (dragPos != new UnityEngine.Vector3(0, 0, 0))
                Deformation();
        }
    }
    /// <summary>
    /// 物体变形主方法
    /// </summary>
    public void Deformation()
    {
        Subdivision.Core.Vector3 dir = new Subdivision.Core.Vector3(dragPos.x, dragPos.y, dragPos.z);
        deform deform = new deform(defPoint, shape, dir);
        deform.force = force;
        deform.radius = radius;
        shape = deform.getShape();
        Destroy(meshFilter.GetComponent<Collider>());       //移除变形前的碰撞器
        meshFilter.mesh = converter.ConvertToMesh(shape);
        meshFilter.gameObject.AddComponent<MeshCollider>();     //增加新的碰撞器
    }

    /// <summary>
    /// 从屏幕获取坐标值（其坐标相对于变形物体）
    /// </summary>
    /// <returns></returns>
    UnityEngine.Vector3 getMousePosition()
    {
        UnityEngine.Vector3 mousePos;
        UnityEngine.Vector3 screenPos;
        UnityEngine.Vector3 mousePositionOnScreen;
        UnityEngine.Vector3 mousePositionInWorld;

        //获取鼠标在相机中（世界中）的位置，转换为屏幕坐标；
        screenPos = Camera.main.WorldToScreenPoint(meshFilter.transform.position);
        //获取鼠标在场景中坐标
        mousePositionOnScreen = Input.mousePosition;
        //让场景中的Z=鼠标坐标的Z
        mousePositionOnScreen.z = screenPos.z;
        //将相机中的坐标转化为世界坐标
        mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);
        mousePos = new UnityEngine.Vector3(mousePositionInWorld.x, mousePositionInWorld.y, mousePositionInWorld.z);
        //将世界坐标内的坐标转化为变形体的局部坐标
        mousePos = meshFilter.transform.InverseTransformPoint(mousePos);
        
        return mousePos;
    }

}
