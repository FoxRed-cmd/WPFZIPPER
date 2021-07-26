using CG.Web.MegaApiClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;

namespace WPFInterface
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		System.Windows.Point point;
		public string MyProperty { get; set; }
		public string MyProperty2 { get; set; }
		bool notifyFlag;
		List<string> files = new List<string>();
		readonly List<string> keys = new List<string>() { "location.X", "location.Y" };
		string pathToZip;
		static string[] path;
		public static string folderID;
		string MegaLogin, MegaFolder, MegaPassword;
		int count = 0;
		NotifyIcon notifyIcon1 = new NotifyIcon();
		NotifyIcon notifyIcon2 = new NotifyIcon();
		OpenFileDialog openFile = new OpenFileDialog();
		SaveFileDialog SaveFileDialog = new SaveFileDialog();
		FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
		ContextMenuStrip contextMenu = new ContextMenuStrip();
		List<string> loadi = new List<string>
		{
			"Идёт извлечение",
			".",
			".",
			"."
		};
		int resize = 26, widthCompare = 320;

		public MainWindow()
		{
			InitializeComponent();
			Closing += (s, e) =>
			{
				point = new System.Windows.Point(Top, Left);
				List<string> values = new List<string>() { point.X.ToString(), point.Y.ToString() };
				XmlDocument document = new XmlDocument();
				document.Load(Assembly.GetExecutingAssembly().Location + ".config");
				XmlNode node = document.SelectSingleNode("//appSettings");
				for (int i = 0; i < keys.Count; i++)
				{
					XmlElement element = node.SelectSingleNode(string.Format("//add[@key='{0}']", keys[i])) as XmlElement;

					if (element != null)
					{
						element.SetAttribute("value", values[i]);
					}

				}
				document.Save(Assembly.GetExecutingAssembly().Location + ".config");
			};
			notifyIcon2.BalloonTipClosed += (sender, e) => { notifyIcon2.Visible = false; };
			openFile.FileOk += (sender, e) =>
			{
				path = openFile.FileNames;
				ExtractZipAsync();
			};
			SaveFileDialog.FileOk += async (sender, e) =>
			{
				if (chBox2.IsChecked == false)
				{
					pathToZip = SaveFileDialog.FileName;
					Task task = new Task(MethodZip);
					task.Start();

					List<string> loadi = new List<string>
					{
						"Идёт сжатие",
						".",
						".",
						"."
					};
					text1.Text = "";
					do
					{
						foreach (var item in loadi)
						{
							text1.Text += item;
							await Task.Delay(200);
						}
						text1.Text = "";

					} while (!task.IsCompleted);

					if (notifyFlag)
					{
						notifyIcon2.BalloonTipText = "Архив создан!";
						notifyIcon2.BalloonTipTitle = "Финал";
						notifyIcon2.BalloonTipIcon = ToolTipIcon.Info;
						notifyIcon2.Icon = SystemIcons.Information;
						notifyIcon2.Visible = true;
						notifyIcon2.ShowBalloonTip(1000);
						Process.Start("explorer.exe", $"/select, {SaveFileDialog.FileName}");
					}
					else
					{
						System.Windows.MessageBox.Show("Архив создан!", "Финал", MessageBoxButton.OK, MessageBoxImage.Information);
						Process.Start("explorer.exe", $"/select, {SaveFileDialog.FileName}");
					}
				}

				if (chBox2.IsChecked == true)
				{
					try
					{
						MegaApiClient megaApiClient = new MegaApiClient();
						megaApiClient.Login(MegaLogin, MegaPassword);

						pathToZip = SaveFileDialog.FileName;
						Task task = new Task(MethodZip);
						task.Start();

						List<string> loadi = new List<string>
						{
							"Идёт сжатие",
							".",
							".",
							"."
						};
						text1.Text = "";
						do
						{
							foreach (var item in loadi)
							{
								text1.Text += item;
								await Task.Delay(200);
							}
							text1.Text = "";

						} while (!task.IsCompleted);

						Task taskUploadToMega = new Task(() =>
						{
							IEnumerable<INode> node = megaApiClient.GetNodes();
							INode folder = node.FirstOrDefault(n => n.Id == MegaFolder);
							if (folder != null)
							{
								megaApiClient.UploadFile(SaveFileDialog.FileName, folder, null);
							}
							else
							{
								megaApiClient.UploadFile(SaveFileDialog.FileName, node.First(), null);
							}
						});
						taskUploadToMega.Start();
						List<string> upload = new List<string>
						{
							"Загрузка на Mega Drive",
							".",
							".",
							"."
						};
						text1.Text = "";
						do
						{
							foreach (var item in upload)
							{
								text1.Text += item;
								await Task.Delay(200);
							}
							text1.Text = "";

						} while (!taskUploadToMega.IsCompleted);

						if (notifyFlag)
						{
							Process.Start("explorer.exe", $"/select, {SaveFileDialog.FileName}");
							notifyIcon2.BalloonTipText = "Архив создан и загружен на Mega Drive!";
							notifyIcon2.BalloonTipTitle = "Финал";
							notifyIcon2.BalloonTipIcon = ToolTipIcon.Info;
							notifyIcon2.Icon = SystemIcons.Information;
							notifyIcon2.Visible = true;
							notifyIcon2.ShowBalloonTip(1000);
						}
						else
						{
							System.Windows.MessageBox.Show("Архив создан и загружен на Mega Drive!", "Финал", MessageBoxButton.OK, MessageBoxImage.Information);
							Process.Start("explorer.exe", $"/select, {SaveFileDialog.FileName}");
						}
					}
					catch (Exception ex)
					{
						System.Windows.MessageBox.Show("При попытке доступа к Mega.io произошла ошибка. Проверьте в настрйках, правильно ли указан логин и пароль или повторите попытку позже." + ex.Message,
							"Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				text1.Text = "Перетащите в область для сжатия";
			};
			tab1.GotFocus += (sender, e) =>
			{
				chBox2.IsEnabled = true;
			};
			tab2.GotFocus += (sender, e) =>
			{
				chBox2.IsEnabled = false;
			};
			notifyIcon1.MouseClick += (s, e) =>
			{
				Hide();
				notifyIcon1.Visible = true;
				notifyIcon2.Visible = false;
			};
			notifyIcon1.MouseDoubleClick += (s, e) =>
			{
				Show();
				notifyIcon1.Visible = false;
				WindowState = WindowState.Normal;
			};
			contextMenu.Items.Add("Создать ZIP", Properties.Resources.free_icon_lightning_bolt_3325155__2_, CreateZipClick);
			contextMenu.Items.Add("Извлечь ZIP", Properties.Resources.free_icon_zipper_2088946, ExtractZipClick);
			contextMenu.Items.Add("Выход", Properties.Resources.free_icon_logout_5087631, ExitClick);
			notifyIcon1.Icon = Properties.Resources.free_icon_zip_1243480 as Icon;
			notifyIcon1.ContextMenuStrip = contextMenu;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NameValueCollection allAppSettings = ConfigurationManager.AppSettings;
			Top = double.Parse(allAppSettings["location.X"]);
			Left = double.Parse(allAppSettings["location.Y"]);
			MegaLogin = allAppSettings["MegaLogin"];
			MegaFolder = allAppSettings["MegaFolder"];
			MegaPassword = Cript.DeShifrovka(allAppSettings["MegaPassword"]);
			string notify = allAppSettings["NotifyFlag"];
			if (notify == "true")
			{
				notifyFlag = true;
			}
			else
			{
				notifyFlag = false;
			}
		}

		private void Border_Drop(object sender, System.Windows.DragEventArgs e)
		{
			path = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
			if (path.Length == 1)
			{
				string pathName = Path.GetFileName(path[0]);
				string[] name = pathName.Split('.');
				SaveFileDialog.FileName = name[0] + ".zip";
				SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				SaveFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
				SaveFileDialog.Title = "Путь к сохранению архива";
				SaveFileDialog.ShowDialog();
			}
			else
			{
				SaveFileDialog.FileName = Guid.NewGuid().ToString().Remove(0, 24);
				SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				SaveFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
				SaveFileDialog.Title = "Путь к сохранению архива";
				SaveFileDialog.ShowDialog();

			}
		}

		private void Border_Drop_1(object sender, System.Windows.DragEventArgs e)
		{
			path = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
			foreach (var item in path)
			{
				if (item.Contains(".zip"))
				{
					count++;
				}
			}
			if (path.Length == count)
			{
				count = 0;
				ExtractZipAsync();
			}
			else
			{
				count = 0;
				System.Windows.MessageBox.Show("Вы пытайтесь разархивировать папку или расширение отличное от .zip!", "Folder Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void ExtractZipAsync()
		{
			folderBrowserDialog1.ShowDialog();
			if (folderBrowserDialog1.SelectedPath != null)
			{
				string pathToExtract;
				if (path.Length == 1)
				{
					pathToExtract = folderBrowserDialog1.SelectedPath ?? "";
					if (pathToExtract != "")
					{
						Task task = new Task(() =>
						{
							string[] nameFolder = Path.GetFileName(path[0]).Split('.');
							//pathToExtract += $@"\{nameFolder[0]}";
							ZipFile.ExtractToDirectory(path[0], pathToExtract);
							pathToExtract = folderBrowserDialog1.SelectedPath;
						});
						task.Start();

						text2.Text = "";
						do
						{
							foreach (var item in loadi)
							{
								text2.Text += item;
								await Task.Delay(200);
							}
							text2.Text = "";

						} while (!task.IsCompleted);

						text2.Text = "Перетащите в область для извлечения";

						if (notifyFlag)
						{
							notifyIcon2.BalloonTipText = "Файлы извлечены";
							notifyIcon2.BalloonTipTitle = "Финал";
							notifyIcon2.BalloonTipIcon = ToolTipIcon.Info;
							notifyIcon2.Icon = SystemIcons.Information;
							notifyIcon2.Visible = true;
							notifyIcon2.ShowBalloonTip(1000);
							Process.Start("explorer.exe", $"/open, {folderBrowserDialog1.SelectedPath}");
						}
						else
						{
							System.Windows.MessageBox.Show("Файлы извлечены", "Финал", MessageBoxButton.OK, MessageBoxImage.Information);
							Process.Start("explorer.exe", $"/open, {folderBrowserDialog1.SelectedPath}");
						}

					}
				}
				else
				{
					pathToExtract = folderBrowserDialog1.SelectedPath ?? "";
					if (pathToExtract != "")
					{
						Task task = new Task(() =>
						{
							foreach (var item in path)
							{
								string[] nameFolder = Path.GetFileName(item).Split('.');
								pathToExtract += $@"\{nameFolder[0]}";
								ZipFile.ExtractToDirectory(item, pathToExtract);
								pathToExtract = folderBrowserDialog1.SelectedPath;
							}
						});
						task.Start();

						text2.Text = "";
						do
						{
							foreach (var item in loadi)
							{
								text2.Text += item;
								await Task.Delay(200);
							}
							text2.Text = "";

						} while (!task.IsCompleted);

						text2.Text = "Перетащите в область для извлечения";

						if (notifyFlag)
						{
							notifyIcon2.BalloonTipText = "Файлы извлечены";
							notifyIcon2.BalloonTipTitle = "Финал";
							notifyIcon2.BalloonTipIcon = ToolTipIcon.Info;
							notifyIcon2.Icon = SystemIcons.Information;
							notifyIcon2.Visible = true;
							notifyIcon2.ShowBalloonTip(1000);
							Process.Start("explorer.exe", $"/open, {folderBrowserDialog1.SelectedPath}");
						}
						else
						{
							System.Windows.MessageBox.Show("Файлы извлечены", "Финал", MessageBoxButton.OK, MessageBoxImage.Information);
							Process.Start("explorer.exe", $"/open, {folderBrowserDialog1.SelectedPath}");
						}

					}
				}
			}
			folderBrowserDialog1.Reset();
		}

		void MethodZip()
		{
			if (pathToZip != null)
			{
				if (Directory.Exists(path[0]) && path.Length == 1)
				{
					ZipFile.CreateFromDirectory(path[0], pathToZip, CompressionLevel.Optimal, true);
				}
				else if (File.Exists(path[0]) && path.Length == 1)
				{
					using (ZipArchive zip = ZipFile.Open(pathToZip, ZipArchiveMode.Create))
					{
						zip.CreateEntryFromFile(path[0], Path.GetFileName(path[0]), CompressionLevel.Optimal);
					}
				}
				else if (path.Length > 1)
				{
					foreach (var item in path)
					{
						using (FileStream zipToOpen = new FileStream(pathToZip, FileMode.OpenOrCreate))
						{
							if (File.Exists(item))
							{
								using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
								{
									archive.CreateEntryFromFile(item, Path.GetFileName(item), CompressionLevel.Optimal);
								}
							}
						}
					}
					foreach (var item in path)
					{
						using (FileStream zipToOpen = new FileStream(pathToZip, FileMode.OpenOrCreate))
						{
							if (Directory.Exists(item))
							{
								using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
								{
									archive.CreateEntryFromDirectory(item, CompressionLevel.Optimal, true);
								}
							}
						}
					}
				}
			}
			else
			{
				System.Windows.MessageBox.Show("Вы не указали, куда сохранить архив!", "path is null error!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void btn1_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.ContextMenu context = FindResource("FileContext") as System.Windows.Controls.ContextMenu;
			context.PlacementTarget = sender as System.Windows.Controls.Button;
			context.IsOpen = true;
		}

		private void btn2_Click(object sender, RoutedEventArgs e)
		{
			if (!System.Windows.Application.Current.Windows.OfType<Window1>().Any())
			{
				Window1 window = new Window1() { FolderID = folderID, Top = this.Top + 10, Left = this.Left + 10 };
				window.Owner = this;
				window.Show();
			}
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			folderBrowserDialog1.ShowDialog();
			string pathAloneFolder = folderBrowserDialog1.SelectedPath ?? "";
			path = new string[] { pathAloneFolder };
			if (path[0] != "")
			{
				string pathName = Path.GetFileName(path[0]);
				string[] name = pathName.Split('.');
				SaveFileDialog.FileName = name[0] + ".zip";
				SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				SaveFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
				SaveFileDialog.Title = "Путь к сохранению архива";
				SaveFileDialog.ShowDialog();
			}
			folderBrowserDialog1.Reset();
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			openFile.Multiselect = true;
			openFile.Title = "Извлечь";
			openFile.Filter = "zip files (*.zip)|*.zip";
			openFile.ShowDialog();
		}

		private void MenuItem_Click_2(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void MenuItem_Click_3(object sender, RoutedEventArgs e)
		{
			notifyIcon1.Icon = Properties.Resources.free_icon_zip_1243480 as Icon;
			Hide();
			notifyIcon1.Visible = true;
			notifyIcon2.Visible = false;
		}

		private void ExitClick(object sender, EventArgs e)
		{
			Close();
		}

		private void ExtractZipClick(object sender, EventArgs e)
		{
			openFile.Multiselect = true;
			openFile.Title = "Извлечь";
			openFile.Filter = "zip files (*.zip)|*.zip";
			openFile.ShowDialog();
		}

		private void CreateZipClick(object sender, EventArgs e)
		{
			folderBrowserDialog1.ShowDialog();
			string pathAloneFolder = folderBrowserDialog1.SelectedPath ?? "";
			path = new string[] { pathAloneFolder };
			if (path[0] != "")
			{
				string pathName = Path.GetFileName(path[0]);
				string[] name = pathName.Split('.');
				SaveFileDialog.FileName = name[0] + ".zip";
				SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				SaveFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
				SaveFileDialog.Title = "Путь к сохранению архива";
				SaveFileDialog.ShowDialog();
			}
			folderBrowserDialog1.Reset();
		}

		private void MenuItem_Checked(object sender, RoutedEventArgs e)
		{
			Show();
			notifyIcon1.Visible = false;
			WindowState = WindowState.Normal;
		}

		private void btn4_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			notifyIcon1.Visible = true;
			notifyIcon2.Visible = false;
		}

		private void btn3_Click(object sender, RoutedEventArgs e)
		{
			if (!System.Windows.Application.Current.Windows.OfType<Window2>().Any())
			{
				Window2 w = new Window2() { Top = this.Top + 10, Left = this.Left + 10 };
				w.Owner = this;
				w.Show();
			}
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			RowDefinitionCollection rows = contextRow.RowDefinitions;
			if (Width < 370 && Width > widthCompare)
			{
				text1.FontSize = Width / resize;
				text2.FontSize = Width / resize;
			}
			else if (Width < widthCompare)
			{
				rows[0].Height = new GridLength(50);
				text1.FontSize = Width / resize;
				text2.FontSize = Width / resize;
			}
			else if (Width == MinWidth)
			{
				text1.FontSize = Width / resize;
				text2.FontSize = Width / resize;
			}
			else if (Width > widthCompare)
			{
				rows[0].Height = new GridLength(25);
				text1.FontSize = 15;
				text2.FontSize = 15;
			}
		}
	}
}
