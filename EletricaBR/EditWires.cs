using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace EasyEletrica
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public partial class EditWires : System.Windows.Forms.Form
    {
        VirtualConduit vc = null;
        Document doc;
        public EditWires(Document doc, VirtualConduit vc)
        {
            InitializeComponent();
            this.vc = vc;
            this.doc = doc;
            foreach (WiringType wt in vc.wires)
            {
                string switches = "";
                foreach (String s in wt.switchID)
                {
                    switches += s;
                }
                this.listBox1.Items.Add(wt.circuit + switches);

                this.listBox1.SelectedItem = this.listBox1.Items[0];

                this.textBox1.Text = wt.circuit + switches;
                this.textBox2.Text = wt.bitola;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            String switches = "";
            foreach (String s in vc.wires[index].switchID)
            {
                switches += s;
            }
            this.textBox1.Text = vc.wires[index].circuit + switches;
            this.textBox2.Text = vc.wires[index].bitola;
        }

        /*private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ElementId ei = vc.tagIds[this.listBox1.SelectedIndex];
            Parameter param = this.doc.GetElement(ei).LookupParameter("cima");
            using (Transaction trans = new Transaction(doc, "Editando fiação"))
            {
                trans.Start();
                param.Set(textBox1.Text);
                trans.Commit();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ElementId ei = vc.tagIds[this.listBox1.SelectedIndex];
            Parameter param = this.doc.GetElement(ei).LookupParameter("baixo");
            using (Transaction trans = new Transaction(doc, "Editando fiação"))
            {
                trans.Start();
                param.Set(textBox2.Text);
                trans.Commit();
            }
        }*/
    }
}
