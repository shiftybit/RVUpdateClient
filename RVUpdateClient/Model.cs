using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using LibGit2Sharp;

namespace RVUpdateClient
{
	public class Model
	{
		static Action<string> WriteLine;
		static Action<bool> ProgressToggle;
		public string ModDirectory { get; private set; }
		public string ContentDirectory { get; private set; }
		public string Upstream { get; private set; }
		public ArrayList MixFiles;
		private Thread thread;

		private bool ModDirectoryExists
		{
			get	{ return (Directory.Exists(this.ModDirectory));	}
		}

		public bool RepoValid
		{
			get	{return (this.ModDirectoryExists && Repository.IsValid(this.ModDirectory));	}
		}

		public bool ContentValid
		{
			get { return CheckModContent(); }
		}

		public Model(Action<string> writeLine, Action<bool> progressToggle)
		{
			WriteLine = writeLine;
			ProgressToggle = progressToggle;

			MixFiles = new ArrayList
			{
				"langmd.mix",
				"language.mix",
				"ra2.mix",
				"ra2md.mix",
				"theme.mix",
				"thememd.mix"
			};

			//string documents = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string documents = @"D:\Documents\";
			//string appData = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string appData = @"D:\AppData\";
			Upstream = @"https://github.com/mustaphatr/Romanovs-Vengeance";
			ContentDirectory = Path.Combine(documents, @"OpenRA\Content\ra2");
			ModDirectory = Path.Combine(appData, @"Romanovs-Vengeance");
			WriteLine("Data Model Initialized");
			WriteLine("Ra2 Content Location: " + ContentDirectory);
			WriteLine("RV Build Directory " + ModDirectory);
		}

		/// <summary>
		/// Directory.Delete will fail if there are any files marked as read only.
		/// </summary>
		void setAttributesNormal(DirectoryInfo dir)
		{
			foreach (var subDir in dir.GetDirectories())
				setAttributesNormal(subDir);
			foreach (var file in dir.GetFiles())
				file.Attributes = FileAttributes.Normal;
		}

		public void DeleteModDirectory()
		{
			System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(ModDirectory);
			if (ModDirectoryExists)
				setAttributesNormal(dir);
				Directory.Delete(ModDirectory, true);
		}

		public bool CheckModContent()
		{
			foreach(string file in MixFiles)
			{
				var fullPath = Path.Combine(ContentDirectory, file);
				if (File.Exists(fullPath))
					WriteLine(fullPath + " Exists");
				else
				{
					WriteLine("Error: " + fullPath + " Does Not Exist");
					return false;
				}
			}
			return true;
		}

		private void CloneRepo()
		{
			WriteLine("Initializing Clone of Repository");
			ProgressToggle(true);

			Repository.Clone(Upstream, ModDirectory);
			WriteLine("Clone Complete");
			ProgressToggle(false);
		}

		public bool IsPullNeeded()
		{
			// Todo: this currently isn't working. behind never populates with data. 
			// Skipping this so we can get the other parts working
			using (var repo = new Repository(ModDirectory))
			{
				string logMessage = "";
				var remote = repo.Network.Remotes["origin"];
				var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
				Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);
				var upstream = repo.Branches.Where(b => b.FriendlyName == "origin/master").FirstOrDefault();
				var behind = upstream.TrackingDetails.BehindBy;
				WriteLine("You are behind by " + behind + " updates");
			}
			return false;
		}
		public void BuildMod()
		{
			WriteLine("Initializing Build.");
			Agent.RunTest();
			WriteLine("Current Directory: " + Agent.RunString("pwd"));
		}
		public void UpdateMod()
		{
			/// Todo: Check if the repo is in valid state. Perform Git pull. 
			IsPullNeeded();  
			//DeleteModDirectory();
			//CloneRepo();
			//BuildMod();
		}

		public bool SanityCheck()
		{
			CheckModContent();
			return true;
		}

	}
}
