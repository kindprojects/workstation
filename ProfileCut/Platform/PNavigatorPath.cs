using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform
{
    public class PNavigatorPartPath
    {
        public string Level { set; get; }
        public int PositionInLevel { set; get; }
        public PNavigatorPartPath(string level, int positionInLevel)
        {
            Level = level;
            PositionInLevel = positionInLevel;
        }
    }

    public class PNavigatorPath
    {
        public List<PNavigatorPartPath> Parts { set; get; }

        public PNavigatorPath()
        {
            Parts = new List<PNavigatorPartPath>();
        }

        public string GetStringPath()
        {
            string ret = "";

            for (int ii = 0; ii < Parts.Count(); ii++)
            {
                PNavigatorPartPath part = Parts[ii];
                ret += part.Level + ":" + part.PositionInLevel.ToString();
                if (ii < Parts.Count() - 1)
                {
                    ret += "/";
                }
            }

            return ret;
        }

        public int IndexOf(string level)
        {
            int ret = -1;

            for (int ii = 0; ii < Parts.Count(); ii++)
            {
                if (Parts[ii].Level.ToLower() == level.ToLower())
                {
                    ret = ii;
                    break;
                }
            }

            return ret;
        }
    }
}
