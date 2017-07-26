using System;
using System.Collections.Generic;

namespace Pathfinding {

     [Serializable]
     public struct PathData {

         public readonly int x;
         public readonly int y;
         public readonly int t;

         public PathData(int x, int y, int t) {
             this.x = x;
             this.y = y;
             this.t = t;
         }

         public override bool Equals(System.Object obj) {
             if (obj == null) {
                 return false;
             }

             PathData d = (PathData)obj;
             return (x == d.x) && (y == d.y) && (t == d.t);
         }

         public bool Equals(PathData d) {
             return (x == d.x) && (y == d.y) && (t == d.t);
         }

         public override int GetHashCode() {
             return x ^ y ^ t;
         }

         public static bool operator == (PathData a, PathData b) {
             return (a.x == b.x) && (a.y == b.y) && (a.t == b.t);
         }

         public static bool operator != (PathData a, PathData b) {
             return !(a == b);
         }

         public override string ToString() {
             return "<PathData> x: "+x+" y: "+y+" t: "+t+" }";
         }
     }
    
    public class PathData2Comparer : IEqualityComparer<PathData> {
        public bool Equals(PathData a, PathData b) {
            return (a.x == b.x) && (a.y == b.y) && (a.t == b.t);
        }

        public int GetHashCode(PathData obj) {
            return obj.GetHashCode();
        }
    }
}
