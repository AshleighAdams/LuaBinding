
using System;
using System.Text.RegularExpressions;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.CodeCompletion;

namespace LuaBinding
{
	public class LuaTextEditorCompletion : CompletionTextEditorExtension
	{
		public override bool CanRunCompletionCommand()
		{
			return true;
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