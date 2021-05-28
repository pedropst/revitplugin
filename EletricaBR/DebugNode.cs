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
    public partial class DebugNode : System.Windows.Forms.Form
    {
        UIDocument uidoc;
        Document doc;
        public DebugNode(UIDocument uidoc, Document doc)
        {
            this.uidoc = uidoc;
            this.doc = doc;
            InitializeComponent();

        }

        public void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            List<ElementId> lista = new List<ElementId>();
            foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
            {
                //if (tn.Text != "PATH" && tn.Text != "PIECES")
                if (!tn.Text.Contains("Nó") && !tn.Text.Contains("Eletroduto") && !tn.Text.Contains("."))
                {
                    ElementId ei = new ElementId(Convert.ToInt32(tn.Text));
                    lista.Add(ei);
                }
                foreach (TreeNode nn in tn.Nodes)
                {
                    if (!tn.Text.Contains("Nó") && !tn.Text.Contains("Eletroduto"))
                    {
                        ElementId ei = new ElementId(Convert.ToInt32(tn.Text));
                        lista.Add(ei);
                    }
                }
            }
            uidoc.Selection.SetElementIds(lista);
        }
    }
}
