using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Platform;

namespace ProfileCut
{
    public partial class FMain : Form
    {
        private RFbLink _fbDb;
        private RConfig _conf;

        // представление модели
        private PPlatform _viewModel;

        public FMain()
        {
            InitializeComponent();    
            
            _conf = new RConfig();
            _fbDb = new RFbLink(_conf.ConnectionString);
            _viewModel = new PPlatform(_fbDb, _conf.ModelCode, true);
        }

           
        private void FMain_Load(object sender, EventArgs e)
        {
            
        }
    }
}