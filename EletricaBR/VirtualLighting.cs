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
    public class VirtualLighting
    {
        public String circuit = "";
        public String switchId = "";
        public String id = "";
        public int qntInterruptor = 0;
        public String type = "";
        public HashSet<VirtualConduit> inConduits = new HashSet<VirtualConduit>();
        public List<Element> switches = new List<Element>();
        public List<Element> lamps = new List<Element>();
        public HashSet<VirtualConduit> othersConduits = new HashSet<VirtualConduit>();
        public HashSet<VirtualConduit> refConduits = new HashSet<VirtualConduit>();

        public HashSet<VirtualConduit> pathToFirstSwitch = new HashSet<VirtualConduit>();
        public HashSet<VirtualConduit> pathBetweenSwitchAndLamps = new HashSet<VirtualConduit>();

        public HashSet<VirtualConduit> pathToSecondSwitch = new HashSet<VirtualConduit>();
        public HashSet<VirtualConduit> pathBetweenSwitches = new HashSet<VirtualConduit>();
        public HashSet<VirtualConduit> pathBetweenSecondSwitchAndLamps = new HashSet<VirtualConduit>();

        public HashSet<VirtualConduit> pathToLamps = new HashSet<VirtualConduit>();

        public VirtualLighting(String circuit, String switchId, String id)
        {
            this.circuit = circuit;
            this.switchId = switchId;
            this.id = id;
        }
    }
}
