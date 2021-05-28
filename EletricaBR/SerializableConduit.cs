using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TCC
{
    public class SerializableConduit
    {
        public List<String> elements = new List<string>();
        public List<WiringType> wires = new List<WiringType>();
        public List<String> tagIds = new List<string>();
        public XYZ midpoint = null;
        public List<SerializableConduit> beforeConduit = new List<SerializableConduit>();
        public List<SerializableConduit> afterConduit = new List<SerializableConduit>();
        public int conduitId = 1;
        public SerializableConduit() { }

        public SerializableConduit(List<String> elements, List<WiringType> wires, List<String> tagIds, XYZ midpoint, List<SerializableConduit> beforeConduit, List<SerializableConduit> afterConduit, int id)
        {
            this.elements = elements;
            this.wires = wires;
            this.tagIds = tagIds;
            this.midpoint = midpoint;
            this.beforeConduit = beforeConduit;
            this.afterConduit = afterConduit;
            this.conduitId = id;
        }
    }
}
