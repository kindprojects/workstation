using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using Model;
using Awesomium.ComponentModel;
using Awesomium.Core;
using Awesomium.Web;
using Awesomium.Windows;

namespace ProfileCut
{
    public partial class FMain : Form
    {
        public RModel Model;
        
        private RBaseObject _currentMasterObject;
        private RFbLink _fbDb;
        private RConfig _conf;
        private RModelObjectNavigator _navigator;
        private RBaseObject _prevObject;
        //private int _oldId;
        private bool _domIsReady;
        private JSObject _jsObject;
        private RHardwareCommands _hardware;

        public delegate void Navigated(object sender, EventArgs e);
        
        public FMain()
        {
            InitializeComponent();

            _conf = new RConfig();
            _domIsReady = false;            

            _navigator = new RModelObjectNavigator();
            _navigator.Setup("", panelNavigator);
            _navigator.OnNavigated += OnNavigated;

            _hardware = new RHardwareCommands();
            //_hardware.Setup(panelHardware, _conf.HardwareCommands);

            string processedConnectionString = this._genLocalDBPathIfLocalDB(_conf.ConnectionString);
            
            _fbDb = new RFbLink(processedConnectionString);
            if (_conf.MasterItemsUpdateIntervalMs > 0)
            {
                timer1.Interval = _conf.MasterItemsUpdateIntervalMs;
                timer1.Start();
            }
            ModelLoad();

            //timer1.Start();
        }
        
        private string _genLocalDBPathIfLocalDB(string connectionString)
        {
            // строит путь к файлу БД от текущей папки, если в качестве сервера указан localhost или 127.0.0.1
            FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder builder = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder(connectionString);
            string server = builder.DataSource.ToLower();
            if (server == "localhost" || server == "127.0.0.1")
            {
				if (!builder.Database.Contains(Path.VolumeSeparatorChar))
				{
					// только если путь не полный, а относительный
					string dbPath = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar;
					dbPath += builder.Database;
					builder.Database = dbPath;
				}
            }
            return builder.ConnectionString;
        }
        
        private void OnNavigated(object sender, RBaseObject obj)
        {
            // навигация
            if (obj != null)
            {
                _scrollToElement(obj.Id.ToString(), 60);
                Console.WriteLine(obj.Id + "(" + _navigator.GetPathString() + ")");
            }
            _updateActiveHtmlElement(_prevObject, obj, true);
            _prevObject = obj;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ModelLoad();
        }

        private void ModelLoad()
        {

            Model = new RModel(this._fbDb, _conf.ModelCode, true);            
            RefreshOptimizationList(Model);
            this._fbDb.DisconnectDB();            
            
            GC.Collect();
        }

        private void RefreshOptimizationList(RModel model)
        {
            string dummy="";
            RCollection coll = this.Model.Data.GetCollection(_conf.MasterCollection, false);
            if (coll != null)
            {
                listBoxOptimizations.Items.Clear();
                string listItemTemplate = _conf.MasterItemTemplate;
                for (int ii = 0; ii < coll.Count(); ii++)
                {
                    RBaseObject obj = coll.GetObject(ii);
                    string text = this.Model.Templates.TransformText(listItemTemplate, obj, ref dummy, false);
                    listBoxOptimizations.DisplayMember = "DispTitle";
                    listBoxOptimizations.ValueMember = "Object";
                    listBoxOptimizations.Items.Add(new RMasterItem()
                    {
                        DispTitle = text,
                        Object = obj
                    });
                }
            }
            else
            {
                throw new Exception("Указанная в конфигурации коллекция не описана в модели");
            }

            if (_currentMasterObject != null)
            {
                string val = "";
                if (_currentMasterObject.GetAttr("grorderid", out val))
                {
                    coll = Model.Data.GetCollection(_conf.MasterCollection, false);
                    if (coll != null)
                    {
                        RBaseObject obj = coll.FindObjectByAttrValue("grorderid", val);
                        this._selectListItemByObject(obj);
                    }
                }
            }
        }

        private void _selectListItemByObject(RBaseObject obj)
        {
            listBoxOptimizations.SelectedIndexChanged -= new System.EventHandler(this.listBox1_SelectedIndexChanged);
            try
            {
                for (int ii = 0; ii < listBoxOptimizations.Items.Count; ii++)
                {
                    if ((listBoxOptimizations.Items[ii] as RMasterItem).Object == obj)
                    {
                        listBoxOptimizations.SelectedIndex = ii;                        
                    }
                }
            }
            finally
            {
                listBoxOptimizations.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)       
        {
            string path = "";
            ListBox list_box = (sender as ListBox);

            if (list_box.SelectedItem != null)
            {
                RBaseObject obj = (list_box.SelectedItem as RMasterItem).Object;
                _currentMasterObject = obj;
                    
                string text = this.Model.Templates.TransformText(_conf.DetailTemplate, obj, ref path, true);
                _fbDb.DisconnectDB();

                _navigator.Setup(path, panelNavigator);
                
                webControlDetails.LoadHTML(_addScriptsToBody(text));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ModelLoad();
        }

        private void Awesomium_Windows_Forms_WebControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
           // MessageBox.Show(e.KeyCode.ToString());
        }

        private void Awesomium_Windows_Forms_WebControl_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.KeyCode.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_domIsReady)
            {
                string mysite = webControlDetails.ExecuteJavascriptWithResult("document.documentElement.outerHTML").ToString();
                Clipboard.SetText(mysite);
                MessageBox.Show(mysite);
            }
        }

        private void _addClassToElement(string id, string className)
        {
            string js = "function addClass(o, c) {"
            + " if (o.className.indexOf(c) == -1) {"
            + " o.className = o.className + ' '+c+' '; "
            + " } "
            + " } "
            + " addClass(document.getElementById('" + id + "'), '" + className + "');";

            webControlDetails.ExecuteJavascript(js);  
        }

        private void _removeClassFromElement(string id, string className)
        {
            string js = "function removeClass(o, c) {"
            + " o.className = o.className.replace(c, '');"
            + " }"
            + " removeClass(document.getElementById('" + id + "'), ' " + className + " ');";
           
            webControlDetails.ExecuteJavascript(js);  
        }

        private void _scrollToElement(string id, int yOffset)
        {
            string str = "function jumpTo(elementId, yOffset){"
                + "var e = document.getElementById(elementId);"
                + "if (e){"
                + " var left = 0;"
                + " var top = 0;"
                + " do {"
                + " left += e.offsetLeft;"
                + " top += e.offsetTop;"
                + " } while(e = e.offsetParent);"
                + " window.scrollTo(left, top - yOffset);"
                + " }}"
                + " jumpTo ('" + id + "', "+ yOffset.ToString() +");";
            
            webControlDetails.ExecuteJavascript(str);            
        }
    
        private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {       
            _navigator.SetObject(this._currentMasterObject);
            //_hardware.SetObject(this._currentMasterObject);

            _domIsReady = true;
            _jsObject = webControlDetails.CreateGlobalJavascriptObject("app");

            _jsBind();
        }

        private void _jsBind()
        {
            _jsObject.Bind("bodyOnClick", true, _jsBodyClickHandler);
        }

        private void _jsBodyClickHandler(object sender, JavascriptMethodEventArgs args)
        {
            if (args.Arguments.Count() > 0 && !args.Arguments[0].IsUndefined)
            {
                string id = args.Arguments[0];
                RBaseObject obj = this._currentMasterObject.FindObjectById(Convert.ToInt32(id));                
                
                this._navigator.Pointer = obj;               
                
                _updateActiveHtmlElement(_prevObject, obj, false);
                
                _prevObject = obj;
            }
        }       
 
        private void _updateActiveHtmlElement(RBaseObject deselectObject, RBaseObject selectObject, bool doScroll)
        {
            if (deselectObject != null) 
            {
                _removeClassFromElement(deselectObject.Id.ToString(), _conf.SelectedHtmlElementClass);
            }
            if (selectObject != null)
            {
                _addClassToElement(selectObject.Id.ToString(), _conf.SelectedHtmlElementClass);

                if (doScroll)
                    _scrollToElement(selectObject.Id.ToString(), 60);
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

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            ModelLoad();
        }

        private void Awesomium_Windows_Forms_WebControl_ShowCreatedWebView(object sender, ShowCreatedWebViewEventArgs e)
        {

        }
    }
}