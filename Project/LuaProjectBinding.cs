
using System;
using System.IO;
using System.Xml;

using MonoDevelop.Projects;

namespace LuaBinding
{
	public class LuaProjectBinding : IProjectBinding
	{
		public string Name {
			get { return "Lua"; }
		}

		public Project CreateProject (ProjectCreateInformation info, XmlElement project_options)
		{
			return new LuaProject(Name, info, project_options);
		}

		public Project CreateSingleFileProject (string source_file)
		{
			return LuaProject.FromSingleFile(Name, source_file);
		}

		public bool CanCreateSingleFileProject (string source_file)
		{
			return Path.GetExtension(source_file) == "lua";
		}
	}
}