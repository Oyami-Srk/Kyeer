using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileWatcherTest {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		FileSystemWatcher fsw = new FileSystemWatcher();

		private void Form1_Load(object sender, EventArgs e) {
			fsw.Path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			fsw.IncludeSubdirectories = true;
			fsw.Filter = "*";
			fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastWrite;
			fsw.Created += Fsw_Created;

			fsw.EnableRaisingEvents = true;
		}

		private void Fsw_Created(object sender, FileSystemEventArgs e) {
			if (this.textBox1.InvokeRequired){
				this.textBox1.Invoke(new setLogText(set), new object[] { e }); 
			}
		}

		delegate void setLogText(FileSystemEventArgs e);
		private void set(FileSystemEventArgs e){  //更新UI界面  
			textBox1.Text += e.Name + ": " + e.ChangeType.ToString() + "\n";
		}
	}
}
