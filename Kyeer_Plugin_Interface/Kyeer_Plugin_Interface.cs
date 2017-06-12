using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kyeer {
	public enum Kyeer_Plugin_Initialization_Method {
		Normal = 1,
		Debug,
		Test
	};

	public enum Kyeer_Plugin_Release_Method {
		Normal = 1,
		ManualGC
	};

	public enum Kyeer_Plugin_Status {
		Normal = 1,
		Waiting,
		Operating,
		Crash,
		Initializing,
		NotActive,
		Active,
		Loaded
	};

	public struct Kyeer_Plugin_ID {
		public int Index;
		public string Key;
		public Kyeer_Plugin_Status Status;
		public bool Initialized;
		
		public List<string> Authorized_keys;

		public Kyeer_Plugin_Release_Method Release_Method;
		public Kyeer_Plugin_Initialization_Method Initialization_Method;
		public string Path;
	};

	public class RequestData_EventArgs : EventArgs {
		public bool Succees;
		public string Key;
		public object Data;
	}

	public interface Kyeer_Plugin_Interface {
		int Register(ref Kyeer_Plugin_ID ID);  //return The version of KyeerLoader
		event EventHandler OnRequestData;

		// Core Functions
		void Initialization();
		void Release();
		void Enable();
		void Disable();
		MenuItem[] GetMenuItems();

		Kyeer_Plugin_Status GetLastestStatus();
		void GetSharedData(ref Object data);
	}

}
