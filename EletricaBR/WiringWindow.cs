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

namespace TCC
{
    public partial class WiringWindow : System.Windows.Forms.Form
    {
        public WiringWindow(UIDocument uidoc, Document doc)
        {
            InitializeComponent();
            /*dt.Columns.Add("CIRCUITO");
            dt.Columns.Add("BITOLA");
            dt.Columns.Add("F");
            dt.Columns.Add("N");
            dt.Columns.Add("R");
            dt.Columns.Add("T");*/

        }
        public void SettingData(DataTable temp)
        {
            dataGridView1.Show();
            dataGridView1.DataSource = temp;
        }

        private void WiringWindow_Load(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
