
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Core;

namespace LuaBinding
{
	public class LuaTextEditorCompletion : CompletionTextEditorExtension
	{
		Regex rx_is_local = new Regex( @"^\s*local\s+((([A-z_][A-z0-9_]*))(\s*,\s*([A-z_][A-z0-9_]*))*)?\s*$", RegexOptions.Compiled );
		Regex rx_locals   = new Regex( @"local\s+(([A-z_][A-z0-9_]*))(\s*,\s*([A-z_][A-z0-9_]*))*", RegexOptions.Compiled );

		public override bool CanRunCompletionCommand()
		{
			string line = this.Editor.GetLineText( this.Editor.Caret.Line );

			{ // Are we in a definition?
				string to_left = line.Substring( 0, Math.Min( this.Editor.Caret.Column, line.Length ) );
				if( rx_is_local.IsMatch( to_left ) )
					return false; // nope, we're defining a local variable
			}

			{ // Are we in a comment?
				// TODO: this
			}

			return true;
		}

		public override ICompletionDataList HandleCodeCompletion(CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{

			return base.HandleCodeCompletion(completionContext, completionChar, ref triggerWordLength);
		}

		public override ICompletionDataList CodeCompletionCommand(CodeCompletionContext completionContext)
		{
			CompletionDataList ret = new CompletionDataList();

			ret.Add( "table.insert" );
			ret.Add( "table.remove" );
			ret.Add( "print" );
			ret.Add( "type" );

			return ret;
			//return base.CodeCompletionCommand(completionContext);
		}
	}
}