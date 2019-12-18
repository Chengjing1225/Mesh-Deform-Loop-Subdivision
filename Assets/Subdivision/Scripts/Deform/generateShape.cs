using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Subdivision.Core;
using UnityEngine;


class generateShape 
{
    //public Shape generate(Shape shape, ADJDEFPOINT adjPoints)
    //{

    //    public Shape newShape = new Shape();



    //    return newShape;
    //}

    ///// <summary>
    ///// 构造新顶点
    ///// </summary>
    ///// <param name="shape"></param>
    //public Shape CreateNewShape(Shape shape, List<ADJDEFPOINT>  adjPoints)
    //{
    //    List<Point>  points = shape.AllPoints;           

    //    foreach(ADJDEFPOINT adjP in adjPoints)
    //    {                
    //        foreach(Point p in points)
    //        {
    //            if (p == adjP.point)
    //            {
    //                //p.Position = adjP.point.Successor.Position;         
    //                //print("p:" + p);
    //                //print("adjp:" + adjP);
    //            }
    //        }
    //    }
    //    return shape;

    //}

    public Shape generate(Shape shape, List<ADJDEFPOINT> adjPoints)
    {
        Shape newShape = new Shape();
        CreateFaces(shape, newShape, adjPoints);
        return newShape;
    }
     /// <summary>
     /// 创建面
     /// </summary>
     /// <param name="shape">原</param>
     /// <param name="newShape">新</param>
     /// <param name="adjPoints">变形点</param>
    private void CreateFaces(Shape shape,Shape newShape, List<ADJDEFPOINT> adjPoints)
    {
        List<Face> faces = shape.Faces;
        List<Edge> existingEdges = new List<Edge>();
        foreach (Face face in faces)
        {
            if (face.AllPoints.Count() == 3)
            {
                CreateTriangleFace(existingEdges, newShape, face,adjPoints);
            }
        }
    }

    /// <summary>
    /// 创建三角形
    /// </summary>
    /// <param name="existingEdges">已存在的边</param>
    /// <param name="newShape">细分</param>
    /// <param name="face">被细分的面</param>
    private void CreateTriangleFace(List<Edge> existingEdges, Shape newShape, Face face, List<ADJDEFPOINT> adjPoints)
    {
        List<Point> points = face.AllPoints;
        Point a = points[0];
        Point b = points[1];
        Point c = points[2];
        foreach(ADJDEFPOINT adjP in adjPoints)
        {
            if(adjP.point == a)
            {
                a = adjP.point.Successor;
            }
            else if (adjP.point == b)
            {
                b = adjP.point.Successor;
            }
            else if (adjP.point == c)
            {
                c = adjP.point.Successor;
            }
        }
        

        //for a triangle face (a,b,c): 
        //   (a, edge_pointab, face_pointabc, edge_pointca)
        //   (b, edge_pointbc, face_pointabc, edge_pointab)
        //   (c, edge_pointca, face_pointabc, edge_pointbc)
        Point facePoint = face.FacePoint;

        newShape.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, a, b, c));
        
    }
    /// <summary>
    /// 创建四边形
    /// </summary>
    /// <param name="existingEdges"></param>
    /// <param name="subdivided"></param>
    /// <param name="face"></param>
    private void CreateQuadFace(List<Edge> existingEdges, Shape subdivided, Face face)
    {
        //                  0 1 2 -> 3 
        //for a quad face (a,b,c,d): 
        //   (a, edge_pointab, face_pointabcd, edge_pointda)
        //   (b, edge_pointbc, face_pointabcd, edge_pointab)
        //   (c, edge_pointcd, face_pointabcd, edge_pointbc)
        //   (d, edge_pointda, face_pointabcd, edge_pointcd)
        List<Point> points = face.AllPoints;
        Point a = points[0].Successor;
        Point b = points[1].Successor;
        Point c = points[2].Successor;
        Point d = points[3].Successor;

        Point facePoint = face.FacePoint;

        subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, a, face.Edges[0].EdgePoint, facePoint, face.Edges[3].EdgePoint));
        subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, b, face.Edges[1].EdgePoint, facePoint, face.Edges[0].EdgePoint));
        subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, c, face.Edges[2].EdgePoint, facePoint, face.Edges[1].EdgePoint));
        subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, d, face.Edges[3].EdgePoint, facePoint, face.Edges[2].EdgePoint));

        SubdivisionUtilities.VerifyThatThereAreNoEdgeDuplicates(existingEdges);
    }



    /// <summary>
    /// 计算内部父节点的权重
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    double CalcVertexWeight(int num)
    {
        double onePerNum = 1.0d / num;
        double inner = 0.375d + 0.25d * Math.Cos(2.0d * Math.PI * onePerNum);
        double outer = onePerNum * (0.625f - inner * inner);

        return outer;
    }
}

