using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using Model;

namespace ProfileCut
{   
    class RHardwareCommands
    {
        private RBaseObject _object;

        public void Setup(Control controlsOwner, RConfigHardwareCommands commands)
        {
            _buildControls(controlsOwner, commands);
        }

        private void _buildControls(Control owner, RConfigHardwareCommands commands)
        {            
            int left = 0;
            foreach (RConfigHardwareCommand command in commands.items)
            {
                left = left + _createLevelControls(owner, command, left);
            }
        }

        private int _createLevelControls(Control owner, RConfigHardwareCommand command, int left)
        {
            //const int btnWidth = 40;
            
            Button button = new Button();
            button.Text = command.Text;
            button.AutoSize = true;
            button.Height = owner.Height;
            button.Left = left;
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            
            owner.Controls.Add(button);

            return button.Width + 1;
        }

        public void SetObject(RBaseObject  obj)
        {
            _object = obj;            
        }
    }
}
