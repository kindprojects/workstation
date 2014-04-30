using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace Platform2
{
    public class PNavigator : IPNavigator
    {
        private PNavigatorPath _path;

        private IPObject _base;

        private PObject _pointer;
        
        public IPObject Pointer
        {
            set
            {
				if (value.IsChildOf(this._base.Id))
                {
                    PNavigatorPath path = this.GetPathTo(value);

                    if (path.Parts.Count() > _path.Parts.Count())
                        throw new Exception("Попытка установить указатель навигатора за пределы описанного в нём пути");

                    int[] positions = new int[path.Parts.Count()];
                    for (int ii = 0; ii < path.Parts.Count(); ii++)
                    {
                        if (path.Parts[ii].Level.ToLower() != _path.Parts[ii].Level.ToLower())
                            throw new Exception("Попытка установить указатель навигатора по неверному пути");
						
                        positions[ii] = path.Parts[ii].PositionInLevel;
                    }
                    // все ок, можно заполнить индексы
                    for (int ii = 0; ii < path.Parts.Count(); ii++)
                    {
                        _path.Parts[ii].PositionInLevel = positions[ii];
                    }

                    _pointer = value;
                }
                else
                {
                    throw new Exception("Указанный объект не принадлежит корневому объекту навигатора");
                }
            }
            get
            {
                return _pointer;
            }
        }

        public delegate void NavigatedEventHandler(object sender, IPObject o);
        public event NavigatedEventHandler OnNavigated;

        public PNavigator(IPObject owner)
        {
            _base = owner;
            _path = new PNavigatorPath();
        }
        
        public IPObject GetObjectAtPathLevel(int depth)
        {
            if (depth >= _path.Parts.Count())
                throw new Exception(String.Format("Уровень {0} не существует", depth));

            // объект которому пренадлежит навигатор
            IPObject o = this._base;
            for (int ii = 0; ii <= depth; ii++)
            {
                IPCollection collection = o.GetCollection(_path.Parts[ii].Level, false);
                if (collection != null)
                {
                    o = collection.GetObject(_path.Parts[ii].PositionInLevel);
                }
            }

            return o;
        }
		public static void ParseNavigationSetup(string path, out PNavigatorPath navPath, out List<string> levelsAliases)
		{
			levelsAliases = new List<string>();
			navPath = new PNavigatorPath();
			try
			{
				foreach (Match match in Regex.Matches(path, @"([^:/\\]+(?::[^:/\\]+)?)"))
				{
					MatchCollection partMatches = Regex.Matches(match.Groups[1].ToString(), @"([^:]+)(?::([^:]+))?", RegexOptions.IgnoreCase);
					foreach (Match partMatch in partMatches)
					{
						navPath.Parts.Add(new PNavigatorPathPart(partMatch.Groups[1].Value.ToString(), 0));
						levelsAliases.Add(partMatch.Groups[2].Value.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Неверный формат пути. " + ex.Message);
			}
		}
        // collection:index/collection:index/...
        private void _parseToLevels(string path)
        {
            try
            {
                _path.Parts.Clear();

                foreach (Match match in Regex.Matches(path, @"([^:/\\]+(?::\d+)?)"))
                {
					string part = match.Groups[1].ToString();
					MatchCollection partMatches = Regex.Matches(part, @"([^:]+)(?::([^:]+))?", RegexOptions.IgnoreCase); ;

                    foreach (Match partMatch in partMatches)
                    {
                        _path.Parts.Add(new PNavigatorPart(partMatch.Groups[1].Value.ToString(), Convert.ToInt32(partMatch.Groups[2].Value.ToString())));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не верный формат пути. " + ex.Message);
            }
        }

        private void _updatePointer()
        {
            if (_base == null)
            {
                throw new Exception("Текущий объект не задан");
            }

            PObject owner = _base;
            for (int ii = 0; ii < _path.Parts.Count(); ii++)
            {
                string level = _path.Parts[ii].Level;
                int position = _path.Parts[ii].PositionInLevel;

                PCollection collection = owner.GetCollection(level, false);
                if (collection == null)
                {
                    throw new Exception("Указанная в конфигурации коллекция '" + level + "' не описана в переданном объекте");
                }
                else
                {
                    if (position < 0)
                    {
                        position = collection.Count() - 1;
                        _path.Parts[ii].PositionInLevel = position;
                    }
                    PObject obj = collection.GetObject(position);
                    if (obj != null)
                    {
                        this.Pointer = obj;
                        owner = obj;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    
        public IPObject Navigate(int depth, NAV_DIRECTION dir)
        {
            if (_base == null)
                throw new Exception("Текущий объект не задан");

            int cnt = depth + 1;

            if (cnt > _path.Parts.Count())
                throw new Exception("Задана глубина больше чем уровней в навигаторе");

            List<int> newPath = new List<int>();
            for (int ii = 0; ii < cnt; ii++)
            {
                newPath.Add(_path.Parts[ii].PositionInLevel);
            }

            if (_tryGetNewPath(ref newPath, dir))
            {
                for (int ii = 0; ii < _path.Parts.Count(); ii++)
                {
                    _path.Parts[ii].PositionInLevel = (ii < cnt) ? newPath[ii] : 0;
                }
                _updatePointer();

                return Pointer;
            }
            else
            {
                return Pointer;
            }
        }    

        public IPObject Navigate(string path)
        {
            _parseToLevels(path);
            _updatePointer();

            return Pointer;
        }
     
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
     
        private int _indexOf(string level)
        {
            int levelIndex = _path.IndexOf(level);
            if (levelIndex < 0)
            {
                throw new Exception("Указанный уровень не принадлежит навигатору");
            }
            return levelIndex;
        }

        private bool _validatePath(List<int> path, int toIndex)
        {
            PObject o = _base;
            if (o == null)
                throw new Exception("Текущий объект не задан");

            if (_path.Parts.Count() < path.Count())
                throw new Exception(String.Format("Неправильный путь (path: {0} > levels: {1})", path.Count().ToString(), _path.Parts.Count().ToString()));

            int index = path[toIndex];
            for (int ii = 0; ii <= toIndex; ii++)
            {
                string collName = _path.Parts[ii].Level;
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
}
