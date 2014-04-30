using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform2
{
    public class PNavigatorPath
    {
        public List<PNavigatorPathPart> Parts { set; get; }

        public PNavigatorPath()
        {
            Parts = new List<PNavigatorPathPart>();         
        }

        public string GetStringPath() 
        {
            string ret = "";

            for (int ii = 0; ii < Parts.Count(); ii++)
            {
                PNavigatorPathPart part = Parts[ii];
                ret += part.Level + ":" + part.PositionInLevel.ToString();
                if (ii < Parts.Count() - 1)
                {
                    ret += "/";
                }
            }

            return ret;
        }
    }
	public class PNavigatorPathPart
	{
		public string Level { set; get; }
		public int PositionInLevel { set; get; }

		public PNavigatorPathPart(string level, int positionInLevel)
		{
			Level = level;
			PositionInLevel = positionInLevel;
		}
	}
}
