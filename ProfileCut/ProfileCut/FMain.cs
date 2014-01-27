using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Api;

namespace ProfileCut
{
    public partial class FMain : Form
    {
        private RConfig _conf;

        // представление модели
        private AModel _viewModel;

        public FMain()
        {
            InitializeComponent();    
            
            _conf = new RConfig();            
            _viewModel = new AModel(_conf.ConnectionString, _conf.ModelCode, true);
            _viewModel.GetRoot().NavigatorInitialize(_conf.MasterCollection);                
        }
           
        private void FMain_Load(object sender, EventArgs e)
        {
            listBoxOptimizations.DisplayMember = "DispTitle";
            listBoxOptimizations.ValueMember = "Object";

           _fillListBoxOptimizations();
        }

        private void _fillListBoxOptimizations()
        {
            ABaseObject root = _viewModel.GetRoot();
            ABaseObject obj = root.NavigatorPointer();
            do
            {                
                obj = root.NavigatorPointer();
                listBoxOptimizations.Items.Add(new RMasterItem() {
                    DispTitle = _viewModel.Transform(_conf.MasterItemTemplate, obj),
                    Object = obj
                });
            } while(obj != root.Navigate(0, 0));
        }

        private void listBoxOptimizations_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox list_box = (sender as ListBox);

            if (list_box.SelectedItem != null)
            {
                ABaseObject obj = (list_box.SelectedItem as RMasterItem).Object;
            }
        }

        private string _addScriptsToBody(string text)
        {
            string ret = text;

            string script = "<script type='text/javascript'>"
                + " function bodyOnClick(e) {"
                + " var element = e.target;"
                + " while (element && !element.id){"
                + " element = element.parentNode;"
                + " } "
                + " app.bodyOnClick(element ? element.id : undefined);"
                + " }"
                + " </script>";

            var index = ret.IndexOf("</body>");
            if (index > 0)
            {
                ret = ret.Insert(index, script);
            }

            return ret;
        }
    }
}