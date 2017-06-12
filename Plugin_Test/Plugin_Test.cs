using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kyeer;

namespace Kyeer {
	public class Kyeer_Plugin : Kyeer.Kyeer_Plugin_Interface {
		public event EventHandler OnRequestData;

		public void Disable() {
			MessageBox.Show("Plugin Disabled!");
		}

		public void Enable() {
			MessageBox.Show("Plugin Enabled!");
		}

		public Kyeer_Plugin_Status GetLastestStatus() {
			return Kyeer_Plugin_Status.Normal;
		}

		public MenuItem[] GetMenuItems() {
			return new MenuItem[] {
				new MenuItem("Hi!!", new EventHandler((s,e) => MessageBox.Show("Hi~~")))
			};
		}
		
		public void GetSharedData(ref object data) {
			data = "Nothing Will Be Shared";
		}

		public void Initialization() {
			
		}
		
		public int Register(ref Kyeer_Plugin_ID ID) {
			Kyeer_Plugin_ID newID = ID;
			newID.Authorized_keys.Add("Plugin_SharedTest");
			newID.Initialized = true;
			newID.Status = Kyeer_Plugin_Status.NotActive;
			ID = newID;
			return 1;
		}

		public void Release() {
			MessageBox.Show("Plugin Unloaded!");
		}
	}
}
