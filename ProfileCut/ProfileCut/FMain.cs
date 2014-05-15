using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.IO;
using Awesomium.Core;

using ModuleConnect;
using Repository;
using Platform2;

namespace ProfileCut
{
    public partial class FMain : Form, IMHost
    {
        private RAppConfig _conf;

        private IStorage _modelStorage;

        private PModel _data;
        protected PNavigatorPath navMasterPath;

        protected PNavigatorPath navDetailPath { set; get; }
        protected PNavigator navDetails;
        protected bool scrollDetailViewOnNavigate;

        private JSObject _jsObject;
        private IPObject _master;

        protected IPObject master
        {
            set
            {
                IPObject prev = _master;
                try
                {
                    timerListOptimizationsRefresh.Stop();

                    // пересоздадим команды
                    _createAppCommandsButtons(panelAppCommands, _conf.Commands.Buttons);

                    // обновление контента
                    enableChildControls(panelNavigator, false);
                    enableChildControls(panelAppCommands, false);
					List<string> navLevels;
					List<string> navCaptions;
					string html = genOptHtml(value, out navLevels, out navCaptions);					
					if (html != "")
					{
                        genNavigation(panelNavigator, navLevels, navCaptions);
						_master = value; // если обновление удалось, то можно сменить объект
						loadOptHtml(html);                        
					}

                    timerListOptimizationsRefresh.Start();
                }
                finally
                {
                    // обновление списка (если выбран другой элемент, в списке это должно быть видно)
                    if (_master == null)
                        listBoxOptimizations.SelectedItem = null;
                    else
                    {
                        int masterId = _master.Id;
                        foreach (RMasterItem item in listBoxOptimizations.Items)
                        {
                            if (item.Object.Id == masterId)
                            {
                                listBoxOptimizations.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
                enableChildControls(panelNavigator, true);
                enableChildControls(panelAppCommands, true);
                
                updateAppCommandsAvailability(panelAppCommands);
            }
            get { return _master; }
        }

        public FMain()
        {
            // инициализация компонентов
            InitializeComponent();
            listBoxOptimizations.DisplayMember = "Title";
            listBoxOptimizations.ValueMember = "Object";
            webControlDetails.Crashed += webControlDetails_Crashed;

            // инициализация внутренних переменных
            this.Text = "Распил " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // загрузка конфигурации
            _conf = RAppConfig.Load(Path.GetDirectoryName(Application.ExecutablePath) + "\\config.json");

			// настройка навигации
			this.navMasterPath = new PNavigatorPath(_conf.MasterCollectionPath + ":0");
			// команды
            _createAppCommandsButtons(panelAppCommands, _conf.Commands.Buttons);
            enableChildControls(panelAppCommands, false);

            // подключаем хранилище данных
            this._modelStorage = new SStorageFB(_conf.ConnectionString);

			// загружать модель не будем. Это сделает событие таймера
            //_reloadModel(this._modelStorage, _conf.ModelCode, _conf.MasterItemTemplate, restorePosition: false);
        }

        private void FMain_Load(object sender, EventArgs e)
        {
			WindowState = FormWindowState.Maximized;
			timerListOptimizationsRefresh.Interval = _conf.MasterItemsUpdateIntervalMs;
			timer1_Tick(timerListOptimizationsRefresh, new EventArgs());
			timerListOptimizationsRefresh.Start();
        }

        public bool QueryValue(string varName, bool caseSensitive, out string value)
        {
            value = null;
            string lowerName = varName.ToLower();
            foreach (RAppConfigVar var in _conf.HostVars)
            {
                if (var.ParamName == varName || (!caseSensitive && var.ParamName.ToLower() == lowerName))
                {
                    value = var.Value;
                    return true;
                }
            }
            return false;
        }

        private void listBoxOptimizations_SelectedIndexChanged(object sender, EventArgs e)
        {
			try
			{                
				RMasterItem item = listBoxOptimizations.SelectedItem as RMasterItem;
				int itemId = item != null ? item.Object.Id : 0;
				int masterId = this.master != null ? this.master.Id : 0;
				if (itemId != masterId)
				{                    
					this.master = (item != null ? item.Object : null);
					//Clipboard.SetText(this.master.ToXElement().ToString());
				}
			}catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка при обработке оптимизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
        }

        protected string genOptHtml(IPObject obj, out List<string> navLevels, out List<string> navCaptions)
		{
			string html = "";
			// по-умолчанию пустые списки
			navLevels = new List<string>();
			navCaptions = new List<string>();
            if (obj != null)
            {
                string template;
                if (!obj.GetAttr(_conf.DetailTemplate, true, out template))
                    throw new Exception(string.Format(@"Шаблон просмотра деталей (атрибут {0}) не найден!", _conf.DetailTemplate));

                using (FFormat form = new FFormat(this.Width / 2 - 150, this.Height / 2 - 81, obj, template, this, null))
                {
					form.ShowDialog();
					if (form.ExceptionMessage != "")
						throw new Exception(form.ExceptionMessage);
					html = form.GeneratedHtml;
					// заменяем
					navLevels = form.parsedNavLevels;
					navCaptions = form.parsedNavCaptions;
				}
			}

			if (html != "")
				html = _addScriptsToBody(html);
			return html;
        }
		protected void loadOptHtml(string html)
		{
			string tmpFilePath = Application.StartupPath + "\\test2.htm";
			StreamWriter wr = new StreamWriter(tmpFilePath);
			try
			{
				wr.Write(html);
			}
			finally
			{
				wr.Close();
			}
			if (!webControlDetails.IsLive)
				throw new Exception("Not live");
			webControlDetails.Source = new Uri("file:///" + tmpFilePath.Replace('\\', '/'));
			webControlDetails.Update();
		}

		protected void genNavigation(Control owner, List<string>levels, List<string>captions)
		{
			if (levels != null)
			{
				for (int i = 0; i < levels.Count; i++)
				{
					if (!levels[i].EndsWith(":0"))
						levels[i] += ":0";
				}
				this.navDetailPath = new PNavigatorPath(string.Join("/", levels.ToArray()));
			}
            // кнопки для навигации
			_createNavButtons(owner, captions);
		}

        void webControlDetails_Crashed(object sender, CrashedEventArgs e)
        {
            throw new Exception("Awesomium crashed\r\n" + e.ToString());
        }

        private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            try
            {
				if (this.master != null)
				{
					_jsObject = webControlDetails.CreateGlobalJavascriptObject("app");
					_jsBind();

                    _addProcessedClass();

                    //enableChildControls(panelNavigator, true);
                    //enableChildControls(panelAppCommands, true);

					navDetailPath.resetPositions();
					navDetails = new PNavigator(this.master, this.navDetailPath);
					navDetails.OnNavigated += navDetails_OnNavigated;
					this.scrollDetailViewOnNavigate = true;
					navDetails.Navigate(navDetailPath);
				}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Document Ready handler exception:\r\n" + ex.Message, "Ошибка в обработчике", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void navDetails_OnNavigated(object sender, NavigatedEventArgs e)
        {
            int prevId = e.prevObject != null ? e.prevObject.Id : 0;
            int newId = e.newObject != null ? e.newObject.Id : 0;
			_updateActiveHtmlElement(prevId, newId, doScroll: this.scrollDetailViewOnNavigate);

            foreach (Control ctrl in panelAppCommands.Controls)
            {
                if (ctrl is RAppCommandButton)
                {
                    RAppCommandButton btn = (RAppCommandButton)ctrl;

					// если CSS-класс выделения не задан, то мы не должны накаплиавть объекты в очереди (т.к. нельзя их увидеть)
					if (String.IsNullOrEmpty(btn.AppCommand.SelectedCssClass))
						btn.ObjectsQueue.Clear();
                    
					if (e.newObject != null)
					{
						IPObject targetObj;
						string targetVal = "";
						// подходит ли объект
						if (e.newObject.FindAttr(btn.AppCommand.TargetAttr, out targetObj, out targetVal))
						{
							// если не было - добавляем, иначе убираем
							if (btn.ObjectsQueue.IndexOf(targetObj.Id) < 0)
								_addObjectToCommandQueue(targetObj.Id, btn);
							else
								_removeObjectFromCommandQueue(targetObj.Id, btn);
						}
                    }
                }
            }

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
                    _scrollTo(selectObjectId.ToString(), 120);
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

        private void _addClassToListObjects(List<int> objects, string className)
        {
            if (objects.Count() > 0)
            {
                string js = "function f(os, c) {"
                    + "for (ii = 0; ii < os.length; ii ++){"
                    + "el = document.getElementById(os[ii]);"
                    + "if (el){"
                    + "if (el.className.indexOf(c) == -1) el.className = el.className + ' ' + c + ' ';}}}";

                js = js + _createFunctionCall(objects, className);

                webControlDetails.ExecuteJavascript(js);
            }
        }
        
        private void _removeClassFromListObjects(List<int> objects, string className)
        {
            if (objects.Count() > 0)
            {
                string js = "function f(os, c) {"
                    + "for (ii = 0; ii < os.length; ii ++){"
                    + "el = document.getElementById(os[ii]);"
                    + "if (el) {"
                    + "el.className = el.className.replace(c, '');}}}";

                js = js + _createFunctionCall(objects, className);

                webControlDetails.ExecuteJavascript(js);
            }
        }

        private string _createFunctionCall(List<int> objects, string className)
        {
            string js = "var ids = [";
            foreach(int id in objects)
            {
                js = js + "'" + id.ToString() + "',";
            }
            js = js.Substring(0, js.Length - 1) + "];";
            js = js + "f(ids, '" + className + "');";

            return js;
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
            try
			{
                if (args.Arguments.Length > 0 && !args.Arguments[0].IsUndefined)
                {
                    string arg0 = args.Arguments[0];
                    int id = 0;
                    try
                    {
                        id = Convert.ToInt32(arg0);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Объект указан неверно (" + arg0 + ")", ex);
                    }

                    IPObject obj = _master.GetObjectById(id);
                    if (obj == null)
                        throw new Exception(string.Format(@"Не удалось найти объект по ID={0}", id));

                    if (navDetails != null)
                    {
                        PNavigatorPath path = navDetails.GetPathTo(obj);
                        this.scrollDetailViewOnNavigate = false;
                        navDetails.Navigate(path);
                    }
                    else
                    {
                        throw new Exception("navDetails не создан!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("jsBodyClick handler exception:\r\n" + ex.Message, "Ошибка в обработчике", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected void _addObjectToCommandQueue(int id, RAppCommandButton commandBtn)
        {
            if (!String.IsNullOrEmpty(commandBtn.AppCommand.SelectedCssClass))
            {
                // добавим класс выбранного элемента
                _addClass(id.ToString(), commandBtn.AppCommand.SelectedCssClass);
			}
			// добавим в список выбранных
			commandBtn.ObjectsQueue.Add(id);
        }

        protected void _removeObjectFromCommandQueue(int id, RAppCommandButton commandBtn)
        {
			int index = commandBtn.ObjectsQueue.IndexOf(id);

            if (!String.IsNullOrEmpty(commandBtn.AppCommand.SelectedCssClass))
			{
				// удалим класс обработанного элемента
				_removeClass(id.ToString(), commandBtn.AppCommand.SelectedCssClass);
            }
			// положение объекта в списке неизвестно?
			if (index >= 0)
			{
				// удалим из списка выбарных
				commandBtn.ObjectsQueue.RemoveAt(index);
			}
        }

        // устанавливает атрибут объекта и сохраняет в БД  
        protected void _setObjectAttr(IPObject obj, string attrName, string val)
        {
            obj.SetAttr(attrName, val);
            obj.StorageUpdateAttr(attrName);
        }

        private void _createNavButtons(Control owner, List<string> names)
        {
			const int padding = 10;
			owner.Controls.Clear();
            int depth = 0;
            int x = 0;
			if (names != null)
			{
				foreach (string name in names)
				{
					x = _createLevelButtons(owner, depth, name, x)+padding;
					depth++;
				}
			}
        }
        private void _createAppCommandsButtons(Control owner, List<RAppCommand> cmdList)
        {
            owner.Controls.Clear();
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
            if (b.ObjectsQueue.Count > 0)
            {
                foreach (int id in b.ObjectsQueue)
                {
                    IPObject targetObj = _master.GetObjectById(id);
                    if (targetObj != null)
                    {
                        string targetAttr;
                        if (targetObj.GetAttr(b.AppCommand.TargetAttr, false, out targetAttr))
                        {
                            string scriptTemplate;
                            if (targetObj.GetAttr(targetAttr, true, out scriptTemplate))
                            {
                                string script = PTemplates.FormatObject(targetObj, scriptTemplate, this, b.AppCommand, null, null);
                                MScriptManager.Execute(Path.GetDirectoryName(Application.ExecutablePath)
                                    , this
                                    , script
                                    , new ModuleFinishedHandler(this._moduleFinishedCallback)
                                );
                                    
                                // удалим css класс - выбран
                                _removeClass(targetObj.Id.ToString(), b.AppCommand.SelectedCssClass);

                                // установим объекту признак - обработан
                                _setObjectAttr(targetObj, b.AppCommand.ProcessedAttr, "1");
                            }
                            else
                            {
                                throw new Exception(string.Format(@"Не найден атрибут '{0}', который должен содержать скрипт для команды {1}", targetAttr, b.AppCommand.Name));
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format(@"У объекта c ID={0} не найден атрибут '{1}', который должен содержать имя атрибута со скриптом для команды",
                                id.ToString(), b.AppCommand.TargetAttr));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format(@"Не удалось найти объект по ID={0}", id));
                    }
                }
                b.ObjectsQueue.Clear();
                _addProcessedClass();
                updateAppCommandsAvailability(panelAppCommands);
            }
        }

        protected void _addProcessedClass()
        {            
            foreach(var cmd in _conf.Commands.Buttons)
            {
                List<int> addObjList = new List<int>();
                foreach (IPObject obj in _master.GetObjectsIndex().Values)
                {
                    string val = "";                    
                    if (obj.GetAttr(cmd.ProcessedAttr, false, out val))
                    {
                        if (val == "1")
                            addObjList.Add(obj.Id);                     
                    }
                }                
                _addClassToListObjects(addObjList, cmd.ProcessedCssClass);                
            }                            
        }

        private void _moduleFinishedCallback(IModule module)
        {

        }

        private int _createLevelButtons(Control owner, int depthTag, string text, int x)
        {
			int minBtnWidth = _conf.NavigatorMinButtonWidth;
			const int spacing = 1;

            Panel p = new Panel();
            p.Location = new System.Drawing.Point(x, 0);

            Label l = new Label();
            p.Controls.Add(l);
            l.Text = text;
            l.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			l.Font = this.Font;// new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            l.AutoSize = true;
            int labWidth = Math.Max(l.Width, minBtnWidth*2)+spacing*2;
			int btnWidth = (labWidth - spacing) / 2;
			l.Height = 12;
            l.AutoSize = false;
            l.Width = labWidth;
			l.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            p.Width = labWidth;
			//p.BorderStyle = BorderStyle.FixedSingle;

            RNavigatorButton b = new RNavigatorButton(depthTag, NAV_DIRECTION.UP);
            p.Controls.Add(b);
            b.Left = l.Left;
            b.Width = btnWidth;
			b.Top = l.Height;
            b.Height = owner.Height - l.Height;

            b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::ProfileCut.Resource.arrow151;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;


			b = new RNavigatorButton(depthTag, NAV_DIRECTION.DOWN);
			p.Controls.Add(b);
			b.Left = l.Left + btnWidth + spacing;
			b.Width = btnWidth;
			b.Top = l.Height;
			b.Height = owner.Height - l.Height;

			b.Click += new System.EventHandler(this._navButtonClick);
			b.BackgroundImage = global::ProfileCut.Resource.caret;
			b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			
			/*
            b = new RNavigatorButton(depthTag, NAV_DIRECTION.DOWN);
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = owner.Height;
            b.Left = l.Width + btnWidth + 1;
            b.Top = 0;
            b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::ProfileCut.Resource.caret;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;*/

            owner.Controls.Add(p);
            p.Parent = owner;

            return p.Width + x;
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
                    
                    if (btn.ObjectsQueue.Count() > 0)
                        btn.Enabled = true;
                    else
                        btn.Enabled = false;
                }
            }
        }

        private void _navButtonClick(object sender, EventArgs e)        
        {                       
            RNavigatorButton b = sender as RNavigatorButton;
            this.scrollDetailViewOnNavigate = true;
            if (navDetails != null)
            {
                _clearSelectedObjetsListsAndRemoveSelectedClass(panelAppCommands);
                navDetails.Navigate(b.Depth, b.Direction, overStep: true);
            }
        }

        private void _clearSelectedObjetsListsAndRemoveSelectedClass(Control owner)
        {
            List<int> ids = new List<int>();
            foreach (Control ctrl in owner.Controls)
            {
                if (ctrl is RAppCommandButton)
                {
                    RAppCommandButton btn = (RAppCommandButton)ctrl;
                    _removeClassFromListObjects(btn.ObjectsQueue, btn.AppCommand.SelectedCssClass);
                    btn.ObjectsQueue.Clear();
                }
            }            
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (this._modelStorage != null)
                _reloadModel(this._modelStorage, _conf.ModelCode, _conf.MasterItemTemplate, restorePosition: true);
            else
                throw new Exception(@"Хранилище модели не загружено");
        }

        private void _reloadModel(IStorage storage, string modelCode, string masterItemTemplateName, bool restorePosition)
        {
            int currentId = 0;
            if (restorePosition && (listBoxOptimizations.SelectedItem != null))
                currentId = (this.listBoxOptimizations.SelectedItem as RMasterItem).Object.Id;

            // загрузка модели
            _data = new PModel(storage, modelCode, deferredLoad: true);

            _refreshOptimizationList(currentId);
        }

        private void _refreshOptimizationList(int selectObjectWithId)
        {
            listBoxOptimizations.Items.Clear();

            navMasterPath.resetPositions();
            PNavigator navMasterItems = new PNavigator(_data.Root, navMasterPath);

            int selectIndex = -1;

            do
            {
                IPObject obj = navMasterItems.Pointer;
                if (obj != null)
                {
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
            while (navMasterItems.Navigate(0, NAV_DIRECTION.DOWN, false) != null);

            //while (navMasterItems.Navigate(0, NAV_DIRECTION.DOWN, false) != null)
            //{
            //    IPObject obj = navMasterItems.Pointer;

            //    RMasterItem item = new RMasterItem();
            //    item.Title = formatMasterItem(obj);
            //    item.Object = obj;
            //    int index = listBoxOptimizations.Items.Add(item);
            //    if (obj.Id == selectObjectWithId)
            //        selectIndex = index;
            //}
            //if (selectIndex >= 0)
            //    listBoxOptimizations.SelectedIndex = selectIndex;
        }

        protected string formatMasterItem(IPObject obj)
        {
            string template;
            if (!obj.GetAttr(_conf.MasterItemTemplate, true, out template))
                throw new Exception(string.Format(@"Шаблон наименования оптимизации (атрибут {0}) не найден!", _conf.MasterItemTemplate));

            return PTemplates.FormatObject(obj, template, host:this, overloads:null, worker:null, navInfo:null);
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
			if (this.master != null){
                string attr = "";
                if (!master.GetAttr("CUTOPT", false, out attr))
                    attr = "";
                master.SetAttr("CUTOPT", attr == "" ? "#" : "");
                master.StorageUpdateAttr("CUTOPT");
				// обновить текст отдельной строчки listBox'а не получается - не реагирует. Приходится формировать список целиком
				_refreshOptimizationList(master.Id);
            }
        }

        private void buttonHide_Click(object sender, EventArgs e)
        {
            if (splitContainerAwesomium.Panel1Collapsed)
                splitContainerAwesomium.Panel1Collapsed = false;
            else
                splitContainerAwesomium.Panel1Collapsed = true;
        }

		private void btnExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			_reloadModel(this._modelStorage, _conf.ModelCode, _conf.MasterItemTemplate, restorePosition: true);
		}
    }
}