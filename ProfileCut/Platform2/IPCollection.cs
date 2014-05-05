﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform2
{
	public interface IPCollection
	{
		IPObject GetObject(int index);
		IPObject ownerObject { get; }
		string CollectionName { get; }
		int IndexOf(IPObject o);
		int Count{get;}
	}
}