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
    class UnifilarSettings
    {
        public UnifilarSettings()
        {
            string input = Interaction.InputBox("Insira a nova distância", "PEDRO ELÉTRICA", "", -1, -1);
            if (input.Contains(','))
                input.Replace(',', '.');
            Properties.Settings.Default.distIDReDR = System.Convert.ToDouble(input);
            Properties.Settings.Default.Save();
        }

    }
}
