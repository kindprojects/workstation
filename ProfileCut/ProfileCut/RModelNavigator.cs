using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;

namespace Model
{
    enum NAV_DIRECTION { UP, DOWN };
    public class RModelObjectNavigator
    {
        private List<RModelObjectNavigatorPathLevel> _levels;
        private RBaseObject _current;

        private RBaseObject _pointer;
        public RBaseObject Pointer 
        { 
            set 
            {
                if (value != null && _current == null){
                    throw new Exception("Текущий объект не задан");
                }
                else if (value.IsChildOf(_current))
                {
                    List<RObjectLevelPath> path = this._current.GetPathTo(value);
                    for (int ii = 0; ii < path.Count(); ii++)
                    {
                        this._levels[ii].Index = path[ii].Index;
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

        public delegate void NavigatedEventHandler(object sender, RBaseObject o);
        public event NavigatedEventHandler OnNavigated;
   
        private void _parseToLevels(string path)
        {
            _levels = new List<RModelObjectNavigatorPathLevel>();
            foreach (Match match in Regex.Matches(path, @"([^:/\\]+(?::[^:/\\]+)?)"))
            {
                _levels.Add(new RModelObjectNavigatorPathLevel(match.Groups[1].ToString()));
            }
        }

        public void Setup(string path, Control controlsOwner)
        {
            this._parseToLevels(path);
            if (controlsOwner != null)
                this._buildControls(controlsOwner);
            _enableControls((this._current != null));
        }

        public void SetObject(RBaseObject obj)
        {
            this._current = obj;
            foreach (RModelObjectNavigatorPathLevel level in _levels)
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
            _enableControls((this._current != null));
        }

        private void _enableControls(bool e)
        {
            foreach (RModelObjectNavigatorPathLevel lvl in _levels)
            {
                lvl.UiControl.Enabled = e;
            }
        }

        public string GetPathString()
        {
            string ret = "";
            foreach (RModelObjectNavigatorPathLevel lvl in _levels)
            {
                ret += ((ret!="")?"/":"") + lvl.CollectionName + "[" + lvl.Index.ToString() + "]";
            }
            return ret;
        }

        private void _updatePointer()
        {
            if (this._current == null)
                throw new Exception("Текущий объект не задан");
            RBaseObject o = this._current;
            foreach (RModelObjectNavigatorPathLevel lvl in _levels)
            {
                RCollection coll = o.GetCollection(lvl.CollectionName, false);
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
            OnNavigated(this, this.Pointer);
        }

        private void _buildControls(Control owner)
        {
            int x = 0;
            foreach (RModelObjectNavigatorPathLevel lvl in this._levels)
            {
                if (lvl.UiControlName != "")
                {
                    Control c = _createLevelControls(owner, lvl, x);
                    lvl.UiControl = c;
                    x += c.Width;
                }
            }
        }

        private Control _createLevelControls(Control owner, RModelObjectNavigatorPathLevel level, int x)
        {
            const int btnWidth = 40;

            Panel p = new Panel();
            p.Location = new System.Drawing.Point(x, 0);

            Label l = new Label();
            p.Controls.Add(l);
            l.Text = level.UiControlName;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            l.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            l.AutoSize = true;
            int labWidth = l.Width;            
            l.AutoSize = false;
            l.Width = labWidth;
            l.Height = owner.Height;            
            p.Width = labWidth + 2 * btnWidth + 1;

            RNavigatorButton b = new RNavigatorButton(level, NAV_DIRECTION.UP);
            //b.Text = "UP";
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = owner.Height;
            b.Left = l.Width;
            b.Top = 0;
            b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::ProfileCut.Resource.arrow151;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            b = new RNavigatorButton(level, NAV_DIRECTION.DOWN);
            //b.Text = "DOWN";
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = owner.Height;
            b.Left = l.Width+btnWidth + 1;
            b.Top = 0;
            b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::ProfileCut.Resource.caret;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            owner.Controls.Add(p);
            p.Parent = owner;
            
            return p;
        }

        private void _navButtonClick(object sender, EventArgs e)
        {
            RNavigatorButton b = (RNavigatorButton)sender;
            this._navigate(b.NavigatorLevel, b.NavDirection);
        }

        private bool _navigate(RModelObjectNavigatorPathLevel level, NAV_DIRECTION dir)
        {
            if (this._current == null)
                throw new Exception("Текущий объект не задан");
            int cnt = _indexOf(level) + 1;
            int[] newPath = new int[cnt];
            for (int ii = 0; ii < cnt; ii++)
            {
                newPath[ii] = _levels[ii].Index;
            }

            if (_tryGetNewPath(ref newPath, dir))
            {
                for (int ii = 0; ii < _levels.Count(); ii++)
                {
                    _levels[ii].Index = (ii < cnt) ? newPath[ii] : 0;
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
            for (int ii = path.Count()-1; ii >= 0; ii--)
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

        
        //private bool _tryGetNewPath(int[] path, NAV_DIRECTION dir, int depth)
        //{
        //    int index = path.Count() - 1 - depth;

        //    int currentDepthIndex = path[index];

        //    path[index] += (dir == NAV_DIRECTION.UP) ? -1 : 1;

        //    if (_validatePath(path))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return _tryGetNewPath(path, dir, depth + 1);
        //    }
        //}

        private int _indexOf(RModelObjectNavigatorPathLevel level)
        {
            int levelIndex = _levels.IndexOf(level);
            if (levelIndex < 0)
            {
                throw new Exception("Указанный уровень не принадлежит навигатору");
            }
            return levelIndex;
        }
        
        private bool _validatePath(int[] path, int toIndex)
        {
            RBaseObject o = this._current;
            if (o == null)
                throw new Exception("Текущий объект не задан");
            if (this._levels.Count() < path.Count())
                throw new Exception("Неправильный путь (path:"+this._levels.Count().ToString()+" > levels:"+this._levels.Count().ToString()+")");
            int index = path[toIndex];
            for (int ii = 0; ii <= toIndex; ii++)
            {
                string collName = this._levels[ii].CollectionName;
                RCollection coll = o.GetCollection(collName, false);
                if (coll == null)
                    throw new Exception("Указанный в конфигурации путь не соответствует модели (коллекция "+collName+" не найдена)");
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

    public class RModelObjectNavigatorPathLevel
    {
        public string CollectionName { private set; get; }
        public int Index { set; get; }
        public string UiControlName { private set; get; }

        public Control UiControl { set; get; }

        public RModelObjectNavigatorPathLevel(string path)
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
