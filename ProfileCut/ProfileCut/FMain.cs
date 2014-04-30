using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using Awesomium.ComponentModel;
using Awesomium.Core;
using Awesomium.Web;
using Awesomium.Windows;

using ModuleConnect;
using Repository;
using Platform2;

namespace ProfileCut
{
    public partial class FMain : Form, IMValueGetter
    {
        private JSObject _jsObject;

        private RAppConfig _conf;

        private PModel _data;

		protected IPObject master {
			set {
				try{
					master = setMaster(value);
				}finally{
					int i = listBoxOptimizations.Items.IndexOf(master);
					if (i >= 0 && i != listBoxOptimizations.SelectedIndex)
						listBoxOptimizations.SelectedIndex = i;
				}
			} get;
		}

		protected PNavigator navMaster;

        private int _previousId;

        private PNavigatorPath _navigatorPath;

        private bool _domIsBusy;

        public FMain()
        {
            InitializeComponent();

			// инициализация внутренних переменных
			_domIsBusy = false;
			this.Text = "Распил " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			
			// загрузка конфигурации
			_conf = RAppConfig.Load(Path.GetDirectoryName(Application.ExecutablePath) + "\\config.json");

			// навигация C3
			List<string> navButtonsCaptions;
			PNavigator.ParseNavigationSetup(_conf.Navigation, out _navigatorPath, out navButtonsCaptions);
			_createNavButtons(panelNavigator, navButtonsCaptions);
			enableChildControls(panelNavigator, false);
			enableChildControls(panelAppCommands, false);

			// загрузка модели
            _data = new PModel(new SStorageFB(_conf.ConnectionString), _conf.ModelCode, true);
			// установка навигатора
			navMaster = new PNavigator(_data.Root);
			navMaster.OnNavigated += navMaster_OnNavigated;

			// переходим к записи по-умолчанию
			master = navMaster.Navigate(0, NAV_DIRECTION.STAY); // может не надо?
        }

		void navMaster_OnNavigated(object sender, IPObject o)
		{
			this.master = o; // setter сделает остальную работу
		}

        public bool QueryValue(string varName, bool caseSensitive, out string value)
        {
			value = null;
            return false;
        }

        private void FMain_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            listBoxOptimizations.DisplayMember = "Title";
            listBoxOptimizations.ValueMember = "Object";

            _refreshOptimizationList();
        }

        private void listBoxOptimizations_SelectedIndexChanged(object sender, EventArgs e)
        {
			RMasterItem item = listBoxOptimizations.SelectedItem as RMasterItem;
			if (item != null && item != this.master)
			{
				this.master = item.Object;
			}
        }

		protected IPObject setMaster(IPObject newMaster){
			_loadOptHtml(newMaster);
			return newMaster;
		}

        // загружает html только если выбранная оптимизация отлична от текущей
        private void _loadOptHtml(IPObject obj)
		{
			string template;
			if (obj.GetAttr(_conf.DetailTemplate, true, out template))
				throw new Exception(string.Format(@"Атрибут {0} не найден!", _conf.DetailTemplate));
			
			string html = PTemplates.FormatObject(obj, template, this, null);

			html = _addScriptsToBody(html);

			if (_domIsBusy)
				throw new Exception(@"DOM занят!");
			_domIsBusy = true;
			webControlDetails.LoadHTML(html);
        }

        private void _updateActiveHtmlElement(int deselectObjectId, int selectObjectId, bool doScroll)
        {
            if (deselectObjectId > 0)
            {
                _removeClass(deselectObjectId.ToString(), _conf.SelectedHtmlElementClass);                
            }
            if (selectObjectId > 0)
            {
                _addClass(selectObjectId.ToString(), _conf.SelectedHtmlElementClass);

                if (doScroll)
                {
                    _scrollTo(selectObjectId.ToString(), 60);
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
                + " element = element.parentNode;} "
                + " app.bodyOnClick(element ? element.id : undefined);}"
                + " </script>";

            var index = ret.IndexOf("</body>");
            if (index > 0)
            {
                ret = ret.Insert(index, script);
            }

            return ret;
        }

        private void _addClass(string id, string className)
        {
            string js = "function f(o, c) {"
            + " if (o.className.indexOf(c) == -1) o.className = o.className + ' ' + c + ' ';}"
            + " f(document.getElementById('" + id + "'), '" + className + "');";

            webControlDetails.ExecuteJavascript(js);
        }

        private void _scrollTo(string id, int yOffset)
        {
            string str = "function f(elementId, yOffset){"
                + "var e = document.getElementById(elementId);"
                + "if (e){"
                + " var left = 0;"
                + " var top = 0;"
                + " do {"
                + " left += e.offsetLeft;"
                + " top += e.offsetTop;"
                + " } while(e = e.offsetParent);"
                + " window.scrollTo(left, top - yOffset);}}"
                + " f('" + id + "', " + yOffset.ToString() + ");";

            webControlDetails.ExecuteJavascript(str);
        }

        private void _removeClass(string id, string className)
        {
            string js = "function f(o, c) {"
            + " o.className = o.className.replace(c, '');}"
            + " f(document.getElementById('" + id + "'), ' " + className + "');";

            webControlDetails.ExecuteJavascript(js);
        }

        private void _addOrRemoveClass(string id, string className)
        {
            string js = "function f(id, name) {"
                + " var e = document.getElementById(id);"
                + " if (e) {"
                + " var c = e.className;"
                + " if (c.indexOf(' ' + name) == -1) c += ' ' + name; "
                + " else c = c.replace(' ' + name, '');"
                + " e.className = c;}}"
                + " f(" + id + ", '" + className + "');";

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
				if (args.Arguments.Length <= 0)
					throw new Exception(@"_jsBodyClickHandler(args.lenght == 0) должен быть id объекта");
				string arg0 = args.Arguments[0];
				int id = Convert.ToInt32(arg0);
                //ABaseObject obj = _master.GetObjectById(Convert.ToInt32(id));
				IPObject obj;
				if (!_data.objectsIndex.TryGetValue(id, out obj))
					throw new Exception(string.Format(@"Не удалось найти объект по ID={0}", id));

				navMaster.Pointer = obj; // master.SetNavigatorPointer(obj);
                _updateActiveHtmlElement(_previousId, obj.Id, false);
                _previousId = obj.Id;

                updateAppCommandsAvailability(panelAppCommands);
            }
        }

        private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            _jsObject = webControlDetails.CreateGlobalJavascriptObject("app");
            _jsBind();

            var obj = master.Navigate(_navigatorPath.GetStringPath());
            _updateActiveHtmlElement(0, obj.Id, true);
            _previousId = obj.Id;
            _navButtonsEnable(panelNavigator);

            _domIsBusy = false;

            // загрузим новую оптимизацию, если выбранная оптимизация отлична от текущей
            // такая ситуация возможна при быстром перемещении по списку оптимизаций          
            //_reloadHtml();

            updateAppCommandsAvailability(panelAppCommands);
        }

		private void _createNavButtons(Control owner, List<string> names)
        {
            int depth = 0;
            int x = 0;
            foreach (string name in names)
            {
				x = _createLevelButtons(owner, depth, name, x);
                depth++;
            }
        }
		private void _createAppCommandsButtons(Control owner, List<RAppCommand> cmdList)
		{
			int x = 0;
			for (int ii = cmdList.Count() - 1; ii >= 0; ii--)
			{
				RAppCommand cmd = cmdList[ii];
				RAppCommandButton b = new RAppCommandButton(cmd);
				b.Text = cmd.Text;
				b.AutoSize = true;
				b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				b.Height = owner.Height;
				b.Left = owner.Width - b.Width - x;
				b.Click += new System.EventHandler(this._commandButtonClick);
				owner.Controls.Add(b);
				x += b.Width + 2;
			}
		}
        private int _createCommandButton(Control owner, RAppCommand command, int x)
        {

            return b.Width + 2;
        }

        private void _commandButtonClick(object sender, EventArgs e)
        {
            RAppCommandButton b = (RAppCommandButton)sender;
            _printButtonPress = b;

            if (master != null && b.AttrTemplate != "")
            {
                IPObject pointer = master.GetNavigatorPointer();
                if (pointer != null)
                {
					string commands = pointer.FindAndFormat(b.AttrTemplate);//, b.TemplateOverloads.GetTemplateOverloadsDictonary());
					MScriptManager.Execute(Path.GetDirectoryName(Application.ExecutablePath),
						commands, new ModuleFinishedHandler(this._moduleFinishedCallback));
                }
            }
        }

        private void _moduleFinishedCallback(IModule module)
        {

        }

        private int _createLevelButtons(Control owner, int depthTag, string text, int x)
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

            RNavigatorButton b = new RNavigatorButton(depthTag, NAV_DIRECTION.UP);
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = owner.Height;
            b.Left = l.Width;
            b.Top = 0;
            b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::ProfileCut.Resource.arrow151;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            b = new RNavigatorButton(depthTag, NAV_DIRECTION.DOWN);
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
        protected void enableChildControls(Control owner, bool enabled)
        {
            owner.Enabled = enabled;
            foreach (Control ctrl in owner.Controls)
            {
                ctrl.Enabled = enabled;
            }
        }

        private void updateAppCommandsAvailability(Control owner)
        {
            foreach (Control ctrl in owner.Controls)
            {
				if (ctrl is RAppCommandButton)
				{
					RAppCommandButton btn = (RAppCommandButton)ctrl;
					IPObject obj;
					string val;
					if (this.master != null)
						btn.Enabled = this.master.FindAttr(btn.appCommand.AttrTemplate, out obj, out val);
					else
						btn.Enabled = false;
				}
            }
        }

        private void _navButtonClick(object sender, EventArgs e)
        {
            RNavigatorButton b = (RNavigatorButton)sender;
            IPObject obj = master.Navigate(b.Depth, b.Direction);

            _updateActiveHtmlElement(_previousId, obj.Id, false);
            _previousId = obj.Id;

            updateAppCommandsAvailability(panelAppCommands);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            _refreshOptimization();
        }

        private void _refreshOptimization()
        {
            object selectedItem = this.listBoxOptimizations.SelectedItem;

            _data = new PModel(_conf.ConnectionString, _conf.ModelCode, true, this);
            _data.Root.Navigate(_conf.MasterCollectionPath + ":0");
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
            IPObject root = _data.Root;
            IPObject obj;
            do
            {
                obj = root.GetNavigatorPointer();
				string ready;
				obj.GetAttr("READY", false, out ready);

				//if (ready == "1")
				//{
					RMasterItem item = new RMasterItem();
					item.Title = obj.Format(_conf.MasterItemTemplate);
					item.Object = obj;
					listBoxOptimizations.Items.Add(item);
				//}
                
            } while (obj.Id != root.Navigate(0, NAV_DIRECTION.DOWN).Id);
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

        private IPObject _getObjectWihtAttrTempate(string attrTemplate)
        {
            IPObject ret = null;

            for (int ii = _navigatorPath.Parts.Count() - 1; ii >= 0; ii--)
            {
                IPObject obj = master.GetObjectAtNavigatorPathLevel(ii);
                string attrVal = "";
                if (obj.GetAttr(attrTemplate, false, out attrVal))
                {
                    if (attrVal != null && attrVal != "")
                    {
                        ret = obj;

                        break;
                    }
                }
            }

            // поищем в мастере
            if (ret == null)
            {
                string masterAttrVal = "";
                if (master.GetAttr(attrTemplate, false, out masterAttrVal))
                {
                    if (masterAttrVal != null && masterAttrVal != "")
                    {
                        ret = master;
                    }
                }
            }

            return ret;
        }

        private void buttonCut_Click(object sender, EventArgs e)
        {
            if (listBoxOptimizations.SelectedItem != null)
            {
                string attr = "";
                if (master.GetAttr("CUTOPT", false, out attr))
                {
                    if (attr == "")
                        master.SetAttr("CUTOPT", "#");
                    else
                        master.SetAttr("CUTOPT", "");
                }
                else
                    master.SetAttr("CUTOPT", "#");

                master.StorageUpdateAttr("CUTOPT");

                int index = listBoxOptimizations.SelectedIndex;
                listBoxOptimizations.SelectedIndexChanged -= new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
                try
                {
                    _refreshOptimization();
                }
                finally
                {
                    listBoxOptimizations.SelectedIndexChanged += new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
                    listBoxOptimizations.SelectedIndex = index;
                }
            }
        }
    }
}