using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Reflection;

using System.ComponentModel.Design.Serialization;
using Platform2;

namespace ProfileCut
{
	public class RAppCommandButton : Button
	{
        public RAppCommand AppCommand { set; get; }
        public List<int> SelectedObjects { set; get; }
		
        public RAppCommandButton(RAppCommand cmd)
		{
			this.Text = cmd.Name;
			this.AppCommand = cmd;

            SelectedObjects = new List<int>();
		}
	}
}
