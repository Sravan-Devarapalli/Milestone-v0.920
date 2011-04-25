using System;

namespace PraticeManagement.Controls
{
	public class NavigatingEventArgs : EventArgs
	{
		public bool Cancel
		{
			get;
			set;
		}
	}

	public delegate void NavigatingEventHandler(object sender, NavigatingEventArgs e);
}

