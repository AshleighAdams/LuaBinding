
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
			// Keywords
			"_G\tand",
			"_G\tbreak",
			"_G\tdo",
			"_G\telse",
			"_G\telseif",
			"_G\tend",
			"_G\tfalse",
			"_G\tfor",
			"_G\tfunction",
			"_G\tgoto",
			"_G\tif",
			"_G\tin",
			"_G\tlocal",
			"_G\tnil",
			"_G\tnot",
			"_G\tor",
			"_G\trepeat",
			"_G\treturn",
			"_G\tthen",
			"_G\ttrue",
			"_G\tuntil",
			"_G\twhile",
			// Globals
			"_G\tassert\t(value [, message])",
			"_G\tcollectgarbage\t(opt [, arg])",
			"_G\tdofile\t(filename)",
			"_G\terror\t(message [, level])",
			"_G\tgetmetatable\t(object)",
			"_G\tipairs(\t(table)",
			"_G\tload\t(func [, chunkname])",
			"_G\tloadfile\t([filename])",
			"_G\tloadstring\t((string [, chunkname])",
			"_G\tnext\t(table [, index])",
			"_G\tpairs\t(table)",
			"_G\tpcall\t(func, ...)",
			"_G\tprint\t(...)",
			"_G\trawequal\t(v1, v2)",
			"_G\trawget\t(table, index)",
			"_G\trawset\t(table, index, value)",
			"_G\tselect\t(index, ...)",
			"_G\tsetmetatable\t(table, metatable)",
			"_G\ttonumber\t(value [, base])",
			"_G\ttostring\t(value)",
			"_G\ttype\t(value)",
			"_G\tunpack\t(list [, i [, j]])",
			"_G\txpcall\t(func, err)",
			"_G\t_G\t#",
			"_G\t_VERSION\t#",
			// coroutine libary
			"_G\tcoroutine\t{}",
			"coroutine\tcreate\t(func)",
			"coroutine\tresume\t(co [, val1, ...])",
			"coroutine\trunning\t()",
			"coroutine\tstatus\t(co)",
			"coroutine\twrap\t(func)",
			"coroutine\tyield\t(...)",
			// debug libary
			"_G\tdebug\t{}",
			"debug\tdebug\t()",
			"debug\tgethook\t([thread])",
			"debug\tgetinfo\t([thread,] function [, what])",
			"debug\tgetlocal\t([thread,] level, local)",
			"debug\tgetmetatable\t(object)",
			"debug\tgetregistry\t()",
			"debug\tgetupvalue\t(func, up)",
			"debug\tsethook\t([thread,] hook, mask [, count])",
			"debug\tsetlocal\t([thread,] level, local, value)",
			"debug\tsetmetatable\t(object, table)",
			"debug\tsetupvalue\t(func, up, value)",
			"debug\ttraceback\t([thread,] [message] [, level])",
			// io libary
			"_G\tio\t{}",
			"io\tclose\t([file])",
			"io\tflush\t()",
			"io\tinput\t([file])",
			"io\tlines\t([filename])",
			"io\topen\t(filename [, mode])",
			"io\toutput\t([file])",
			"io\tpopen\t(prog [, mode])",
			"io\tread\t(...)",
			"io\ttempfile\t()",
			"io\ttype\t(obj)",
			"io\twrite\t(...)",
			// math libary
			"_G\tmath\t{}",
			"math\tabs\t(x)",
			"math\tacos\t(x)",
			"math\tatan\t(x)",
			"math\tatan2\t(x, y)",
			"math\tceil\t(x)",
			"math\tcosh\t(x)",
			"math\tdeg\t(x)",
			"math\texp\t(x)",
			"math\tfloor\t(x)",
			"math\tfmod\t(x, y)",
			"math\tfrexp\t(x)",
			"math\thuge\t#",
			"math\tldexp\t(m, e)",
			"math\tlog\t(x)",
			"math\tlog10\t(x)",
			"math\tmax\t(x, ...)",
			"math\tmin\t(x, ...)",
			"math\tmodf\t(x)",
			"math\tpi\t#",
			"math\tpow\t(x, y)",
			"math\trad\t(x)",
			"math\trandom\t([m [, n]])",
			"math\trandomseed\t(x)",
			"math\tsin\t(x)",
			"math\tsinh\t(x)",
			"math\tsqrt\t(x)",
			"math\ttan\t(x)",
			"math\ttanh\t(x)",
			// os libary
			"_G\tos\t{}",
			"os\tclock\t()",
			"os\tdate\t([format [, time]])",
			"os\tdifftime\t(t2, t1)",
			"os\texecute\t([command])",
			"os\texit\t([code])",
			"os\tgetenv\t(varname)",
			"os\tremove\t(filename)",
			"os\trename\t(oldname, newname)",
			"os\tsetlocale\t(locale [, category])",
			"os\ttime\t([table])",
			"os\ttmpname\t()",
			// package libary
			"_G\tpackage\t{}",
			"_G\tmodule\tname [, ...]()",
			"_G\trequire\t(modname)",
			"package\tcpath\t#",
			"package\tloaded\t#",
			"package\tloaders\t#",
			"package\tloadlib\t(libname, funcname)",
			"package\tpath\t#",
			"package\tpreload\t#",
			"package\tseeall\t(module)",
			// string libary
			"_G\tstring\t{}",
			"string\tbyte\t(string [, from [, to]])",
			"string\tchar\t(...)",
			"string\tdump\t(function)",
			"string\tfind\t(string, pattern [, init [, plain]])",
			"string\tformat\t(string, ...)",
			"string\tgmatch\t(string, pattern)",
			"string\tgsub\t(string, pattern, repl [, n])",
			"string\tlen\t(string)",
			"string\tlower\t(string)",
			"string\tmatch\t(string, pattern [, init])",
			"string\trep\t(string, count)",
			"string\treverse\t(string)",
			"string\tsub\t(string, from [, to])",
			"string\tupper\t(string)",
			// table libary
			"_G\ttable\t{}",
			"table\tconcat\t(table [, sep [, i [, j]]])",
			"table\tinsert\t(table, [pos,] value)",
			"table\tmaxn\t(table)",
			"table\tremove\t(table [, pos])",
			"table\tsort\t(table [, comp])"
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

			{ // TODO: Are we in a string?
			}

			// in strings and stuff
			return true; // base.CanRunCompletionCommand();
		}

		public override ICompletionDataList HandleCodeCompletion(CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{
			if( !CanRunCompletionCommand() )
				return null;

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
					else if( arg == "{}" )
					{
						icon = MonoDevelop.Ide.Gui.Stock.NameSpace;
						arg = "";
					}
					else if( arg == "#" )
					{
						icon = MonoDevelop.Ide.Gui.Stock.Literal;
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