using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ModuleNamespace
{
    public abstract class MAlignable : MPrintable
    {
        public int HorAlign { set; get; }
        public int VerAlign { set; get; }
        public float Width { set; get; }
        public float Height { set; get; }
        /// <summary>
        /// расположение объекта относительно своей локальной системы координат
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public RectangleF CalcPositionFromSelfOrigin(Graphics context)
        {            
            RectangleF ret = new RectangleF();
            
            if (HorAlign == -1) // L
            {
                ret.X = 0;
            }
            else if (HorAlign == 0) // C
            {
                ret.X = -this.Width / 2.0F;
            }
            else // R
            {
                ret.X = -this.Width;
            }
            ret.Width = this.Width;
                        
            SizeF strSize = this.MeasureObject(context);
            if (VerAlign == -1) // L
            {
                ret.Y = 0;

            }
            else if (VerAlign == 0) // C
            {
                ret.Y = -strSize.Height / 2.0F;
            }
            else // R
            {
                ret.Y = -strSize.Height;
            }
            ret.Height = strSize.Height;

            return ret;
        }
    }
}
