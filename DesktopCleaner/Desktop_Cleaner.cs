using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kyeer;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace Kyeer {
	public class Kyeer_Plugin : Kyeer.Kyeer_Plugin_Interface {
		public event EventHandler OnRequestData;
		private FileSystemWatcher Watcher =	new FileSystemWatcher();
		private Dictionary<string, Thread> Filer = new Dictionary<string, Thread>();
		Object locker = new Object();


		private string TargetPath = @"D:\Document\Ego\Daily";
		private string[] DonotmoveExt = { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };

		private void AutoMove(Object obj) {
			string FullPath = (string)obj;
			if (CheckStrInStrs(System.IO.Path.GetExtension(FullPath), DonotmoveExt)) {
				if (!System.IO.Directory.Exists(FullPath))
					goto end;
			}
			if (System.IO.Path.GetFileName(FullPath)[0] == '_' 
				&& System.IO.Path.GetFileName(FullPath)[1] == '_')
				goto end;
			Thread.Sleep(10 * 1000);

			if (System.IO.File.Exists(FullPath)) {
				FileInfo i = new FileInfo(FullPath);
				if (CheckQQImage(i.Name)) {
					string path = RenameQQImage(i.FullName);
					FileInfo fi = new FileInfo(path);
					try {
						fi.MoveTo(GetDailyPath() + "\\QQImages\\" + fi.Name);
					} catch (Exception e) { }
				} else if (CheckRenamedQQImage(i.Name))
					try {
						i.MoveTo(GetDailyPath() + "\\QQImages\\" + i.Name);
					} catch (Exception e) { } else {
					string add = "";
					if (System.IO.File.Exists(i.FullName))
						add = "-2";
					int ret = 0;
					retry:
					try {
						i.MoveTo(GetDailyPath() + "\\" + System.IO.Path.GetFileNameWithoutExtension(i.Name) + add + i.Extension);
					} catch (IOException e) {
						Thread.Sleep(1000);
						if (ret++ >= 10) {
							var r = MessageBox.Show("重试超过 10 次, 是否清零?", "无法访问桌面文件", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
							if (r == DialogResult.Yes)
								ret = 0;
							else
								goto end;
						}
						goto retry;
					}
					Debug.Print("File Moved! " + i.Name);
				}
			} else {
				if (System.IO.Directory.Exists(FullPath)) {
					DirectoryInfo i = new DirectoryInfo(FullPath);
					int ret = 0;
					retry:
					try {
						i.MoveTo(GetDailyPath() + "\\" + i.Name);
					} catch (IOException e) {
						Thread.Sleep(1000);
						if (ret++ >= 10) {
							var r = MessageBox.Show("重试超过 10 次, 是否清零?", "无法访问桌面文件夹", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
							if (r == DialogResult.Yes)
								ret = 0;
							else
								goto end;
						}
						goto retry;
					}
				} else
					Debug.Print("File dosen't exist!");
			}
			end:
			lock (locker) {
				Filer.Remove(System.IO.Path.GetFileName(FullPath));
			}
			return;
		}

		private bool CheckQQImage(string name) {
			if (System.IO.Path.GetFileNameWithoutExtension(name).Length == 23)
				return true;
			return false;
		}

		private bool CheckRenamedQQImage(string name) {
			return Regex.IsMatch(System.IO.Path.GetFileNameWithoutExtension(name), @"QQ-\d{6}-\d{6}-\d{2}");
		}

		private string RenameQQImage(string path) {
			FileInfo fi = new FileInfo(path);
			fi.MoveTo(fi.DirectoryName + "\\QQ-" + fi.LastWriteTime.ToString("yyMMdd-HHmmss-ff") + fi.Extension);
			return fi.DirectoryName + "\\QQ-" + fi.LastWriteTime.ToString("yyMMdd-HHmmss-ff") + fi.Extension;
		}

		private string GetDailyPath() {
			string path = TargetPath + "\\D" + DateTime.Now.ToString("yyMMdd");
			if (!System.IO.Directory.Exists(path))
				System.IO.Directory.CreateDirectory(path);
			if (!System.IO.Directory.Exists(path + "\\QQImages"))
				System.IO.Directory.CreateDirectory(path + "\\QQImages");
			//Debug.Print(path);
			return path;
		}

		private bool CheckStrInStrs(string a, string[] b) {
			foreach (var i in b)
				if (i == a)
					return false;
			return true;
		}

		private void CleanDesktop() {
			string dp = Watcher.Path;
			DirectoryInfo di = new DirectoryInfo(dp);
			foreach (var i in di.GetFiles()) {
				if (CheckQQImage(i.Name)) {
					string path = RenameQQImage(i.FullName);
					FileInfo fi = new FileInfo(path);
					fi.MoveTo(GetDailyPath() + "\\QQImages\\" + fi.Name);
				} else if (CheckRenamedQQImage(i.Name))
					i.MoveTo(GetDailyPath() + "\\QQImages\\" + i.Name);
				else if (CheckStrInStrs(i.Extension, DonotmoveExt))
					continue;
				else if (i.Name[0] == '_' && i.Name[1] == '_')
					continue;
				else {
					i.MoveTo(GetDailyPath() + "\\" + i.Name);
				}
			}

			foreach(var i in di.GetDirectories()) {
				if (i.Name[0] == '_' && i.Name[1] == '_')
					continue;
				i.MoveTo(GetDailyPath() + "\\" + i.Name);
			}
		}

		public void Disable() {
			Watcher.EnableRaisingEvents = false;
		}

		public void Enable() {
			Watcher.EnableRaisingEvents = true;
		}

		public Kyeer_Plugin_Status GetLastestStatus() {
			return Kyeer_Plugin_Status.Normal;
		}

		public MenuItem[] GetMenuItems() {
			return new MenuItem[] {
				new MenuItem("Clean", new EventHandler((s,e) => CleanDesktop())),
				new MenuItem("Open", new EventHandler((s,e) => System.Diagnostics.Process.Start(GetDailyPath())))
			};
		}

		public void GetSharedData(ref object data) {
			MessageBox.Show("该模组(DesktopCleaner)未提供其他插件访问分享数据的权限\n可能某些恶意插件使用了非法代码绕过检查!\n请留意.", "非法请求", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			throw new Exception("No Data Can be SHARED! Perhaps a plugin has illegal code! Watch out!");
		}

		public void Initialization() {
			Watcher.Path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			Watcher.IncludeSubdirectories = false;
			Watcher.Filter = "*";
			Watcher.Created += Fsw_Created;
			Watcher.Renamed += Watcher_Renamed;
		}

		private void Watcher_Renamed(object sender, RenamedEventArgs e) {
			if (CheckRenamedQQImage(e.Name))
				return;
			Debug.Print(e.Name + ": " + e.ChangeType.ToString());
			Filer[e.OldName].Abort();
			Filer.Remove(e.OldName);
			Thread t = new Thread(new ParameterizedThreadStart(AutoMove));
			t.Start(e.FullPath);
			Filer.Add(e.Name, t);
		}

		private void Fsw_Created(object sender, FileSystemEventArgs e) {
			//throw new NotImplementedException();'
			Debug.Print(e.Name + ": " + e.ChangeType.ToString());
			Thread t = new Thread(new ParameterizedThreadStart(AutoMove));
			t.Start(e.FullPath);
			Filer.Add(e.Name, t);
		}

		public int Register(ref Kyeer_Plugin_ID ID) {
			Kyeer_Plugin_ID newID = ID;
			newID.Status = Kyeer_Plugin_Status.NotActive;
			ID = newID;
			return 1;
		}

		public void Release() {
			Watcher.Dispose();
		}
	}
}
