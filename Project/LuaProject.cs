using System;
using System.IO;
using System.Xml;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace LuaBinding
{
	public class LuaProject : Project
	{
		const string ProjectTypeName = "Lua";

		public LuaProject()
		{
		}

		public LuaProject( string language_name, ProjectCreateInformation info, XmlElement project_options )
		{
			if( !String.Equals( language_name, ProjectTypeName ) )
				throw new ArgumentException( "Not a Lua project: " + language_name );

			if( info != null )
			{
				Name = info.ProjectName;
			}

			CreateDefaultConfigurations();
		}

		public override string ProjectType {
			get { return ProjectTypeName; }
		}

		public static LuaProject FromSingleFile( string language_name, string file_name )
		{
			var projectInfo = new ProjectCreateInformation() {
				ProjectName = Path.GetFileNameWithoutExtension( file_name ),
				SolutionPath = Path.GetDirectoryName( file_name ),
				ProjectBasePath = Path.GetDirectoryName( file_name )
			};

			var project = new LuaProject( language_name, projectInfo, null );
			project.AddFile( new ProjectFile( file_name ) );
			return project;
		}

		public override SolutionItemConfiguration CreateConfiguration( string name )
		{
			return new LuaConfiguration( name );
		}

		protected override bool OnGetCanExecute( ExecutionContext context, ConfigurationSelector configuration )
		{
			return true;
		}

		protected override void DoExecute( IProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configuration )
		{
			if( !CheckCanExecute( configuration ) )
				return;

			var config = (LuaConfiguration)GetConfiguration( configuration );
			IConsole console = config.ExternalConsole ?
				context.ExternalConsoleFactory.CreateConsole( !config.PauseConsoleOutput ) :
					context.ConsoleFactory.CreateConsole( !config.PauseConsoleOutput );

			var aggregatedMonitor = new AggregatedOperationMonitor( monitor );

			try
			{
				var executionCommand = CreateExecutionCommand( configuration, config );
				if( !context.ExecutionHandler.CanExecute( executionCommand ) )
				{
					monitor.ReportError( GettextCatalog.GetString( "Cannot execute application. The selected execution mode " +
					"is not supported for Lua projects" ), null );
					return;
				}

				var asyncOp = context.ExecutionHandler.Execute( executionCommand, console );
				aggregatedMonitor.AddOperation( asyncOp );
				asyncOp.WaitForCompleted();

				monitor.Log.WriteLine( "The application exited with code: " + asyncOp.ExitCode );

			}
			catch( Exception exc )
			{
				monitor.ReportError( GettextCatalog.GetString( "Cannot execute \"{0}\"", config.MainFile ), exc );
			}
			finally
			{
				console.Dispose();
				aggregatedMonitor.Dispose();
			}
		}

		bool CheckCanExecute( ConfigurationSelector configuration )
		{
			/*
			if( !IronManager.IsInterpreterPathValid() )
			{
				MessageService.ShowError( "Interpreter not set", "A valid interpreter has not been set." );
				return false;
			}
			*/

			var config = (LuaConfiguration)GetConfiguration( configuration );

			{ // check that the interpreter is set
				FilePath lua = (FilePath)PropertyService.Get<string>( "Lua.DefaultInterpreterPath" );
				FilePath lua51 = (FilePath)PropertyService.Get<string>( "Lua.51InterpreterPath" );
				FilePath lua52 = (FilePath)PropertyService.Get<string>( "Lua.52InterpreterPath" );
				FilePath luajit = (FilePath)PropertyService.Get<string>( "Lua.JITInterpreterPath" );

				FilePath LuaPath;

				switch( config.LangVersion )
				{
				case LangVersion.Lua:
					LuaPath = lua;
					break;
				case LangVersion.Lua51:
					LuaPath = lua51;
					break;
				case LangVersion.Lua52:
					LuaPath = lua52;
					break;
				case LangVersion.LuaJIT:
					LuaPath = luajit;
					break;
				}

				if( string.IsNullOrWhiteSpace( LuaPath ) )
					return false;
			}

			if( String.IsNullOrEmpty( config.MainFile ) )
			{
				MessageService.ShowError( "Main file not set", "Main file has not been set." );
				return false;
			}

			if( !IsFileInProject( config.MainFile ) )
			{
				MonoDevelop.Ide.MessageService.ShowError( "Main file is missing", string.Format("The file `{0}` is not in the project!", config.MainFile) );
				return false;
			}

			return true;
		}

		protected virtual LuaExecutionCommand CreateExecutionCommand( ConfigurationSelector config_sel, LuaConfiguration configuration )
		{
			LangVersion vers = configuration.LangVersion;

			FilePath lua = (FilePath)PropertyService.Get<string>( "Lua.DefaultInterpreterPath" );
			FilePath lua51 = (FilePath)PropertyService.Get<string>( "Lua.51InterpreterPath" );
			FilePath lua52 = (FilePath)PropertyService.Get<string>( "Lua.52InterpreterPath" );
			FilePath luajit = (FilePath)PropertyService.Get<string>( "Lua.JITInterpreterPath" );

			FilePath LuaPath;

			switch( vers )
			{
			case LangVersion.Lua:
				LuaPath = lua;
				break;
			case LangVersion.Lua51:
				LuaPath = lua51;
				break;
			case LangVersion.Lua52:
				LuaPath = lua52;
				break;
			case LangVersion.LuaJIT:
				LuaPath = luajit;
				break;
			}
			
			string arguments = configuration.MainFile + " " + configuration.InterpreterArguments;
			
			var command = new LuaExecutionCommand( LuaPath ) {
				Arguments = arguments,
				WorkingDirectory = BaseDirectory,
				EnvironmentVariables = configuration.GetParsedEnvironmentVariables(),
				Configuration = configuration
			};

			return command;
		}

		void CreateDefaultConfigurations()
		{
			var releaseconfig = CreateConfiguration( "Release" ) as LuaConfiguration;
			Configurations.Add( releaseconfig );
		}
	}
}
