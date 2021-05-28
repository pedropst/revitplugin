using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace EasyEletrica
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class GetSpecificedParameter
    {
        String bitola;
        const Double distDReDISJZERADA = 0.0068;
        public List<List<String>> GetElementsInfo(ExternalCommandData commandData, UIDocument uidoc, Document doc, string quadro, Element element)
        {
            List<List<String>> CircuitsInfoOrdered = new List<List<String>>();
            if (quadro == null)
            {
                TaskDialog.Show("Quadro inválido", "Quadro inválido");
                List<List<String>> CircuitsInfoOrdered1 = null;
                return CircuitsInfoOrdered1;
            }
            else
            {
                try
                {
                    //Getting all elements of electrical circuit category
                    FilteredElementCollector collector = new FilteredElementCollector(doc);
                    ElementQuickFilter filterOfElectricalElements = new ElementCategoryFilter(BuiltInCategory.OST_ElectricalCircuit);
                    IList<Element> ElementOfCorrectPanel = collector.WherePasses(filterOfElectricalElements).WhereElementIsNotElementType().ToElements();

                    ElementQuickFilter filterOfDiagramaUnifilar = new ElementCategoryFilter(BuiltInCategory.OST_GenericAnnotation);
                    IList<Element> ElementDiagramaUnifilar = collector.WherePasses(filterOfElectricalElements).WhereElementIsNotElementType().ToElements();

                    //Verifying which panel is connected to this elements
                    if (ElementOfCorrectPanel.Count > 0)
                    {
                        IList<Element> ElementOfCorrectPanelFinal = new List<Element>();

                        //Elements connected to QD1
                        foreach (Element e in ElementOfCorrectPanel)
                        {
                            if (e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_PANEL_PARAM).AsString().ToUpper() == quadro.ToUpper())
                            {
                                ElementOfCorrectPanelFinal.Add(e);
                            }
                        }
                        int qntCircuitos = ElementOfCorrectPanelFinal.Count();

                        List<List<String>> groupDr = new List<List<string>>();
                        for (int i = 0; i < qntCircuitos; i++)
                        {
                            String x = " ";
                            List<String> temp = new List<string>();
                            temp.Add(x);
                            groupDr.Add(temp);
                        }

                        
                        foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem circuit in ElementOfCorrectPanelFinal)
                        {

                            if (circuit.LookupParameter("DR").AsString() == "0" || circuit.LookupParameter("DR").AsString() == " ")
                            {
                                groupDr[0].Add(circuit.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString());
                            }
                            else
                            {
                                for (int i = 1; i < qntCircuitos; i++)
                                {
                                    if (circuit.LookupParameter("DR").AsString() == i.ToString())
                                    {
                                        groupDr[1].Add(circuit.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString());
                                    }
                                    else if (circuit.LookupParameter("DR").AsString() == i.ToString())
                                    {
                                        groupDr[2].Add(circuit.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString());
                                    }
                                    else if (circuit.LookupParameter("DR").AsString() == i.ToString())
                                    {
                                        groupDr[3].Add(circuit.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString());
                                    }
                                    else if (circuit.LookupParameter("DR").AsString() == i.ToString())
                                    {
                                        groupDr[4].Add(circuit.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString());
                                    }
                                    else
                                    {
                                        groupDr[0].Add(circuit.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString());
                                    }
                                }
                            }
                        }


                        int qntDr = 0;
                        List<List<String>> realDRgroup = new List<List<string>>();
                        foreach (List<String> sublist in groupDr)
                        {
                            if (sublist.Count > 1)
                            {
                                realDRgroup.Add(sublist);
                                qntDr++;
                            }
                        }

                        List<String> prDR = new List<string>();
                        foreach (List<String> sublist in realDRgroup)
                        {
                            prDR.Add(sublist[1]);
                        }

                        
                        List<List<String>> ParamOfCircuits = new List<List<String>>();
                        List<List<String>> ParamOfCircuitsOrdered = new List<List<String>>();
                        int somador = 0;
                        foreach (Element e in ElementOfCorrectPanelFinal)
                        {
                            somador++;
                            if (e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 1)
                            {
                                // PARÂMETRO DO NÚMERO DO CIRCUITO
                                String par0 = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();

                                //DR
                                String par6 = "0,00";
                                String par7 = "";
                                String par8 = "";
                                foreach (String s in prDR)
                                {
                                    if (par0 == s)
                                    {
                                        par6 = "0,02250";
                                        par7 = "1";
                                        par8 = ""; //VALOR DO DR
                                    }
                                }


                                


                                // PARÂMETRO DO NOME DO CIRCUITO
                                String par1 = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NAME).AsString();

                                // PARÂMETRO DA POTÊNCIA
                                Double cAUX1 = e.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE).AsDouble();
                                Double cAUX2 = e.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PARAM).AsDouble();
                                int cAUX3 = System.Convert.ToInt32(System.Math.Ceiling(127 * cAUX2));
                                String par2 = "" + cAUX3.ToString();

                                // PARÂMETRO DO DISJUNTOR
                                Double dAUX = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_RATING_PARAM).AsDouble();
                                String par3 = "" + dAUX.ToString();

                                // PARÂMETRO DA BITOLA
                                String par4 = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_SIZE_PARAM).AsString();


                                // PARÂMETROS DAS FASES
                                String par5 = "";
                                if (e.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEA_PARAM).AsDouble() != 0)
                                {
                                    par5 = " A";

                                }
                                else if (e.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEB_PARAM).AsDouble() != 0)
                                {
                                    par5 = " B";
                                }
                                else if (e.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEC_PARAM).AsDouble() != 0)
                                {
                                    par5 = " C";
                                }


                                // PARÂMETRO IDR
                                //par6 = "0,0068"; // PEGAR SE TEM DR OU NÃO, DESCOBRIR COMO
                                //String par7 = "";
                                //par8 = "";

                                /*if (par6 == "0,00")
                                {
                                    par7 = "0";
                                    par8 = "0";
                                }
                                else
                                {
                                    par7 = "1";
                                    par8 = "20";
                                }*/

                                // PARÂMETRO DOS DESENHOS DE FASE
                                String par9 = "0";
                                String par10 = "1";
                                String par11 = "0";

                                // SUMINDO COM OS MÓDULOS NÃO UTILIZADOS
                                String par12 = "";
                                if (somador > qntCircuitos)
                                {
                                    par12 = "0";
                                }
                                else
                                {
                                    par12 = "1";
                                }


                                ParamOfCircuits.Add(new List<String> { par0, par1, par2, par3, par4, par5, par6, par7, par8, par9, par10, par11, par12 });
                            }
                            else
                            {
                                // PARÂMETRO DO NÚMERO DO CIRCUITO
                                String a = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString();
                                String pattern = @",";
                                String[] aAUX = System.Text.RegularExpressions.Regex.Split(a, pattern);
                                String par0 = "" + aAUX[0];

                                // PARÂMETRO DO NOME DO CIRCUITO
                                String par1 = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NAME).AsString();

                                // PARÂMETRO DA POTÊNCIA
                                Double cAUX1 = e.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE).AsDouble();
                                Double cAUX2 = e.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PARAM).AsDouble();
                                int cAUX3 = System.Convert.ToInt32(System.Math.Ceiling(220 * cAUX2));
                                String par2 = "" + cAUX3.ToString();

                                // PARÂMETRO DO DISJUNTOR
                                Double dAUX = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_RATING_PARAM).AsDouble();
                                String par3 = "" + dAUX.ToString();

                                // PARÂMETRO DA BITOLA
                                String par4 = e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_SIZE_PARAM).AsString();


                                // PARÂMETROS DAS FASES
                                String[] par5AUX = new String[3];
                                if (e.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEA_PARAM).AsDouble() != 0)
                                {
                                    par5AUX[0] = " A";
                                }
                                if (e.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEB_PARAM).AsDouble() != 0)
                                {
                                    par5AUX[1] = " B";
                                }
                                if (e.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_CURRENT_PHASEC_PARAM).AsDouble() != 0)
                                {
                                    par5AUX[2] = " C";
                                }
                                String par5 = "" + par5AUX[0] + par5AUX[1] + par5AUX[2];


                                // PARÂMETRO IDR
                                String par6 = "0,00"; // PEGAR SE TEM DR OU NÃO, DESCOBRIR COMO
                                String par7 = "";
                                String par8 = "";

                                if (par6 == "0,0068")
                                {
                                    par7 = "0";
                                    par8 = "0";
                                }
                                else
                                {
                                    par7 = "1";
                                    par8 = "20";
                                }

                                // PARÂMETRO DOS DESENHOS DE FASE
                                String par9, par10, par11;
                                par9 = par10 = par11 = "0";
                                if (e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 2)
                                {                                                                           
                                    par9 = "1";                                                             
                                    par10 = "0";
                                    par11 = "1";
                                }
                                else if (e.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger() == 3)
                                {
                                    par9 = "1";
                                    par10 = "1";
                                    par11 = "1";
                                }

                                // SUMINDO COM OS MÓDULOS NÃO UTILIZADOS
                                String par12 = "";
                                if (somador > qntCircuitos)
                                {
                                    par12 = "0";
                                }
                                else
                                {
                                    par12 = "1";
                                }

                                ParamOfCircuits.Add(new List<String> { par0, par1, par2, par3, par4, par5, par6, par7, par8, par9, par10, par11, par12 });
                            }

                        }

                        var order = from sublist in ParamOfCircuits orderby System.Convert.ToInt32(sublist[0]) ascending select sublist;
                        foreach (List<String> sublist in order)
                        {
                            ParamOfCircuitsOrdered.Add(sublist);
                        }

                        List<String> ParamOfUnifilarDiagram = new List<String>();
                        String[] ParamOfUnifilarDiagramArray;
                        String[,] ParamOfUnifilarDiagramMap = new String[10,13];
                        List<String> ParamOfUnifilarDiagramOrdered = new List<String>();
                        List<List<String>> ParamToSet = new List<List<String>>();
                        ParamOfUnifilarDiagramAdding(ParamOfUnifilarDiagram);
                        ParamOfUnifilarDiagramArray = ParamOfUnifilarDiagram.ToArray();

                        

                        for (int p = 0; p < 10; p++)
                        {
                            for (int o = 0; o < ParamOfUnifilarDiagram.Count(); o++)
                            {
                                ParamOfUnifilarDiagramMap[p,o] = ParamOfUnifilarDiagramArray[o] + p.ToString();
                            }

                        }


                        for (int p = 0; p < 10; p++)
                        {
                            for (int o = 0; o < ParamOfUnifilarDiagram.Count(); o++)
                            {
                                ParamOfUnifilarDiagramArray[o] = ParamOfUnifilarDiagramMap[p,o];                            
                            }
                            ParamToSet.Add(ParamOfUnifilarDiagramArray.ToList<String>());
                        }
                        
                        using (Transaction trans = new Transaction(doc, "Set Parameter"))
                        {
                            trans.Start();

                            for (int f = 0; f < qntCircuitos; f++)
                            {
                                for (int g = 0; g < 14; g++)
                                {
                                    Parameter param = element.LookupParameter(ParamToSet[f][g]);
                                    if ((g == 0 || g == 1 || g == 2 || g == 3 || g == 5 || g == 8))
                                    {
                                        param.Set(ParamOfCircuitsOrdered[f][g]);
                                    }
                                    else if (g > 8)
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

                            for (int f = qntCircuitos; f < 10; f++)
                            {
                                Parameter param = element.LookupParameter(ParamToSet[f][12]);
                                param.Set((int)System.Convert.ToInt32("0"));
                            }
                            trans.Commit();
                        }
                    }
                    return CircuitsInfoOrdered;
                }
                catch (Exception e)
                {
                    List<List<String>> CircuitsInfoOrdered1 = null;
                    return CircuitsInfoOrdered1;
                }
            }

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
        }

        public String Bitola(String bitolaTeste)
        {
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