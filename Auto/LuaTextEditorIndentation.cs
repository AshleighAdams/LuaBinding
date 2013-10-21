
using System;
using System.Text.RegularExpressions;
using MonoDevelop.Ide.Gui.Content;

namespace LuaBinding
{
	public class LuaTextEditorIndentation : TextEditorExtension
	{
		/*
		Regex function_reg = new Regex(@"function(\s+.+\s*|\s*)\(.*\)", RegexOptions.Compiled);
		public override bool KeyPress (Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{
			if( keyChar == '}' )
			{
				var ret = base.KeyPress(key, keyChar, modifier);
				base.KeyPress(Gdk.Key.Tab, '\t', Gdk.ModifierType.ShiftMask);
				return ret;
			}
			else if (key == Gdk.Key.Return) 
			{
				string lastline = Editor.GetLineText(Editor.Caret.Line);
				string trimmedline = lastline.Trim();
				bool indent = false;

				if (trimmedline.EndsWith("{") ) // || trimmedline.EndsWith("then") ||
					trimmedline.EndsWith("do") || function_reg.IsMatch(trimmedline) )
				{
					indent = true;
				}

				if (indent)
				{
					base.KeyPress(key, keyChar, modifier);
					Editor.InsertAtCaret("\t");
					return false;
				}
			}

			return base.KeyPress (key, keyChar, modifier);
		}
		*/
	}
}