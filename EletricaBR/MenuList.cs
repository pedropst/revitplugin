using System;
using System.Collections.Generic;

namespace EasyEletrica
{
    public partial class MenuList : System.Windows.Forms.Form
    {
        List<String> retorno = new List<string>();
        public MenuList(String label, List<String> itens)
        {
            InitializeComponent();
            this.label1.Text = label;

            foreach (String s in itens)
            {
                this.comboBox1.Items.Add(s);
            }
            this.comboBox1.SelectedItem = this.comboBox1.Items[0];
        }

        public List<String> ItemSelected()
        {
            return this.retorno;
        }

        public void button1_Click(object sender, EventArgs e)
        {
            retorno = new List<String>{ this.comboBox1.SelectedItem.ToString(), this.textBox1.Text };
            this.Close();
            //uidoc.Selection.SetElementIds(new List<ElementId> { collector.First(a => a.Name == this.comboBox1.SelectedItem.ToString()).Id });
            //ItemSelected(collector.First(a => a.Name == this.comboBox1.SelectedItem.ToString()).Id);
            //return collector.First(a => a.Name == ItemSelected()).Id;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
