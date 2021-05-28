using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace TCC
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ChangingCircuit : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Element> selecionados = new List<Element>();
            foreach (ElementId ei in uidoc.Selection.GetElementIds())
            {
                selecionados.Add(doc.GetElement(ei));
            }

            FilteredElementCollector panels = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_ElectricalEquipment);

            List<String> panels_name = new List<string>();
            foreach (FamilyInstance fi in panels)
            {
                panels_name.Add(fi.Name);
            }

            MenuList ml = new MenuList("Selecione a caixa:", panels_name);
            Application.Run(ml);
            var list = ml.ItemSelected();
            //System.Diagnostics.Debug.WriteLine(ml.ItemSelected());
            Element panel = panels.First(a => a.Name == list.First());

            Autodesk.Revit.DB.Electrical.ElectricalSystem electricalSystem = null;
            foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem es in (panel as FamilyInstance).MEPModel.ElectricalSystems)
            {
                if (es.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() == list.Last())
                {
                    electricalSystem = es;
                }
            }
            if (electricalSystem == null)
            {
                return Result.Failed;
            }

            ElementSet eset = new ElementSet();
            foreach (Element e in selecionados)
            {
                eset.Insert(e);
            }


            using (Transaction trans = new Transaction(doc, "Trocando circuitos"))
            {
                trans.Start();
                electricalSystem.AddToCircuit(eset);
                trans.Commit();
            }
            
            /*WiringWindow ww = new WiringWindow(uidoc, doc);
            ww.Show();

            DataTable temp = new DataTable();
            temp.Columns.Add("CIRCUITO");
            temp.Columns.Add("BITOLA");
            temp.Columns.Add("F");
            temp.Columns.Add("N");
            temp.Columns.Add("R");
            temp.Columns.Add("T");

            foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem es in (panel as FamilyInstance).MEPModel.ElectricalSystems)
            {
                DataRow row = temp.NewRow();

                row["CIRCUITO"] = es.CircuitNumber.ToString();
                row["BITOLA"] = "teste2";
                row["F"] = "teste3";
                row["N"] = "teste4";
                row["R"] = "teste5";
                row["T"] = "teste6";
                temp.Rows.Add(row);
            }              
                           
            ww.SettingData(temp);*/


            return Result.Succeeded;
        }

    }
}
