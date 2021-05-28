using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.VisualBasic;

namespace EletricaBR
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class SettingPathClass : IExternalCommand
    {
        static List<NodeClass> nodeList = new List<NodeClass>();
        static List<NodeClass> nodeListSearched = new List<NodeClass>();

        public List<vConduitClass> vConduitList = new List<vConduitClass>();
        public vPanelClass vPanel = null;

        GetSpecificedParameter unifilar = new GetSpecificedParameter();
        GetElementByClicking elemento = new GetElementByClicking();


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            string quadro = Interaction.InputBox("INSIRA O NOME DO QUADRO", "PEDRO ELÉTRICA", "", -1, -1);
            List<BuiltInCategory> categorias = new List<BuiltInCategory>();
            categorias.Add(BuiltInCategory.OST_ElectricalEquipment);
            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(categorias);
            IList<Element> quadros = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType().ToElements();
            bool verif = false;

            foreach (Element e in quadros)
            {
                if (e.Name == quadro.ToUpper())
                {
                    verif = true;
                    break;
                }
            }
            if (verif == true)
            {
                Element element = elemento.GetElement(commandData, uidoc, doc);
                unifilar.GetElementsInfo(commandData, uidoc, doc, quadro, element);
            }
            else
            {
                TaskDialog.Show("PEDRO ELÉTRICA", "Nenhum quadro encontrado com esse nome.");
            }

            throw new NotImplementedException();
        }

        SettingPathClass(Document doc, UIDocument uidoc, ExternalCommandData commandData)
        {



        }




        

        public void receivingNode(NodeClass node)
        {
            nodeList.Add(node);
        }

        public void removingNode(NodeClass node)
        {
             nodeListSearched.Add(node);
        }

        public List<NodeClass> getToExplore()
        {
            return nodeList;
        }

        public List<NodeClass> getAlreadyExplored()
        {
            return nodeListSearched;
        }

        public void resetClass()
        {
            nodeList.Clear();
            nodeListSearched.Clear();
        }

        public void ResetAlreadyExplored()
        {
            nodeListSearched.Clear();
        }


    }
}
