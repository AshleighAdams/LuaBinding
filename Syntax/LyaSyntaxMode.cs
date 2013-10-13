
using System;
using System.Collections.Generic;
using MonoDevelop.Ide;
using MonoDevelop.Core;

using Mono.TextEditor;
using Mono.TextEditor.Highlighting;

namespace LuaBinding
{
	class LuaSyntaxMode : SyntaxMode
	{
		public LuaSyntaxMode()
		{
			var provider = new ResourceXmlProvider(typeof(LuaSyntaxMode).Assembly, "LuaSyntaxMode.xml");
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
		}
	}
}

