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

namespace TCC
{
    public partial class MenuPanel : System.Windows.Forms.Form
    {
        List<VirtualConduit> allConduits = new List<VirtualConduit>();
        List<String> conduit_names = new List<String>();
        List<ElementId> elementIds = new List<ElementId>();
        UIDocument uidoc;
        Document doc;

        public MenuPanel(Document doc, UIDocument uidoc, List<String> panels, List<VirtualConduit> allConduits)
        {
            InitializeComponent();
            this.uidoc = uidoc;
            this.allConduits = allConduits;
            this.doc = doc;
            foreach (String s in panels)
            {
                this.comboBox1.Items.Add(s);
            }
            this.comboBox1.SelectedItem = this.comboBox1.Items[0];
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            uidoc.Selection.SetElementIds(allConduits.ElementAt(index).elements);
            uidoc.ShowElements(allConduits.ElementAt(index).elements);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (VirtualConduit vc in this.allConduits)
            {
                this.listBox1.Items.Add("Eletroduto " + (this.allConduits.IndexOf(vc) + 1));
                this.conduit_names.Add("Eletroduto " + (this.allConduits.IndexOf(vc) + 1));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            uidoc.Selection.SetElementIds(allConduits.ElementAt(index).tagIds);
            uidoc.ShowElements(allConduits.ElementAt(index).tagIds);
            EditWires ew = new EditWires(doc, allConduits.ElementAt(index));
            ew.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
