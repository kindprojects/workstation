using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform2
{
    internal class PNavigatorPart
    {
        internal string Level { set; get; }
        internal int PositionInLevel { set; get; }
        internal PNavigatorPart(string level, int positionInLevel)
        {
            Level = level;
            PositionInLevel = positionInLevel;
        }
    }

    internal class PNavigatorPath
    {
        internal List<PNavigatorPart> Parts { set; get; }

        internal PNavigatorPath()
        {
            Parts = new List<PNavigatorPart>();
        }

        //public string GetStringPath()
        //{
        //    string ret = "";

        //    for (int ii = 0; ii < Parts.Count(); ii++)
        //    {
        //        PNavigatorPart part = Parts[ii];
        //        ret += part.Level + ":" + part.PositionInLevel.ToString();
        //        if (ii < Parts.Count() - 1)
        //        {
        //            ret += "/";
        //        }
        //    }

        //    return ret;
        //}

        internal int IndexOf(string level)
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
