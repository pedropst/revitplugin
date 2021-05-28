using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.VisualBasic;

namespace TCC
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class UnifilarDiagram : IExternalCommand
    {
        GetSpecificedParameter unifilar = new GetSpecificedParameter();
        GetElementByClicking elemento = new GetElementByClicking();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            //Get Document
            Document doc = uidoc.Document;

            string quadro = Interaction.InputBox("INSIRA O NOME DO QUADRO", "PEDRO ELÉTRICA", "", -1, -1);

            List<BuiltInCategory> categorias = new List<BuiltInCategory>();
            categorias.Add(BuiltInCategory.OST_ElectricalEquipment);

            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(categorias);

            IList<Element> quadros = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType().ToElements();
            bool verif = false;
            Element panel = null;
            foreach (Element e in quadros)
            {
                if (e.Name == quadro.ToUpper())
                {
                    panel = e;
                    verif = true;
                    break;
                }
            }

            if (verif == false)
            {
                TaskDialog.Show("PEDRO ELÉTRICA", "Nenhum quadro encontrado com esse nome.");
            }
            else
            {
                List<Autodesk.Revit.DB.Electrical.ElectricalSystem> circuitList = new List<Autodesk.Revit.DB.Electrical.ElectricalSystem>();
                PanelClass panelclass = new PanelClass(doc, panel);
                foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem circuit in panelclass.circuits)
                {
                    circuitList.Add(circuit);
                }


                foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem c in circuitList)
                {
                    Parameter DR = c.LookupParameter("DR");
                    Parameter vDR = c.LookupParameter("vDR");
                    if (DR.AsString() == "" || DR.AsString() == " " || DR.AsString() == "   " || DR == null)
                    {
                        using (Transaction trans = new Transaction(doc, "DRgroup e vDR"))
                        {
                            trans.Start();
                            DR.Set("0");
                            vDR.Set("0");
                            trans.Commit();
                        }
                    }
                }

                var order0 = from sublist in circuitList orderby sublist.StartSlot ascending select sublist;
                List<Autodesk.Revit.DB.Electrical.ElectricalSystem> circuitListDef0 = new List<Autodesk.Revit.DB.Electrical.ElectricalSystem>();
                foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem c in order0)
                {
                    circuitListDef0.Add(c);
                }


                var order = from sublist in circuitListDef0 orderby System.Convert.ToInt32(sublist.LookupParameter("DR").AsString()) ascending select sublist;
                List<Autodesk.Revit.DB.Electrical.ElectricalSystem> circuitListDef = new List<Autodesk.Revit.DB.Electrical.ElectricalSystem>();
                foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem c in order)
                {
                    circuitListDef.Add(c);
                }


                List<List<String>> ParamOfCircuits = new List<List<String>>();
                List<List<String>> ParamOfCircuitsOrdered = new List<List<String>>();
                int somador = 0;
                String drNumber = "";
                foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem c in circuitListDef)
                {
                    somador++;
                    if (c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 1)
                    {
                        // PARÂMETRO DO NÚMERO DO CIRCUITO
                        String par0 = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();

                        String par6 = "0,00";
                        String par7 = "0";
                        String par8 = "";
                        String par13 = "0";
                        String par14 = "0";

                        //DR
                        if (c.LookupParameter("DR").AsString() == "0")
                        {
                            par14 = "0";
                            //nada
                        }
                        else
                        {
                            par6 = "0,0232";
                            par13 = "1";
                            par8 = c.LookupParameter("vDR").AsString();
                            if (drNumber == c.LookupParameter("DR").AsString())
                            {
                                par14 = "1";
                                par7 = "0";
                            }
                            else
                            {
                                par14 = "0";
                                par7 = "1";
                                drNumber = c.LookupParameter("DR").AsString();

                            }

                        }


                        // PARÂMETRO DO NOME DO CIRCUITO
                        String par1 = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NAME).AsString();

                        // PARÂMETRO DA POTÊNCIA
                        Double cAUX1 = c.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE).AsDouble();
                        Double cAUX2 = c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PARAM).AsDouble();
                        int cAUX3 = System.Convert.ToInt32(System.Math.Ceiling(127 * cAUX2));
                        String par2 = "" + cAUX3.ToString();

                        // PARÂMETRO DO DISJUNTOR
                        Double dAUX = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_RATING_PARAM).AsDouble();
                        String par3 = "" + dAUX.ToString();

                        // PARÂMETRO DA BITOLA
                        String par4 = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_SIZE_PARAM).AsString();


                        // PARÂMETROS DAS FASES
                        String par5 = "";
                        if (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEA_PARAM).AsDouble() != 0)
                        {
                            par5 = " A";

                        }
                        else if (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEB_PARAM).AsDouble() != 0)
                        {
                            par5 = " B";
                        }
                        else if (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEC_PARAM).AsDouble() != 0)
                        {
                            par5 = " C";
                        }


                        // PARÂMETRO DOS DESENHOS DE FASE
                        String par9 = "0";
                        String par10 = "1";
                        String par11 = "0";

                        // SUMINDO COM OS MÓDULOS NÃO UTILIZADOS
                        String par12 = "";
                        if (somador > circuitList.Count)
                        {
                            par12 = "0";
                        }
                        else
                        {
                            par12 = "1";
                        }


                        ParamOfCircuits.Add(new List<String> { par0, par1, par2, par3, par4, par5, par6, par7, par8, par9, par10, par11, par12, par13, par14 });
                    }
                    else
                    {
                        // PARÂMETRO DO NÚMERO DO CIRCUITO
                        String a = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                        String pattern = @",";
                        String[] aAUX = System.Text.RegularExpressions.Regex.Split(a, pattern);
                        String par0 = "" + aAUX[0];

                        String par6 = "0,00";
                        String par7 = "0";
                        String par8 = "";
                        String par13 = "0";
                        String par14 = "0";

                        //DR
                        if (c.LookupParameter("DR").AsString() == "0")
                        {
                            par14 = "0";
                            //nada
                        }
                        else
                        {
                            par6 = "0,0232";
                            par13 = "1";
                            par8 = c.LookupParameter("vDR").AsString();
                            if (drNumber == c.LookupParameter("DR").AsString())
                            {
                                par14 = "1";
                                par7 = "0";
                            }
                            else
                            {
                                par14 = "0";
                                par7 = "1";
                                drNumber = c.LookupParameter("DR").AsString();

                            }

                        }

                        // PARÂMETRO DO NOME DO CIRCUITO
                        String par1 = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NAME).AsString();

                        // PARÂMETRO DA POTÊNCIA
                        Double cAUX1 = c.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE).AsDouble();
                        Double cAUX2 = c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PARAM).AsDouble();
                        int cAUX3 = System.Convert.ToInt32(System.Math.Ceiling(220 * cAUX2));
                        String par2 = "" + cAUX3.ToString();

                        // PARÂMETRO DO DISJUNTOR
                        Double dAUX = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_RATING_PARAM).AsDouble();
                        String par3 = "" + dAUX.ToString();

                        // PARÂMETRO DA BITOLA
                        String par4 = c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_SIZE_PARAM).AsString();


                        // PARÂMETROS DAS FASES
                        String[] par5AUX = new String[3];
                        if (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEA_PARAM).AsDouble() != 0)
                        {
                            par5AUX[0] = " A";
                        }
                        if (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEB_PARAM).AsDouble() != 0)
                        {
                            par5AUX[1] = " B";
                        }
                        if (c.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEC_PARAM).AsDouble() != 0)
                        {
                            par5AUX[2] = " C";
                        }
                        String par5 = "" + par5AUX[0] + par5AUX[1] + par5AUX[2];


                        // PARÂMETRO DOS DESENHOS DE FASE
                        String par9, par10, par11;
                        par9 = par10 = par11 = "0";
                        if (c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 2)
                        {
                            par9 = "1";
                            par10 = "0";
                            par11 = "1";
                        }
                        else if (c.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 3)
                        {
                            par9 = "1";
                            par10 = "1";
                            par11 = "1";
                        }

                        // SUMINDO COM OS MÓDULOS NÃO UTILIZADOS
                        String par12 = "";
                        if (somador > circuitList.Count)
                        {
                            par12 = "0";
                        }
                        else
                        {
                            par12 = "1";
                        }

                        ParamOfCircuits.Add(new List<String> { par0, par1, par2, par3, par4, par5, par6, par7, par8, par9, par10, par11, par12 , par13, par14});
                    }
                }


                ParamOfCircuitsOrdered = ParamOfCircuits;

                List<String> ParamOfUnifilarDiagram = new List<String>();
                String[] ParamOfUnifilarDiagramArray;
                String[,] ParamOfUnifilarDiagramMap = new String[10, 15];
                List<String> ParamOfUnifilarDiagramOrdered = new List<String>();
                List<List<String>> ParamToSet = new List<List<String>>();
                ParamOfUnifilarDiagramAdding(ParamOfUnifilarDiagram);
                ParamOfUnifilarDiagramArray = ParamOfUnifilarDiagram.ToArray();



                for (int p = 0; p < 10; p++)
                {
                    for (int o = 0; o < ParamOfUnifilarDiagram.Count; o++)
                    {
                        ParamOfUnifilarDiagramMap[p, o] = ParamOfUnifilarDiagramArray[o] + p.ToString();
                    }

                }


                for (int p = 0; p < 10; p++)
                {
                    for (int o = 0; o < ParamOfUnifilarDiagram.Count; o++)
                    {
                        ParamOfUnifilarDiagramArray[o] = ParamOfUnifilarDiagramMap[p, o];
                    }
                    ParamToSet.Add(ParamOfUnifilarDiagramArray.ToList<String>());
                }


                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                ElementId diagramId = pickedObj.ElementId;
                Element diagram = doc.GetElement(diagramId);


                using (Transaction trans = new Transaction(doc, "Set Parameter"))
                {
                    trans.Start();

                    for (int f = 0; f < circuitList.Count; f++)
                    {
                        for (int g = 0; g < ParamOfUnifilarDiagram.Count; g++)
                        {
                            Parameter param = diagram.LookupParameter(ParamToSet[f][g]);
                            if ((g == 0 || g == 1 || g == 2 || g == 3 || g == 5 || g == 8))
                            {
                                param.Set(ParamOfCircuitsOrdered[f][g]);
                            }
                            else if (g == 14 || g == 13 || g == 7 || g > 8)
                            {
                                param.Set((int)System.Convert.ToInt32(ParamOfCircuitsOrdered[f][g]));
                            }
                            else if (g == 6)
                            {
                                param.Set(System.Convert.ToDouble(ParamOfCircuitsOrdered[f][g]) / 0.3048);
                            }
                            else if (g == 4)
                            {
                                param.Set(Bitola(ParamOfCircuitsOrdered[f][g]));
                            }

                        }
                    }

                    for (int f = circuitList.Count; f < 10; f++)
                    {
                        Parameter param = diagram.LookupParameter(ParamToSet[f][12]);
                        param.Set((int)System.Convert.ToInt32("0"));
                    }
                    trans.Commit();
                }





            }


           // TaskDialog.Show("Messagem", "Hello World");
            return Result.Succeeded;
        }
        public void ParamOfUnifilarDiagramAdding(List<String> ParamOfUnifilarDiagram)
        {
            ParamOfUnifilarDiagram.Add("Unifilar-Numero");
            ParamOfUnifilarDiagram.Add("Unifilar-Nome");
            ParamOfUnifilarDiagram.Add("Unifilar-Potencia");
            ParamOfUnifilarDiagram.Add("Unifilar-Disjuntor");
            ParamOfUnifilarDiagram.Add("Unifilar-Bitola");
            ParamOfUnifilarDiagram.Add("Unifilar-Fases");
            ParamOfUnifilarDiagram.Add("Unifilar-SemIDR");  //6
            ParamOfUnifilarDiagram.Add("Unifilar-HideIDR"); //7
            ParamOfUnifilarDiagram.Add("Unifilar-IDR");     //8
            ParamOfUnifilarDiagram.Add("Unifilar-PoloP");
            ParamOfUnifilarDiagram.Add("Unifilar-PoloS");
            ParamOfUnifilarDiagram.Add("Unifilar-PoloT");
            ParamOfUnifilarDiagram.Add("Unifilar-Visible");
            ParamOfUnifilarDiagram.Add("Unifilar-SegBar");
            ParamOfUnifilarDiagram.Add("Unifilar-SepBar");
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
            else
            {
                bitola = bitolaTeste;
            }
            return bitola;
        }
    }
}
