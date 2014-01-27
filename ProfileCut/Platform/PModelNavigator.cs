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
    public class PModelObjectNavigator
    {
        private List<string> _levels;
        private List<int> _positionInLevel;

        private PBaseObject _owner;

        //private PBaseObject _current;
        //private PBaseObject _pointer;

        public PBaseObject Pointer;
        private PBaseObject _pointer
        {
            set
            {
                if (value != null && _owner == null)
                {
                    throw new Exception("Текущий объект не задан");
                }
                else if (value.IsChildOf(_owner))
                {
                    List<int> path = this._owner.GetPathTo(value);
                    for (int ii = 0; ii < path.Count(); ii++)
                    {
                        _positionInLevel[ii] = path[ii];
                    }
                    Pointer = value;
                }
                else
                {
                    throw new Exception("Указанный объект не принадлежит текущему");
                }
            }
            get
            {
                return Pointer;
            }
        }

        public delegate void NavigatedEventHandler(object sender, PBaseObject o);
        public event NavigatedEventHandler OnNavigated;

        public PModelObjectNavigator(PBaseObject owner)
        {
            _owner = owner;
        }


        // collection:index/collection:index/...
        private void _parseToLevels(string path)
        {
            try 
            {
                foreach (Match match in Regex.Matches(path, @"([^:/\\]+(?::[^:/\\]+)?)"))
                {
                    MatchCollection partMatches = _parsePart(match.Groups[1].ToString());
                    
                    foreach(Match partMatch in partMatches)
                    {
                        _levels.Add(partMatch.Groups[1].Value.ToString());
                        _positionInLevel.Add(Convert.ToInt32(partMatch.Groups[2].Value.ToString()));
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не верный формат пути. " + ex.Message);
            }
        }

        // collection:index
        private MatchCollection _parsePart(string partPath)
        {
            return Regex.Matches(partPath, @"([^:]+)(?::([^:]+))?", RegexOptions.IgnoreCase);
        }

        public PBaseObject Setup(string path)
        {
            _levels = new List<string>();
            _positionInLevel = new List<int>();
            _parseToLevels(path);
            _updatePointer();

            return Pointer;
        }

        //public void SetObject(PBaseObject obj)
        //{
        //    this._current = obj;
        //    foreach (PModelObjectNavigatorPathLevel level in Levels)
        //    {
        //        level.Index = 0;
        //    }
        //    if (this._current == null)
        //    {
        //        this.Pointer = null;
        //    }
        //    else
        //    {
        //        _updatePointer();
        //    }
        //    //_enableControls((this._current != null));
        //}

        //private void _enableControls(bool e)
        //{
        //    foreach (PModelObjectNavigatorPathLevel lvl in Levels)
        //    {
        //        lvl.UiControl.Enabled = e;
        //    }
        //}

        //public string GetPathString()
        //{
        //    string ret = "";


        //    foreach (string lvl in _levels)
        //    {
        //        ret += ((ret != "") ? "/" : "") + lvl + "[" + lvl.Index.ToString() + "]";
        //    }
        //    return ret;
        //}

        private void _updatePointer()
        {
            if (_owner == null)
            {
                throw new Exception("Текущий объект не задан");
            }

            //PBaseObject o = _owner;
            for (int ii = 0; ii < _levels.Count(); ii++)
            {
                string level = _levels[ii];
                int position = _positionInLevel[ii];

                PCollection collection = _owner.GetCollection(level, false);
                if (collection == null)
                {
                    throw new Exception("Указанная в конфигурации коллекция '" + level + "' не описана в переданном объекте");
                }
                else {
                    PBaseObject obj = collection.GetObject(position);
                    if (obj != null) {
                        this.Pointer = obj;
                    }
                    else {
                        break;
                    }
                }
            }
        }

        public PBaseObject Navigate(int depth, NAV_DIRECTION dir)
        {
            if (_owner == null)
                throw new Exception("Текущий объект не задан");

            List<int> newPath = _positionInLevel;                        
            newPath[depth] = newPath[depth] + 1;

            if (_tryGetNewPath(ref newPath, dir))
            {

                _positionInLevel = newPath;

                //for (int ii = 0; ii < _levels.Count(); ii++)
                //{
                //    _positionInLevel[ii] = (ii < cnt) ? newPath[ii] : 0;
                //}
                _updatePointer();

                return Pointer;
            }
            else
            {
                return null;
            }
        }

        //public PBaseObject Navigate(int depth, NAV_DIRECTION dir)
        //{
        //    if (_owner == null)
        //        throw new Exception("Текущий объект не задан");

        //    int cnt = _positionInLevel[depth] + 1;
        //    int[] newPath = new int[cnt];
        //    for (int ii = 0; ii < cnt; ii++)
        //    {
        //        newPath[ii] = _positionInLevel[ii];
        //    }

        //    if (_tryGetNewPath(ref newPath, dir))
        //    {
        //        for (int ii = 0; ii < _levels.Count(); ii++)
        //        {
        //            _positionInLevel[ii]  = (ii < cnt) ? newPath[ii] : 0;
        //        }
        //        _updatePointer();

        //        return Pointer;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public PBaseObject Navigate(int[] path)
        {
            if (_owner == null)
                throw new Exception("Текущий объект не задан");

            PBaseObject obj = _owner;
            for (int ii = 0; ii < path.Length; ii++)
            {
                PCollection collection = obj.GetCollection(_levels[ii], true);
                if (collection != null)
                {
                    // есть объект?
                    obj = collection.GetObject(ii);
                    if (obj != null)
                    {
                        _positionInLevel[ii] = path[ii];
                    }
                    else
                    {
                        for (int jj = ii; jj < _positionInLevel.Count(); jj++)
                            _positionInLevel[jj] = 0;

                        break;
                    }
                }
            }

            _updatePointer();

            return Pointer;
        }

        public PBaseObject Navigate(string path)
        {
            _parseToLevels(path);
            _updatePointer();

            return Pointer;            
        }

        //public PBaseObject Navigate(int depth, NAV_DIRECTION direction)
        //{
        //    if (_owner == null)
        //        throw new Exception("Текущий объект не задан");

        //    PBaseObject obj = Pointer;

        //    // есть уровень?
        //    if (depth < _levels.Count())
        //    {
        //        // навигация в текущем уровне?
        //        if (Pointer._ownerCollection.Name == _levels[depth])
        //        {
        //            int index = Pointer._ownerCollection.IndexOf(Pointer);
        //            if (index != -1)
        //            {
        //                if (direction == NAV_DIRECTION.DOWN) 
        //                {
        //                    // можно вниз?
        //                    if (index < Pointer._ownerCollection.Count() - 1)
        //                    {
        //                        _positionInLevel[depth] = _positionInLevel[depth] + 1;                                
        //                    }
        //                }
        //                else {
        //                    // можно вверх?
        //                    if (index > 0) {
        //                        _positionInLevel[depth] = _positionInLevel[depth] + 1;

        //                    }
        //                }
        //                obj = Pointer._ownerCollection.GetObject(_positionInLevel[depth]);
        //            }
                    
        //        }
        //        else
        //        {
        //            PCollection collection = _owner.GetCollection(_levels[depth], true)
        //            if (collection != null)
        //            {
        //                if (direction == NAV_DIRECTION.DOWN)
        //                {

        //                }
        //            }
        //        }
        //    }
        //    Pointer = obj;

        //    return obj;
        //}

        //public PBaseObject Navigate(string level, NAV_DIRECTION dir)
        //{
        //    if (_owner == null)
        //        throw new Exception("Текущий объект не задан");
        //    int cnt = _indexOf(level) + 1;
        //    int[] newPath = new int[cnt];
        //    for (int ii = 0; ii < cnt; ii++)
        //    {
        //        newPath[ii] = Levels[ii].Index;
        //    }

        //    if (_tryGetNewPath(ref newPath, dir))
        //    {
        //        for (int ii = 0; ii < Levels.Count(); ii++)
        //        {
        //            Levels[ii].Index = (ii < cnt) ? newPath[ii] : 0;
        //        }                
        //        return _updatePointer();
        //    }
        //    else
        //    {
        //        return Pointer;
        //    }
        //}

        private bool _tryGetNewPath(ref List<int> path, NAV_DIRECTION dir)
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

        //private bool _tryGetNewPath(ref int[] path, NAV_DIRECTION dir)
        //{
        //    for (int ii = path.Count() - 1; ii >= 0; ii--)
        //    {
        //        path[ii] += (dir == NAV_DIRECTION.UP) ? -1 : 1;
        //        if (_validatePath(path, ii))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            path[ii] = (dir == NAV_DIRECTION.UP) ? -1 : 0;
        //        }
        //    }
            
        //    return false;
        //}

        private int _indexOf(string level)
        {
            int levelIndex = _levels.IndexOf(level);
            if (levelIndex < 0)
            {
                throw new Exception("Указанный уровень не принадлежит навигатору");
            }
            return levelIndex;
        }

        private bool _validatePath(List<int> path, int toIndex)
        {
            PBaseObject o = _owner;
            if (o == null)
                throw new Exception("Текущий объект не задан");

            if (_levels.Count() < path.Count())
                throw new Exception("Неправильный путь (path:" + _levels.Count().ToString() + " > levels:" + _levels.Count().ToString() + ")");

            int index = path[toIndex];
            for (int ii = 0; ii <= toIndex; ii++)
            {
                string collName = _levels[ii];
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

        //private bool _validatePath(int[] path, int toIndex)
        //{
        //    PBaseObject o = _owner;
        //    if (o == null)
        //        throw new Exception("Текущий объект не задан");

        //    if (_levels.Count() < path.Count())
        //        throw new Exception("Неправильный путь (path:" + _levels.Count().ToString() + " > levels:" + _levels.Count().ToString() + ")");

        //    int index = path[toIndex];
        //    for (int ii = 0; ii <= toIndex; ii++)
        //    {
        //        string collName = _levels[ii];
        //        PCollection coll = o.GetCollection(collName, false);
        //        if (coll == null)
        //            throw new Exception("Указанный в конфигурации путь не соответствует модели (коллекция " + collName + " не найдена)");

        //        index = path[ii];
        //        if (index < 0 || index >= coll.Count())
        //            return false;

        //        o = coll.GetObject(path[ii]);
        //        if (o == null)
        //            return false;
        //    }
        //    return true;
        //}
    }

    // string[], int[]
    //public class PModelObjectNavigatorPathLevel
    //{
    //    public string CollectionName { private set; get; }
    //    public int Index { set; get; }
    //    public string UiControlName { private set; get; }

    //    public Control UiControl { set; get; }

    //    public PModelObjectNavigatorPathLevel(string path)
    //    {
    //        Match match = Regex.Match(path, @"([^:]+)(?::([^:]+))?", RegexOptions.IgnoreCase);
    //        CollectionName = match.Groups[1].Value;

    //        if (match.Groups[1] != null)
    //        {
    //            UiControlName = match.Groups[2].Value;
    //        }
    //    }
    //}
}
