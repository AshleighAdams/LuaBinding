// 
// LuaLanguageBinding.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Xml;

using MonoDevelop.Projects;
using MonoDevelop.Core;

namespace LuaBinding
{
	class LuaLanguageBinding : IDotNetLanguageBinding
	{
		public string Language {
			get {
				return "Lua";
			}
		}
		
		public string ProjectStockIcon {
			get { 
				return "md-project";
			}
		}
		
		public bool IsSourceCodeFile (FilePath fileName)
		{
			return string.Compare (Path.GetExtension (fileName), ".lua", true) == 0;
		}
		
		public BuildResult Compile (ProjectItemCollection projectItems, DotNetProjectConfiguration configuration, ConfigurationSelector configSelector, IProgressMonitor monitor)
		{
			return LuaCompilerManager.Compile (projectItems, configuration, configSelector, monitor);
		}
		
		public ConfigurationParameters CreateCompilationParameters (XmlElement projectOptions)
		{
			return new LuaCompilerParameters();
		}
	
		public ProjectParameters CreateProjectParameters (XmlElement projectOptions)
		{
			return null;
		}
		
		public string SingleLineCommentTag { get { return "--"; } }
		public string BlockCommentStartTag { get { return "--[["; } }
		public string BlockCommentEndTag { get { return "]]"; } }
		
		public System.CodeDom.Compiler.CodeDomProvider GetCodeDomProvider ()
		{
			return null;
		}
		
		public FilePath GetFileName (FilePath baseName)
		{
			return baseName + ".lua";
		}
		
		public ClrVersion[] GetSupportedClrVersions ()
		{
			return new ClrVersion[] { 
				ClrVersion.Net_1_1, 
				ClrVersion.Net_2_0,
				ClrVersion.Clr_2_1,
				ClrVersion.Net_4_0,
				ClrVersion.Net_4_5,
			};
		}
	}
}
