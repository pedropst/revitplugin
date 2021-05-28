using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace EasyEletrica
{
    public partial class SwitchSystem : Form
    {
        List<VirtualLighting> allLights = new List<VirtualLighting>();
        List<ElementId> refConduits_ids = new List<ElementId>();
        List<ElementId> othersConduits_ids = new List<ElementId>();
        List<ElementId> pathToFirstSwitch_ids = new List<ElementId>();
        List<ElementId> pathBetweenSwitchAndLamps_ids = new List<ElementId>();
        List<ElementId> pathToSecondSwitch_ids = new List<ElementId>();
        List<ElementId> pathBetweenSwitches_ids = new List<ElementId>();
        List<ElementId> pathBetweenSecondSwitchAndLamps_ids = new List<ElementId>();
        List<ElementId> inConduits_ids = new List<ElementId>();
        List<ElementId> pathToLamps_ids = new List<ElementId>();
        UIDocument uidoc;
        public SwitchSystem(UIDocument uidoc, List<VirtualLighting> allLights)
        {
            this.allLights = allLights;
            this.uidoc = uidoc;
            InitializeComponent();
            foreach (VirtualLighting vl in allLights)
            {
                comboBox10.Items.Add(vl.id);
            }

            comboBox10.SelectedIndex = 0;
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            VirtualLighting vl = (from v in allLights where v.id == comboBox10.Text select v).First();
            refConduits_ids.Clear();
            othersConduits_ids.Clear();
            pathToFirstSwitch_ids.Clear();
            pathBetweenSwitchAndLamps_ids.Clear();
            pathToSecondSwitch_ids.Clear();
            pathBetweenSwitches_ids.Clear();
            pathBetweenSecondSwitchAndLamps_ids.Clear();
            inConduits_ids.Clear();
            pathToLamps_ids.Clear();
            foreach (VirtualConduit vc in vl.refConduits)
            {
                foreach (ElementId ei in vc.elements)
                {
                    refConduits_ids.Add(ei);
                }
            }
            foreach (VirtualConduit vc in vl.othersConduits)
            {
                foreach (ElementId ei in vc.elements)
                {
                    othersConduits_ids.Add(ei);
                }
            }
            foreach (VirtualConduit vc in vl.pathToFirstSwitch)
            {
                foreach (ElementId ei in vc.elements)
                {
                    pathToFirstSwitch_ids.Add(ei);
                }
            }
            foreach (VirtualConduit vc in vl.pathBetweenSwitchAndLamps)
            {
                foreach (ElementId ei in vc.elements)
                {
                    pathBetweenSwitchAndLamps_ids.Add(ei);
                }
            }
            foreach (VirtualConduit vc in vl.pathToSecondSwitch)
            {
                foreach (ElementId ei in vc.elements)
                {
                    pathToSecondSwitch_ids.Add(ei);
                }
            }
            foreach (VirtualConduit vc in vl.pathBetweenSwitches)
            {
                foreach (ElementId ei in vc.elements)
                {
                    pathBetweenSwitches_ids.Add(ei);
                }
            }
            foreach (VirtualConduit vc in vl.pathBetweenSecondSwitchAndLamps)
            {
                foreach (ElementId ei in vc.elements)
                {
                    pathBetweenSecondSwitchAndLamps_ids.Add(ei);
                }
            }
            foreach (VirtualConduit vc in vl.inConduits)
            {
                foreach (ElementId ei in vc.elements)
                {
                    inConduits_ids.Add(ei);
                }
            }
            foreach (VirtualConduit vc in vl.pathToLamps)
            {
                foreach (ElementId ei in vc.elements)
                {
                    pathToLamps_ids.Add(ei);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(refConduits_ids);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(othersConduits_ids);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(pathToFirstSwitch_ids);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(pathBetweenSwitchAndLamps_ids);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(pathToSecondSwitch_ids);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(pathBetweenSwitches_ids);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(pathBetweenSecondSwitchAndLamps_ids);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(pathToLamps_ids);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            uidoc.Selection.SetElementIds(inConduits_ids);
        }
    }
}
