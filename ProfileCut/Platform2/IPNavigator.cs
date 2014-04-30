﻿using System;
using System.Collections.Generic;

namespace Platform2
{
	public enum NAV_DIRECTION { UP, DOWN, STAY };

	public interface IPNavigator
	{
		IPObject Navigate(int depth, NAV_DIRECTION dir);

		IPObject Navigate(string path);

		IPObject GetObjectAtPathLevel(int depth);
		IPObject Pointer { set; get; }
	}
}