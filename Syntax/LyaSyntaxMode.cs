
using System;
using System.Collections.Generic;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Core;

using Mono.TextEditor;
using Mono.TextEditor.Highlighting;

namespace LuaBinding
{
	class LuaSyntaxMode : SyntaxMode
	{
		string GetSyntaxMode()
		{
			if( this.Document == null )
				return "LuaSyntaxMode.xml";
			
			LuaProject project = IdeApp.Workspace.GetProjectContainingFile( this.Document.FileName ) as LuaProject;

			if( project != null )
			{
				LuaConfiguration config = project.DefaultConfiguration as LuaConfiguration;

				Console.WriteLine( "Using {0} Lua highlighting", config.LangVersion );
				switch( config.LangVersion )
				{
				case LangVersion.Lua: // TODO: Make these use their own, maybe
				case LangVersion.Lua52:
				case LangVersion.Lua51:
				case LangVersion.LuaJIT:
					return "LuaSyntaxMode.xml";
				case LangVersion.GarrysMod:
					return "GarrysModLuaSyntaxMode.xml";
				}
			}

			return "LuaSyntaxMode.xml";
		}

		public LuaSyntaxMode()
		{
			this.DocumentSet += delegate
			{
				if(this.Document == null)
					return;

				this.Document.FileNameChanged += delegate
				{
					var provider = new ResourceStreamProvider(typeof(LuaSyntaxMode).Assembly, GetSyntaxMode());

					var reader = provider.Open();
					var basemode = SyntaxMode.Read( reader );

					this.rules = new List<Rule>( basemode.Rules );
					this.keywords = new List<Keywords>( basemode.Keywords );
					this.spans = basemode.Spans;
					this.matches = basemode.Matches;
					this.prevMarker = basemode.PrevMarker;
					this.SemanticRules = basemode.SemanticRules;
					this.keywordTable = basemode.keywordTable;
					this.properties = basemode.Properties;
				};
			};
		}


	}
}

