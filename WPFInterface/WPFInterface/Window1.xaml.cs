using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace WPFInterface
{
	/// <summary>
	/// Логика взаимодействия для Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		OpenFileDialog OpenFileDialog = null;
		FolderBrowserDialog FolderBrowserDialog = new FolderBrowserDialog();
		public string FolderID { get; set; }
		string key3 = "NotifyFlag";
		string key4 = "MegaLogin";
		string key5 = "MegaPassword";
		string key6 = "MegaFolder";
		string key7 = "SaveFlag";
		string key8 = "defZIP";
		string key9 = "defEXT";
		public Window1()
		{
			InitializeComponent();
			rb3.Click += (s, e) =>
			{
				defEXT.IsEnabled = false;
				defEXT.Text = "";
				defZIP.IsEnabled = false;
				defZIP.Text = "";
				btn2.IsEnabled = false;
				btn3.IsEnabled = false;
			};
			rb4.Click += (s, e) =>
			{
				defEXT.IsEnabled = true;
				defZIP.IsEnabled = true;
				btn2.IsEnabled = true;
				btn3.IsEnabled = true;
			};
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NameValueCollection allAppSettings = ConfigurationManager.AppSettings;
			txt3.Text = allAppSettings[key4];
			txt4.Password = Cript.DeShifrovka(allAppSettings[key5]);
			txt5.Text = allAppSettings[key6];
			string notifyChecked = allAppSettings[key3];
			string saveChecked = allAppSettings[key7];
			defZIP.Text = allAppSettings[key8];
			defEXT.Text = allAppSettings[key9];
			if (saveChecked == "true")
			{
				rb3.IsChecked = true;
				rb4.IsChecked = false;
				defEXT.IsEnabled = false;
				defZIP.IsEnabled = false;
				btn2.IsEnabled = false;
				btn3.IsEnabled = false;
			}
			else
			{
				rb3.IsChecked = false;
				rb4.IsChecked = true;
				defEXT.IsEnabled = true;
				defZIP.IsEnabled = true;
				btn2.IsEnabled = true;
				btn3.IsEnabled = true;
			}
			if (notifyChecked == "true")
			{
				rb1.IsChecked = true;
				rb2.IsChecked = false;
			}
			else
			{
				rb2.IsChecked = true;
				rb1.IsChecked = false;
			}
		}

		private void btn1_Click(object sender, RoutedEventArgs e)
		{
			string password = Cript.Shifrovka(txt4.Password);
			int indexFolderPathMega = txt5.Text.LastIndexOf('/');
			string pathFolderMega = txt5.Text.Remove(0, indexFolderPathMega + 1);
			XmlDocument document = new XmlDocument();
			document.Load(Assembly.GetExecutingAssembly().Location + ".config");
			XmlNode node = document.SelectSingleNode("//appSettings");
			XmlElement element4 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key4)) as XmlElement;
			if (element4 != null)
			{
				element4.SetAttribute("value", txt3.Text);
			}
			XmlElement element5 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key5)) as XmlElement;
			if (element5 != null)
			{
				element5.SetAttribute("value", password);
			}
			XmlElement element6 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key6)) as XmlElement;
			if (element6 != null)
			{
				element6.SetAttribute("value", pathFolderMega);
			}
			XmlElement element8 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key8)) as XmlElement;
			if (element8 != null)
			{
				element8.SetAttribute("value", defZIP.Text);
			}
			XmlElement element9 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key9)) as XmlElement;
			if (element9 != null)
			{
				element9.SetAttribute("value", defEXT.Text);
			}
			XmlElement element3 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key3)) as XmlElement;
			if (rb1.IsChecked == true)
			{
				if (element3 != null)
				{
					element3.SetAttribute("value", "true");
				}
			}
			else if (rb2.IsChecked == true)
			{
				if (element3 != null)
				{
					element3.SetAttribute("value", "false");
				}
			}
			XmlElement element7 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key7)) as XmlElement;
			if (rb3.IsChecked == true)
			{
				if (element7 != null)
				{
					element7.SetAttribute("value", "true");
				}
			}
			else if (rb4.IsChecked == true)
			{
				if (element7 != null)
				{
					element7.SetAttribute("value", "false");
				}
			}
			document.Save(Assembly.GetExecutingAssembly().Location + ".config");
			System.Windows.Application.Current.Shutdown();
			System.Windows.Forms.Application.Restart();
		}

		private void btn2_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog = new OpenFileDialog();
			OpenFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
			OpenFileDialog.ShowDialog();
		}

		private void link1_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(link1.NavigateUri.ToString());
		}

		private void btn2_Click_1(object sender, RoutedEventArgs e)
		{
			DialogResult result = FolderBrowserDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				defZIP.Text = FolderBrowserDialog.SelectedPath;
			}
		}

		private void btn3_Click(object sender, RoutedEventArgs e)
		{
			DialogResult result = FolderBrowserDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				defEXT.Text = FolderBrowserDialog.SelectedPath;
			}
		}

		private void btn5_Click(object sender, RoutedEventArgs e)
		{
			XmlDocument document = new XmlDocument();
			document.Load(Assembly.GetExecutingAssembly().Location + ".config");
			XmlNode node = document.SelectSingleNode("//appSettings");
			XmlElement element4 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key4)) as XmlElement;
			if (element4 != null)
			{
				element4.SetAttribute("value", "");
			}
			XmlElement element5 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key5)) as XmlElement;
			if (element5 != null)
			{
				element5.SetAttribute("value", "");
			}
			XmlElement element6 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key6)) as XmlElement;
			if (element6 != null)
			{
				element6.SetAttribute("value", "");
			}
			XmlElement element8 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key8)) as XmlElement;
			if (element8 != null)
			{
				element8.SetAttribute("value", "");
			}
			XmlElement element9 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key9)) as XmlElement;
			if (element9 != null)
			{
				element9.SetAttribute("value", "");
			}
			XmlElement element3 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key3)) as XmlElement;
			if (element3 != null)
			{
				element3.SetAttribute("value", "true");
			}
			XmlElement element7 = node.SelectSingleNode(string.Format("//add[@key='{0}']", key7)) as XmlElement;
			if (element7 != null)
			{
				element7.SetAttribute("value", "true");
			}
			document.Save(Assembly.GetExecutingAssembly().Location + ".config");
			System.Windows.Application.Current.Shutdown();
			System.Windows.Forms.Application.Restart();
		}
	}
}
