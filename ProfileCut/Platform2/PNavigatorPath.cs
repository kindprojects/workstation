using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Platform2
{
    public class PNavigatorPath
    {
        public List<PNavigatorPathPart> Parts { set; get; }

        public PNavigatorPath(string init)
        {
            Parts = new List<PNavigatorPathPart>();
			if (init != null)
				parse(init);
        }
		public string ToString(int maxLevels=-1)
		{
			StringBuilder b = new StringBuilder();
			if (maxLevels > this.Parts.Count)
				throw new Exception(string.Format("Запрошенное количество уровней({0}) превышает доступное({1})", maxLevels, this.Parts.Count));
			int cnt = maxLevels<=0?this.Parts.Count:maxLevels;
			for (int i = 0; i < cnt; i++)
			{
				if (i > 0)
					b.Append("/");
				PNavigatorPathPart part = this.Parts[i];
				b.AppendFormat("{0}:{1}", part.LevelName, part.PositionInLevel);
			}
			return b.ToString();
		}

		public bool isInPath(PNavigatorPath refPath){
			int refCnt = refPath.Parts.Count;
			if (this.Parts.Count > refCnt)
				return false;
			int cnt = this.Parts.Count;
			for (int i = 0; i < cnt; i++)
			{
				if (this.Parts[i].LevelName.ToLower() != refPath.Parts[i].LevelName.ToLower())
					return false;
			}
			return true;
		}
		public void resetPositions()
		{
			foreach (PNavigatorPathPart part in Parts)
			{
				part.PositionInLevel = 0;
			}
		}
		// прогоняет весь путь, заменяя индекс -1 на индекс последнего элемента
		public void Normalize(IPObject withObject){
			int cnt = this.Parts.Count;
			IPObject o = withObject;
			for (int i = 0; i < cnt; i++)
			{
				PNavigatorPathPart part = this.Parts[i];
				IPCollection coll = o.GetCollection(part.LevelName);
				int index = part.PositionInLevel;
				if (index == -1)
				{
					index = coll.Count - 1;
					part.PositionInLevel = index;
				}
				o = coll.GetObject(index);
			}
		}
		public int copyPositions(PNavigatorPath from, bool partial){
			int cnt = Parts.Count;
			int cntFrom = from.Parts.Count;
			if (cntFrom > cnt && !partial)
				throw new Exception(@"Длина пути для копирования превышает длину пути для замены");
			for (int i = 0; i < cnt; i++)
			{
				if (i < cntFrom)
				{
					if (this.Parts[i].LevelName != from.Parts[i].LevelName)
						throw new Exception(string.Format(@"Имена коллекций в путях не совпадают ({0}:{1}<>{2})", i, this.Parts[i].LevelName, from.Parts[i].LevelName));
					Parts[i].PositionInLevel = from.Parts[i].PositionInLevel;
				}else
					Parts[i].PositionInLevel = 0;
			}
			return cntFrom;
		}
		protected void parse(string path)
		{
			Parts.Clear();

			MatchCollection levels = Regex.Matches(path, @"([^:/]+):(\d+)");
			if (levels.Count == 0)
				throw new Exception("Неверный формат пути навигатора: '"+path+"'");
			foreach (Match level in levels)
			{
				string collection = level.Groups[1].Value;
				int index = Convert.ToInt32(level.Groups[2].Value);

				Parts.Add(new PNavigatorPathPart(collection, index));
			}
		}
    }
	public class PNavigatorPathPart
	{
		public string LevelName { set; get; }
		public int PositionInLevel { set; get; }

		public PNavigatorPathPart(string level, int positionInLevel)
		{
			LevelName = level;
			PositionInLevel = positionInLevel;
		}
	}
}
