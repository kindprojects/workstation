using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProfileCut
{
    public partial class RMainPanel : UserControl
    {
        public RMainPanel()
        {
            InitializeComponent();
        }

        public void AddControlToLayoutTable(Control control, int columnNumber)
        {
            this.tableLayoutPanelMain.Controls.Add(control, columnNumber, 0);
        }
    }
}
