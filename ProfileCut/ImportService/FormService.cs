using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;

namespace ImportService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                 ConcurrencyMode = ConcurrencyMode.Multiple,
                 UseSynchronizationContext = true)] 
    public partial class FormService : Form
    {
        public FormService()
        {            
            InitializeComponent();
        }

        private void FormService_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Maximized == WindowState)
                Hide();
        }

        private void FormService_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
        }

        protected void ServiceStart()
        {           
        }
    }
}
