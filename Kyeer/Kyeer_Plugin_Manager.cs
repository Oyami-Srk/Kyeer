using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kyeer {
	class Kyeer_Plugin_Manager {
		Kyeer_Plugin_Initialization_Method Init_Method = Kyeer_Plugin_Initialization_Method.Normal;
		Kyeer_Plugin_Release_Method Release_Method = Kyeer_Plugin_Release_Method.Normal;
		int Version = 1;

		private Dictionary<string, Kyeer_Plugin_ID> IDs = new Dictionary<string, Kyeer_Plugin_ID>();
		private Dictionary<string, Kyeer_Plugin_Interface> Objs = new Dictionary<string, Kyeer_Plugin_Interface>();

		private void OnRequestEvent(Object sender, EventArgs e) {
			sender = (Kyeer_Plugin_Interface)sender;
			RequestData_EventArgs args = (RequestData_EventArgs)e;
			args.Succees = false;
			if (!CheckKey(args.Key))
				return;
			foreach(var i in Objs)
				if(i.Value == sender)
					foreach(var j in IDs[args.Key].Authorized_keys)
						if(j == i.Key) {
							args.Succees = true;
							Objs[args.Key].GetSharedData(ref args.Data);
						}
		}

		private bool CheckKey(string Name) {
			foreach (string i in IDs.Keys)
				if (i == Name)
					return true;
			return false;
		}

		private bool CheckVer(int version) {
			if (version != Version)
				return false;
			return true;
		}

		public void LoadPlugin(string Name) {
			string Plugin_Path = System.Environment.CurrentDirectory + "\\Plugins\\" + Name + "\\";
			if (!System.IO.File.Exists(Plugin_Path + Name + ".dll")) {
				MessageBox.Show("插件 " + Name + " 的核心文件不存在!", "插件错误", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			try {
				Kyeer.Kyeer_Plugin_Interface obj = (Kyeer.Kyeer_Plugin_Interface)Assembly.LoadFile(Plugin_Path + Name + ".dll").CreateInstance("Kyeer.Kyeer_Plugin");
				obj.OnRequestData += OnRequestEvent;
				//Register ID
				Kyeer_Plugin_ID ID;
				ID.Index = IDs.Count;
				ID.Key = Name;
				ID.Path = Plugin_Path;
				ID.Authorized_keys = new List<string>();
				ID.Status = Kyeer_Plugin_Status.Loaded;
				ID.Initialized = false;
				ID.Initialization_Method = Init_Method;
				ID.Release_Method = Release_Method;

				if (!CheckVer(obj.Register(ref ID))) {
					MessageBox.Show("插件 " + Name + " 和当前版本的Kyeer主程序不兼容", "插件错误", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				IDs.Add(Name, ID);
				Objs.Add(Name, obj);
			} catch (Exception e) {
				MessageBox.Show("加载插件 " + Name + " 时发生致命错误: " + e.Message, "插件错误", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public void EnablePlugin(string Name) {
			if (!CheckKey(Name))
				return;
			if (!IDs[Name].Initialized)
				Objs[Name].Initialization();
			System.Diagnostics.Debug.Print(IDs[Name].Status.ToString());
			if (IDs[Name].Status == Kyeer_Plugin_Status.NotActive) {
				Objs[Name].Enable();
				Kyeer_Plugin_ID newID = IDs[Name];
				newID.Status = Kyeer_Plugin_Status.Active;
				IDs[Name] = newID;
			}
		}

		public void DisablePlugin(string Name) {
			if (!CheckKey(Name))
				return;
			if (!IDs[Name].Initialized)
				return;
			if (IDs[Name].Status != Kyeer_Plugin_Status.NotActive) {
				Objs[Name].Disable();
				Kyeer_Plugin_ID newID = IDs[Name];
				newID.Status = Kyeer_Plugin_Status.NotActive;
				IDs[Name] = newID;
			}
		}

		public void ReleasePlugin(string Name) {
			if (!CheckKey(Name))
				return;
			DisablePlugin(Name);
			Objs[Name].Release();
			IDs.Remove(Name);
			Objs.Remove(Name);
		}

		public System.Windows.Forms.MenuItem[] GetPluginMenuItems(string Name) {
			if (!CheckKey(Name))
				return null;
			return Objs[Name].GetMenuItems();
		}
	}
}
