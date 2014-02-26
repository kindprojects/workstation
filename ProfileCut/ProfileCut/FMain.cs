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
using Awesomium.ComponentModel;
using Awesomium.Core;
using Awesomium.Web;
using Awesomium.Windows;
using System.Text.RegularExpressions;
using Api;
using ModuleConnect;


namespace ProfileCut
{
    public partial class FMain : Form
    {
        private JSObject _jsObject;

        //private RConfig _conf;
        private RAppConfig _conf;


        // представление модели
        private AModel _viewModel;
        
        private ABaseObject _master;
        private int _previousId;

        private RNavigatorPath _startNavigatorPath;

        private List<string> _buttonNames;

        private bool _domIsReady;       

        public FMain()
        {
            InitializeComponent();

            _startNavigatorPath = new RNavigatorPath();

            _buttonNames = new List<string>();

            _conf = new RAppConfig().Load();
            _parseNavigation(_conf.Navigation);
            _createButtons(_buttonNames);

            _viewModel = new AModel(_conf.ConnectionString, _conf.ModelCode, false);
            _master = _viewModel.GetRoot().Navigate(_conf.MasterCollectionPath + ":0");

            _navButtonsDisable(panelNavigator);
            _printButtonsDisable(panelPrinterButtons);
            
            _domIsReady = true;

            this.Text = "Распил " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
           
        private void FMain_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            listBoxOptimizations.DisplayMember = "DispTitle";
            listBoxOptimizations.ValueMember = "Object";

            _refreshOptimizationList();
        }
 
        private void listBoxOptimizations_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox list_box = (sender as ListBox);

            // нельзя грузить html в awesomium если загрузка уже идет
            if (_domIsReady)
            {
                _loadHtml();
            }
        }

        // загружает в awesomium html выбранной оптимизации
        private void _loadHtml()
        {
            if (listBoxOptimizations.SelectedItem != null)
            {                
                ABaseObject obj = (listBoxOptimizations.SelectedItem as RMasterItem).Object;

                if (obj != null)
                {
                    _domIsReady = false;

                    _master = obj;
                    string html = _viewModel.Transform(_conf.DetailTemplate, obj, null);
                    webControlDetails.LoadHTML(_addScriptsToBody(html));
                }
            }
        }

        // загружает html только если выбранная оптимизация отлична от текущей
        private void _reloadHtml()
        {
            if (listBoxOptimizations.SelectedItem != null)
            {                
                ABaseObject obj = (listBoxOptimizations.SelectedItem as RMasterItem).Object;

                if (obj != null)
                {
                    if (obj.Id != _master.Id)
                    {
                        _domIsReady = false;

                        _master = obj;
                        string html = _viewModel.Transform(_conf.DetailTemplate, obj, null);
                        webControlDetails.LoadHTML(_addScriptsToBody(html));
                    }
                }
            }
        }

        private void _updateActiveHtmlElement(int deselectObjectId, int selectObjectId, bool doScroll)
        {
            if (deselectObjectId > 0)
            {
                _removeClassFromElement(deselectObjectId.ToString(), _conf.SelectedHtmlElementClass);
            }
            if (selectObjectId > 0)
            {
                _addClassToElement(selectObjectId.ToString(), _conf.SelectedHtmlElementClass);

                if (doScroll)
                {
                    _scrollToElement(selectObjectId.ToString(), 60);
                }
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

            webControlDetails.ExecuteJavascript(str);
        }
    

        private void _removeClassFromElement(string id, string className)
        {
            string js = "function removeClass(o, c) {"
            + " o.className = o.className.replace(c, '');"
            + " }"
            + " removeClass(document.getElementById('" + id + "'), ' " + className + " ');";

            webControlDetails.ExecuteJavascript(js);
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
                ABaseObject obj = _master.GetObjectById(Convert.ToInt32(id));
                
                _master.SetNavigatorPointer(obj);
                _updateActiveHtmlElement(_previousId, obj.Id, false);                
                _previousId = obj.Id;

                _printButtonsEnable(panelPrinterButtons);
            }
        }           

        private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            _jsObject = webControlDetails.CreateGlobalJavascriptObject("app");
            _jsBind();

            var obj = _master.Navigate(_startNavigatorPath.GetStringPath());
            _updateActiveHtmlElement(0, obj.Id, true);
            _previousId = obj.Id;
            _navButtonsEnable(panelNavigator);

            _domIsReady = true;

            // загрузим новую оптимизацию, если выбранная оптимизация отлична от текущей
            // такая ситуация возможна при быстром перемещении по списку оптимизаций          
            _reloadHtml();

            _printButtonsEnable(panelPrinterButtons);
        }

        private void _parseNavigation(string path)
        {
            try 
            {
                foreach (Match match in Regex.Matches(path, @"([^:/\\]+(?::[^:/\\]+)?)"))
                {
                    MatchCollection partMatches =  Regex.Matches(match.Groups[1].ToString(), @"([^:]+)(?::([^:]+))?", RegexOptions.IgnoreCase);                    
                    foreach(Match partMatch in partMatches)
                    {
                        _startNavigatorPath.Parts.Add(new RNavigatorPartPath(partMatch.Groups[1].Value.ToString(), 0));                      
                        _buttonNames.Add(partMatch.Groups[2].Value.ToString());
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не верный формат пути. " + ex.Message);
            }
        }
        
        private void _createButtons(List<string> names)
        {
            int depth = 0;
            int x = 0;
            foreach(string name in names)
            {
                x = _createDepthButtons(panelNavigator, depth, name, x);
                depth++;
            }

            x = 0;
            for (int ii = _conf.Commands.Buttons.Count() - 1; ii >= 0; ii--)
            {
                x = _createPrintButton(panelPrinterButtons, _conf.Commands.Buttons[ii], x);
            }
        }
        private int _createPrintButton(Control owner, RAppButton config, int x)
        {
            //RPrinterButton b = new RPrinterButton(config.Text, config.Module, config.NameSpace, config.Class, config.Printer, config.AttrTemplate);
            RPrinterButton b = new RPrinterButton(config.Text, config.AttrTemplate, config.TemplateOverloads.PrinterName);
            b.AutoSize = true;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            b.Height = owner.Height;
            b.Left = owner.Width - b.Width - x;
            b.Click += new System.EventHandler(this._printButtonClick);                    
                    
            owner.Controls.Add(b);

            return b.Width + 2;
        }
        private void _printButtonClick(object sender, EventArgs e)
        {            
            RPrinterButton b = (RPrinterButton)sender;
            if (_master != null)
            {                
                ABaseObject pointer = _master.GetNavigatorPointer();
                //if (pointer != null && b.AttrTemplate != "" && b.ModuleFileName != "")
                if (pointer != null && b.AttrTemplate != "")
                {
                    ABaseObject o = _getObjectWihtAttrTempate(b.AttrTemplate);
                    
                    // string temp = _getPrintTemplateName(pointer, b.AttrTemplate);
                    if (o != null)
                    {
                        string commands = _viewModel.Transform(o.GetAttr(b.AttrTemplate, false), o, b.GetOverload());
                        MScriptManager.Execute(Path.GetDirectoryName(Application.ExecutablePath), 
                            commands, new ModuleFinishedHandler(this.ModuleFinishedCallback));
                    }
                }
            }
        }
        
        private void ModuleFinishedCallback(IModule module)
        {

        }

        private int _createDepthButtons(Control owner, int depth, string text, int x)
        {
            const int btnWidth = 40;

            Panel p = new Panel();
            p.Location = new System.Drawing.Point(x, 0);

            Label l = new Label();
            p.Controls.Add(l);
            l.Text = text;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            l.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            l.AutoSize = true;
            int labWidth = l.Width;
            l.AutoSize = false;
            l.Width = labWidth;
            l.Height = owner.Height;
            p.Width = labWidth + 2 * btnWidth + 1;

            RNavigatorButton b = new RNavigatorButton(depth, 1);
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = owner.Height;
            b.Left = l.Width;
            b.Top = 0;
            b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::ProfileCut.Resource.arrow151;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            b = new RNavigatorButton(depth, 0); 
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = owner.Height;
            b.Left = l.Width + btnWidth + 1;
            b.Top = 0;
            b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::ProfileCut.Resource.caret;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            owner.Controls.Add(p);
            p.Parent = owner;

            return p.Width;
        }

        private void _navButtonsEnable(Control owner)
        {
            owner.Enabled = true;
            foreach(Control button in owner.Controls)
            {
                button.Enabled = true;
            }
        }
        private void _navButtonsDisable(Control owner)
        {
            owner.Enabled = false;
            foreach (Control button in owner.Controls)
            {
                button.Enabled = false;
            }
        }     

        private void _printButtonsEnable(Control owner)
        {
            foreach (RPrinterButton button in owner.Controls)
            {
                ABaseObject o = _getObjectWihtAttrTempate(button.AttrTemplate);
                if (o != null)
                {
                    button.Enabled = true;
                }
                else
                {
                    button.Enabled = false;
                }
            }                
        }

        private void _printButtonsDisable(Control owner)
        {
            foreach (RPrinterButton button in owner.Controls)
            {
                button.Enabled = false;
            }
        }
       
        private void _navButtonClick(object sender, EventArgs e)
        {
            RNavigatorButton b = (RNavigatorButton)sender;            
            ABaseObject obj = _master.Navigate(b.Depth, b.Direction);
            
            _updateActiveHtmlElement(_previousId, obj.Id, false);
            _previousId = obj.Id;

            _printButtonsEnable(panelPrinterButtons);
        }
     
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            object selectedItem = this.listBoxOptimizations.SelectedItem;

            _viewModel = new AModel(_conf.ConnectionString, _conf.ModelCode, true);
            _viewModel.GetRoot().Navigate(_conf.MasterCollectionPath+":0");           
            _refreshOptimizationList();

            if (selectedItem != null)
            {
                int id = (selectedItem as RMasterItem).Object.Id;
                _selectListItemById(id);
            }
        }

        private void _refreshOptimizationList()
        {
            listBoxOptimizations.Items.Clear();
            ABaseObject root = _viewModel.GetRoot();
            ABaseObject obj;
            do
            {
                obj = root.GetNavigatorPointer();
                listBoxOptimizations.Items.Add(new RMasterItem()
                {
                    DispTitle = _viewModel.Transform(_conf.MasterItemTemplate, obj, null),
                    Object = obj
                });
            } while (obj.Id != root.Navigate(0, 0).Id);
        }

        private void _selectListItemById(int id)
        {
            listBoxOptimizations.SelectedIndexChanged -= new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
            try
            {
                for (int ii = 0; ii < listBoxOptimizations.Items.Count; ii++)
                {
                    if ((listBoxOptimizations.Items[ii] as RMasterItem).Object.Id == id)
                    {
                        listBoxOptimizations.SelectedIndex = ii;
                    }
                }
            }
            finally
            {
               listBoxOptimizations.SelectedIndexChanged += new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
            }
        }

        private ABaseObject _getObjectWihtAttrTempate(string attrTemplate)
        {
            ABaseObject ret = null;

            for (int ii = _startNavigatorPath.Parts.Count() - 1; ii >= 0; ii--)
            {
                ABaseObject obj = _master.GetObjectByDepth(ii);
                string attrVal = obj.GetAttr(attrTemplate, false);

                if (attrVal != null && attrVal != "")
                {
                    ret = obj;

                    break;
                }
            }

            // поищем в мастере
            if (ret == null)
            {
                string masterAttrVal = _master.GetAttr(attrTemplate, false);
                if (masterAttrVal != null && masterAttrVal != "")
                {
                    ret = _master;
                }
            }

            return ret;
        }
    }
}

