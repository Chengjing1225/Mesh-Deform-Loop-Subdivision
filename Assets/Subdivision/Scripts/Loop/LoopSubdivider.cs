using System;
using System.Linq;
using System.Collections.Generic;

namespace Subdivision.Core {

    public class LoopSubdivider : ISubdivider
    {
         public Shape Subdivide(Shape shape)
        {

            Shape subdivided = new Shape();
            
            CreateEdgePoints(shape);
            
            CreateVertexPoints(shape);
            
            CreateFaces(shape, subdivided);

            return subdivided;
        }
        public Shape SubdividePlane(Shape shape)
        {

            Shape subdivided = new Shape();
            
            CreatePlaneEdgePoints(shape);
            
            CreatePlaneVertexPoints(shape);
            
            CreateFaces(shape, subdivided);

            return subdivided;
        }
        
        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <param name="shape"></param>
        private void CreateEdgePoints(Shape shape)
        {
            List<Edge> edges = shape.AllEdges;
            foreach (Edge edge in edges)
            {
                if (edge.IsOnBorderOfHole)          //边界子节点
                {
                    Vector3 position =
                        Average(
                            edge.Points[0],
                            edge.Points[1]);
                    edge.EdgePoint = new Point(position);
                }
                else                //内部子节点
                {
                    Point thirdP0 = edge.Faces[0].AllPoints.SingleOrDefault(p => (p != edge.Points[0] && p != edge.Points[1]));
                    Point thirdP1 = edge.Faces[1].AllPoints.SingleOrDefault(p => (p != edge.Points[0] && p != edge.Points[1]));
                    Vector3 position =
                        edge.Points[0].Position * 0.375f
                        + edge.Points[1].Position * 0.375f
                        + thirdP0.Position * 0.125f
                        + thirdP1.Position * 0.125f;
                    edge.EdgePoint = new Point(position);
                }
            }
        }
        /// <summary>
        /// 计算父节点的位置
        /// </summary>
        /// <param name="shape"></param>
        private void CreateVertexPoints(Shape shape)
        {
            List<Point> allPoints = shape.AllPoints;
            List<Edge> allEdges = shape.AllEdges;

            foreach (Point oldPoint in allPoints)
            {
                if (oldPoint.IsOnBorderOfHole)          //边界父节点
                {
                    //oldPoint.Successor = oldPoint;
                    oldPoint.Successor = CreateVertexPointForBorderPoint(oldPoint);
                }
                else                                                        //内部父节点
                {
                    oldPoint.Successor =  CreateVertexPoint(oldPoint);
                    //  oldPoint.Successor = oldPoint;
                }
            }
        }
        /// <summary>
        /// 计算父内部节点的位置
        /// </summary>
        /// <param name="oldPoint">内部父节点</param>
        /// <returns></returns>
        private Point CreateVertexPoint(Point oldPoint)
        {

            int num = oldPoint.Edges.Count;
            double weight = CalcVertexWeight(num);
            Vector3 result = (1.0d - num * weight) * oldPoint.Position ;

            for (int i = 0; i < num; i++)
            {
                result += oldPoint.Edges[i].Points.SingleOrDefault(p => p != oldPoint).Position * weight;
            }
            
            return new Point(result);
        }

        /// <summary>
        /// 计算边界父节点的位置
        /// </summary>
        /// <param name="oldPoint">边界父节点</param>
        /// <returns></returns>
        private Point CreateVertexPointForBorderPoint(Point oldPoint)
        {
            List<Vector3> positions = oldPoint.Edges.Where(e => e.IsOnBorderOfHole).Select(e => e.Middle).ToList();
            positions.Add(oldPoint.Position);

            return new Point(Vector3.Average(positions));
        }

        /// <summary>
        /// 创建平面子节点
        /// </summary>
        /// <param name="shape"></param>
        private void CreatePlaneEdgePoints(Shape shape)
        {
            List<Edge> edges = shape.AllEdges;
            foreach (Edge edge in edges)
            {
                Vector3 position =
                        Average(
                            edge.Points[0],
                            edge.Points[1]);
                edge.EdgePoint = new Point(position);
            }
        }

        /// <summary>
        /// 计算平面父节点的位置
        /// </summary>
        /// <param name="shape"></param>
        private void CreatePlaneVertexPoints(Shape shape)
        {
            List<Point> allPoints = shape.AllPoints;
            List<Edge> allEdges = shape.AllEdges;

            foreach (Point oldPoint in allPoints)
            {
                oldPoint.Successor = oldPoint;
            }
        }


        /// <summary>
        /// 创建面
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="subdivided">细分体</param>
        private void CreateFaces(Shape shape, Shape subdivided)
        {
            List<Face> faces = shape.Faces;
            List<Edge> existingEdges = new List<Edge>();
            foreach (Face face in faces)
            {
                if (face.AllPoints.Count() == 3)
                {
                    CreateTriangleFace(existingEdges, subdivided, face);
                }
                else if (face.AllPoints.Count() == 4)
                {
                    CreateQuadFace(existingEdges, subdivided, face);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unhandled facetype (point count={0})!", face.AllPoints.Count()));
                }
            }
            SubdivisionUtilities.VerifyThatThereAreNoEdgeDuplicates(existingEdges);
        }

        /// <summary>
        /// 创建三角形
        /// </summary>
        /// <param name="existingEdges">已存在的边</param>
        /// <param name="subdivided">细分</param>
        /// <param name="face">被细分的面</param>
        private void CreateTriangleFace(List<Edge> existingEdges, Shape subdivided, Face face)
        {
            List<Point> points = face.AllPoints;
            Point a = points[0].Successor;
            Point b = points[1].Successor;
            Point c = points[2].Successor;

            //for a triangle face (a,b,c): 
            //   (a, edge_pointab, face_pointabc, edge_pointca)
            //   (b, edge_pointbc, face_pointabc, edge_pointab)
            //   (c, edge_pointca, face_pointabc, edge_pointbc)
            Point facePoint = face.FacePoint;

            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, a, face.Edges[0].EdgePoint, face.Edges[2].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, b, face.Edges[1].EdgePoint, face.Edges[0].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, c, face.Edges[2].EdgePoint, face.Edges[1].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, face.Edges[0].EdgePoint, face.Edges[1].EdgePoint, face.Edges[2].EdgePoint));

            SubdivisionUtilities.VerifyThatThereAreNoEdgeDuplicates(existingEdges);
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

        private Vector3 Average(IEnumerable<Point> points)
        {
            return Vector3.Average(points.Select(p => p.Position));
        }

        private Vector3 Average(params Point[] points)
        {
            return Vector3.Average(points.Select(p => p.Position));
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

}
