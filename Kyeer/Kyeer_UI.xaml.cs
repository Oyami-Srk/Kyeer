using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using
using System.Windows.Forms;

namespace Kyeer {
	public partial class Kyeer_UI : Window {
		Kyeer_Plugin_Manager Plugin_Manager = new Kyeer_Plugin_Manager();
		NotifyIcon notify = new NotifyIcon();
		bool AutoLoad = true;
		bool AutoEnable = true;

		private void LoadPlugin(Object s, EventArgs e) {
			foreach (var i in System.IO.Directory.GetDirectories(System.Environment.CurrentDirectory + "\\Plugins\\")) {
				foreach (var j in notify.ContextMenu.MenuItems[1].MenuItems)
					if (System.IO.Path.GetFileName(i) == j.ToString())
						break;

				notify.ContextMenu.MenuItems[1].MenuItems.Add(System.IO.Path.GetFileName(i), new MenuItem[]{
							new MenuItem("Load", new EventHandler((a,b) => {
								Plugin_Manager.LoadPlugin(System.IO.Path.GetFileName(i));
								foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
									if (System.IO.Path.GetFileName(i) == j.Text) {
										j.MenuItems[1].Enabled = true;
										j.MenuItems[2].Enabled = true;
										j.MenuItems[3].Enabled = false;
										j.MenuItems[0].Enabled = false;
										foreach(MenuItem z in Plugin_Manager.GetPluginMenuItems(System.IO.Path.GetFileName(i))){
											z.Enabled = false;
											j.MenuItems.Add(z);
										}
									}
								})),
							new MenuItem("Unload", new EventHandler((a,b) => {
								Plugin_Manager.ReleasePlugin(System.IO.Path.GetFileName(i));
								foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
									if (System.IO.Path.GetFileName(i) == j.Text)
										notify.ContextMenu.MenuItems[1].MenuItems.Remove(j);
							})),
							new MenuItem("Enable", new EventHandler((a,b) => {
								Plugin_Manager.EnablePlugin(System.IO.Path.GetFileName(i));
								foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
									if (System.IO.Path.GetFileName(i) == j.Text) {
										j.MenuItems[2].Enabled = false;
										j.MenuItems[3].Enabled = true;
									}
								foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
									if (System.IO.Path.GetFileName(i) == j.Text){
										int w = 0;
										foreach(MenuItem z in j.MenuItems) {
											if(++w <= 3)
												continue;
											z.Enabled = true;
										}
									}
							})),
							new MenuItem("Disable", new EventHandler((a,b) => {
								Plugin_Manager.DisablePlugin(System.IO.Path.GetFileName(i));
								foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
									if (System.IO.Path.GetFileName(i) == j.Text) {
										j.MenuItems[2].Enabled = true;
										j.MenuItems[3].Enabled = false;
									}
								foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
									if (System.IO.Path.GetFileName(i) == j.Text){
										int w = 0;
										foreach(MenuItem z in j.MenuItems) {
											if(++w <= 3)
												continue;
											z.Enabled = false;
										}
									}
							})),
							new MenuItem("-")
						});
				foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems) {
					if (System.IO.Path.GetFileName(i) == j.Text) {
						j.MenuItems[1].Enabled = false;
						j.MenuItems[2].Enabled = false;
						j.MenuItems[3].Enabled = false;
					}
				}
				if (AutoLoad) {
					Plugin_Manager.LoadPlugin(System.IO.Path.GetFileName(i));
					foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
						if (System.IO.Path.GetFileName(i) == j.Text) {
							j.MenuItems[1].Enabled = true;
							j.MenuItems[2].Enabled = true;
							j.MenuItems[3].Enabled = false;
							j.MenuItems[0].Enabled = false;
							foreach (MenuItem z in Plugin_Manager.GetPluginMenuItems(System.IO.Path.GetFileName(i))) {
								z.Enabled = false;
								j.MenuItems.Add(z);
							}
						}
				}

				if(AutoLoad && AutoEnable) {
					Plugin_Manager.EnablePlugin(System.IO.Path.GetFileName(i));
					foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
						if (System.IO.Path.GetFileName(i) == j.Text) {
							j.MenuItems[2].Enabled = false;
							j.MenuItems[3].Enabled = true;
						}
					foreach (MenuItem j in notify.ContextMenu.MenuItems[1].MenuItems)
						if (System.IO.Path.GetFileName(i) == j.Text) {
							int w = 0;
							foreach (MenuItem z in j.MenuItems) {
								if (++w <= 3)
									continue;
								z.Enabled = true;
							}
						}
				}
			}
		}

		public Kyeer_UI() {
			InitializeComponent();

			this.Width = this.ellipse.Width;
			this.Height = this.ellipse.Height;
			this.MouseLeftButtonDown += (s, e) => this.DragMove();

			notify.Icon = new System.Drawing.Icon("Kyeer_Icon_512.ico");
			notify.Text = "Stile Alive";
			notify.BalloonTipText = "Hey! How's it going?";
			notify.BalloonTipTitle = "Kyeer!";
			notify.MouseDoubleClick += (s, e) => this.Visibility = this.IsVisible ? Visibility.Hidden : Visibility.Visible;

			notify.ContextMenu = new ContextMenu(new MenuItem[] {
				new MenuItem("Load", LoadPlugin),
				new MenuItem("Plugins", new MenuItem[]{ }),
				new MenuItem("-"),
				new System.Windows.Forms.MenuItem("Exit", new EventHandler((s,e) => this.Close()))
			});

			notify.Visible = true;
			LoadPlugin(null, null);
		}
	}
}