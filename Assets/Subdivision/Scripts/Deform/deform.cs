using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using Subdivision.Core;
using UnityEngine.Collections;

//public struct DEFPOINT
//{
//    /// <summary>
//    /// 变形点的位置
//    /// </summary>    
//    public Vector3 defPosition;
//    public Vector3 defNormal;       //暂时无用
//};
public struct ADJDEFPOINT
{
    /// <summary>
    /// 变形点
    /// </summary>
    public Point point;
    ///// <summary>
    ///// 变形点是否被包含
    ///// </summary>
    //public bool isInclude;
    /// <summary>
    /// 变形点离变形中心的距离
    /// </summary>
    public double distance;
    ///// <summary>
    ///// 是否为边界点
    ///// </summary>
    //public bool isBorderPoint;
    ///// <summary>
    ///// 边界点对应的半边
    ///// </summary>
    //public List<Edge>  borderEdges;
    ///// <summary>
    ///// 变形后的位置
    ///// </summary>
    //public Vector3 Successor;
}




public class deform
{

    public deform(Vector3 def, Shape shape, Vector3 dir)
    {
        this.defPoint = def;
        this.shape = shape;
        this.direction = dir;
    }

    // DEFPOINT defP { get; set; }
    /// <summary>
    /// 触及点的位置坐标
    /// </summary>
    Vector3 defPoint;
    /// <summary>
    /// 模型形状
    /// </summary>
    Shape shape { get; set; }
    /// <summary>
    /// 变形中心点
    /// </summary>
    Point centerPoint;
    /// <summary>
    /// 变形点的集合
    /// </summary>
    List<ADJDEFPOINT> points = new List<ADJDEFPOINT>();
    /// <summary>
    /// 变形方向
    /// </summary>
    Vector3 direction = new Vector3();


    //[ReadOnly] public UnityEngine.Vector3 center;
    [ReadOnly] public float radius;
    [ReadOnly] public float force;


    /// <summary>
    /// 寻找最近的顶点
    /// </summary>
    /// <returns></returns>
    public Point SearchAdjPoint()
    {
        double minDistance = 100;
        Point point = new Point(defPoint);
        
        foreach(var v in shape.AllPoints)
        {
            double distance = Distance(defPoint, v.Position);
            if(distance < minDistance)
            {
                //foreach(var f in v.AllFaces)
                //{
                    minDistance = distance;
                    point = v;                 
               // }
            }                               
        }
        this.centerPoint = point;
        return point;  
    }

    /// <summary>
    /// 寻求变形点的集合
    /// </summary>
    /// <returns></returns>
    public List<ADJDEFPOINT> SearchDeformPoint()
    {      

        Point centerPoint = SearchAdjPoint();
        ADJDEFPOINT defPoint = new ADJDEFPOINT();
        defPoint.point = centerPoint;        
        //defPoint.distance = 0;
        //defPoint.isInclude = false;
        //defPoint.isBorderPoint = false;
        //defPoint.borderEdges = new List<Edge>();
        points.Add(defPoint);
        int i = 0;

        while (i != points.Count)
        {
            //ADJDEFPOINT defPoint = new ADJDEFPOINT();
            foreach (Edge edge in points[i].point.Edges)
            {
                Point p = edge.Points[0];
                if (edge.Points[0] == centerPoint)
                    p = edge.Points[1];
                double dis = Distance(p, centerPoint);
                if (dis  < radius || dis == radius)
                {
                    defPoint.point = p;                    
                    defPoint.distance = dis;
                    if (!points.Contains(defPoint))
                        points.Add(defPoint);
                }
            }
           // SearchPoints(points[i],i);
            i++;
        }

        return points;
    }


    /// <summary>
    /// 计算变形点的位移位置
    /// </summary>
    public List<ADJDEFPOINT> DefPoistion()
    {
        List<ADJDEFPOINT> points = SearchDeformPoint();
        foreach(var v in points)
        {
            double weight = -((v.distance * v.distance) / (radius * radius)) + 1;
            //double cosV = v.point.Normal.AngleCos(direction);
            v.point.Successor = new Point(v.point.Position + direction * weight);            
        }
        return points;
    }

    public Shape getShape()
    {
        generateShape newShapeClass = new generateShape();

        Shape newShape = newShapeClass.generate(shape, DefPoistion());

        return newShape;
    } 





    /// <summary>
    /// 计算距离
    /// </summary>
    /// <param name="vec0"></param>
    /// <param name="vec1"></param>
    /// <returns></returns>
    double Distance(Vector3 vec0, Vector3 vec1)
    {
        double distance;
        double x = (vec0.X - vec1.X);
        double y = (vec0.Y - vec1.Y);
        double z = (vec0.Z - vec1.Z);
        Vector3 disVec = new Vector3((vec0.X - vec1.X), (vec0.Y - vec1.Y), (vec0.Z - vec1.Z));
        distance = disVec.Length();
        return distance;
    }
    double Distance(Point P0, Point P1)
    {
        double distance;
        distance = Distance(P0.Position, P1.Position);
        return distance;
    }
}
