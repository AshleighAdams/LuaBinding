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
		
		public bool IsSourceCodeFile (FilePath file_name)
		{
			return string.Compare (Path.GetExtension (file_name), ".lua", true) == 0;
		}
		
		public BuildResult Compile (ProjectItemCollection project_items, DotNetProjectConfiguration configuration, ConfigurationSelector config_selector, IProgressMonitor monitor)
		{
			return LuaCompilerManager.Compile (project_items, configuration, config_selector, monitor);
		}
		
		public ConfigurationParameters CreateCompilationParameters (XmlElement project_options)
		{
			return new LuaCompilerParameters();
		}
	
		public ProjectParameters CreateProjectParameters (XmlElement project_options)
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
		
		public FilePath GetFileName (FilePath base_name)
		{
			return base_name + ".lua";
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
