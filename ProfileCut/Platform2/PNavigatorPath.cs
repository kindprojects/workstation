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
		public int copyPositions(PNavigatorPath from){
			int cnt = Parts.Count;
			int cntFrom = from.Parts.Count;
			if (cntFrom > cnt)
				throw new Exception(@"Длина пути для копирования превышает длину пути для замены");
			for (int i = 0; i < cnt; i++)
			{
				if (i < cntFrom)
					Parts[i].PositionInLevel = from.Parts[i].PositionInLevel;
				else
					Parts[i].PositionInLevel = 0;
			}
			return cntFrom;
		}
		protected void parse(string path)
		{
			Parts.Clear();

			MatchCollection levels = Regex.Matches(path, @"([^:/]+):(\d+)");
			if (levels.Count == 0)
				throw new Exception("Неверный формат пути: '"+path+"'");
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
