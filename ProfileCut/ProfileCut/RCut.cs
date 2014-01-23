using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using Awesomium.ComponentModel;
using Awesomium.Core;
using Awesomium.Web;
using Awesomium.Windows;
using Awesomium.Windows.Forms;
using Platform;
 
namespace ProfileCut
{
    /*public class RCut
    {
        private ListBox _optimizationsList;     
        private WebControl _detailsWebControl;
        private Panel _navigatorControl;

        private RConfig _conf;
        private bool _domIsReady;
        private RFbLink _fbDb;
        private PModel _model;
        private PBaseObject _currentMasterObject;
        private PBaseObject _prevObject;
        private JSObject _jsObject;        

        public RCut(ListBox optimizationsList, WebControl detailsWebControl, Panel navigatorControl)
        {
            _optimizationsList = optimizationsList;
            _optimizationsList.SelectedIndexChanged += new System.EventHandler(_optimizationList_SelectedIndexChanged);

            _detailsWebControl = detailsWebControl;
            _detailsWebControl.DocumentReady += new Awesomium.Core.UrlEventHandler(_detailsWebControl_DocumentReady);

            _navigatorControl = navigatorControl;

            _conf = new RConfig();
            _fbDb = new RFbLink(_conf.ConnectionString);
            _model = new PModel(_fbDb, _conf.ModelCode, true);
            _refreshOptimizationList();                        

            _fbDb.DisconnectDB();
        }

        private void _refreshOptimizationList()
        {
            string path = "";
            PCollection coll = _model.Data.GetCollection(_conf.MasterCollection, false);
            if (coll != null)
            {
                _optimizationsList.Items.Clear();
                string listItemTemplate = _conf.MasterItemTemplate;
                for (int ii = 0; ii < coll.Count(); ii++)
                {
                    PBaseObject obj = coll.GetObject(ii);
                    string text = _model.Templates.TransformText(listItemTemplate, obj, ref path, false);
                    _optimizationsList.DisplayMember = "DispTitle";
                    _optimizationsList.ValueMember = "Object";
                    _optimizationsList.Items.Add(new PMasterItem()
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
                    coll = _model.Data.GetCollection(_conf.MasterCollection, false);
                    if (coll != null)
                    {
                        PBaseObject obj = coll.FindObjectByAttrValue("grorderid", val);
                        this._selectListItemByObject(obj);
                    }
                }
            }
        }


        private void _selectListItemByObject(PBaseObject obj)
        {
            //_optimizationList.SelectedIndexChanged -= new System.EventHandler(this.listBox1_SelectedIndexChanged);
            try
            {
                for (int ii = 0; ii < _optimizationsList.Items.Count; ii++)
                {
                    if ((_optimizationsList.Items[ii] as PMasterItem).Object == obj)
                    {
                        _optimizationsList.SelectedIndex = ii;                        
                    }
                }
            }
            finally
            {
                //_optimizationList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
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

        private void _jsBind()
        {
            _jsObject.Bind("bodyOnClick", true, _jsBodyClickHandler);
        }

        private void _jsBodyClickHandler(object sender, JavascriptMethodEventArgs args)
        {
            if (args.Arguments.Count() > 0 && !args.Arguments[0].IsUndefined)
            {
                string id = args.Arguments[0];
                PBaseObject obj = this._currentMasterObject.FindObjectById(Convert.ToInt32(id));

                this._currentMasterObject.NavigatorSetPointer(obj);

                _updateActiveHtmlElement(_prevObject, obj, false);
                _prevObject = obj;
            }
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
                + " jumpTo ('" + id + "', " + yOffset.ToString() + ");";

            _detailsWebControl.ExecuteJavascript(str);
        }

        private void _addClassToElement(string id, string className)
        {
            string js = "function addClass(o, c) {"
            + " if (o.className.indexOf(c) == -1) {"
            + " o.className = o.className + ' '+c+' '; "
            + " } "
            + " } "
            + " addClass(document.getElementById('" + id + "'), '" + className + "');";

            _detailsWebControl.ExecuteJavascript(js);
        }

        private void _removeClassFromElement(string id, string className)
        {
            string js = "function removeClass(o, c) {"
            + " o.className = o.className.replace(c, '');"
            + " }"
            + " removeClass(document.getElementById('" + id + "'), ' " + className + " ');";

            _detailsWebControl.ExecuteJavascript(js);
        }

        private void OnNavigated(object sender, PBaseObject obj)
        {
            // навигация
            if (obj != null)
            {
                _scrollToElement(obj.Id.ToString(), 60);
            }
            _updateActiveHtmlElement(_prevObject, obj, true);
            _prevObject = obj;

        }

        private void _updateActiveHtmlElement(PBaseObject deselectObject, PBaseObject selectObject, bool doScroll)
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

        private void _buildControls()
        {
            _navigatorControl.Controls.Clear();                

            int x = 0;
            this._navigatorControl.Controls.Clear();
            foreach (PModelObjectNavigatorPathLevel lvl in _currentMasterObject.GetLevels()) 
            {
                //if (lvl.UiControlName != "")
                //{
                    Control c = _createLevelControls(lvl, x);
                    lvl.UiControl = c;
                    x += c.Width;
                //}
            }
        }


        private Control _createLevelControls(PModelObjectNavigatorPathLevel level, int x)
        {
            const int btnWidth = 40;

            Panel p = new Panel();
            p.Location = new Point(x, 0);

            Label l = new Label();
            p.Controls.Add(l);
            l.Text = level.UiControlName;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            l.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            l.AutoSize = true;
            int labWidth = l.Width;
            l.AutoSize = false;
            l.Width = labWidth;
            l.Height = this._navigatorControl.Height;
            p.Width = labWidth + 2 * btnWidth + 1;

            RNavigatorButton b = new RNavigatorButton(level, NAV_DIRECTION.UP);
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = _navigatorControl.Height;
            b.Left = l.Width;
            b.Top = 0;
            b.Click += new System.EventHandler(_navButtonClick);
            b.BackgroundImage = global::Platform.Resource.arrow151;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            b = new RNavigatorButton(level, NAV_DIRECTION.DOWN);
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = this._navigatorControl.Height;
            b.Left = l.Width + btnWidth + 1;
            b.Top = 0;
            b.Click += new System.EventHandler(_navButtonClick);
            b.BackgroundImage = global::Platform.Resource.caret;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            this._navigatorControl.Controls.Add(p);
            p.Parent = this._navigatorControl;

            return p;
        }

        private void _optimizationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = "";
            ListBox list_box = (sender as ListBox);

            if (list_box.SelectedItem != null)
            {
                PBaseObject obj = (list_box.SelectedItem as PMasterItem).Object;
                _currentMasterObject = obj;

                string text = _model.Templates.TransformText(_conf.DetailTemplate, obj, ref path, true);
                _fbDb.DisconnectDB();

                _prevObject = obj.NavigatorInitialization(path);

                _detailsWebControl.LoadHTML(_addScriptsToBody(text));

                _buildControls();
            }
        }

        private void _detailsWebControl_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            string path = "";
            string text = _model.Templates.TransformText(_conf.DetailTemplate, _currentMasterObject, ref path, true);
            _fbDb.DisconnectDB();

            PBaseObject obj = this._currentMasterObject.NavigatorInitialization(path);
            if(obj != null)
            {
                _scrollToElement(obj.Id.ToString(), 60);
            }
            _updateActiveHtmlElement(_prevObject, obj, true);

            _domIsReady = true;
            _jsObject =  _detailsWebControl.CreateGlobalJavascriptObject("app");

            _jsBind();
        }

        private void _navButtonClick(object sender, EventArgs e)
        {
            RNavigatorButton b = (RNavigatorButton)sender;
            this._currentMasterObject.NavigatorNavigate(b.NavigatorLevel, b.NavDirection);
        }
    }*/
}
