using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace Platform2
{
	public enum NAV_DIRECTION { UP, DOWN };
    public class PNavigator
    {
        private PNavigatorPath _path;

        private IPObject _base;
        
        public IPObject Pointer {
			internal set{
				Pointer = value; 
				if (this.OnNavigated != null)
					OnNavigated(this, value); 
			}
			get { return Pointer; }
		}

        public delegate void NavigatedEventHandler(object sender, IPObject o);
		public event NavigatedEventHandler OnNavigated;

        public PNavigator(IPObject owner, PNavigatorPath path)
        {
            _base = owner;
			_path = path!=null?path:new PNavigatorPath(null);
        }
		public PNavigatorPath GetPathTo(IPObject child){
			IPObject o = child;
			if (child.Id == this._base.Id)
				throw new Exception(string.Format(@"Попытка получить путь от объекта до самого себя (id={0})", child.Id));

			int baseId = this._base.Id;
			PNavigatorPath retPath = new PNavigatorPath(null);
			while (o.onwerCollection != null)
			{
				IPCollection coll = o.onwerCollection;
				retPath.Parts.Insert(0, new PNavigatorPathPart(coll.CollectionName, coll.IndexOf(o)));
				o = coll.ownerObject;
				if (o != null && o.Id == baseId)
					return retPath;
			}
			throw new Exception(string.Format(@"Объект id={0} не принадлежит объекту id={1}", child.Id, this._base.Id));
		}
        
        public IPObject GetObjectAtPathLevel(int depth, bool partialReturn)
        {
            if (depth < 0 || depth >= _path.Parts.Count())
                throw new Exception(String.Format("Уровень {0} не существует", depth));

            IPObject o = this._base;
            for (int ii = 0; ii <= depth; ii++)
            {
				PNavigatorPathPart level = _path.Parts[ii];
                IPCollection collection = o.GetCollection(level.LevelName);
                if (collection == null)
					throw new Exception(string.Format(@"Коллекция {0} не найдена на уровне {1}", level.LevelName, ii));
				try{
					o = collection.GetObject(level.PositionInLevel);
				}catch{
					if (partialReturn)
						return o; // если объект с запрашиваемым индексом не найден - вернём тот, что был найден последний раз
					else
						throw;
				}
            }
            return o;
        }

        public IPObject Navigate(int depth, NAV_DIRECTION dir, bool overStep)
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

            if (_tryGetNewPath(ref newPath, dir, overStep))
            {
                for (int ii = 0; ii < _path.Parts.Count; ii++)
                {
					_path.Parts[ii].PositionInLevel = (ii < cnt) ? newPath[ii] : 0;
                }
				this.Pointer = this.GetObjectAtPathLevel(depth, false);
                return Pointer;
            }
            else
            {
                return null;
            }
        }
		
		public IPObject Navigate(PNavigatorPath path)
		{
			_path.copyPositions(from:path);

			this.Pointer = this.GetObjectAtPathLevel(path.Parts.Count-1, partialReturn: true);
			return this.Pointer;
		}
     
        private bool _tryGetNewPath(ref List<int> path, NAV_DIRECTION dir, bool overStep)
        {
            for (int ii = path.Count() - 1; ii >= 0; ii--)
            {
                path[ii] += (dir == NAV_DIRECTION.UP) ? -1 : 1;
                if (_validatePath(path, ii))
                {
                    return true;
                }
                else if (overStep)
                {
                    path[ii] = (dir == NAV_DIRECTION.UP) ? -1 : 0;
                }else
				{
					return false;
				}
            }

            return false;
        }

        private bool _validatePath(List<int> path, int toIndex)
        {
            IPObject o = _base;
            if (o == null)
                throw new Exception("Текущий объект не задан");

            if (path.Count() > _path.Parts.Count())
                throw new Exception(string.Format("Неправильный путь (path: {0} > levels: {1})", path.Count, _path.Parts.Count));

            int index = path[toIndex];
            for (int ii = 0; ii <= toIndex; ii++)
            {
                string collName = _path.Parts[ii].LevelName;
                IPCollection coll = o.GetCollection(collName);
                if (coll == null)
                    throw new Exception("Указанный в конфигурации путь не соответствует модели (коллекция " + collName + " не найдена)");

                index = path[ii];
                if (index < 0 || index >= coll.Count)
                    return false;

				try { 
					o = coll.GetObject(path[ii]);
				}catch{
					o = null;
				}
				if (o == null)
					return false;
            }
            return true;
        }
    }
}
