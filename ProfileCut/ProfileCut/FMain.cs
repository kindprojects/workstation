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
        private AApi _viewModel;

        public FMain()
        {
            InitializeComponent();    
            
            _conf = new RConfig();            
            _viewModel = new AApi(_conf.ConnectionString, _conf.ModelCode, true);
        }

           
        private void FMain_Load(object sender, EventArgs e)
        {
           _fillListBoxOptimizations();
        }

        private void _fillListBoxOptimizations()
        { 
            //List<IPBaseObject> masterList = _viewModel.GetMasterList(_conf.MasterCollection);
            //foreach(IPBaseObject item in masterList)
            //{
            //    string text = _viewModel.Transform(_conf.MasterItemTemplate, item);
            //    listBoxOptimizations.DisplayMember = "Text";
            //    listBoxOptimizations.ValueMember = "Object";
            //    listBoxOptimizations.Items.Add(new RMasterItem()
            //    {
            //        Text = text,
            //        Object = item
            //    });
            //}
        }

        private void listBoxOptimizations_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ListBox listBox = (sender as ListBox);
            //if (listBox.SelectedItem != null)
            //{
            //    IPBaseObject obj = (listBox.SelectedItem as RMasterItem).Object;
            //    _currentMasterObject = obj;

            //    string html = _viewModel.Transform(_conf.DetailTemplate, obj);
            //    _fbDb.DisconnectDB();

            //    webControlDetails.LoadHTML(_addScriptsToBody(html));
            //}
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