/* Dado um ponto inicial, encontrar e ordenar os eletrodutos da instalação elétrica:
 * O ponto inicial é o quadro geral;
 * Considerar todos os eletrodutos, em exceção aqueles que vão para outros quadros 
 * elétricos;
 * O "caminho" sempre consiste entre a conexão de um eletroduto a uma curva, 
 * caixa de passagem, lâmpada;
 * Utilizar o algoritmo Depth-First Search para encontrar o caminho;
 * Considerar as caixas que derivam em mais eletrodutos como os "nodes";
 * Desconsiderar as caixas que não derivam, só tem uma entrada e uma saída;
 * Busca é finalizada quando todos os nodes e seus respectivos eletrodutos são analisados.
 * Tomar cuidado com a conexão com a "classe wiring";
 * Todo eletroduto/curva/caixa possuem conexões, é preciso analisar cada conexão para 
 * determinar qual é o próximo eletroduto;
 * Nunca há conexão direta entre dois eletrodutos, nem entre duas curvas, ou duas caixas. 
 * Considere eletroduto como curve, e o restante como model,
 * portanto, nunca há conexão entre dois models, nem entre duas curves, sempre será 
 * model-curve-model, ou curve-model-curve.
 */

/* Utilizar a função GetConnectorsFromCurve para pegar os models que estão ligado ao 
 * eletroduto;
 * Utilizar a função GetConnectorsFromeModel para pegar os eletrodutos que estão ligados 
 * ao model (caixa, curva).
 */

/* Criar um while que permanece funcionando enquanto houver nodes ou eletrodutos a 
 * serem analisados;
 * Salvar todos os IDs dos nodes, e dos eletrodutos, de forma organizada, possibilitando a 
 * verificação do caminho a partir da ordem armazenada.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace TCC
{
    class DFS
    {
        public List<VirtualNode> allNodes = new List<VirtualNode>();

        public List<VirtualNode> Main(Element selectedElement, Document doc)
        {
            List<VirtualNode> nodes = new List<VirtualNode>();
            List<ElementId> conduitsToExploreId = new List<ElementId>();
            List<ElementId> toAddConduits = new List<ElementId>();
            List<ElementId> nodesToExploreId = new List<ElementId>();
            List<ElementId> toAddNodes = new List<ElementId>();
            List<ElementId> alreadySearched = new List<ElementId>();
            List<ElementId> alreadySearchedNode = new List<ElementId>();
            nodesToExploreId.Add(selectedElement.Id);
            VirtualNode firstNode = new VirtualNode(selectedElement.Id);
            nodes.Add(firstNode);
            List<Connector> firstModel = new List<Connector>();
            firstModel = GetConnectorsFromModel(selectedElement);
            foreach (Connector c in firstModel)
            {
                firstNode.setOutElements(c.Owner.Id);
            }

            do
            {
                int j = 0;
                Element node = doc.GetElement(nodesToExploreId[0]);
                alreadySearched.Add(node.Id);
                alreadySearchedNode.Add(node.Id);
                VirtualNode currentN = null;
                foreach (VirtualNode vn in nodes)
                {
                    if (vn.nodeId == nodesToExploreId[0])
                    {
                        currentN = vn;
                    }
                }
                foreach (ElementId ei in currentN.outElements)
                {
                    List<ElementId> tempList = new List<ElementId>();
                    tempList.Add(ei);
                    currentN.path.Add(tempList);
                }
                List<List<ElementId>> toAddPath = new List<List<ElementId>>();
                foreach (ElementId list_ei in currentN.outElements)
                {
                    j++;
                    toAddPath.Add(new List<ElementId>());
                    conduitsToExploreId.Add(list_ei);

                    //while (conduitsToExploreId.Count() != 0);
                    do
                    {
                        foreach (ElementId ei in alreadySearched)
                        {
                            if (conduitsToExploreId.Contains(ei))
                            {
                                toAddPath.Last<List<ElementId>>().Add(ei);
                                conduitsToExploreId.Remove(ei);
                            }
                        }

                        foreach (ElementId ei in conduitsToExploreId)
                        {
                            if (doc.GetElement(ei).Category.Name == "Conduits")
                            {
                                FilteredElementCollector filterCurve = new FilteredElementCollector(doc, conduitsToExploreId).OfClass(typeof(MEPCurve));
                                alreadySearched.Add(ei);
                                currentN.setNextElements(ei);
                                foreach (MEPCurve mep in filterCurve)
                                {
                                    foreach (Connector c in GetConnectorsFromCurve(mep, alreadySearched))
                                    {
                                        Element tempElement = doc.GetElement(c.Owner.Id);
                                        if (!alreadySearched.Contains(tempElement.Id) && !conduitsToExploreId.Contains(tempElement.Id) && !toAddConduits.Contains(tempElement.Id))
                                        {
                                            toAddConduits.Add(tempElement.Id);
                                        }
                                    }
                                }
                            }
                            else if (doc.GetElement(ei).Category.Name == "Conduit Fittings")
                            {
                                ExploringModelTypeElement(doc, ei, alreadySearched, toAddNodes, conduitsToExploreId, toAddConduits, nodesToExploreId, nodes, currentN);
                            }
                            else if (doc.GetElement(ei).Category.Name == "Lighting Fixtures")
                            {
                                Element nn = doc.GetElement(ei);
                                String circuit = nn.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() + "N";
                                LocationPoint pos = nn.Location as LocationPoint;
                                XYZ aa = pos.Point;
                                ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
                                ExploringModelTypeElement(doc, ei, alreadySearched, toAddNodes, conduitsToExploreId, toAddConduits, nodesToExploreId, nodes, currentN);
                            }
                            else if (doc.GetElement(ei).Category.Name == "Lighting Devices")
                            {
                                Element nn = doc.GetElement(ei);
                                String circuit = nn.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                                String interruptor = nn.get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString();
                                ExploringModelTypeElement(doc, ei, alreadySearched, toAddNodes, conduitsToExploreId, toAddConduits, nodesToExploreId, nodes, currentN);
                            }
                            else if (doc.GetElement(ei).Category.Name == "Electrical Fixtures")
                            {
                                Element nn = doc.GetElement(ei);
                                String circuit = nn.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() + "FNT";
                                ExploringModelTypeElement(doc, ei, alreadySearched, toAddNodes, conduitsToExploreId, toAddConduits, nodesToExploreId, nodes, currentN);
                            }
                            else if (doc.GetElement(ei).Category.Name == "Electrical Equipment")
                            {
                                Element nn = doc.GetElement(ei);
                                ExploringModelTypeElement(doc, ei, alreadySearched, toAddNodes, conduitsToExploreId, toAddConduits, nodesToExploreId, nodes, currentN);
                            }
                        }

                        foreach (ElementId ei in toAddConduits)
                        {
                            conduitsToExploreId.Add(ei);
                        }
                        toAddConduits.Clear();
                    } while (conduitsToExploreId.Count() != 0);
                }
                currentN.path = toAddPath;
                foreach (ElementId e in toAddNodes)
                {
                    nodesToExploreId.Add(e);
                }
                toAddNodes.Clear();
                foreach (ElementId ei in alreadySearchedNode)
                {
                    if (nodesToExploreId.Contains(ei))
                    {
                        nodesToExploreId.Remove(ei);
                    }
                }
            } while (nodesToExploreId.Count() != 0);
            return nodes;
        }

        public void ExploringModelTypeElement(Document doc, ElementId elementId, List<ElementId> alreadySearched, List<ElementId> toAddNodes, List<ElementId> conduitsToExploreId, List<ElementId> toAdd, List<ElementId> nodesToExploreId, List<VirtualNode> nodes, VirtualNode currentN)
        {
            Element element = doc.GetElement(elementId);
            List<Connector> outputModel = GetConnectorsFromModel(element);
            if (!isNode(outputModel, element, toAddNodes, nodes))
            {
                alreadySearched.Add(elementId);
                currentN.setNextElements(elementId);
                foreach (Connector c in outputModel)
                {
                    if (!alreadySearched.Contains(c.Owner.Id) && !conduitsToExploreId.Contains(c.Owner.Id) && !toAdd.Contains(c.Owner.Id) && !toAddNodes.Contains(c.Owner.Id) && !nodesToExploreId.Contains(c.Owner.Id))
                    {
                        toAdd.Add(c.Owner.Id);
                    }
                }
            }
            else
            {
                VirtualNode current_node = nodes.Last<VirtualNode>();
                current_node.beforeNode = currentN;
                currentN.afterNodes.Add(current_node);
                foreach (Connector c in outputModel)
                {
                    if (!alreadySearched.Contains(c.Owner.Id) && !conduitsToExploreId.Contains(c.Owner.Id) && !toAdd.Contains(c.Owner.Id) && !toAddNodes.Contains(c.Owner.Id) && !nodesToExploreId.Contains(c.Owner.Id))
                    {
                        current_node.setOutElements(c.Owner.Id);
                    }
                    if (alreadySearched.Contains(c.Owner.Id))
                    {
                        current_node.setInElement(c.Owner.Id);
                    }
                }
                alreadySearched.Add(elementId);
            }
        }

        public bool isNode(List<Connector> connectorList, Element element, List<ElementId> result, List<VirtualNode> nodes)
        {
            if (connectorList.Count() - 1 > 1 && element.Category.Name != "Electrical Equipment")
            {
                VirtualNode newNode = new VirtualNode(element.Id);
                nodes.Add(newNode);
                result.Add(element.Id);
                return true;
            }
            else
            {
                return false;
            }
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

        public List<Connector> GetConnectorsFromCurve(MEPCurve mepCurve, List<ElementId> alreadySearched)
        {
            ConnectorSet connectorSet = mepCurve.ConnectorManager.Connectors;

            List<Connector> connectorList = new List<Connector>();

            foreach (Connector connector in connectorSet)
            {
                if (connector != null && (connector.ConnectorType == ConnectorType.End || connector.ConnectorType == ConnectorType.Curve || connector.ConnectorType == ConnectorType.Physical) && connector.IsConnected == true)
                {
                    ConnectorSet cs = connector.AllRefs;
                    ConnectorSetIterator csi = cs.ForwardIterator();
                    while (csi.MoveNext())
                    {
                        Connector current = csi.Current as Connector;
                        if (!alreadySearched.Contains(current.Owner.Id) && current.ConnectorType != ConnectorType.Logical && current.Owner.Category.Name != "Wires" && current.Owner.Category.Name != "Electrical Circuits")
                            connectorList.Add(current);
                    }
                }
            }
            return connectorList;
        }
    }
}