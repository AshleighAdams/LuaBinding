using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

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
		
		public override bool IsCompileable(string file_name)
		{
			return file_name.ToLower().EndsWith( ".lua" );
		}

		protected override bool OnGetCanExecute( ExecutionContext context, ConfigurationSelector configuration )
		{
			// TODO: Check interpreter paths from here...
			return true;
		}

		protected override BuildResult DoBuild(IProgressMonitor monitor, ConfigurationSelector configuration)
		{
			return LuaCompilerManager.Compile(this.Items, this.DefaultConfiguration as LuaConfiguration, configuration, monitor);
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
				string param = string.Format("\"{0}\" {1}", config.MainFile, config.CommandLineParameters);

				IProcessAsyncOperation op = 
				    Runtime.ProcessService.StartConsoleProcess(GetLuaPath( config.LangVersion ),
				                                               param, BaseDirectory,
					                                           config.EnvironmentVariables, console, null);

				aggregatedMonitor.AddOperation( op );
				op.WaitForCompleted();
				monitor.Log.WriteLine( "The application exited with code: " + op.ExitCode );
				/*
				var executionCommand = //CreateExecutionCommand( configuration, config );
					new NativeExecutionCommand( GetLuaPath( config.LangVersion ), 
					                           config.CommandLineParameters, 
					                           BaseDirectory );


				if( !context.ExecutionHandler.CanExecute( executionCommand ) )
				{
					monitor.ReportError( GettextCatalog.GetString( "Cannot execute application. The selected execution mode " +
					"is not supported for Lua projects" ), null );
					return;
				}

				IProcessAsyncOperation asyncOp = context.ExecutionHandler.Execute( executionCommand, console );
				aggregatedMonitor.AddOperation( asyncOp );
				asyncOp.WaitForCompleted();

				monitor.Log.WriteLine( "The application exited with code: " + asyncOp.ExitCode );
				*/
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
				FilePath LuaPath = GetLuaPath( config.LangVersion );

				if( string.IsNullOrWhiteSpace( LuaPath ) )
					return false;
			}

			if( String.IsNullOrEmpty( config.MainFile ) )
			{
				MessageService.ShowError( "Main file not set", "Main file has not been set." );
				return false;
			}

			if( !File.Exists(BaseDirectory + "/" + config.MainFile) )
			{
				MessageService.ShowError( "Main file is missing", string.Format("The file `{0}` does not exist!", config.MainFile) );
				return false;
			}

			return true;
		}

		static FilePath GetLuaPath(LangVersion ver)
		{
			switch( ver )
			{
			case LangVersion.Lua:
				return (FilePath)PropertyService.Get<string>( "Lua.DefaultInterpreterPath" );
			case LangVersion.Lua51:
				return (FilePath)PropertyService.Get<string>( "Lua.51InterpreterPath" );
			case LangVersion.Lua52:
				return (FilePath)PropertyService.Get<string>( "Lua.52InterpreterPath" );
			case LangVersion.LuaJIT:
				return (FilePath)PropertyService.Get<string>( "Lua.JITInterpreterPath" );
			}

			return null;
		}

		protected virtual LuaExecutionCommand CreateExecutionCommand( ConfigurationSelector config_sel, LuaConfiguration configuration )
		{
			LangVersion vers = configuration.LangVersion;
			FilePath LuaPath = GetLuaPath( vers );

			string arguments = "\"" + configuration.MainFile + "\" " + configuration.CommandLineParameters;
			
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
