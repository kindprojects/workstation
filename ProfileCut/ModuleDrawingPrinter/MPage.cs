using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleDrawingPrinter
{
    public class MPage
    {
        public float Dpm { set; get; }
        public float Width { set; get; }
        public float Height { set; get; }
        public int FieldLeft { set; get; } 
        public int FieldTop { set; get; } 
        public int FieldRight { set; get; }
        public int FieldBottom { set; get; }

        public List<Object> Content { set; get; }

        public MPage(float dpm, float width, float height, int fieldLeft, int fieldTop, int fieldRight, int fieldBottom)
        {
            Dpm = dpm;

            Width = (width - fieldLeft - fieldRight) * dpm * 0.351F;
            Height = (height - fieldTop - fieldBottom) * dpm * 0.351F;

            FieldLeft = Convert.ToInt32(Math.Floor(fieldLeft * dpm) * 0.351F);
            FieldTop = Convert.ToInt32(Math.Floor(fieldTop * dpm) * 0.351F);
            FieldRight = Convert.ToInt32(Math.Floor(fieldRight * dpm) * 0.351F);
            FieldBottom = Convert.ToInt32(Math.Floor(fieldBottom * dpm) * 0.351F);

            Content = new List<object>();
        }
    }
}
