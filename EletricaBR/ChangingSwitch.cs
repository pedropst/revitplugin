using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.VisualBasic;

namespace EasyEletrica
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ChangingSwitch : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            GetElementByClicking getElementByClicking = new GetElementByClicking();
            Element selecionado = getElementByClicking.GetElement(commandData, uidoc, doc);

            string new_switch = Interaction.InputBox("Insira qual é o novo retorno", "PEDRO ELÉTRICA", "", -1, -1);
            Parameter switch_param = selecionado.get_Parameter(BuiltInParameter.RBS_ELEC_SWITCH_ID_PARAM);


            using (Transaction trans = new Transaction(doc, "Trocando retorno"))
            {
                trans.Start();
                switch_param.Set(new_switch);
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
}
