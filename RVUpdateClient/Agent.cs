using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Reflection;

namespace RVUpdateClient
{
	class Agent
	{
		private static PowerShell powershell;

		static Action<string> WriteLine;

		public static void Initialize(Action<String> writeLine = null)
		{
			WriteLine = writeLine;
			InitialSessionState state = InitialSessionState.CreateDefault();
			SessionStateVariableEntry agentEntry = new SessionStateVariableEntry("Agent", typeof(Agent), "The Agent");
			state.Variables.Add(agentEntry);
			powershell = PowerShell.Create(state);
			WriteLine("PowerShell Agent Initialized");
		}

		public static string RunString(string text)
		{
			powershell.AddScript(text);
			string returnString = "";
			foreach (dynamic item in powershell.Invoke().ToList())
			{
				returnString += item.ToString();
			}
			return returnString;
		}

		public static void RunTest()
		{
			WriteLine("RunTest has been Run");
		}

		public static string GetFirstPowerShell()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(".ps1"));
			string result = "";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}
			return result;
		}
	}
}
