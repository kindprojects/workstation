using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProfileCut
{
    public class RNavigatorPartPath
    {
        public string Level { set; get; }
        public int PositionInLevel { set; get; }

        public RNavigatorPartPath(string level, int positionInLevel)
        {
            Level = level;
            PositionInLevel = positionInLevel;
        }
    }

    public class RNavigatorPath
    {
        public List<RNavigatorPartPath> Parts { set; get; }

        public RNavigatorPath()
        {
            Parts = new List<RNavigatorPartPath>();         
        }

        public string GetStringPath() 
        {
            string ret = "";

            for (int ii = 0; ii < Parts.Count(); ii++)
            {
                RNavigatorPartPath part = Parts[ii];
                ret += part.Level + ":" + part.PositionInLevel.ToString();
                if (ii < Parts.Count() - 1)
                {
                    ret += "/";
                }
            }

            return ret;
        }
    }
}
