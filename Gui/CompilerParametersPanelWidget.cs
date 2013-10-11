// 
// CompilerParametersPanelWidget.cs
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
using MonoDevelop.Core;
using MonoDevelop.Projects;
using Gtk;

namespace LuaBinding
{
	[System.ComponentModel.ToolboxItem( true )]
	partial class CompilerParametersPanelWidget : Gtk.Bin
	{
		ListStore _VersionsStore;

		public CompilerParametersPanelWidget()
		{
			this.Build();

			_VersionsStore = new ListStore( typeof(string), typeof(LangVersion) );
			_VersionsStore.AppendValues( "Default", LangVersion.Lua );
			_VersionsStore.AppendValues( "Lua 5.1", LangVersion.Lua51 );
			_VersionsStore.AppendValues( "Lua 5.2", LangVersion.Lua52 );
			_VersionsStore.AppendValues( "LuaJIT", LangVersion.LuaJIT );
			LanguageVersion.Model = _VersionsStore;
		}

		public string DefaultFile {
			get { return MainFileEntry.Text ?? String.Empty; }
			set { MainFileEntry.Text = value ?? String.Empty; }
		}

		public LangVersion LangVersion {
			get
			{
				switch( LanguageVersion.Active )
				{
				case 0:
					return LangVersion.Lua;
				case 1:
					return LangVersion.Lua51;
				case 2:
					return LangVersion.Lua52;
				case 3:
					return LangVersion.LuaJIT;
				}

				return LangVersion.Lua; // fallback
			}
			set
			{
				switch( value )
				{
				case LangVersion.Lua:
					LanguageVersion.Active = 0;
					break;
				case LangVersion.Lua51:
					LanguageVersion.Active = 1;
					break;
				case LangVersion.Lua52:
					LanguageVersion.Active = 2;
					break;
				case LangVersion.LuaJIT:
					LanguageVersion.Active = 3;
					break;
				}
			}
		}

		DotNetProject project;
		DotNetProjectConfiguration configuration;

		public void Load( DotNetProject project, DotNetProjectConfiguration configuration )
		{
			this.project = project;
			this.configuration = configuration;
		}

		public void Store()
		{
			project.CompileTarget = CompileTarget.Exe;
			configuration.DebugMode = false;
		}
	}

	class CompilerParametersPanel : MonoDevelop.Ide.Gui.Dialogs.MultiConfigItemOptionsPanel
	{
		CompilerParametersPanelWidget widget;

		public override Widget CreatePanelWidget()
		{
			return widget = new CompilerParametersPanelWidget();
		}

		public override void LoadConfigData()
		{
			var config = CurrentConfiguration as LuaConfiguration;

			widget.DefaultFile = config.MainFile;
			widget.LangVersion = config.LangVersion;
		}

		public override void ApplyChanges()
		{
			var config = CurrentConfiguration as LuaConfiguration;

			config.MainFile = widget.DefaultFile;
			config.LangVersion = widget.LangVersion;
		}
	}
}
