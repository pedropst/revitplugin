using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using System.Numerics;

namespace TCC
{
    [Serializable()]
    public class VirtualConduit
    {
        public List<ElementId> elements = new List<ElementId>();
        public List<String> circuits = new List<string>();
        public string firstElement;
        public List<VirtualConduit> beforeConduit = new List<VirtualConduit>();
        public List<VirtualConduit> afterConduit = new List<VirtualConduit>();
        public VirtualNode parentNode;
        public static int id = 1;
        public int conduitId = 0;
        public XYZ midPoint = null;
        public List<ElementId> lighting = new List<ElementId>();
        public List<Vector3> lumPos = new List<Vector3>();
        public List<VirtualLighting> light = new List<VirtualLighting>();
        public XYZ positionOfTag = null;
        public List<WiringType> wires = new List<WiringType>();
        public string orietation = "";
        public List<ElementId> tagIds = new List<ElementId>();
        public double comprimento;
        public List<Tomada> tomadas = new List<Tomada>();
        public List<Interruptor> interruptores = new List<Interruptor>();
        public List<Lampada> lampadas = new List<Lampada>();

        public VirtualConduit() { }
        public VirtualConduit(Document doc, List<ElementId> conduits, List<String> circuitsFromThisPiece)
        {
            this.elements = conduits;
            this.firstElement = conduits.First().ToString();
            this.circuits = circuitsFromThisPiece;
            this.conduitId = generateId();

            GettingMidPoint(doc);
        }

        public void DetermineOrietation(Document doc)
        {
            double distanceHorizontal = 0;
            double distanceVertical = 0;
            foreach (ElementId ei in elements)
            {
                if (doc.GetElement(ei).Category.Name == "Conduits")
                {
                    LocationCurve curve = doc.GetElement(ei).Location as LocationCurve;
                    XYZ point1 = curve.Curve.GetEndPoint(0);
                    XYZ point2 = curve.Curve.GetEndPoint(1);
                    distanceHorizontal += Math.Abs(point1.X - point2.X);
                    distanceHorizontal += Math.Abs(point1.Y - point2.Y);
                    distanceVertical += Math.Abs(point1.Z - point2.Z);
                }
            }

            if (distanceHorizontal*1.2f >= distanceVertical)
            {
                this.orietation = "HORIZONTAL";
            }
            else
            {
                this.orietation = "VERTICAL";
            }
        }

        public void GettingMidPoint(Document doc)
        {
            double length = 0;
            XYZ point1 = null;
            XYZ point2 = null;
            List <LocationCurve> list_lc = new List<LocationCurve>();
            foreach (ElementId ei in this.elements)
            {
                Element e1 = doc.GetElement(ei);
                if (e1.Category.Name == "Conduits")
                {
                    if (Math.Abs((e1.Location as LocationCurve).Curve.GetEndPoint(0).Z - (e1.Location as LocationCurve).Curve.GetEndPoint(1).Z) <= 0.01)
                    {
                        length += (e1.Location as LocationCurve).Curve.Length;
                        list_lc.Add(e1.Location as LocationCurve);
                    }
                }
            }
            comprimento = length;

            double compare = 0;
            foreach (ElementId ei in this.elements)
            {
                if (doc.GetElement(ei).Category.Name == "Conduits")
                {
                    LocationCurve lc = doc.GetElement(ei).Location as LocationCurve;
                    compare += lc.Curve.Length;
                    if (compare > (length / 2))
                    {
                        point1 = lc.Curve.GetEndPoint(0);
                        point2 = lc.Curve.GetEndPoint(1);

                        break;
                    }
                }
            }
            this.midPoint = (point1 + point2) / 2;


            /*Element e = doc.GetElement(this.elements.First());
            XYZ point1 = (e.Location as LocationCurve).Curve.GetEndPoint(0);
            e = doc.GetElement(this.elements[this.elements.Count - 2]);
            XYZ point2 = (e.Location as LocationCurve).Curve.GetEndPoint(1);
            this.midPoint = (point1 + point2)/2;*/
        }

        public static int generateId()
        {
            return id++;
        }
    }
}
