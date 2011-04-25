using System;

namespace PraticeManagement.Controls
{
	/// <summary>
	/// Provides an exception for the VisibilitySwitch class.
	/// </summary>
	public class ControlNotFoundException : Exception
	{
		private const string MessageFormat = "The control {0} not fount in the current context.";

		public ControlNotFoundException(string controlId)
			: base(string.Format(MessageFormat, controlId))
		{
		}
	}
}

