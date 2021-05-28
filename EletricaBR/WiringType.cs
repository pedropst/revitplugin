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
    public class WiringType
    {
        public String circuit;
        public HashSet<String> switchID = new HashSet<string>();
        public String bitola;
        public bool fase = false;
        public bool neutro = false;
        public bool retorno = false;
        public bool terra = false;
        public String id;
        public int qntRetorno = 0;
        public int qntFase = 0;
        public bool isBifasico = false;
        public bool isTrifasico = false;

        public WiringType() { }

        public WiringType(String circuit, String switchID)
        {
            this.circuit = circuit;
            if (switchID != "")
            {
                this.switchID.Add(switchID);
            }
            this.id = circuit + switchID;
        }

        public WiringType(String circuit)
        {
            this.circuit = circuit;
            this.id = circuit + switchID;
        }

        public void GettingWireSize(FamilyInstance instPanel)
        {
            Autodesk.Revit.DB.Electrical.ElectricalSystemSet set = instPanel.MEPModel.AssignedElectricalSystems;
            Autodesk.Revit.DB.Electrical.ElectricalSystemSetIterator seti = set.ForwardIterator();
            while (seti.MoveNext())
            {
                if ((seti.Current as Autodesk.Revit.DB.Electrical.ElectricalSystem).get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsString() == this.circuit)
                {
                    this.bitola = (seti.Current as Autodesk.Revit.DB.Electrical.ElectricalSystem).LookupParameter("Seção do Condutor Adotado (mm²)").AsDouble().ToString();
                }
            }
        }

        public String Output()
        {
            String s = "";
            if (this.fase)
            {
                if (isBifasico)
                {
                    s += "FF";
                }
                else if (isTrifasico)
                {
                    s += "FFF";
                }
                else
                {
                    s += "F";
                }
            }
            if (this.neutro)
            {
                s += "N";
            }
            if (this.terra)
            {
                s += "T";
            }
            if (this.retorno)
            {
                for (int i = 0; i < qntRetorno; i++)
                {
                    s += "R";
                }
            }
            return s;
        }
    }
}
