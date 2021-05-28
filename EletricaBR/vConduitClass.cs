using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace EletricaBR
{
    public class vConduitClass
    {
        public List<ElementId> eletrodutoList = new List<ElementId>();
        public ElementId conectado; //FINAL ELEMENT
        public List<String> circuitList = new List<String>();
        public List<ElementId> elementBefore; //ELEMENTPARENT
        public vConduitClass eletrodutoParent;
        public List<vConduitClass> eletrodutoChild = new List<vConduitClass>();
        public ElementId idParent, idChild;
        int id;
        bool betweenElements, elementAtEnd;
        public bool childIsANode = false;
        NodeClass nodeParent;
        public XYZ midPoint;
        public XYZ startPoint;
        public XYZ endPoint;
        public PolyLine projection;
        CircuitClass circuitClass;
        public List<VirtualWiringClass> wiring = new List<VirtualWiringClass>();
        public List<Curve> curves = new List<Curve>();


        public void SetChild(List<vConduitClass> child)
        {
            this.eletrodutoChild = child;
        }

        public void SetParent(vConduitClass parent)
        {
            this.eletrodutoParent = parent;
        }

        public void SetWiring(VirtualWiringClass wire)
        {
            if (this.wiring.Count > 0)
            {
                List<VirtualWiringClass> temp = new List<VirtualWiringClass>();
                List<String> alreadyCircuit = new List<string>();
                foreach (VirtualWiringClass wire1 in this.wiring)
                {
                    if (wire1.circuitNumber == wire.circuitNumber)
                    {
                        alreadyCircuit.Add(wire1.circuitNumber);
                        wire1.outp += wire.outp;
                        if (wire1.outp.Contains("F") && wire1.outp.Contains("N") && wire1.outp.Contains("T") && wire1.outp.Count() == 3)
                        {
                            wire1.outp = "FNT";
                            wire1.id = wire1.circuitNumber + wire1.outp;
                        }
                        else if (wire1.outp.Contains("F") && wire1.outp.Contains("N"))
                        {
                            wire1.outp = "FN";
                            wire1.id = wire1.circuitNumber + wire1.outp;
                        }
                        else if (wire1.outp.Contains("F") && wire1.outp.Contains("R"))
                        {
                            wire1.outp = "FR";
                            wire1.id = wire1.circuitNumber + wire1.outp;
                        }
                        else if (wire1.outp.Contains("F"))  //ISSO DEVE SER DELETADO QUANDO FIZER A PARTE DE RETORNO CORRETAMENTE
                        {
                            wire1.outp = "FR";
                            wire1.id = wire1.circuitNumber + wire1.outp;
                        }
                    }
                    else
                    {
                        if (!alreadyCircuit.Contains(wire.circuitNumber))
                        {
                            temp.Add(wire);
                        }
                    }
                }
                foreach (VirtualWiringClass wireTemp in temp)
                {
                    this.wiring.Add(wireTemp);
                    
                }
                temp.Clear();
            }
            else
            {
                this.wiring.Add(wire);
            }
        }


        public vConduitClass(List<ElementId> pieceOfPath, ElementId idChild, Document doc, NodeClass node, int id)
        {
            this.eletrodutoList = pieceOfPath;
            this.idChild = idChild;
            this.nodeParent = node;
            this.id = id;
            this.idParent = GetPredecessorElement(doc)[0];

            MidPoint(doc);
        }

        public void CircuitType(Document doc)
        {
            Element atEnd = doc.GetElement(this.idChild);
            String categoryName = atEnd.Category.Name;
            String circuitNumber = atEnd.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
            switch (categoryName)
            { 
                case "Electrical Fixtures":
                    if (atEnd.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE).AsString() == "127.00")
                    {
                        String circuitType = "FNT";
                        //CircuitClass ccElementAtEnd = new CircuitClass(this, circuitNumber, circuitType);
                    }
                    break;
                case "Lighting Fixtures":
                    if (atEnd.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE).AsString() == "127.00")
                    {
                        String switchId = atEnd.get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString();
                        String circuitType = "RNT";
                        //CircuitClass ccElementAtEnd = new CircuitClass(this, circuitNumber, circuitType);
                    }
                    break;
                case "Lighting Devices":
                    if (atEnd.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE).AsString() == "127.00")
                    {
                        String circuitType = "FNT";
                        //CircuitClass ccElementAtEnd = new CircuitClass(this, circuitNumber, circuitType);
                    }
                    break;
            }
        }

        public List<vConduitClass> GetEletrodutosConectados(vConduitClass eletroduto)
        {
            List<vConduitClass> conectado = new List<vConduitClass>();
            foreach (vConduitClass ele in eletroduto.eletrodutoChild)
            {
                conectado.Add(ele);
            }
            conectado.Add(eletroduto.eletrodutoParent);
            return conectado;
        }

        public void MidPoint(Document doc)
        {
            List<XYZ> startPoints = new List<XYZ>();
            List<XYZ> endPoints = new List<XYZ>();
            List<XYZ> allPoints = new List<XYZ>();
            List<Curve> curveList = new List<Curve>();
            foreach (ElementId ei in this.eletrodutoList)
            {
                if (doc.GetElement(ei).Category.Name == "Conduits")
                {
                    LocationCurve lc = doc.GetElement(ei).Location as LocationCurve;
                    Curve curve = lc.Curve;
                    this.curves.Add(curve);
                    curveList.Add(curve);


                    /*XYZ zero = XYZ.Zero;
                    //LocationCurve lc = doc.GetElement(ei).Location as LocationCurve;
                    //Curve curve = lc.Curve;                
                    IList<XYZ> pointss = curve.Tessellate();
                    double x1, y1, z1, x2, y2, z2;
                    x1 = pointss[0].X;
                    y1 = pointss[0].Y;
                    z1 = 10.00;
                    this.startPoint = new XYZ(x1, y1, 10.00);

                    x2 = pointss[pointss.Count - 1].X;
                    y2 = pointss[pointss.Count - 1].Y;
                    z2 = 10.00;
                    this.endPoint = new XYZ(x2, y2, 10.00);*/

                    //startPoint = curve.GetEndPoint(0);
                    //endPoint = curve.GetEndPoint(1);
                    //this.midPoint = curve.Evaluate(0.5, true);
                }

                //startPoints.Add(startPoint);
                //endPoints.Add(endPoint);
            }

            this.startPoint = (doc.GetElement(this.eletrodutoList[0]).Location as LocationCurve).Curve.GetEndPoint(0);
            Curve total = null;
            foreach (Curve c in curveList)
            {
                allPoints.Add(c.GetEndPoint(0));
                allPoints.Add(c.GetEndPoint(1));
            }

            PolyLine pl1 = PolyLine.Create(allPoints);
            this.projection = pl1;
            this.midPoint = pl1.Evaluate(0.5);





            /*List<XYZ> points = new List<XYZ>();
            if (startPoints.Count > 1)
            {
                points = startPoints;
                PolyLine pl = PolyLine.Create(points);
                this.midPoint = pl.Evaluate(0.5);
            }
            else
            {
                points.Add(startPoints[0]);
                points.Add(endPoints[0]);
                PolyLine pl = PolyLine.Create(points);
                this.midPoint = pl.Evaluate(0.5);
            }*/
        }
        public vConduitClass Lookup(ElementId elementParent)
        {
            if (this.idChild == elementParent)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public void AddCircuit(List<String> circuitos, bool retorno)
        {
            if (retorno == true)
            {
                foreach (String s in circuitos)
                {
                    if (!circuitList.Contains(s))
                    {
                        this.circuitList.Add(s);
                    }
                }
            }
            else
            {
                foreach (String s in circuitos)
                {
                    if (!circuitList.Contains(s) && s != "R")
                    {
                        this.circuitList.Add(s);
                    }
                }
            }
        }

        public List<ElementId> GetPredecessorElement(Document doc)   //EM TEORIA É PARA PEGAR O ELEMENTO ANTES DO ELETRODUTO (NODE OU INSTANCIA QUALQUER)
        {
            List<ElementId> temp = new List<ElementId>();
            temp.Add(eletrodutoList[0]);
            List<ElementId> connectorsIds = new List<ElementId>();
            FilteredElementCollector filterCurve = new FilteredElementCollector(doc, temp).OfClass(typeof(MEPCurve));
            List<ElementId> alreadySearched = new List<ElementId>();
            List<Connector> connectorList = new List<Connector>();
            foreach (MEPCurve mep in filterCurve)
            {
                foreach (Connector c in GetConnectorsFromCurve(mep, connectorsIds))
                {
                    connectorsIds.Add(c.Owner.Id);
                }
            }

            return connectorsIds;
        }

        public List<Connector> GetConnectorsFromCurve(MEPCurve mepCurve, List<ElementId> alreadySearched)
        {
            //1. Get connector set of MEPCurve
            ConnectorSet connectorSet = mepCurve.ConnectorManager.Connectors;
            //2. Initialise empty list of connectors
            List<Connector> connectorList = new List<Connector>();
            //3. Loop through connector set and add to list
            foreach (Connector connector in connectorSet)
            {
                if (connector != null && (connector.ConnectorType == ConnectorType.End || connector.ConnectorType == ConnectorType.Curve || connector.ConnectorType == ConnectorType.Physical) && connector.IsConnected == true)
                {
                    ConnectorSet cs = connector.AllRefs;
                    ConnectorSetIterator csi = cs.ForwardIterator();
                    while (csi.MoveNext())
                    {
                        Connector current = csi.Current as Connector;
                        if (!this.eletrodutoList.Contains(current.Owner.Id) && current.ConnectorType != ConnectorType.Logical && current.Owner.Category.Name != "Wires" && current.Owner.Category.Name != "Electrical Circuits")
                        {
                            connectorList.Add(current); //ERRO PEGANDO ELEMENTO DO ELETRODUTO LIST EM VEZ DE PEGAR SOMENTE O ELEMENTO ANTECESSOR
                        }
                    }
                }
            }
            return connectorList;
        }
    }
}
