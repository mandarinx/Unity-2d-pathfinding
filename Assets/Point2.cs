using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mandarin {

     [Serializable]
     public struct Point2 {

         public readonly int x;
         public readonly int y;

         public Point2(int x, int y) {
             this.x = x;
             this.y = y;
         }

         public Point2(float x, float y) {
             this.x = (int)x;
             this.y = (int)y;
         }

         public Point2(Vector2 v2) {
             x = (int)Mathf.Floor(v2.x);
             y = (int)Mathf.Floor(v2.y);
         }

         public override bool Equals(System.Object obj) {
             if (obj == null) {
                 return false;
             }

             Point2 p = (Point2)obj;
             return (x == p.x) && (y == p.y);
         }

         public bool Equals(Point2 p) {
             return (x == p.x) && (y == p.y);
         }

         public override int GetHashCode() {
             return x ^ y;
         }

         public static bool operator == (Point2 a, Point2 b) {
//             return a.Equals(b);
             return (a.x == b.x) && (a.y == b.y);
         }

         public static bool operator != (Point2 a, Point2 b) {
             return !(a == b);
         }

         public static Point2 operator +(Point2 a, Vector3 b)  {
             return new Point2(a.x + (int)b.x, a.y + (int)b.y);
         }

         public static Point2 operator +(Point2 a, Point2 b)  {
             return new Point2(a.x + b.x, a.y + b.y);
         }

         public static Point2 operator -(Point2 a, Point2 b)  {
             return new Point2(a.x - b.x, a.y - b.y);
         }

         public static Point2 operator *(Point2 a, int v)  {
             return new Point2(a.x * v, a.y * v);
         }

         public static Point2 zero {
             get { return new Point2(); }
         }

         public static Point2 one {
             get { return new Point2(1, 1); }
         }

         public static Point2 up {
             get { return new Point2(0, 1); }
         }

         public static Point2 down {
             get { return new Point2(0, -1); }
         }

         public static Point2 right {
             get { return new Point2(1, 0); }
         }

         public static Point2 left {
             get { return new Point2(-1, 0); }
         }

         public override string ToString() {
             return "Point2 { x: "+x+" y: "+y+" }";
         }
     }
    
    public class Point2Comparer : IEqualityComparer<Point2> {
        public bool Equals(Point2 a, Point2 b) {
            return (a.x == b.x) && (a.y == b.y);
        }

        public int GetHashCode(Point2 obj) {
            return obj.GetHashCode();
        }
    }
}
