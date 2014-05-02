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
		private RAppConfig _conf;

		private IStorage _modelStorage;

        private PModel _data;
		protected PNavigatorPath navMasterPath;
		protected PNavigator navMaster;

		protected PNavigatorPath navDetailPath;
		protected PNavigator navDetails;

		private bool _domIsBusy;
		private JSObject _jsObject;
		private int _lastId;

		protected IPObject master {
			set
			{
				IPObject prev = master;
				try{
					// обновление контента
					enableChildControls(panelNavigator, false);
					enableChildControls(panelAppCommands, false);
					_loadOptHtml(value);
					master = value; // если обновление удалось, то можно сменить объект
				}finally{
					// обновление списка (если выбран другой элемент, в списке это должно быть видно)
					if (master == null)
						listBoxOptimizations.SelectedItem = null;
					else if (master != prev)
					{
						foreach (RMasterItem item in listBoxOptimizations.Items)
						{
							if (item.Object == master)
							{
								listBoxOptimizations.SelectedItem = item;
								break;
							}
						}
					}
				}
			}
			get { return master; }
		}

        public FMain()
        {
			// инициализация компонентов
            InitializeComponent();
			listBoxOptimizations.DisplayMember = "Title";
			listBoxOptimizations.ValueMember = "Object";

			// инициализация внутренних переменных
			_domIsBusy = false;
			this.Text = "Распил " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			
			// загрузка конфигурации
			_conf = RAppConfig.Load(Path.GetDirectoryName(Application.ExecutablePath) + "\\config.json");
			
			// навигация
			List<string> navDetailLevels;
			List<string> navButtonsCaptions;
			RAppConfig.ParseNavigationSetup(_conf.Navigation, out navDetailLevels, out navButtonsCaptions);
			// настройка навигации
			this.navMasterPath = new PNavigatorPath(_conf.MasterCollectionPath+":0");
			for (int i = 0; i < navDetailLevels.Count; i++)
				navDetailLevels[i] += ":0";
			this.navDetailPath = new PNavigatorPath(string.Join("/", navDetailLevels.ToArray()));
			// кнопки для навигации
			_createNavButtons(panelNavigator, navButtonsCaptions);
			enableChildControls(panelNavigator, false);
			enableChildControls(panelAppCommands, false);

			// подключаем хранилище данных
			this._modelStorage = new SStorageFB(_conf.ConnectionString);

			_reloadModel(this._modelStorage, _conf.ModelCode, _conf.MasterItemTemplate, restorePosition: false);
        }

        private void FMain_Load(object sender, EventArgs e)
        {
			WindowState = FormWindowState.Maximized;
        }

		void navMaster_OnNavigated(object sender, NavigatedEventArgs e)
		{
			this.master = e.newObject; // setter сделает остальную работу
		}

		public bool QueryValue(string varName, bool caseSensitive, out string value)
		{
			value = null;
			return false;
		}

        private void listBoxOptimizations_SelectedIndexChanged(object sender, EventArgs e)
        {
			RMasterItem item = listBoxOptimizations.SelectedItem as RMasterItem;
			if (item == null)
			{
				this.master = null;
			}
			else if (item.Object != this.master) // это важно
			{
				this.master = item.Object;
			}
        }

        private void _loadOptHtml(IPObject obj)
		{
			if (obj != null)
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
        }

		private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
		{
			_jsObject = webControlDetails.CreateGlobalJavascriptObject("app");
			_jsBind();

			navDetailPath.resetPositions();
			navDetails = new PNavigator(this.master, this.navDetailPath);
			var obj = navDetails.Navigate(navDetailPath);

			_updateActiveHtmlElement(0, obj.Id, doScroll: true);
			_lastId = obj.Id;
			enableChildControls(panelNavigator, true);
			enableChildControls(panelAppCommands, true);

			_domIsBusy = false;

			updateAppCommandsAvailability(panelAppCommands);
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

				PNavigatorPath path = navDetails.GetPathTo(obj);
				navDetails.Navigate(path);
                _updateActiveHtmlElement(_lastId, obj.Id, false);
                _lastId = obj.Id;

                updateAppCommandsAvailability(panelAppCommands);
            }
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
				b.Text = cmd.Name;
				b.AutoSize = true;
				b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				b.Height = owner.Height;
				b.Left = owner.Width - b.Width - x;
				b.Click += new System.EventHandler(this._commandButtonClick);
				owner.Controls.Add(b);
				x += b.Width + 2;
			}
		}

        private void _commandButtonClick(object sender, EventArgs e)
        {
            RAppCommandButton b = sender as RAppCommandButton;

			IPObject pointer = navDetails.Pointer;
			string targetAttr = b.appCommand.TargetAttr;
            if (pointer != null && targetAttr != "")
            {
				IPObject objTarget;
				string scriptAttr;
				if (pointer.FindAttr(targetAttr, out objTarget, out scriptAttr)){
					string scriptTemplate;
					if (objTarget.GetAttr(scriptAttr, true, out scriptTemplate))
					{
						string script = PTemplates.FormatObject(objTarget, scriptTemplate, this, b.appCommand);
						MScriptManager.Execute(Path.GetDirectoryName(Application.ExecutablePath),
							script, new ModuleFinishedHandler(this._moduleFinishedCallback));
					}
					else
					{
						throw new Exception(string.Format(@"Не найден атрибут '{0}', который должен содержать скрипт для команды {1}", scriptAttr, b.appCommand.Name));
					}
				}
				else
				{
					throw new Exception(string.Format(@"Не найден объект, содержащий целевой атрибут '{0}'", targetAttr));
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
					if (this.navDetails.Pointer != null)
						btn.Enabled = this.navDetails.Pointer.FindAttr(btn.appCommand.TargetAttr, out obj, out val);
					else
						btn.Enabled = false;
				}
            }
        }

        private void _navButtonClick(object sender, EventArgs e)
        {
            RNavigatorButton b = sender as RNavigatorButton;
            IPObject obj = navDetails.Navigate(b.Depth, b.Direction, overStep:true);

            _updateActiveHtmlElement(_lastId, obj.Id, false);
            _lastId = obj.Id;

            updateAppCommandsAvailability(panelAppCommands);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
			if (this._modelStorage != null)
				_reloadModel(this._modelStorage, _conf.ModelCode, _conf.MasterItemTemplate, restorePosition:true);
			else
				throw new Exception(@"Хранилище модели не загружено");
        }

        private void _reloadModel(IStorage storage, string modelCode, string masterItemTemplateName, bool restorePosition)
        {
			int currentId = 0;
			if (restorePosition)
				currentId = (this.listBoxOptimizations.SelectedItem as RMasterItem).Object.Id;

			// загрузка модели
			_data = new PModel(storage, modelCode, deferredLoad: true);

			// установка навигатора
			navMaster = new PNavigator(_data.Root, this.navMasterPath);
			navMaster.OnNavigated += navMaster_OnNavigated;

			_refreshOptimizationList(currentId);
        }

        private void _refreshOptimizationList(int selectObjectWithId)
        {
            listBoxOptimizations.Items.Clear();

			PNavigator navMasterItems = new PNavigator(_data.Root, navMasterPath);

			int selectIndex = -1;
			while (navMasterItems.Navigate(0, NAV_DIRECTION.DOWN, false) != null)
            {
                IPObject obj = navMasterItems.Pointer;

				RMasterItem item = new RMasterItem();
				item.Title = formatMasterItem(obj);
				item.Object = obj;
				int index = listBoxOptimizations.Items.Add(item);
				if (obj.Id == selectObjectWithId)
					selectIndex = index;
            }
			if (selectIndex >= 0)
				listBoxOptimizations.SelectedIndex = selectIndex;
        }

		protected string formatMasterItem(IPObject obj){
			string template;
			if (!obj.GetAttr(_conf.MasterItemTemplate, true, out template))
				throw new Exception(string.Format(@"Шаблон наименования оптимизации (атрибут {0}) не найден!", _conf.MasterItemTemplate));
			
			return PTemplates.FormatObject(obj, template, this, null);
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

        private void buttonMarkOptimization_Click(object sender, EventArgs e)
        {
            if (listBoxOptimizations.SelectedItem != null)
            {
                string attr = "";
                if (!master.GetAttr("CUTOPT", false, out attr))
					attr = "";
				master.SetAttr("CUTOPT", attr==""?"#":"");
                master.StorageUpdateAttr("CUTOPT");
				RMasterItem item = listBoxOptimizations.SelectedItem as RMasterItem;
				if (item != null)
					item.Title = formatMasterItem(item.Object);
            }
        }
    }
}