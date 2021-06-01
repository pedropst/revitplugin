using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Drawing;
using System.Windows.Forms;

namespace EasyEletrica
{
    public struct Tomada
    {
        public List<string> circuits;
        public VirtualConduit conduit;
        public List<WiringType> wires;

        public Tomada(Document doc, ElementId ei)
        {
            FamilyInstance element = doc.GetElement(ei) as FamilyInstance;
            List<String> _circuits = new List<string>();
            List<WiringType> _wires = new List<WiringType>();
            foreach (ElementId subId in element.GetSubComponentIds())
            {
                if (doc.GetElement(subId).Category.Name == "Electrical Fixtures")
                {

                    MEPModel mep = (doc.GetElement(subId) as FamilyInstance).MEPModel;
                    if (mep.ElectricalSystems != null)
                    {
                        String circuitNumber = doc.GetElement(subId).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                        _circuits.Add(circuitNumber);
                        WiringType wt = new WiringType(circuitNumber);

                        Autodesk.Revit.DB.Electrical.ElectricalSystemSet ess = mep.ElectricalSystems;
                        Autodesk.Revit.DB.Electrical.ElectricalSystemSetIterator essi = ess.ForwardIterator();

                        while (essi.MoveNext())
                        {

                            if ((essi.Current as Autodesk.Revit.DB.Electrical.ElectricalSystem).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 2)
                            {
                                wt.isBifasico = true;
                            }
                            else if ((essi.Current as Autodesk.Revit.DB.Electrical.ElectricalSystem).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 3)
                            {
                                wt.isTrifasico = true;
                            }
                            else
                            {
                                wt.fase = true;
                                wt.neutro = true;
                            }
                            break;
                        }

                        wt.terra = true;
                        if (!_wires.Contains(wt))
                        {
                            _wires.Add(wt);
                        }
                    }


                    /*if (doc.GetElement(subId).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 2)
                    {
                        wt.isBifasico = true;
                    }
                    else if (doc.GetElement(subId).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 3)
                    {
                        wt.isTrifasico = true;
                    }
                    else
                    {
                        wt.fase = true;
                        wt.neutro = true;
                    }*/


                }
            }

            this.circuits = _circuits;
            this.conduit = null;
            this.wires = _wires;
        }
    }
    public struct Interruptor
    {
        public List<string> circuits;
        public List<string> switches;
        public VirtualConduit conduit;
        public List<WiringType> wires;
        public bool isParalelo;
        public bool isIntermediario;

        public Interruptor(Document doc, ElementId ei)
        {
            FamilyInstance element = doc.GetElement(ei) as FamilyInstance;
            List<String> _circuits = new List<string>();
            List<String> _switches = new List<string>();
            List<WiringType> _wires = new List<WiringType>();

            foreach (ElementId subId in element.GetSubComponentIds())
            {
                if (doc.GetElement(subId).Category.Name == "Lighting Devices")
                {
                    String circuitNumber = doc.GetElement(subId).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                    String switchId = doc.GetElement(subId).get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString();
                    _circuits.Add(circuitNumber);
                    _switches.Add(switchId);
                    WiringType wt = new WiringType(circuitNumber, switchId);
                    wt.fase = true;
                    _wires.Add(wt);
                }
                else if (doc.GetElement(subId).Category.Name == "Eletrical Fixtures")
                {
                    String circuitNumber = doc.GetElement(subId).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                    _circuits.Add(circuitNumber);
                    WiringType wt = new WiringType(circuitNumber);
                    if (doc.GetElement(subId).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsDouble() == 2)
                    {
                        wt.isBifasico = true;
                    }
                    else if (doc.GetElement(subId).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsDouble() == 3)
                    {
                        wt.isTrifasico = true;
                    }
                    else
                    {
                        wt.fase = true;
                        wt.neutro = true;
                    }
                    wt.terra = true;
                    _wires.Add(wt);
                }
            }

            this.circuits = _circuits;
            this.conduit = null;
            this.wires = _wires;
            this.switches = _switches;
            this.isParalelo = false;
            this.isIntermediario = false;
        }

    }
    public struct Lampada
    {
        public List<string> circuits;
        public List<string> switches;
        public VirtualConduit conduit;
        public List<WiringType> wires;

        public Lampada(Document doc, ElementId ei)
        {
            FamilyInstance element = doc.GetElement(ei) as FamilyInstance;
            List<String> _circuits = new List<string>();
            List<String> _switches = new List<string>();
            List<WiringType> _wires = new List<WiringType>();

            String circuitNumber = doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
            String switchId = doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString();
            _circuits.Add(circuitNumber);
            _switches.Add(switchId);
            WiringType wt = new WiringType(circuitNumber, switchId);
            wt.neutro = true;
            _wires.Add(wt);

            this.circuits = _circuits;
            this.conduit = null;
            this.wires = _wires;
            this.switches = _switches;
        }

    }

    [Serializable]
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Automation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region <-- CRIANDO AS VARIÁVEIS -->
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<VirtualLighting> allLights = new List<VirtualLighting>();
            List<VirtualConduit> allConduits = new List<VirtualConduit>();
            List<VirtualNode> allNodes = new List<VirtualNode>();
            DFS sortingConduits = new DFS();
            #endregion

            #region <-- SELECIONANDO O QUADRO -->
            FilteredElementCollector panels = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_ElectricalEquipment);
            List<String> panels_name = (from p in panels where p != null select (p as FamilyInstance).Name).ToList();

                MenuList ml = new MenuList("Selecione o quadro: ", panels_name);
                ml.label2.Visible = false;
                ml.textBox1.Visible = false;
                System.Drawing.Point pt = new System.Drawing.Point(244, 100);
                ml.button1.Location = pt;
                Size size = new Size(344, 150);
                ml.Size = size;
                Application.Run(ml);

            Element panel = panels.First(a => a.Name == ml.ItemSelected().First());
            #endregion

            #region <-- ENCONTRANDO E ORDENANDO OS ELETRODUTOS -->
            allNodes = sortingConduits.Main(panel, doc);

            foreach (VirtualNode vn in allNodes)
            {
                foreach (VirtualConduit vc in vn.CuttingPieces(doc, vn, panel.Name))
                {
                    allConduits.Add(vc);
                }
            }

            AssemblingConduitToNodes(allNodes, allConduits);

            allNodes.Reverse();
            AssemblingConduitsParentsAndChilds(allNodes);
            #endregion

            /*foreach (VirtualConduit vc in allConduits)
            {
                ElementId ei = vc.elements.Last();
                if (doc.GetElement(ei).Category.Name == "Electrical Fixtures")
                {
                    Tomada t = new Tomada(doc, ei);
                    vc.tomadas.Add(t);
                }
                else if (doc.GetElement(ei).Category.Name == "Lighting Fixtures")
                {
                    Lampada l = new Lampada(doc, ei);
                    vc.lampadas.Add(l);
                }
                else if (doc.GetElement(ei).Category.Name == "Lighting Devices")
                {
                    Interruptor i = new Interruptor(doc, ei);
                    vc.interruptores.Add(i);
                }
            }*/

            #region <-- DISTRUBUINDO FIAÇÃO DAS TOMADAS -->
            allConduits.Reverse();
            ConduitCircuitsDistribution(allConduits);
            #endregion

            #region <-- DETERMINANDO FIAÇÃO DA ILUMINAÇÃO -->
            allLights = SwitchDistribution(doc, allConduits);
            #endregion

            #region <-- DISTRUIBUINDO FIAÇÃO DA ILUMINAÇÃO -->
            foreach (VirtualLighting vl in allLights)
            {
                foreach (VirtualConduit vc in vl.pathToFirstSwitch)
                {
                    bool verif = false;
                    foreach (WiringType wt in vc.wires)
                    {
                        if (wt.circuit == vl.circuit)
                        {
                            verif = true;
                            wt.fase = true;
                        }
                    }
                    if (!verif)
                    {
                        WiringType wtt = new WiringType(vl.circuit, vl.switchId);
                        wtt.fase = true;
                        vc.wires.Add(wtt);
                    }
                }

                foreach (VirtualConduit vc in vl.pathBetweenSwitchAndLamps)
                {
                    bool verif = false;
                    foreach (WiringType wt in vc.wires)
                    {
                        if (wt.id == vl.id)
                        {
                            verif = true;
                            wt.retorno = true;
                            wt.qntRetorno += 1;
                            wt.switchID.Add(vl.switchId);
                        }
                    }
                    if (!verif)
                    {
                        WiringType wtt = new WiringType(vl.circuit, vl.switchId);
                        wtt.retorno = true;
                        wtt.qntRetorno += 1;
                        wtt.switchID.Add(vl.switchId);
                        vc.wires.Add(wtt);
                    }
                }

                foreach (VirtualConduit vc in vl.pathBetweenSwitches)
                {
                    bool verif = false;
                    foreach (WiringType wt in vc.wires)
                    {
                        if (wt.id == vl.id)
                        {
                            verif = true;
                            wt.retorno = true;
                            wt.switchID.Add(vl.switchId);
                            wt.qntRetorno += 2;
                        }
                    }
                    if (!verif)
                    {
                        WiringType wtt = new WiringType(vl.circuit, vl.switchId);
                        wtt.retorno = true;
                        wtt.qntRetorno += 2;
                        wtt.switchID.Add(vl.switchId);
                        vc.wires.Add(wtt);
                    }
                }

                foreach (VirtualConduit vc in vl.pathBetweenSecondSwitchAndLamps)
                {
                    bool verif = false;
                    foreach (WiringType wt in vc.wires)
                    {
                        if (wt.id == vl.id)
                        {
                            verif = true;
                            wt.retorno = true;
                            wt.switchID.Add(vl.switchId);
                            wt.qntRetorno += 1;
                        }
                    }
                    if (!verif)
                    {
                        WiringType wtt = new WiringType(vl.circuit, vl.switchId);
                        wtt.retorno = true;
                        wtt.switchID.Add(vl.switchId);
                        wtt.qntRetorno += 1;
                        vc.wires.Add(wtt);
                    }
                }

                foreach (VirtualConduit vc in vl.pathToLamps)
                {
                    bool verif = false;
                    foreach (WiringType wt in vc.wires)
                    {
                        if (wt.circuit == vl.circuit)
                        {
                            verif = true;
                            wt.neutro = true;
                        }
                    }
                    if (!verif)
                    {
                        WiringType wtt = new WiringType(vl.circuit, vl.switchId);
                        wtt.neutro = true;
                        vc.wires.Add(wtt);
                    }
                }
            }
            #endregion
            
            #region <-- AGRUPANDO FIAÇÕES DE MESMO CIRCUITO -->
            foreach (VirtualConduit vc in allConduits)
            {
                List<WiringType> toRemove = new List<WiringType>();
                foreach (WiringType wt in vc.wires)
                {
                    foreach (WiringType wtt in vc.wires)
                    {
                        if (wt != wtt && !toRemove.Contains(wt))
                        {
                            if (wt.circuit == wtt.circuit)
                            {
                                wt.fase = wt.fase || wtt.fase;
                                wt.neutro = wt.neutro || wtt.neutro;
                                wt.terra = wt.terra || wtt.terra;
                                if (wt.retorno == false)
                                {
                                    wt.switchID.Clear();
                                }
                                wt.retorno = wt.retorno || wtt.retorno;
                                if (wt.qntRetorno + wtt.qntRetorno <= 3)
                                {
                                    if (wtt.retorno == true)
                                    {
                                        foreach (String s in wtt.switchID)
                                        {
                                            wt.switchID.Add(s);
                                        }
                                        wt.qntRetorno += wtt.qntRetorno;
                                        toRemove.Add(wtt);
                                    }
                                }
                                else
                                {
                                    wtt.fase = false;
                                    wtt.neutro = false;
                                    wtt.terra = false;
                                }
                            }
                        }
                    }
                }
                foreach (WiringType wt in vc.wires)
                {
                    if (wt.circuit == "")
                    {
                        toRemove.Add(wt);
                    }
                }

                foreach (WiringType wt in toRemove)
                {
                    vc.wires.Remove(wt);
                }
            }
            #endregion
            
            #region <-- ORDENANDO FIAÇÕES EM ORDEM CRESCENTE -->
            foreach (VirtualConduit vc in allConduits)
            {
                var sort = vc.wires.OrderBy(a => System.Convert.ToInt32(a.circuit.Replace(",", "")));//.ThenBy(b => from c in b.switchID where b.switchID.Count > 0 select c).ToList(); // b => b.switchID.ElementAt(0));.ToList()
                foreach (WiringType wt in vc.wires)
                {
                    var sort1 = wt.switchID.OrderBy(a => a);
                    wt.switchID = sort1.ToHashSet();
                }
                vc.wires = sort.ToList();
            }
            #endregion

            #region <-- ORDENANDO ELETRODUTOS CONFORME POSIÇÃO NO EIXO Y -->
            allConduits = allConduits.OrderBy(v => v.midPoint.Y).ToList();
            #endregion

            #region <-- INICIANDO TRANSAÇÃO -->
            using (Transaction trans_pdoc = new Transaction(doc, "Arrumando fiação"))
            {
                FamilySymbol toCopy = null;
                trans_pdoc.Start();
                HashSet<XYZ> origins = new HashSet<XYZ>();
                HashSet<System.Drawing.Rectangle> recs = new HashSet<System.Drawing.Rectangle>();

                List<RectangleF> recfs = new List<RectangleF>();

                #region <-- EVITANDO INTERSEÇÕES -->
                FilteredElementCollector fillRegionTypes = new FilteredElementCollector(doc).OfClass(typeof(FilledRegionType));
                IEnumerable<FilledRegionType> myPatterns = from pattern in fillRegionTypes.Cast<FilledRegionType>() where pattern.Name.Equals("Diagonal Crosshatch") select pattern;
                void Criando(List<XYZ> pointss)
                {
                    foreach (FilledRegionType frt in fillRegionTypes)
                    {
                        List<CurveLoop> profileloops
                          = new List<CurveLoop>();

                        Autodesk.Revit.DB.XYZ[] pointus = pointss.ToArray();

                        CurveLoop profileloop = new CurveLoop();

                        for (int i = 0; i < pointus.Length - 1; i++)
                        {
                            Line line = Line.CreateBound(pointus[i], pointus[i + 1]);
                            profileloop.Append(line);
                        }
                        profileloops.Add(profileloop);

                        ElementId activeViewId = doc.ActiveView.Id;

                        FilledRegion filledRegion = FilledRegion.Create(
                          doc, frt.Id, activeViewId, profileloops);

                        break;
                    }
                }

                FilteredElementCollector lamps = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_LightingFixtures);
                foreach (Element e in lamps)
                {
                    PointF pos = new PointF((float)(e.Location as LocationPoint).Point.X - 0.7f, (float)(e.Location as LocationPoint).Point.Y - 0.7f);
                    RectangleF recf = new RectangleF(pos, new SizeF(1.4f, 1.4f));

                    recfs.Add(recf);

                    List<XYZ> coords = new List<XYZ>();
                    coords.Add(new XYZ(recf.Left, recf.Top, 0f));
                    coords.Add(new XYZ(recf.Right, recf.Top, 0f));
                    coords.Add(new XYZ(recf.Right, recf.Bottom, 0f));
                    coords.Add(new XYZ(recf.Left, recf.Bottom, 0f));
                    coords.Add(new XYZ(recf.Left, recf.Top, 0f));

                    //Criando(coords);
                }

                FilteredElementCollector pontosdeparede = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_LightingDevices);
                foreach (Element e in pontosdeparede)
                {
                    int qntElementos = 1;
                    foreach (ElementId ei in (e as FamilyInstance).GetSubComponentIds())
                    {
                        if (doc.GetElement(ei).Category.Name == "Electrical Fixtures" || doc.GetElement(ei).Category.Name == "Lighting Devices")
                        {
                            qntElementos++;
                        }
                    }

                    PointF pos = new PointF((float)(e.Location as LocationPoint).Point.X, (float)(e.Location as LocationPoint).Point.Y - 0.4f);
                    if ((e as FamilyInstance).FacingOrientation.X == -1)
                    {
                        pos = new PointF((float)(e.Location as LocationPoint).Point.X - 0.8f * qntElementos, (float)(e.Location as LocationPoint).Point.Y - 0.4f);
                    }

                    RectangleF recf = new RectangleF(pos, new SizeF(0.8f*qntElementos, 0.8f));

                    recfs.Add(recf);

                    List<XYZ> coords = new List<XYZ>();
                    coords.Add(new XYZ(recf.Left, recf.Top, 0f));
                    coords.Add(new XYZ(recf.Right, recf.Top, 0f));
                    coords.Add(new XYZ(recf.Right, recf.Bottom, 0f));
                    coords.Add(new XYZ(recf.Left, recf.Bottom, 0f));
                    coords.Add(new XYZ(recf.Left, recf.Top, 0f));

                    //Criando(coords);
                }

                FilteredElementCollector pontosdetomadas = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_ElectricalFixtures);
                foreach (Element e in pontosdetomadas)
                {
                    int qntElementos = 1;
                    foreach (ElementId ei in (e as FamilyInstance).GetSubComponentIds())
                    {
                        if (doc.GetElement(ei).Category.Name == "Electrical Fixtures" || doc.GetElement(ei).Category.Name == "Lighting Devices")
                        {
                            qntElementos++;
                        }
                    }

                    PointF pos = new PointF((float)(e.Location as LocationPoint).Point.X, (float)(e.Location as LocationPoint).Point.Y - 0.4f);
                    if ((e as FamilyInstance).FacingOrientation.X == -1)
                    {
                        pos = new PointF((float)(e.Location as LocationPoint).Point.X - 0.8f * qntElementos, (float)(e.Location as LocationPoint).Point.Y - 0.4f);
                    }

                    RectangleF recf = new RectangleF(pos, new SizeF(0.8f * qntElementos, 0.8f));

                    recfs.Add(recf);

                    List<XYZ> coords = new List<XYZ>();
                    coords.Add(new XYZ(recf.Left, recf.Top, 0f));
                    coords.Add(new XYZ(recf.Right, recf.Top, 0f));
                    coords.Add(new XYZ(recf.Right, recf.Bottom, 0f));
                    coords.Add(new XYZ(recf.Left, recf.Bottom, 0f));
                    coords.Add(new XYZ(recf.Left, recf.Top, 0f));
                }

                ElementCategoryFilter fText = new ElementCategoryFilter(BuiltInCategory.OST_ElectricalFixtureTags);
                ElementCategoryFilter sText = new ElementCategoryFilter(BuiltInCategory.OST_LightingDeviceTags);
                LogicalOrFilter filter = new LogicalOrFilter(fText, sText);
                #endregion

                foreach (VirtualConduit vc in allConduits)
                {
                    XYZ origin = vc.midPoint;
                    XYZ leaderOrigin = origin;

                    origin = new XYZ() + origin + new XYZ(1f, -1f, 0);
                    vc.DetermineOrietation(doc);

                    PointF pos = new PointF((float)origin.X, (float)origin.Y - 0.5f);
                    System.Drawing.RectangleF rec = new System.Drawing.RectangleF(pos, new SizeF(0.4f * (vc.wires.Count + 1), 1f));
                    recfs.Add(rec);

                    bool verif = false;
                    int dir = 1;
                    do
                    {
                        verif = false;
                        foreach (RectangleF r in recfs)
                        {
                            if (r.IntersectsWith(rec) && r != rec && vc.wires.Count != 0)
                            {
                                verif = true;
                                List<float> distancias = new List<float>();
                                List<float> distanciasY = new List<float>();
                                float yDistanceTOP = (r.Bottom - rec.Top);
                                distanciasY.Add(yDistanceTOP);
                                float yDistanceBOT = (r.Left - rec.Right);
                                distanciasY.Add(yDistanceBOT);

                                distanciasY = distanciasY.OrderBy(f => Math.Abs(f)).ToList();


                                rec.Offset(0, yDistanceTOP*dir);

                                List<XYZ> lista = new List<XYZ>();
                                lista.Add(new XYZ(rec.Left, rec.Top, 0f));
                                lista.Add(new XYZ(rec.Right, rec.Top, 0f));
                                lista.Add(new XYZ(rec.Right, rec .Bottom, 0f));
                                lista.Add(new XYZ(rec.Left, rec.Bottom, 0f));
                                lista.Add(new XYZ(rec.Left, rec.Top, 0f));
                               // Criando(lista);
                                origin = new XYZ(rec.X, rec.Y + 0.6f, origin.Z);
                            }

                        }

                        if (origin.DistanceTo(leaderOrigin) >= 4f)
                        {
                            origin = new XYZ() + leaderOrigin + new XYZ(1f, -1f, 0);
                            verif = false;
                            dir = -1;
                        }
                    } while (verif);

                    recfs.Remove(recfs.Last());
                    recfs.Add(rec);
                    origins.Add(origin);

                    int intNum = 0;
                    string terra = "0";
                    foreach (WiringType wt in vc.wires)
                    {
                        String newId = "";
                        foreach (String s in wt.switchID)
                        {
                            newId += s;
                        }
                        wt.id = wt.circuit + newId;
                        string outp = wt.Output();
                        wt.GettingWireSize(panel as FamilyInstance);

                        if (!outp.Contains("T"))
                        {
                            FilteredElementCollector collectorOfDoc = new FilteredElementCollector(doc).OfClass(typeof(Family));
                            Family family = collectorOfDoc.FirstOrDefault<Element>(e => e.Name.Equals(outp)) as Family;
                            toCopy = doc.GetElement(family.GetFamilySymbolIds().First()) as FamilySymbol;
                            Element fi = doc.Create.NewFamilyInstance(origin + new XYZ(intNum/ 2.5, 0, 0), toCopy, doc.ActiveView);
                            vc.tagIds.Add(fi.Id);
                            if (vc.orietation == "VERTICAL")
                            {
                                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                                ogs.SetProjectionLineColor(new Autodesk.Revit.DB.Color(0, 0, 255));
                                doc.ActiveView.SetElementOverrides(fi.Id, ogs);
                            }


                            Parameter param = fi.LookupParameter("cima");
                            if (outp.Contains('R'))
                            {
                                param.Set(wt.id);
                            }
                            else
                            {
                                param.Set(wt.circuit);
                            }

                            param = fi.LookupParameter("baixo");
                            if (wt.bitola == "2,5")
                            {
                                param.Set("");
                            }
                            else
                            {
                                param.Set(wt.bitola);
                            }
                            if (wt == vc.wires.First())
                            {
                                (fi as AnnotationSymbol).addLeader();
                                (fi as AnnotationSymbol).GetLeaders().First().End = leaderOrigin;
                            }


                            if (wt.bitola == "2,5")
                            {
                                param = fi.LookupParameter("comp_2.5");
                                //param.Set(vc.comprimento);
                            }
                            else if (wt.bitola == "4")
                            {
                                param = fi.LookupParameter("comp_4");
                                //param.Set(vc.comprimento);
                            }
                        }
                        else
                        {
                            FilteredElementCollector collectorOfDoc = new FilteredElementCollector(doc).OfClass(typeof(Family));
                            Family family = collectorOfDoc.FirstOrDefault<Element>(e => e.Name.Equals(outp.Replace("T",""))) as Family;
                            toCopy = doc.GetElement(family.GetFamilySymbolIds().First()) as FamilySymbol;

                            Element fi = doc.Create.NewFamilyInstance(origin + new XYZ(intNum / 2.5, 0, 0), toCopy, doc.ActiveView);
                            vc.tagIds.Add(fi.Id);

                            if (vc.orietation == "VERTICAL")
                            {
                                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                                ogs.SetProjectionLineColor(new Autodesk.Revit.DB.Color(0, 0, 255));
                                doc.ActiveView.SetElementOverrides(fi.Id, ogs);
                            }

                            Parameter param = fi.LookupParameter("cima");
                            param.Set(wt.circuit);
                            param = fi.LookupParameter("baixo");
                            if (wt.bitola == "2,5")
                            {
                                param.Set("");
                            }
                            else
                            {
                                param.Set(wt.bitola);
                            }
                            if (wt == vc.wires.First())
                            {
                                (fi as AnnotationSymbol).addLeader();
                                (fi as AnnotationSymbol).GetLeaders().First().End = leaderOrigin;
                            }

                            if (wt.bitola == "2,5")
                            {
                                param = fi.LookupParameter("comp_2.5");
                                //param.Set(vc.comprimento);
                            }
                            else if (wt.bitola == "4")
                            {
                                param = fi.LookupParameter("comp_4");
                                //param.Set(vc.comprimento);
                            }

                            family = collectorOfDoc.FirstOrDefault<Element>(e => e.Name.Equals("T")) as Family;
                            toCopy = doc.GetElement(family.GetFamilySymbolIds().First()) as FamilySymbol;

                            if (wt.bitola != null)
                            {
                                if (System.Convert.ToDouble(wt.bitola.Replace(',', '.')) > System.Convert.ToDouble(terra))
                                {
                                    terra = wt.bitola.Replace(',', '.');
                                    fi = doc.Create.NewFamilyInstance(origin + new XYZ(vc.wires.Count / 2.5, 0, 0), toCopy, doc.ActiveView);
                                    vc.tagIds.Add(fi.Id);

                                    if (vc.orietation == "VERTICAL")
                                    {
                                        OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                                        ogs.SetProjectionLineColor(new Autodesk.Revit.DB.Color(0, 0, 255));
                                        doc.ActiveView.SetElementOverrides(fi.Id, ogs);
                                    }

                                    param = fi.LookupParameter("baixo");
                                    if (wt.bitola == "2,5")
                                    {
                                        param.Set("");
                                        //param.Set(vc.comprimento);
                                    }
                                    else
                                    {
                                        param.Set(wt.bitola);
                                        //param.Set(vc.comprimento);
                                    }
                                }
                            }


                            /*if (wt.bitola == "2,5")
                            {
                                param = fi.LookupParameter("comp_2.5");
                                //param.Set(vc.comprimento);
                            }
                            else if (wt.bitola == "4")
                            {
                                param = fi.LookupParameter("comp_4");
                                //param.Set(vc.comprimento);
                            }*/
                        }
                        intNum++;
                    }
                }
                trans_pdoc.Commit();
            }
            #endregion

            SwitchSystem ss = new SwitchSystem(uidoc, allLights);
            //ss.Show();
            return Result.Succeeded;
        }

        public List<VirtualLighting> SwitchDistribution(Document doc, List<VirtualConduit> allConduits)
        {
            //1 e 2
            List<VirtualLighting> retorno = new List<VirtualLighting>();
            List<VirtualLighting> toExplore = new List<VirtualLighting>();
            List<String> ids = new List<String>();
            foreach (VirtualConduit vc in allConduits)
            {
                Element lastElement = doc.GetElement(vc.elements.Last());
                if (lastElement.Category.Name == "Lighting Fixtures")
                {
                    String circuit = lastElement.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                    String switchId = lastElement.get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString();
                    String id = lastElement.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() +
                                lastElement.get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString();
                    if (!ids.Contains(id))
                    {
                        VirtualLighting vl = new VirtualLighting(circuit, switchId, id);
                        vl.inConduits.Add(vc);
                        toExplore.Add(vl);
                        ids.Add(id);
                    }
                    else
                    {
                        foreach (VirtualLighting vl in toExplore)
                        {
                            if (vl.id == id)
                            {
                                vl.inConduits.Add(vc);
                            }
                        }
                    }
                }
                else if(lastElement.Category.Name == "Lighting Devices")
                {
                    FamilyInstance fiLastElement = lastElement as FamilyInstance;

                    foreach (ElementId ei in fiLastElement.GetSubComponentIds())
                    {
                        Element e = doc.GetElement(ei);
                        if (e.Category.Name == "Lighting Devices")
                        {
                            String circuit = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                            String switchId = e.get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString();
                            String id = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() +
                                        e.get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString();
                            if (e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() == "" || e.get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString() == "")
                            {
                                Console.WriteLine(e.Id.ToString());
                            }
                            if (!ids.Contains(id))
                            {
                                VirtualLighting vl = new VirtualLighting(circuit, switchId, id);
                                vl.inConduits.Add(vc);
                                toExplore.Add(vl);
                                ids.Add(id);
                            }
                            else
                            {
                                foreach (VirtualLighting vl in toExplore)
                                {
                                    if (vl.id == id)
                                    {
                                        vl.inConduits.Add(vc);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //4.1
            foreach (VirtualLighting vl in toExplore)
            {
                var sort = vl.inConduits.OrderBy(vc => vc.conduitId);
                vl.inConduits = sort.ToHashSet();


                foreach (VirtualConduit vc in vl.inConduits)
                {
                    if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Devices")
                    {
                        foreach (ElementId ei in (doc.GetElement(vc.elements.Last()) as FamilyInstance).GetSubComponentIds())
                        {
                            if (doc.GetElement(ei).Category.Name == "Lighting Devices")
                            {
                                if (doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() +
                                    doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString() == vl.id)
                                {
                                    vl.qntInterruptor++;
                                }
                            }
                        }
                    }
                }

                //4.2
                VirtualConduit eletroduto = vl.inConduits.First();
                while (eletroduto.beforeConduit.Count > 0)
                {
                    vl.refConduits.Add(eletroduto);
                    eletroduto = eletroduto.beforeConduit.First();
                }
                vl.refConduits.Add(eletroduto);

                HashSet<VirtualConduit> toRemoveList = new HashSet<VirtualConduit>();
                foreach (VirtualConduit vc in vl.inConduits)
                {
                    eletroduto = vc;
                    while (eletroduto.beforeConduit.Count > 0)
                    {
                        if (vl.refConduits.Contains(eletroduto))
                        {
                            toRemoveList.Add(eletroduto);
                        }
                        else
                        {
                            vl.othersConduits.Add(eletroduto);
                        }
                        eletroduto = eletroduto.beforeConduit.First();
                    }
                    //vl.othersConduits.Add(eletroduto);
                }

                foreach (VirtualConduit vc in vl.refConduits)
                {
                    if (!toRemoveList.Contains(vc))
                    {
                        vl.othersConduits.Add(vc);
                    }
                }

                sort = vl.othersConduits.OrderBy(a => a.conduitId);

                vl.othersConduits = sort.ToHashSet();

                if (vl.othersConduits.Count > 1 && !vl.othersConduits.First().afterConduit.Contains(vl.othersConduits.ElementAt(1)))
                {
                    vl.othersConduits.Remove(vl.othersConduits.First());
                }


                if (doc.GetElement(vl.refConduits.First().elements.Last()).Category.Name == "Lighting Devices")
                {
                    foreach (ElementId ei in (doc.GetElement(vl.refConduits.First().elements.Last()) as FamilyInstance).GetSubComponentIds())
                    {
                        if (doc.GetElement(ei).Category.Name == "Lighting Devices")
                        {
                            if (doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() +
                                doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString() == vl.id)
                            {
                                foreach (VirtualConduit vc in vl.refConduits)
                                {
                                    if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Devices")
                                    {
                                        eletroduto = vc;
                                        while (eletroduto.beforeConduit.Count > 0)
                                        {
                                            vl.pathToFirstSwitch.Add(eletroduto);
                                            eletroduto = eletroduto.beforeConduit.First();
                                        }
                                        vl.pathToFirstSwitch.Add(eletroduto);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (VirtualConduit vc in vl.othersConduits)
                    {
                        if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Devices")
                        {
                            eletroduto = vc;
                            while (eletroduto.beforeConduit.Count > 0)
                            {
                                vl.pathToFirstSwitch.Add(eletroduto);
                                eletroduto = eletroduto.beforeConduit.First();
                            }
                            vl.pathToFirstSwitch.Add(eletroduto);
                            break;
                        }
                    }
                }

                /*                else
                {
                    foreach (VirtualConduit vc in vl.othersConduits)
                    {
                        if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Devices")
                        {
                            foreach (ElementId ei in (doc.GetElement(vc.elements.Last()) as FamilyInstance).GetSubComponentIds())
                            {
                                if (doc.GetElement(ei).Category.Name == "Lighting Devices")
                                {
                                    if (doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() +
                                        doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString() == vl.id)
                                    {
                                        eletroduto = vc;
                                        while (eletroduto.beforeConduit.Count > 0)
                                        {
                                            vl.pathToFirstSwitch.Add(eletroduto);
                                            eletroduto = eletroduto.beforeConduit.First();
                                        }
                                        vl.pathToFirstSwitch.Add(eletroduto);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }*/

                if (vl.qntInterruptor >= 2)
                {
                    List<VirtualConduit> firstPath = new List<VirtualConduit>();
                    List<VirtualConduit> secondPath = new List<VirtualConduit>();
                    int count = 0;
                    foreach (VirtualConduit vc in vl.inConduits)
                    {
                        if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Devices")
                        {
                            foreach (ElementId ei in (doc.GetElement(vc.elements.Last()) as FamilyInstance).GetSubComponentIds())
                            {
                                if (doc.GetElement(ei).Category.Name == "Lighting Devices")
                                {
                                    if (doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() +
                                        doc.GetElement(ei).get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM).AsString() == vl.id)
                                    {
                                        eletroduto = vc;
                                        count++;
                                        if (count == 1)
                                        {
                                            while (eletroduto.beforeConduit.Count > 0)
                                            {
                                                firstPath.Add(eletroduto);
                                                eletroduto = eletroduto.beforeConduit.First();
                                            }
                                            firstPath.Add(eletroduto);
                                        }
                                        else
                                        {
                                            while (eletroduto.beforeConduit.Count > 0)
                                            {
                                                secondPath.Add(eletroduto);
                                                eletroduto = eletroduto.beforeConduit.First();
                                            }
                                            secondPath.Add(eletroduto);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (VirtualConduit vc in firstPath)
                    {
                        if (secondPath.Contains(vc))
                        {
                            int corte = secondPath.IndexOf(vc);
                            secondPath.RemoveRange(corte, secondPath.Count - corte);
                            break;
                        }
                        vl.pathBetweenSwitches.Add(vc);
                    }
                    secondPath.Reverse();
                    vl.pathBetweenSwitches.UnionWith(secondPath);
                }

                foreach (VirtualConduit vc in vl.inConduits)
                {
                    eletroduto = vc;
                    if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Devices" && vc != vl.pathToFirstSwitch.First())
                    {
                        while (eletroduto.beforeConduit.Count > 0)
                        {
                            vl.pathToSecondSwitch.Add(eletroduto);
                            eletroduto = eletroduto.beforeConduit.First();
                        }
                        vl.pathToSecondSwitch.Add(eletroduto);
                        break;
                    }
                }

                foreach (VirtualConduit vc in vl.inConduits)
                {
                    if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Fixtures")
                    {
                        eletroduto = vc;
                        while (eletroduto.beforeConduit.Count > 0)
                        {
                            vl.pathToLamps.Add(eletroduto);
                            eletroduto = eletroduto.beforeConduit.First();
                        }
                        vl.pathToLamps.Add(eletroduto);
                    }
                }

                if (vl.qntInterruptor == 1)
                {
                    foreach (VirtualConduit vc in vl.inConduits)
                    {
                        if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Fixtures")
                        {
                            eletroduto = vc;
                            while (eletroduto.beforeConduit.Count > 0 && !vl.pathToFirstSwitch.Contains(eletroduto))
                            {
                                vl.pathBetweenSwitchAndLamps.Add(eletroduto);
                                eletroduto = eletroduto.beforeConduit.First();
                            }
                            vl.pathBetweenSwitchAndLamps.Add(vl.pathToFirstSwitch.First());
                        }
                    }
                    foreach (VirtualConduit vc in vl.pathToFirstSwitch)
                    {
                        if (!vl.refConduits.Contains(vc))
                        {
                            vl.pathBetweenSwitchAndLamps.Add(vc);
                        }
                    }
                }
                else if (vl.qntInterruptor >= 2)
                {
                    foreach (VirtualConduit vc in vl.inConduits)
                    {
                        if (doc.GetElement(vc.elements.Last()).Category.Name == "Lighting Fixtures" && vc != vl.pathToFirstSwitch.First())
                        {
                            eletroduto = vc;
                            while(eletroduto.beforeConduit.Count > 0 && !vl.pathToSecondSwitch.Contains(eletroduto))
                            {
                                vl.pathBetweenSecondSwitchAndLamps.Add(eletroduto);
                                eletroduto = eletroduto.beforeConduit.First();
                            }
                            //vl.pathBetweenSecondSwitchAndLamps.Add(eletroduto);
                        }
                    }
                    foreach (VirtualConduit vc in vl.pathToSecondSwitch)
                    {
                        if (doc.GetElement(vl.inConduits.First().elements.Last()).Category.Name == "Lighting Devices")
                        {
                            if (!vl.refConduits.Contains(vc) && !vl.pathToLamps.Contains(vc))
                            {
                                vl.pathBetweenSecondSwitchAndLamps.Add(vc);
                            }
                        }
                        else
                        {
                            if (!vl.refConduits.Contains(vc))
                            {
                                vl.pathBetweenSecondSwitchAndLamps.Add(vc);
                            }
                        }

                    }
                    vl.pathBetweenSecondSwitchAndLamps.Add(vl.pathToSecondSwitch.First());
                }
            }
            return toExplore;
        }

        public void ConduitCircuitsDistribution(List<VirtualConduit> allConduits)
        {
            /*foreach (VirtualConduit vc in allConduits)
            {
                VirtualConduit toIterate = vc;
                while (toIterate.beforeConduit.Count > 0)
                {
                    foreach (Tomada t in toIterate.tomadas)
                    {
                        if (!toIterate.tomadas.Contains(t))
                        {
                            toIterate.beforeConduit.First().tomadas.Add(t);
                        }
                    }
                    toIterate = toIterate.beforeConduit.First();
                }
            }

            foreach (VirtualConduit vc in allConduits)
            {
                foreach (Tomada t in vc.tomadas)
                {
                    foreach (WiringType wt in t.wires)
                    {
                        //if (!vc.wires.Contains(wt))
                        //{
                            //wt.circuit += "1";
                            vc.wires.Add(wt);
                        //}
                    }
                }
            }*/

            foreach (VirtualConduit vc in allConduits)
            {
                VirtualConduit toIterate = vc;
                do
                {
                    if (toIterate.beforeConduit.Count > 0)
                    {
                        foreach (String c in toIterate.circuits)
                        {
                            bool teste = true;
                            foreach (String c1 in toIterate.beforeConduit.First().circuits)
                            {
                                if (c == c1)
                                {
                                    teste = false;
                                }
                            }
                            if (teste)
                            {
                                toIterate.beforeConduit.First().circuits.Add(c);
                            }

                        }
                        if (toIterate.beforeConduit.Count > 0)
                        {
                            toIterate = toIterate.beforeConduit.First();
                        }
                    }
                } while (toIterate.beforeConduit.Count > 0);
            }

            foreach (VirtualConduit vc in allConduits)
            {
                VirtualConduit toIterate = vc;
                do
                {
                    if (toIterate.beforeConduit.Count > 0)
                    {
                        foreach (WiringType wt in toIterate.wires)
                        {
                            bool teste = true;
                            foreach (WiringType wt1 in toIterate.beforeConduit.First().wires)
                            {
                                if (wt1.id == wt.id)
                                {
                                    teste = false;
                                    if (wt1.fase == false)
                                    {
                                        wt1.fase = wt.fase;
                                    }
                                    if (wt1.neutro == false)
                                    {
                                        wt1.neutro = wt.neutro;
                                    }
                                    if (wt1.terra == false)
                                    {
                                        wt1.terra = wt.terra;
                                    }
                                }
                            }
                            if (teste)
                            {
                                toIterate.beforeConduit.First().wires.Add(wt);
                            }
                        }
                        if (toIterate.beforeConduit.Count > 0)
                        {
                            toIterate = toIterate.beforeConduit.First();
                        }
                    }
                } while (toIterate.beforeConduit.Count > 0);
            }
        }

        public void AssemblingConduitsParentsAndChilds(List<VirtualNode> allNodes)
        {
            foreach (VirtualNode vn in allNodes)
            {
                foreach (List<VirtualConduit> list in vn.conduits)
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i] != list.First() && list[i] != list.Last())
                        {
                            list[i].beforeConduit.Add(list[i - 1]);
                            list[i].afterConduit.Add(list[i + 1]);
                        }
                        else if (list[i] == list.First())
                        {
                            if (vn.beforeConduit.Count > 0)
                                list[i].beforeConduit.Add(vn.beforeConduit.First());
                            if (list.Count > 1)
                            {
                                list[i].afterConduit.Add(list[i + 1]);
                            }
                        }
                        else if (list.Count > 1 && list[i] == list.Last())
                        {
                            list[i].beforeConduit.Add(list[i - 1]);
                        }
                    }
                }
            }
        }

        public void AssemblingConduitToNodes(List<VirtualNode> allNodes, List<VirtualConduit> allConduits)
        {
            foreach (VirtualNode vn in allNodes)
            {
                foreach (VirtualConduit vc in allConduits)
                {
                    if (vn.outElements.Contains(vc.elements.First()))
                    {
                        vn.afterConduits.Add(vc);
                    }
                    if (vn.nodeId == vc.elements.Last())
                    {
                        vn.beforeConduit.Add(vc);
                    }
                }
            }
        }
    }
}
