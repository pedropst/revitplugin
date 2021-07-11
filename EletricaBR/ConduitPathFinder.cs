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
    class ConduitPathFinder
    {
        public ConduitPathFinder(Element panel)
        {
            List<ElementId> elementsAlreadySearched = new List<ElementId>();
            List<Element> nodesElements = new List<Element>();
            List<Element> nodesToExplore = new List<Element>();

            nodesToExplore.Add(panel);
            nodesElements.Add(panel);

            do
            {
                List<ElementId> conduitsToExplore = new List<ElementId>();
                foreach (Connector c in GetConnectorsFromModel(nodesToExplore.First()))
                {
                    conduitsToExplore.Add(c.Owner.Id);
                    Console.WriteLine(c.Owner.Id.ToString());
                }
                conduitsToExplore.RemoveAt(0);
            } while (nodesToExplore.Count > 0);
        }

        public List<Connector> GetConnectorsFromModel(Element element)
        {
            List<Connector> connectorList = new List<Connector>();
            //1. Cast Element to FamilyInstance
            FamilyInstance inst = element as FamilyInstance;
            //2. Get MEPModel Property
            MEPModel mepModel = inst.MEPModel;
            //3. Get connector set of MEPModel
            ConnectorSet connectorSet = mepModel.ConnectorManager.Connectors;
            //4. Initialise empty list of connectors

            //5. Loop through connector set and add to list
            foreach (Connector connector in connectorSet)
            {
                if (connector != null && (connector.ConnectorType == ConnectorType.End || connector.ConnectorType == ConnectorType.Curve || connector.ConnectorType == ConnectorType.Physical) && connector.IsConnected == true)
                {
                    ConnectorSet cs = connector.AllRefs;
                    ConnectorSetIterator csi = cs.ForwardIterator();
                    while (csi.MoveNext())
                    {
                        Connector current = csi.Current as Connector;
                        if (current.ConnectorType != ConnectorType.Logical && current.Owner.Category.Name != "Wires") //!alreadySearched.Contains(current.Owner.Id) &&
                            connectorList.Add(current);
                    }
                }
            }
            return connectorList;
        }

    }
}
