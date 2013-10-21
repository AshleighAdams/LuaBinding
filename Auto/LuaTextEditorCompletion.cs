
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
		string[] Globals = { // TODO: Fill this in
			"_G\tprint\t(...)",
			"_G\ttype\t(value)",
			"_G\ttable\t{}",
			"table\tinsert\t(table, value)",
			"table\tremove\t(table, index)",
			// Keywords
			"_G\tand",
			"_G\tor",
			"_G\ttrue",
			"_G\tfalse",
			"_G\tnil",
			"_G\telse",
			"_G\tif",
			"_G\telseif",
			"_G\tend",
			"_G\tuntil",
			"_G\twhile",
			"_G\tdo"
		};

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
			CompletionDataList ret = new CompletionDataList();
			string fullcontext = "";
			bool has_namespace = false;

			if (completionContext.TriggerOffset > 1)
			{
				completionContext.TriggerOffset = document.Editor.Caret.Offset;
				int pos = completionContext.TriggerOffset - 1;
				
				while( pos > 1 )
				{
					char letter = document.Editor.GetCharAt( pos );

					pos--;
					if( letter == '.' || letter == ':' || letter == ',' || 
						!char.IsLetterOrDigit( letter ) )
					{
						if( letter == '.' || letter == ':' )
							has_namespace = true;
						break;

					}
				}
				
				triggerWordLength = completionContext.TriggerOffset - pos - 2;
				completionContext.TriggerWordLength = triggerWordLength;

				// now, work out the context
				bool did_space = false;
				bool did_namespace = false;

				while( pos > 1 && has_namespace )
				{
					char letter = document.Editor.GetCharAt( pos );
					
					if( char.IsWhiteSpace( letter ) )
						did_space = true;
					else
					if( did_space && !did_namespace && (letter != '.' && letter != ':') )
					{
						break;
					}
					else
					if( letter == '.' || letter == ':' )
					{
						did_namespace = true;
						fullcontext = letter + fullcontext;
					}
					else
					if( char.IsLetterOrDigit( letter ) )
					{
						did_namespace = false;
						did_space = false;
						fullcontext = letter + fullcontext;
					}
					else
						break;
					pos--;
				}
			}

			if( fullcontext.Trim() == "" )
				fullcontext = "_G";
			else
				fullcontext = fullcontext.TrimEnd(".".ToCharArray());

			if( completionChar == '(' || completionChar == '[' ||
				completionChar == ')' || completionChar == ']' ||
			    completionChar == '"' || completionChar == '\''||
				completionChar == ';' )
			{
				return null; // don't show it yet TODO: Add function args
			}

			Console.WriteLine( "trigger: {0} context = {1} hn = {2}", triggerWordLength,  fullcontext, has_namespace);

			foreach( string glob in Globals )
			{
				string[] split = glob.Split( "\t".ToCharArray() );

				if( split[ 0 ] == fullcontext )
				{
					string icon = MonoDevelop.Ide.Gui.Stock.Method;
					string arg = split.Length >= 3 ? split[ 2 ] : "";

					if( arg == "" ) // keyword
					{
						icon = "md-keyword";
						arg = "";
					}
					if( arg == "{}" )
					{
						icon = MonoDevelop.Ide.Gui.Stock.NameSpace;
						arg = "";
					}
					Console.WriteLine( split[ 1 ] );
					var item = ret.Add( split[ 1 ] + arg, icon, "", split[1] );
				}
			}

			/*
			ret.Add( "table.insert" );
			ret.Add( "table.remove" );
			ret.Add( "print" );
			ret.Add( "type" );
			*/

			return ret;
			//return base.HandleCodeCompletion(completionContext, completionChar, ref triggerWordLength);
		}

		public override ICompletionDataList CodeCompletionCommand(CodeCompletionContext completionContext)
		{
			int i = 0;
			return HandleCodeCompletion(completionContext, '\0', ref i);

			//return base.CodeCompletionCommand(completionContext);
		}
	}
}