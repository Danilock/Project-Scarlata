using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathBerserker2d
{
    internal class PolygonSet : IEnumerable<Polygon>
    {
        private B2DynamicTree<Polygon> polygonTree = new B2DynamicTree<Polygon>();

        public void AddPolygon(IClipper clipper, Polygon newPoly)
        {
             if (newPoly.Hull.IsClosed)
            {
                // try to merge the new polygon with the existing once
                List<Polygon> polygonsToInsert = new List<Polygon>(4);
                List<Polygon> resultPolys = new List<Polygon>();
                var iterator = polygonTree.Query(newPoly.BoundingRect);
                while (iterator.MoveNext())
                {
                    var poly = polygonTree.GetUserData(iterator.Current);
                    if (poly.Hull.IsClosed)
                    {
                        var resultType = clipper.Compute(poly, newPoly, BoolOpType.UNION, ref resultPolys);
                        if (resultType == ResultType.Clipped)
                        {
                            // delete poly
                            polygonTree.RemoveProxy(iterator.Current);
                            newPoly = resultPolys[0];
                            resultPolys.Clear();
                        }
                    }
                    else
                    {
                        var resultType = clipper.Compute(poly, newPoly, BoolOpType.DIFFERENCE, ref polygonsToInsert, true);
                        if (resultType == ResultType.Clipped)
                        {
                            polygonTree.RemoveProxy(iterator.Current);
                        }
                    }
                }
                foreach (var poly in polygonsToInsert)
                    polygonTree.CreateProxy(poly.BoundingRect, poly);
                polygonTree.CreateProxy(newPoly.BoundingRect, newPoly);
            }
            else
            {
                List<Polygon> resultPolys = new List<Polygon>();
                List<Polygon> polygonsToTest = new List<Polygon>();

                polygonsToTest.Add(newPoly);

                var iterator = polygonTree.Query(newPoly.BoundingRect);
                while (iterator.MoveNext())
                {
                    var poly = polygonTree.GetUserData(iterator.Current);
                    if (!poly.Hull.IsClosed)
                        continue;
                    foreach (var polyToTest in polygonsToTest)
                    {
                        var resultType = clipper.Compute(polyToTest, poly, BoolOpType.DIFFERENCE, ref resultPolys, true);
                        if (resultType == ResultType.NoOverlap)
                            resultPolys.Add(polyToTest);
                    }
                    var swap = polygonsToTest;
                    polygonsToTest = resultPolys;
                    resultPolys = swap;
                    resultPolys.Clear();
                }

                foreach (var poly in polygonsToTest)
                    polygonTree.CreateProxy(poly.BoundingRect, poly);
            }
        }

        public IEnumerable<Polygon> Query(Rect aabb)
        {
            var iterator = polygonTree.Query(aabb);
            while (iterator.MoveNext())
                yield return polygonTree.GetUserData(iterator.Current);
        }

        public IEnumerator<Polygon> GetEnumerator()
        {
            return ((IEnumerable<Polygon>)polygonTree).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Polygon>)polygonTree).GetEnumerator();
        }
    }
}