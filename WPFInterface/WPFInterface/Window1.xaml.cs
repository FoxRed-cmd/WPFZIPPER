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
		OpenFileDialog OpenFileDialog = new OpenFileDialog();
		public string FolderID { get; set; }
		string key = "FolderGoogleDrive";
		string key2 = "Secret";
		string key3 = "NotifyFlag";
		string key4 = "MegaLogin";
		string key5 = "MegaPassword";
		string key6 = "MegaFolder";
		public Window1()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NameValueCollection allAppSettings = ConfigurationManager.AppSettings;
			txt3.Text = allAppSettings[key4];
			txt4.Password = Cript.DeShifrovka(allAppSettings[key5]);
			txt5.Text = allAppSettings[key6];
			string notifyChecked = allAppSettings[key3];
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
			document.Save(Assembly.GetExecutingAssembly().Location + ".config");
			System.Windows.Application.Current.Shutdown();
			System.Windows.Forms.Application.Restart();
		}

		private void btn2_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
			OpenFileDialog.ShowDialog();
		}

		private void link1_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(link1.NavigateUri.ToString());
		}
	}
}
