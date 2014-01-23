using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Platform;
using System.Windows.Forms;
using System.Drawing;


namespace ProfileCut
{
    public class PProfileCut: PPlatform
    {
        private RConfig _conf;
        private Control _navigatorControl;

        public PProfileCut(IPDBLink db, string modelCode, bool defferedLoad, Control navigatorControl): 
            base(db, modelCode, defferedLoad)
        {
            _conf = new RConfig();
            _navigatorControl = navigatorControl;            
        }

        public void BuildControls()
        {
            int x = 0;
            this._navigatorControl.Controls.Clear();
            foreach (PModelObjectNavigatorPathLevel lvl in Navigator.Levels)
            {
                if (lvl.UiControlName != "")
                {
                    Control c = CreateLevelControls(lvl, x);
                    lvl.UiControl = c;
                    x += c.Width;
                }
            }
        }


        public Control CreateLevelControls(PModelObjectNavigatorPathLevel level, int x)
        {
            const int btnWidth = 40;

            Panel p = new Panel();
            p.Location = new Point(x, 0);

            Label l = new Label();
            p.Controls.Add(l);
            l.Text = level.UiControlName;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            l.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            l.AutoSize = true;
            int labWidth = l.Width;
            l.AutoSize = false;
            l.Width = labWidth;
            l.Height = this._navigatorControl.Height;
            p.Width = labWidth + 2 * btnWidth + 1;

            RNavigatorButton b = new RNavigatorButton(level, NAV_DIRECTION.UP);
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = this._navigatorControl.Height;
            b.Left = l.Width;
            b.Top = 0;
            //b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::Platform.Resource.arrow151;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            b = new RNavigatorButton(level, NAV_DIRECTION.DOWN);
            p.Controls.Add(b);
            b.Width = btnWidth;
            b.Height = this._navigatorControl.Height;
            b.Left = l.Width + btnWidth + 1;
            b.Top = 0;
            //b.Click += new System.EventHandler(this._navButtonClick);
            b.BackgroundImage = global::Platform.Resource.caret;
            b.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            this._navigatorControl.Controls.Add(p);
            p.Parent = this._navigatorControl;

            return p;
        }

        public void MasterSelectedIndexChanged(ListBox listBox) 
        {
            string path = "";
            if (listBox.SelectedItem != null)
            {
                PBaseObject obj = (listBox.SelectedItem as PMasterItem).Object;
                string text = this.Model.Templates.TransformText(_conf.DetailTemplate, obj, ref path, true);
                Navigator.Setup(path);
                BuildControls();
            }
        }
    }
}
