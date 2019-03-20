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

		private bool ModDirectoryExists {
			get
			{
				if (Directory.Exists(this.ModDirectory))
					return true;
				return false;
			}
		}

		public bool RepoValid
		{
			get
			{
				if (this.ModDirectoryExists && Repository.IsValid(this.ModDirectory))
					return true;
				return false;
			}
		}

		public bool ContentValid
		{
			get
			{
				if (CheckModContent())
					return true;
				return false;
			}
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

			string documents = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string appData = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			Upstream = @"https://github.com/mustaphatr/Romanovs-Vengeance";
			ContentDirectory = Path.Combine(documents, @"OpenRA\Content\ra2");
			ModDirectory = Path.Combine(appData, @"Romanovs-Vengeance");
			WriteLine("Data Model Initialized");
			WriteLine("Ra2 Content Location: " + ContentDirectory);
			WriteLine("RV Build Directory " + ModDirectory);
		}

		public void DeleteModDirectory()
		{
			if (ModDirectoryExists)
				Directory.Delete(ModDirectory, true);
		}

		public bool CheckModContent()
		{
			foreach(string file in MixFiles)
			{
				var fullPath = Path.Combine(ContentDirectory, file);
				if (File.Exists(fullPath))
					WriteLine(fullPath + " Exists");
			}
			return true;
		}
		private void CloneRepo()
		{
			WriteLine("Calling New Thread for Clone");
			ProgressToggle(true);

			// Not used at the moment
			var options = new CloneOptions()
			{
				OnTransferProgress = _ => {
					WriteLine("Transferring"); return true;
				},
				OnProgress = progress => { WriteLine("Progressing"); return true; }
			};

			Repository.Clone(Upstream, ModDirectory);
			WriteLine("Clone Complete");
			ProgressToggle(false);
		}

		public bool IsPullNeeded()
		{
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
		public void UpdateMod()
		{
			// Track the number of recursive calls and timeout. 
			if (!RepoValid)
			{
				if(!thread.IsAlive){
					DeleteModDirectory();
					thread = new Thread(CloneRepo);
					thread.Start();
				}
				else
				{
					WriteLine("Update is already running...");
				}
			}
			else
			{
				WriteLine("Mod Directory Valid. Checking for Updates");
				IsPullNeeded();
			}


		}
		public bool SanityCheck()
		{
			CheckModContent();



			return true;
		}

	}
}
