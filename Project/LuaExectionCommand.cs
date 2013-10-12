using System;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;

namespace LuaBinding
{
	public class LuaExecutionCommand : NativeExecutionCommand
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