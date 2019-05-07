using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

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

        public static void RunStringAsync(string text)
        {
            // powershell.AddScript(text);
            // Fill me out
            return;
        }

		public static void RunTest()
		{
			var output = RunString("Get-Date");
			WriteLine(output);
		}
	}
}
