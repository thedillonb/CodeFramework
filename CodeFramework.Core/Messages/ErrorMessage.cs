using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace CodeFramework.Core.Messages
{
	public class ErrorMessage : MvxMessage
	{
		public ErrorMessage(object sender) : base(sender) {}
		public Exception Error;
	}
}

