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
using System.Windows.Threading;
using System.Xml;

namespace WPFInterface
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		System.Windows.Point point;
		bool notifyFlag, saveFlag;

		public string[] args;
		string pathToZip;
		static string[] path;
		public static string folderID;
		string MegaLogin, MegaFolder, MegaPassword;
		string defZIP, defEXT;

		int count = 0, numberFolder = 0;
		int resize = 26, widthCompare = 320;

		NotifyIcon notifyIcon1 = new NotifyIcon();
		NotifyIcon notifyIcon2 = new NotifyIcon();
		MegaApiClient megaApiClient = new MegaApiClient();

		OpenFileDialog openFile = new OpenFileDialog();
		SaveFileDialog SaveFileDialog = new SaveFileDialog();
		FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
		ContextMenuStrip contextMenu = new ContextMenuStrip();

		List<string> files = new List<string>();
		List<string> loadi = new List<string>
		{
			"Идёт извлечение",
			".",
			".",
			"."
		};
		readonly List<string> keys = new List<string>() { "location.X", "location.Y" };

		public MainWindow()
		{
			InitializeComponent();
			args = null;
			args = Environment.GetCommandLineArgs();
			Closing += (s, e) =>
			{
				if (megaApiClient.IsLoggedIn)
				{
					megaApiClient.Logout();
				}
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
				if (saveFlag == true)
				{
					DialogResult result = folderBrowserDialog1.ShowDialog();
					if (result == System.Windows.Forms.DialogResult.OK)
					{
						ExtractZipAsync(folderBrowserDialog1.SelectedPath);
					}
				}
				else if (defEXT != "")
				{
					ExtractZipAsync(defEXT);
				}
				else
				{
					DialogResult result = folderBrowserDialog1.ShowDialog();
					if (result == System.Windows.Forms.DialogResult.OK)
					{
						ExtractZipAsync(folderBrowserDialog1.SelectedPath);
					}
				}
			};
			SaveFileDialog.FileOk += (sender, e) =>
			{
				defZIP = Directory.GetParent(SaveFileDialog.FileName).FullName;
				StartZipAsync(Path.GetFileName(SaveFileDialog.FileName));
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
			_ = Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
			  {
				  try
				  {
					  if (args.Length > 1)
					  {
						  tab2.Focus();
						  path = new string[0];
						  foreach (string item in args)
						  {
							  if (item.Contains(".zip"))
							  {
								  path = path.Append(item).ToArray();
							  }
						  }
						  if (defEXT != "" && path.Length != 0)
						  {
							  ExtractZipAsync(defEXT);
						  }
						  else if (defEXT == "" && path.Length != 0)
						  {
							  DialogResult result = folderBrowserDialog1.ShowDialog();
							  if (result == System.Windows.Forms.DialogResult.OK)
							  {
								  ExtractZipAsync(folderBrowserDialog1.SelectedPath);
							  }
						  }
					  }
				  }
				  catch (Exception)
				  {
					  System.Windows.MessageBox.Show("Что-то пошло не так...\nВозможно архив запаролен.", "Folder Error", MessageBoxButton.OK, MessageBoxImage.Error);
				  }
			  }));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NameValueCollection allAppSettings = ConfigurationManager.AppSettings;
			Top = double.Parse(allAppSettings["location.X"]);
			Left = double.Parse(allAppSettings["location.Y"]);
			MegaLogin = allAppSettings["MegaLogin"];
			MegaFolder = allAppSettings["MegaFolder"];
			MegaPassword = Cript.DeShifrovka(allAppSettings["MegaPassword"]);
			defZIP = allAppSettings["defZIP"];
			defEXT = allAppSettings["defEXT"];
			string notify = allAppSettings["NotifyFlag"];
			string save = allAppSettings["SaveFlag"];
			if (save == "true")
			{
				saveFlag = true;
			}
			else
			{
				saveFlag = false;
			}
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
			string zipName;
			path = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
			if (saveFlag == true)
			{
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
			else if (defZIP != "")
			{
				if (path.Length == 1)
				{
					string pathName = Path.GetFileName(path[0]);
					string[] name = pathName.Split('.');
					zipName = name[0] + ".zip";
				}
				else
				{
					zipName = Guid.NewGuid().ToString().Remove(0, 24) + ".zip";
				}
				StartZipAsync(zipName);
			}
			else
			{
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
				if (saveFlag == true)
				{
					DialogResult result = folderBrowserDialog1.ShowDialog();
					if (result == System.Windows.Forms.DialogResult.OK)
					{
						ExtractZipAsync(folderBrowserDialog1.SelectedPath);
					}
				}
				else if (defEXT != "")
				{
					ExtractZipAsync(defEXT);
				}
				else
				{
					DialogResult result = folderBrowserDialog1.ShowDialog();
					if (result == System.Windows.Forms.DialogResult.OK)
					{
						ExtractZipAsync(folderBrowserDialog1.SelectedPath);
					}
				}
			}
			else
			{
				count = 0;
				System.Windows.MessageBox.Show("Вы пытайтесь разархивировать папку или расширение отличное от .zip!", "Folder Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void ExtractZipAsync(string ExtractFolderPath)
		{
			if (ExtractFolderPath != null)
			{
				string pathToExtract;
				if (path.Length == 1)
				{
					pathToExtract = ExtractFolderPath ?? "";
					if (pathToExtract != "")
					{
						Task task = new Task(() =>
						{
							pathToExtract = ExtractFolderPath;
							string[] nameFolder = Path.GetFileName(path[0]).Split('.');
							pathToExtract += $@"\{nameFolder[0]}";
							bool flagExists = true;
							do
							{
								numberFolder++;
								if (Directory.Exists(pathToExtract))
								{
									if (!Directory.Exists(pathToExtract + $" ({numberFolder})"))
									{
										ZipFile.ExtractToDirectory(path[0], pathToExtract + $" ({numberFolder})");
										numberFolder = 0;
										flagExists = false;
									}
								}
								else
								{
									ZipFile.ExtractToDirectory(path[0], pathToExtract);
									numberFolder = 0;
									flagExists = false;
								}
							} while (flagExists);
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
							Process.Start("explorer.exe", $"/open, {ExtractFolderPath}");
						}
						else
						{
							System.Windows.MessageBox.Show("Файлы извлечены", "Финал", MessageBoxButton.OK, MessageBoxImage.Information);
							Process.Start("explorer.exe", $"/open, {ExtractFolderPath}");
						}

					}
				}
				else
				{
					pathToExtract = ExtractFolderPath ?? "";
					if (pathToExtract != "")
					{
						Task task = new Task(() =>
						{
							foreach (string item in path)
							{
								pathToExtract = ExtractFolderPath;
								string[] nameFolder = Path.GetFileName(item).Split('.');
								pathToExtract += $@"\{nameFolder[0]}";
								bool flagExists = true;
								do
								{
									numberFolder++;
									if (Directory.Exists(pathToExtract))
									{
										if (!Directory.Exists(pathToExtract + $" ({numberFolder})"))
										{
											ZipFile.ExtractToDirectory(item, pathToExtract + $" ({numberFolder})");
											numberFolder = 0;
											flagExists = false;
										}
									}
									else
									{
										ZipFile.ExtractToDirectory(item, pathToExtract);
										numberFolder = 0;
										flagExists = false;
									}
								} while (flagExists);
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
							Process.Start("explorer.exe", $"/open, {ExtractFolderPath}");
						}
						else
						{
							System.Windows.MessageBox.Show("Файлы извлечены", "Финал", MessageBoxButton.OK, MessageBoxImage.Information);
							Process.Start("explorer.exe", $"/open, {ExtractFolderPath}");
						}

					}
				}
			}
			folderBrowserDialog1.Reset();
		}

		private void MethodZip()
		{
			if (pathToZip != null)
			{
				int numberZip = 0;
				do
				{
					numberZip++;
					if (File.Exists(pathToZip))
					{
						pathToZip = pathToZip.Replace(".zip", "");
						pathToZip = pathToZip.Replace($"({numberZip})", "");
						if (!File.Exists(pathToZip + $" ({numberZip}).zip"))
						{
							pathToZip = pathToZip + $" ({numberZip}).zip";
							break;
						}
						pathToZip = pathToZip + ".zip";
					}
					else
					{
						break;
					}
				} while (true);

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

		private async void StartZipAsync(string arhiveName)
		{
			if (chBox2.IsChecked == false)
			{
				pathToZip = defZIP + $"\\{arhiveName}";
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
					Process.Start("explorer.exe", $"/select, {pathToZip}");
				}
				else
				{
					System.Windows.MessageBox.Show("Архив создан!", "Финал", MessageBoxButton.OK, MessageBoxImage.Information);
					Process.Start("explorer.exe", $"/select, {pathToZip}");
				}
			}

			if (chBox2.IsChecked == true)
			{
				try
				{
					if (!megaApiClient.IsLoggedIn)
					{
						await megaApiClient.LoginAsync(MegaLogin, MegaPassword);
					}
					pathToZip = defZIP + $"\\{arhiveName}";
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
							megaApiClient.UploadFile(pathToZip, folder, null);
						}
						else
						{
							megaApiClient.UploadFile(pathToZip, node.First(), null);
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
						Process.Start("explorer.exe", $"/select, {pathToZip}");
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
						Process.Start("explorer.exe", $"/select, {pathToZip}");
					}
				}
				catch (Exception ex)
				{
					System.Windows.MessageBox.Show("При попытке доступа к Mega.io произошла ошибка. Проверьте в настрйках, правильно ли указан логин и пароль или повторите попытку позже." + ex.Message,
						"Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			text1.Text = "Перетащите в область для сжатия";
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
			string zipName;
			folderBrowserDialog1.ShowDialog();
			string pathAloneFolder = folderBrowserDialog1.SelectedPath ?? "";
			path = new string[] { pathAloneFolder };
			if (path[0] != "")
			{
				if (saveFlag == true)
				{
					string pathName = Path.GetFileName(path[0]);
					string[] name = pathName.Split('.');
					SaveFileDialog.FileName = name[0] + ".zip";
					SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
					SaveFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
					SaveFileDialog.Title = "Путь к сохранению архива";
					SaveFileDialog.ShowDialog();
				}
				else if (defZIP != "")
				{
					string pathName = Path.GetFileName(path[0]);
					string[] name = pathName.Split('.');
					zipName = name[0] + ".zip";
					StartZipAsync(zipName);
				}
				else
				{
					string pathName = Path.GetFileName(path[0]);
					string[] name = pathName.Split('.');
					SaveFileDialog.FileName = name[0] + ".zip";
					SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
					SaveFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
					SaveFileDialog.Title = "Путь к сохранению архива";
					SaveFileDialog.ShowDialog();
				}
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

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			args = null;
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
			string zipName;
			folderBrowserDialog1.ShowDialog();
			string pathAloneFolder = folderBrowserDialog1.SelectedPath ?? "";
			path = new string[] { pathAloneFolder };
			if (path[0] != "")
			{
				if (saveFlag == true)
				{
					string pathName = Path.GetFileName(path[0]);
					string[] name = pathName.Split('.');
					SaveFileDialog.FileName = name[0] + ".zip";
					SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
					SaveFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
					SaveFileDialog.Title = "Путь к сохранению архива";
					SaveFileDialog.ShowDialog();
				}
				else if (defZIP != "")
				{
					string pathName = Path.GetFileName(path[0]);
					string[] name = pathName.Split('.');
					zipName = name[0] + ".zip";
					StartZipAsync(zipName);
				}
				else
				{
					string pathName = Path.GetFileName(path[0]);
					string[] name = pathName.Split('.');
					SaveFileDialog.FileName = name[0] + ".zip";
					SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
					SaveFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
					SaveFileDialog.Title = "Путь к сохранению архива";
					SaveFileDialog.ShowDialog();
				}
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
