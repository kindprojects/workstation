using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Reflection;

using System.ComponentModel.Design.Serialization;

namespace ProfileCut
{
	public class RAppCommandButton : Button
	{
		public RAppCommand appCommand;
		public RAppCommandButton(RAppCommand cmd)
		{
			this.Text = cmd.Name;
			this.appCommand = cmd;
		}
	}
}
