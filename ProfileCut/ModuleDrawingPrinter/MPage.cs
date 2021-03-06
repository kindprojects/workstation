﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleNamespace
{
    public class MPage
    {
        public float Width { set; get; }
        public float Height { set; get; }
        public int FieldLeft { set; get; } 
        public int FieldTop { set; get; } 
        public int FieldRight { set; get; }
        public int FieldBottom { set; get; }
        public float OriginY { set; get; }
        public List<Object> Content { set; get; }

        public MPage(float width, float height, int fieldLeft, int fieldTop, int fieldRight, int fieldBottom, float originY)
        {
            Width = (width - fieldLeft - fieldRight);
            Height = (height - fieldTop - fieldBottom);

            FieldLeft = fieldLeft;
            FieldTop = fieldTop;
            FieldRight = fieldRight;
            FieldBottom = fieldBottom;
            OriginY = originY;

            Content = new List<object>();
        }
    }
}
