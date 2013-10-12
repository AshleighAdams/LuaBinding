// 
// LuaCompilerManager.cs
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
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.Assemblies;

namespace LuaBinding
{
	static class LuaCompilerManager
	{
		static void AppendQuoted (StringBuilder sb, string option, string val)
		{
			sb.Append ('"');
			sb.Append (option);
			sb.Append (val);
			sb.Append ("\" ");
		}

		

		public static BuildResult Compile (ProjectItemCollection project_items, LuaConfiguration configuration, ConfigurationSelector config_selector, IProgressMonitor monitor)
		{
//			LuaCompilerParameters compilerParameters = (LuaCompilerParameters)configuration.CompilationParameters ?? new LuaCompilerParameters ();
			
			StringBuilder sb = new StringBuilder ();
			sb.Append("-p ");
			
			List<string> gacRoots = new List<string> ();

			if( configuration.DebugMode )
			{
				// TODO: This
			}
			
			foreach (ProjectFile finfo in project_items.GetAll<ProjectFile> ())
			{
				if (finfo.Subtype == Subtype.Directory)
					continue;

				switch (finfo.BuildAction) 
				{
					case "Compile":
						AppendQuoted (sb, "", finfo.Name);
						break;
					default:
						continue;
				}
			}
			
			string output = "";
			string error  = "";

			/*
			string luac = configuration.TargetRuntime.GetToolPath (configuration.TargetFramework, "luac");
			if (luac == null) {
				BuildResult res = new BuildResult ();
				res.AddError(GettextCatalog.GetString ("Lua compiler (luac) not found."));
				return res;
			}
			*/

			string outstr = sb.ToString();
			monitor.Log.WriteLine(outstr);

			string working_dir = ".";
			if (configuration.ParentItem != null)
			{
				working_dir = configuration.ParentItem.BaseDirectory;
				if (working_dir == null)
					working_dir = ".";
			}

			LoggingService.LogInfo ("luac " + sb.ToString ());
			
			var envVars = configuration.EnvironmentVariables;
			int exitCode = DoCompilation(outstr, working_dir, envVars, gacRoots, ref output, ref error, monitor);
			
			BuildResult result = ParseOutput(output, error);
			if (result.CompilerOutput.Trim().Length != 0)
				monitor.Log.WriteLine(result.CompilerOutput);
			
			//if compiler crashes, output entire error string
			if (result.ErrorCount == 0 && exitCode != 0) {
				if (!string.IsNullOrEmpty (error))
					result.AddError (error);
				else
					result.AddError ("The compiler appears to have crashed without any error output.");
			}
			
			FileService.DeleteFile(output);
			FileService.DeleteFile(error);
			return result;
		}
		
		static BuildResult ParseOutput (string stdout, string stderr)
		{
			BuildResult result = new BuildResult ();
			
			StringBuilder compilerOutput = new StringBuilder ();

			foreach (string s in new string[] { stdout, stderr })
			{
				StreamReader sr = File.OpenText (s);
				while (true)
				{
					string curLine = sr.ReadLine();
					compilerOutput.AppendLine(curLine);
					
					if (curLine == null) 
						break;
					
					curLine = curLine.Trim();
					if (curLine.Length == 0) 
						continue;

					BuildError error = CreateErrorFromString(curLine);
					
					if( error != null )
						result.Append( error );
				}
				sr.Close();
			}

			result.CompilerOutput = compilerOutput.ToString();
			return result;
		}
		
		static int DoCompilation (string outstr, string working_dir, Dictionary<string,string> env_vars, List<string> gac_roots, ref string output, ref string error, IProgressMonitor monitor) 
		{
			output = Path.GetTempFileName();
			error = Path.GetTempFileName();
			
			StreamWriter outwr = new StreamWriter (output);
			StreamWriter errwr = new StreamWriter (error);

			// the default should be valid, 
			string luac = PropertyService.Get<string>( "Lua.DefaultInterpreterPath" );

			if( string.IsNullOrEmpty( luac ) )
			{
				monitor.ReportError( "Can't find Lua compiler (please set the default interpreter path)", new Exception() );
				return 1;
			}

			luac += "c";

			ProcessStartInfo pinfo = new ProcessStartInfo(luac, outstr);
			pinfo.WorkingDirectory = working_dir;
			
			if (gac_roots.Count > 0) 
			{
				// Create the gac prefix string
				string gacPrefix = string.Join ("" + Path.PathSeparator, gac_roots.ToArray ());
				string oldGacVar = Environment.GetEnvironmentVariable ("MONO_GAC_PREFIX");
				if (!string.IsNullOrEmpty (oldGacVar))
					gacPrefix += Path.PathSeparator + oldGacVar;
				pinfo.EnvironmentVariables ["MONO_GAC_PREFIX"] = gacPrefix;
			}

			foreach( KeyValuePair<string, string> kv in env_vars )
				pinfo.EnvironmentVariables.Add( kv.Key, kv.Value );

			pinfo.UseShellExecute = false;
			pinfo.RedirectStandardOutput = true;
			pinfo.RedirectStandardError = true;
			
			MonoDevelop.Core.Execution.ProcessWrapper pw = Runtime.ProcessService.StartProcess(pinfo, outwr, errwr, null);
			pw.WaitForOutput();
			int exitCode = pw.ExitCode;
			outwr.Close();
			errwr.Close();
			pw.Dispose ();
			return exitCode;
		}

		static Regex regex_error = new Regex (@"\s*[\w-.]+:\s*(?<file>.+):(?<line>\d+):\s*(?<message>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static BuildError CreateErrorFromString (string error_string)
		{
			// When IncludeDebugInformation is true, prevents the debug symbols stats from breaking this.
			if (error_string.StartsWith ("Lua ", StringComparison.InvariantCulture))
				return null;
			
			Match match = regex_error.Match(error_string);
			if (!match.Success) 
				return null;
			
			BuildError error = new BuildError ();
			//error.FileName = match.Result ("${file}") ?? "";

			string path = match.Result ("${file}") ?? "";
			error.FileName = path;

			string line = match.Result ("${line}");
			error.Line = string.IsNullOrEmpty(line) ? 0 : Int32.Parse(line);


			error.IsWarning = false;
			//error.ErrorNumber = match.Result("${line}");
			error.ErrorText   = match.Result("${message}");
			return error;
		}
	}
}

