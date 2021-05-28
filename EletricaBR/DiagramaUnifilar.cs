using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace EasyEletrica
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class DiagramaUnifilar : IExternalCommand
    {
        Element diagram = null;
        List<Autodesk.Revit.DB.Electrical.ElectricalSystem> circuitList = new List<Autodesk.Revit.DB.Electrical.ElectricalSystem>();
        List<List<String>> circuitParameters = new List<List<String>>();
        List<List<Parameter>> diagramParameters = new List<List<Parameter>>();
        List<List<String>> parametersOfCircuits = new List<List<String>>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            #region <-- Checking diagrams' family loading situation -->
            FilteredElementCollector familiesCollection = new FilteredElementCollector(doc).OfClass(typeof(Family));
            Family family = familiesCollection.FirstOrDefault<Element>(e => e.Name.Equals("DiagramaUnifilar")) as Family;

            if (family != null)
            {
                FamilySymbol symbol = doc.GetElement(family.GetFamilySymbolIds().ToList()[0]) as FamilySymbol;
                #region <-- Setting the diagrams' position -->
                if (uidoc.CanPlaceElementType(symbol))
                {
                    XYZ position = uidoc.Selection.PickPoint();
                    Autodesk.Revit.DB.View view = doc.ActiveView;
                    List<ElementId> addedElements = new List<ElementId>();
                    using (Transaction trans = new Transaction(doc, "Placing diagram"))
                    {
                        trans.Start();
                        diagram = doc.Create.NewFamilyInstance(position, symbol, view) as Element;
                        void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
                        {
                            addedElements.AddRange(e.GetAddedElementIds());
                        }
                        trans.Commit();
                    }
                }
                else
                {
                    TaskDialog.Show("EasyElétrica", "Não é possível colocar o diagrama nessa vista/layout, tente novamente em outra vista 2D.");
                    return Result.Failed;
                }
                #endregion

                #region <-- Selecting the panel -->
                FilteredElementCollector panelsCollection = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_ElectricalEquipment);
                List<String> panelsName = (from p in panelsCollection where p != null select (p as FamilyInstance).Name).ToList();

                MenuList ml = new MenuList("Selecione o quadro: ", panelsName);
                ml.label2.Visible = false;
                ml.textBox1.Visible = false;
                System.Drawing.Point pt = new System.Drawing.Point(244, 100);
                ml.button1.Location = pt;
                Size size = new Size(344, 150);
                ml.Size = size;
                Application.Run(ml);

                Element panel = panelsCollection.First(a => a.Name == ml.ItemSelected().First());
                #endregion

                #region <-- Getting circuits information -->
                if (panel == null)
                {
                    TaskDialog.Show("PEDRO ELÉTRICA", "Nenhum quadro encontrado com esse nome.");
                    return Result.Failed;
                }
                else
                {
                    #region <-- Getting the circuits from the panel -->
                    List<Autodesk.Revit.DB.Electrical.ElectricalSystem> outOrderCircuitList = new List<Autodesk.Revit.DB.Electrical.ElectricalSystem>();
                    PanelClass panelclass = new PanelClass(doc, panel); // PROTOTYPE VERSION - THE FINAL ONE SHOULD GET THE CIRCUIT LIST FROM PANEL LIST IN THE MAIN CLASS INSTEAD OF CREATING A PANELCLASS OBJECT HERE
                    foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem circuit in panelclass.circuits)
                    {
                        outOrderCircuitList.Add(circuit);
                    }

                    #endregion

                    #region <-- Getting DR parameters -->

                    foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem c in outOrderCircuitList)
                    {
                        Parameter DR = c.LookupParameter("DR");
                        Parameter vDR = c.LookupParameter("vDR");
                        if (DR.AsString() == "" || DR.AsString() == " " || DR.AsString() == "   " || DR == null)
                        {
                            using (Transaction trans = new Transaction(doc, "Inserting DR values and groups to 0, when not specified."))
                            {
                                trans.Start();
                                DR.Set("0");
                                vDR.Set("0");
                                trans.Commit();
                            }
                        }
                    }

                    #endregion

                    #region <-- Sorting by the circuit number -->

                    var inOrder = from sublist in outOrderCircuitList orderby sublist.StartSlot ascending select sublist;
                    List<Autodesk.Revit.DB.Electrical.ElectricalSystem> circuitList = new List<Autodesk.Revit.DB.Electrical.ElectricalSystem>();
                    foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem c in inOrder)
                    {
                        circuitList.Add(c);
                    }

                    #endregion

                    #region <-- Sorting by the DR group number -->

                    inOrder = from sublist in circuitList orderby System.Convert.ToInt32(sublist.LookupParameter("DR").AsString()) ascending select sublist;
                    List<Autodesk.Revit.DB.Electrical.ElectricalSystem> newCircuitList = new List<Autodesk.Revit.DB.Electrical.ElectricalSystem>();
                    foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem c in inOrder)
                    {
                        newCircuitList.Add(c);
                    }

                    #endregion

                    this.circuitList = newCircuitList;


                    int sum = 0;
                    String drNumber = "";
                    DrawDiagram(doc);
                }
                #endregion
            }
            else
            {
                TaskDialog.Show("EasyElétrica", "Família Diagrama Unifilar não está carregada no projeto, ou nome informado está incorreto.");
                return Result.Failed;
            }
            #endregion
            return Result.Succeeded;
        }

        public void DrawDiagram(Document doc)
        {
            int circuitCounter = this.circuitList.Count;
            this.circuitParameters = GettingCircuitsParamaters(this.circuitList);
            this.diagramParameters = GettingDiagramParamaters(this.diagram, circuitCounter);
            using (Transaction trans = new Transaction(doc, "Drawing the diagram"))
            {
                trans.Start();
                String stillSameGroupOfIDR = "";
                for (int a = 0; a < circuitCounter; a++)
                {
                    //SETTING VISIBILITY
                    this.diagramParameters[a][10].Set(1);

                    //SETTING NUMBER
                    this.diagramParameters[a][0].Set(this.circuitParameters[a][0]);

                    //SETTING NAME
                    this.diagramParameters[a][1].Set(this.circuitParameters[a][1]);

                    //SETTING APPARENT LOAD
                    this.diagramParameters[a][2].Set(this.circuitParameters[a][2]);

                    //SETTING WIRE SIZE
                    this.diagramParameters[a][3].Set(this.circuitParameters[a][3]);

                    //RATING
                    this.diagramParameters[a][4].Set(this.circuitParameters[a][4]);

                    //NUMBER OF POLES
                    if (this.circuitParameters[a][5] == "1")
                    {
                        this.diagramParameters[a][5].Set(1);
                    }
                    else if (this.circuitParameters[a][5] == "2")
                    {
                        this.diagramParameters[a][5].Set(2);
                    }
                    else
                    {
                        this.diagramParameters[a][5].Set(3);
                    }

                    //DR
                    if (!(this.circuitParameters[a][9] == "0" || this.circuitParameters[a][9] == ""))
                    {

                        if (stillSameGroupOfIDR == this.circuitParameters[a][9])
                        {
                            this.diagramParameters[a][6].Set(0.02325 / 0.3048);        //distance between IDR position and DJ position
                            this.diagramParameters[a][7].Set(0);
                            this.diagramParameters[a][9].Set(1);
                            this.diagramParameters[a][8].Set(this.circuitParameters[a][10]); //value of DR
                        }
                        else
                        {
                            this.diagramParameters[a][6].Set(0.02325 / 0.3048);        //distance between IDR position and DJ position
                            this.diagramParameters[a][7].Set(1);    //showing IDR
                                                                    //this.diagramParameters[a][8].Set(1);   //showing second bar (the bar of a group of the same DR)
                            this.diagramParameters[a][8].Set(this.circuitParameters[a][10]); //value of DR
                        }
                        stillSameGroupOfIDR = this.circuitParameters[a][9];
                    }
                    else
                    {
                        this.diagramParameters[a][6].Set(0);
                        this.diagramParameters[a][7].Set(0);
                    }
                }
                trans.Commit();
            }
        }

        private List<List<String>> GettingCircuitsParamaters(List<Autodesk.Revit.DB.Electrical.ElectricalSystem> circuitList)
        { 
            List<List<String>> output = new List<List<String>>();
            
            int circuitCounter = circuitList.Count;

            foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem c in circuitList)
            {

                String par1 = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                String par2 = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NAME).AsString();
                String par3 = c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD).AsValueString();
                int pos = par3.IndexOf(" VA", 0);
                par3 = par3.Substring(0, pos + 1).Trim();
                Parameter bitola = c.LookupParameter("Seção do Condutor Adotado (mm²)");
                String par4 = bitola.AsDouble().ToString();
                String par5 = "" + (c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_RATING_PARAM).AsDouble());          
                String par6 = "" + (c.get_Parameter(BuiltInParameter.RBS_ELEC_NUMBER_OF_POLES).AsInteger());   
                String par7 = "" + (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEA_PARAM).AsDouble()); 
                String par8 = "" + (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEB_PARAM).AsDouble());
                String par9 = "" + (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEC_PARAM).AsDouble());
                String par10 = "" + c.LookupParameter("DR").AsString();
                String par11 = "" + c.LookupParameter("vDR").AsString();

               output.Add(new List<String> {par1, par2, par3, par4, par5, par6, par7, par8, par9, par10, par11});
            }
            return output;
        }

        private List<List<Parameter>> GettingDiagramParamaters(Element diagram, int circuitCounter)
        {
            List<List<Parameter>> output = new List<List<Parameter>>();
            List<String> parametersNames = new List<String>{"u_Numero", "u_Nome", "u_Potencia", "u_Bitola", "u_Disjuntor", "u_QuantidadeFases",
                                                            "u_distComIDR", "u_comIDR", "u_IDR", "u_SepBar", "u_Visible"};
            int a = 0;
            foreach(int i in Enumerable.Range(1,circuitCounter))
            {
                output.Add(new List<Parameter> { });
                for (int j = 0; j < parametersNames.Count; j++)
                {
                    if (i < 10)
                    {
                        String parName = parametersNames[j] + "0" + (a+1).ToString();
                        Parameter teste = diagram.LookupParameter(parName);
                        output[a].Add(diagram.LookupParameter(parName));
                    }
                    else
                    {
                        String parName = parametersNames[j] + (a+1).ToString();
                        Parameter teste = diagram.LookupParameter(parName);
                        output[a].Add(diagram.LookupParameter(parName));
                    }

                }
                a++;
            }
            return output;
        }

        public String Bitola(String bitolaTeste)
        {
            string bitola = "";
            if (bitolaTeste.Contains("1,5"))
            {
                bitola = "1.5";
            }
            else if (bitolaTeste.Contains("2,5"))
            {
                bitola = "2.5";
            }
            else if (bitolaTeste.Contains("4"))
            {
                bitola = "4";
            }
            else if (bitolaTeste.Contains("6"))
            {
                bitola = "6";
            }
            else if (bitolaTeste.Contains("10"))
            {
                bitola = "10";
            }
            else if (bitolaTeste.Contains("16"))
            {
                bitola = "16";
            }
            else if (bitolaTeste.Contains("25"))
            {
                bitola = "25";
            }
            else if (bitolaTeste.Contains("35"))
            {
                bitola = "35";
            }
            else if (bitolaTeste.Contains("50"))
            {
                bitola = "50";
            }
            else if (bitolaTeste.Contains("70"))
            {
                bitola = "70";
            }
            else if (bitolaTeste.Contains("95"))
            {
                bitola = "95";
            }
            else if (bitolaTeste.Contains("120"))
            {
                bitola = "120";
            }
            else if (bitolaTeste.Contains("150"))
            {
                bitola = "150";
            }
            else if (bitolaTeste.Contains("185"))
            {
                bitola = "185";
            }
            else if (bitolaTeste.Contains("240"))
            {
                bitola = "240";
            }
            else if (bitolaTeste.Contains("300"))
            {
                bitola = "300";
            }
            else
            {
                bitola = bitolaTeste;
            }
            return bitola;
        }
    }

}
