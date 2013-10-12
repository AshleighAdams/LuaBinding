using System;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Dialogs;

namespace LuaBinding
{
	[System.ComponentModel.ToolboxItem( true )]
	public partial class InterpreterOptions : Gtk.Bin
	{
		public InterpreterOptions()
		{
			this.Build();
		}

		public string LuaDefault {
			get { return InterpreterDefault.Text ?? String.Empty; }
			set { InterpreterDefault.Text = value ?? String.Empty; }
		}

		public string Lua51 {
			get { return Interpreter51.Text ?? String.Empty; }
			set { Interpreter51.Text = value ?? String.Empty; }
		}

		public string Lua52 {
			get { return Interpreter52.Text ?? String.Empty; }
			set { Interpreter52.Text = value ?? String.Empty; }
		}

		public string LuaJIT {
			get { return InterpreterJIT.Text ?? String.Empty; }
			set { InterpreterJIT.Text = value ?? String.Empty; }
		}
	}

	public class InterpreterOptionsBinding : ItemOptionsPanel
	{
		InterpreterOptions panel;

		public override Gtk.Widget CreatePanelWidget ()
		{
			panel = new InterpreterOptions ();
			panel.LuaDefault = PropertyService.Get<string> ("Lua.DefaultInterpreterPath");
			panel.Lua51 = PropertyService.Get<string> ("Lua.51InterpreterPath");
			panel.Lua52 = PropertyService.Get<string> ("Lua.52InterpreterPath");
			panel.LuaJIT = PropertyService.Get<string> ("Lua.JITInterpreterPath");
			
			return panel;
		}

		public override void ApplyChanges ()
		{
			PropertyService.Set("Lua.DefaultInterpreterPath", panel.LuaDefault);
			PropertyService.Set("Lua.51InterpreterPath", panel.Lua51);
			PropertyService.Set("Lua.52InterpreterPath", panel.Lua52);
			PropertyService.Set("Lua.JITInterpreterPath", panel.LuaJIT);
		}
	}
}

