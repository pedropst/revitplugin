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
    public class PanelClass
    {
        public ElementId id;
        public String name;
        public List<String> stringCircuitList = new List<string>();
        public List<Autodesk.Revit.DB.Electrical.ElectricalSystem> circuits = new List<Autodesk.Revit.DB.Electrical.ElectricalSystem>();

        public PanelClass(Document doc, Element panel)
        {
            if (panel.Category.Name == "Electrical Equipment" && panel.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME) != null)
            {
                this.id = panel.Id;
                this.name = panel.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME).AsString();


                List<String> circuitsNotOrdered = new List<String>();
                FamilyInstance instPanel = panel as FamilyInstance;
                MEPModel mep = instPanel.MEPModel;
                Autodesk.Revit.DB.Electrical.ElectricalSystemSet set = instPanel.MEPModel.AssignedElectricalSystems;
                Autodesk.Revit.DB.Electrical.ElectricalSystemSetIterator seti = set.ForwardIterator();
                while (seti.MoveNext())
                {
                    Autodesk.Revit.DB.Electrical.ElectricalSystem wire = seti.Current as Autodesk.Revit.DB.Electrical.ElectricalSystem;
                    circuits.Add(wire);
                    circuitsNotOrdered.Add(wire.CircuitNumber.ToString());
                }

                var order = from String in circuitsNotOrdered orderby System.Convert.ToInt32(String) ascending select String;
                foreach (String s in order)
                {
                    this.stringCircuitList.Add(s);
                }


            }
            else
            {
                TaskDialog.Show("ERRO", "ELEMENTO SELECIONADO NÃO É VÁLIDO COMO QUDARO");
            }
        }

        public Autodesk.Revit.DB.Electrical.ElectricalSystem GetCircuitByCircuitNumber(String circuitNumber)
        {
            Autodesk.Revit.DB.Electrical.ElectricalSystem circuit = null;
            foreach (Autodesk.Revit.DB.Electrical.ElectricalSystem cc in this.circuits)
            {
                if (circuitNumber == cc.CircuitNumber.ToString())
                {
                    circuit = cc;
                }
            }
            return circuit;
        }
    }
}
