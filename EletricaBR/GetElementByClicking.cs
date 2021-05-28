using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace TCC
{
    class GetElementByClicking
    {
        public Element element;
        public Element GetElement(ExternalCommandData commandData, UIDocument uidoc, Document doc)
        {
            //Pick Object
            Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

            //Retrieve Element
            ElementId eleId = pickedObj.ElementId;
            Element ele = doc.GetElement(eleId);

            //Confirming if isn't a null element
            if (pickedObj != null)
            {
                element = ele;
            }
            return element;
        }
    }
}
