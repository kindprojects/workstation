using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Platform
{
    public enum NAV_DIRECTION { UP, DOWN };
    internal class PModelObjectNavigator
    {
        public List<PModelObjectNavigatorPathLevel> Levels;
        
        private PBaseObject _current;
        private PBaseObject _pointer;
        public PBaseObject Pointer
        {
            set
            {
                if (value != null && _current == null)
                {
                    throw new Exception("Текущий объект не задан");
                }
                else if (value.IsChildOf(_current))
                {
                    List<PObjectLevelPath> path = this._current.GetPathTo(value);
                    for (int ii = 0; ii < path.Count(); ii++)
                    {
                        this.Levels[ii].Index = path[ii].Index;
                    }
                    _pointer = value;
                }
                else
                {
                    throw new Exception("Указанный объект не принадлежит текущему");
                }
            }
            get
            {
                return _pointer;
            }
        }

        public delegate void NavigatedEventHandler(object sender, PBaseObject o);
        public event NavigatedEventHandler OnNavigated;

        private void _parseToLevels(string path)
        {
            Levels = new List<PModelObjectNavigatorPathLevel>();
            foreach (Match match in Regex.Matches(path, @"([^:/\\]+(?::[^:/\\]+)?)"))
            {
                Levels.Add(new PModelObjectNavigatorPathLevel(match.Groups[1].ToString()));
            }
        }

        public void Setup(string path)
        {
            _parseToLevels(path);
            
        }

        public void SetObject(PBaseObject obj)
        {
            this._current = obj;
            foreach (PModelObjectNavigatorPathLevel level in Levels)
            {
                level.Index = 0;
            }
            if (this._current == null)
            {
                this.Pointer = null;
            }
            else
            {
                _updatePointer();
            }
            //_enableControls((this._current != null));
        }

        //private void _enableControls(bool e)
        //{
        //    foreach (PModelObjectNavigatorPathLevel lvl in Levels)
        //    {
        //        lvl.UiControl.Enabled = e;
        //    }
        //}

        public string GetPathString()
        {
            string ret = "";
            foreach (PModelObjectNavigatorPathLevel lvl in Levels)
            {
                ret += ((ret != "") ? "/" : "") + lvl.CollectionName + "[" + lvl.Index.ToString() + "]";
            }
            return ret;
        }

        private void _updatePointer()
        {
            if (this._current == null)
            {
                throw new Exception("Текущий объект не задан");
            }

            PBaseObject o = this._current;
            foreach (PModelObjectNavigatorPathLevel lvl in Levels)
            {
                PCollection coll = o.GetCollection(lvl.CollectionName, false);
                if (coll == null)
                {
                    throw new Exception("Указанная в конфигурации коллекция '" + lvl.CollectionName + "' не описана в переданном объекте");
                }
                else
                {
                    int index = lvl.Index;
                    if (index < 0)
                    {
                        index = coll.Count() - 1;
                        lvl.Index = index;
                    }
                    o = coll.GetObject(index);
                    if (o != null)
                    {
                        this.Pointer = o;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //OnNavigated(this, this.Pointer);
        }

        //private void _buildControls(Control owner)
        //{
        //    int x = 0;
        //    owner.Controls.Clear();
        //    foreach (PModelObjectNavigatorPathLevel lvl in this._levels)
        //    {
        //        if (lvl.UiControlName != "")
        //        {
        //            Control c = _createLevelControls(owner, lvl, x);
        //            lvl.UiControl = c;
        //            x += c.Width;
        //        }
        //    }
        //}

        //private Control _createLevelControls(Control owner, PModelObjectNavigatorPathLevel level, int x)
        //{
        //    const int btnWidth = 40;

        //    Panel p = new Panel();
        //    p.Location = new Point(x, 0);

        //    Label l = new Label();
        //    p.Controls.Add(l);
        //    l.Text = level.UiControlName;
        //    l.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        //    l.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        //    l.AutoSize = true;
        //    int labWidth = l.Width;
        //    l.AutoSize = false;
        //    l.Width = labWidth;
        //    l.Height = owner.Height;
        //    p.Width = labWidth + 2 * btnWidth + 1;

        //    PNavigatorButton b = new PNavigatorButton(level, NAV_DIRECTION.UP);
        //    //b.Text = "UP";
        //    p.Controls.Add(b);
        //    b.Width = btnWidth;
        //    b.Height = owner.Height;
        //    b.Left = l.Width;
        //    b.Top = 0;
        //    b.Click += new System.EventHandler(this._navButtonClick);
        //    b.BackgroundImage = global::Platform.Resource.arrow151;
        //    b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        //    b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

        //    b = new PNavigatorButton(level, NAV_DIRECTION.DOWN);
        //    //b.Text = "DOWN";
        //    p.Controls.Add(b);
        //    b.Width = btnWidth;
        //    b.Height = owner.Height;
        //    b.Left = l.Width + btnWidth + 1;
        //    b.Top = 0;
        //    b.Click += new System.EventHandler(this._navButtonClick);
        //    b.BackgroundImage = global::Platform.Resource.caret;
        //    b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        //    b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

        //    owner.Controls.Add(p);
        //    p.Parent = owner;

        //    return p;
        //}

        //private void _navButtonClick(object sender, EventArgs e)
        //{
        //    PNavigatorButton b = (PNavigatorButton)sender;
        //    this._navigate(b.NavigatorLevel, b.NavDirection);
        //}

        public bool Navigate(PModelObjectNavigatorPathLevel level, NAV_DIRECTION dir)
        {
            if (this._current == null)
                throw new Exception("Текущий объект не задан");
            int cnt = _indexOf(level) + 1;
            int[] newPath = new int[cnt];
            for (int ii = 0; ii < cnt; ii++)
            {
                newPath[ii] = Levels[ii].Index;
            }

            if (_tryGetNewPath(ref newPath, dir))
            {
                for (int ii = 0; ii < Levels.Count(); ii++)
                {
                    Levels[ii].Index = (ii < cnt) ? newPath[ii] : 0;
                }
                _updatePointer();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool _tryGetNewPath(ref int[] path, NAV_DIRECTION dir)
        {
            for (int ii = path.Count() - 1; ii >= 0; ii--)
            {
                path[ii] += (dir == NAV_DIRECTION.UP) ? -1 : 1;
                if (_validatePath(path, ii))
                {
                    return true;
                }
                else
                {
                    path[ii] = (dir == NAV_DIRECTION.UP) ? -1 : 0;
                }
            }
            return false;
        }

        private int _indexOf(PModelObjectNavigatorPathLevel level)
        {
            int levelIndex = Levels.IndexOf(level);
            if (levelIndex < 0)
            {
                throw new Exception("Указанный уровень не принадлежит навигатору");
            }
            return levelIndex;
        }

        private bool _validatePath(int[] path, int toIndex)
        {
            PBaseObject o = this._current;
            if (o == null)
                throw new Exception("Текущий объект не задан");
            if (this.Levels.Count() < path.Count())
                throw new Exception("Неправильный путь (path:" + this.Levels.Count().ToString() + " > levels:" + this.Levels.Count().ToString() + ")");
            int index = path[toIndex];
            for (int ii = 0; ii <= toIndex; ii++)
            {
                string collName = this.Levels[ii].CollectionName;
                PCollection coll = o.GetCollection(collName, false);
                if (coll == null)
                    throw new Exception("Указанный в конфигурации путь не соответствует модели (коллекция " + collName + " не найдена)");
                index = path[ii];
                if (index < 0 || index >= coll.Count())
                    return false;
                o = coll.GetObject(path[ii]);
                if (o == null)
                    return false;
            }
            return true;
        }
    }

    public class PModelObjectNavigatorPathLevel
    {
        public string CollectionName { private set; get; }
        public int Index { set; get; }
        public string UiControlName { private set; get; }

        public Control UiControl { set; get; }

        public PModelObjectNavigatorPathLevel(string path)
        {
            Match match = Regex.Match(path, @"([^:]+)(?::([^:]+))?", RegexOptions.IgnoreCase);
            CollectionName = match.Groups[1].Value;

            if (match.Groups[1] != null)
            {
                UiControlName = match.Groups[2].Value;
            }
        }
    }
}
