using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEletrica
{
    public class VirtualNode
    {
        public ElementId nodeId, inElement;
        public VirtualNode beforeNode = null;
        public List<VirtualNode> afterNodes = new List<VirtualNode>();
        public List<ElementId> outElements = new List<ElementId>();
        public List<ElementId> nextElements = new List<ElementId>();
        public List<List<ElementId>> path = new List<List<ElementId>>();
        public List<List<List<ElementId>>> pieces = new List<List<List<ElementId>>>();
        public List<List<VirtualConduit>> conduits = new List<List<VirtualConduit>>();
        public List<String> circuits = new List<string>();
        public List<WiringType> wiringTypes = new List<WiringType>();

        public List<VirtualConduit> beforeConduit = new List<VirtualConduit>();
        public List<VirtualConduit> afterConduits = new List<VirtualConduit>();

        public VirtualNode(ElementId nodeId)
        {
            this.nodeId = nodeId;
        }

        public void setNextElements(ElementId elementId)
        {
            this.nextElements.Add(elementId);
        }
        public void setOutElements(ElementId elementId)
        {
            this.outElements.Add(elementId);
        }

        public void setInElement(ElementId elementId)
        {
            this.inElement = elementId;
        }

        public List<VirtualConduit> CuttingPieces(Document doc, VirtualNode vn, String panel_name)
        {
            List<VirtualConduit> allConduits = new List<VirtualConduit>();
            foreach (List<ElementId> path_list in path)
            {
                pieces.Add(new List<List<ElementId>>());
                List<List<ElementId>> last_grand_piece = pieces.Last<List<List<ElementId>>>();
                conduits.Add(new List<VirtualConduit>());
                foreach (ElementId ei in path_list)
                {
                    if (doc.GetElement(ei).Category.Name == "Lighting Fixtures" || 
                        doc.GetElement(ei).Category.Name == "Lighting Devices" || 
                        doc.GetElement(ei).Category.Name == "Electrical Fixtures" ||
                        doc.GetElement(ei).Category.Name == "Electrical Equipment")
                    {
                        last_grand_piece.Last<List<ElementId>>().Add(ei);
                        List<String> circuit = new List<string>();

                        FamilyInstance instance = doc.GetElement(ei) as FamilyInstance;
                        VirtualConduit vc = new VirtualConduit(doc, last_grand_piece.Last<List<ElementId>>(), circuit);
                        vc.parentNode = vn;

                        if (doc.GetElement(ei).Category.Name != "Electrical Equipment")
                        {
                            foreach (ElementId eii in instance.GetSubComponentIds())
                            {
                                Element element = doc.GetElement(eii);
                                if (element.Category.Name == "Electrical Fixtures")
                                {
                                    String circuitNumber = element.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                                    if (!circuit.Contains(circuitNumber + "FNT") && circuitNumber != null)
                                    {
                                        circuit.Add(circuitNumber + "FNT");
                                        WiringType wt = new WiringType(circuitNumber);
                                        wt.fase = true;
                                        wt.terra = true;
                                        wt.neutro = true;
                                        if (element.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString().Contains(","))
                                        {
                                            wt.isBifasico = true;
                                            wt.neutro = false;
                                        }
                                        if (vc.wires.Count > 0)
                                        {
                                            bool teste = true;
                                            foreach (WiringType wt1 in vc.wires)
                                            {
                                                if (wt1.id == wt.id)
                                                    teste = false;
                                            }
                                            if (teste)
                                                vc.wires.Add(wt);
                                        }
                                        else
                                        {
                                            vc.wires.Add(wt);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (instance.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_SUPPLY_FROM_PARAM).AsString() == panel_name)
                            {
                                MEPModel mep = instance.MEPModel;
                                Autodesk.Revit.DB.Electrical.ElectricalSystemSet set = instance.MEPModel.AssignedElectricalSystems;
                                Autodesk.Revit.DB.Electrical.ElectricalSystemSetIterator seti = set.ForwardIterator();
                                seti.MoveNext();
                                Autodesk.Revit.DB.Electrical.ElectricalSystem wire = seti.Current as Autodesk.Revit.DB.Electrical.ElectricalSystem;
                                String circuitNumber = wire.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();

                                if (instance.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NUMPHASES_PARAM).AsValueString() == "3")
                                {
                                    if (!circuit.Contains(circuitNumber + "FFF") && circuitNumber != null)
                                    {
                                        circuit.Add(circuitNumber + "FFF");
                                        WiringType wt = new WiringType(circuitNumber, " - " + instance.Name);
                                        wt.isTrifasico = true;
                                        wt.fase = true;
                                        wt.neutro = false;
                                        if (vc.wires.Count > 0)
                                        {
                                            bool teste = true;
                                            foreach (WiringType wt1 in vc.wires)
                                            {
                                                if (wt1.id == wt.id)
                                                {
                                                    teste = false;
                                                }
                                                if (teste)
                                                    vc.wires.Add(wt);
                                            }
                                        }
                                        else
                                        {
                                            vc.wires.Add(wt);
                                        }
                                    }
                                }
                            }
                        }
                        
                        allConduits.Add(vc);
                        conduits.Last<List<VirtualConduit>>().Add(vc);
                        if (ei != path_list.Last<ElementId>())
                        {
                            last_grand_piece.Add(new List<ElementId>());
                        }   
                    }
                    else
                    {
                        if (last_grand_piece.Count == 0)
                        {
                            List<ElementId> tempList = new List<ElementId>();
                            tempList.Add(ei);
                            List<List<ElementId>> tempList1 = new List<List<ElementId>>();
                            tempList1.Add(tempList);
                            last_grand_piece.Add(tempList);
                        }
                        else 
                        {
                            last_grand_piece.Last<List<ElementId>>().Add(ei);
                        }
                    }
                }
            }
            return allConduits;
        }
    }
}
