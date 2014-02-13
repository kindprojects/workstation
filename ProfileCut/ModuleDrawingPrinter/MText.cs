using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace ModuleDrawingPrinter
{
    public class MText : MAlignable
    {
        public MPage OwnerPage { set; get; }
        public string Text { set; get; }
        public Font Font { set; get; }
        public Brush Brush { set; get; }
        public float Angle { set; get; }

        public override SizeF MeasureObject(Graphics context)
        {
            return context.MeasureString(this.Text, this.Font);
            // Set character ranges to "First" and "Second".
            CharacterRange[] characterRanges = { new CharacterRange(0, this.Text.Length) };

            // Create rectangle for layout. 
            /*float x = 50.0F;
            float y = 50.0F;
            float width = 35.0F;
            float height = 200.0F;
            RectangleF layoutRect = new RectangleF(x, y, width, height);*/

            // Set string format.
            StringFormat stringFormat = new StringFormat();
            //stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            stringFormat.SetMeasurableCharacterRanges(characterRanges);

            // Draw string to screen.
            context.DrawString(this.Text, this.Font, Brushes.Black, this.X, this.Y, stringFormat);

            // Measure two ranges in string.
            Region[] stringRegions = context.MeasureCharacterRanges(this.Text, this.Font, new RectangleF(this.X, this.Y, this.Width, this.Height), stringFormat);

            // Draw rectangle for first measured range.
            RectangleF measureRect = stringRegions[0].GetBounds(context);
            return new SizeF(measureRect.Width, measureRect.Height);
        }

        public MText(MPage ownerPage, string text, Font font, RectangleF rectangle, int horAlign, int verAlign, float angle)
        {
            OwnerPage = ownerPage;

            Text = text;
            Font = font;
            HorAlign = horAlign;
            VerAlign = verAlign;
            Angle = angle;

            if (angle != 0)
            {
                int xxx = 0;
            }

            if (ownerPage != null)
            {
                this.X = rectangle.Left * ownerPage.Width / 100;
                this.Y = rectangle.Top * ownerPage.Height / 100;
                this.Width = rectangle.Width * ownerPage.Width / 100;
                this.Height = rectangle.Height * ownerPage.Height / 100;
            }
            else
            {
                throw new Exception("Страница не задана");
            }
                        
            Brush = Brushes.Black;
        }
    }
}
