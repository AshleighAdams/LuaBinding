using System;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;

namespace LuaBinding
{
	public class LuaExecutionCommand : DotNetExecutionCommand
	{
		public LuaExecutionCommand (string command) : base (command)
		{
		}

		public LuaConfiguration Configuration {
			get;
			set;
		}
	}
}