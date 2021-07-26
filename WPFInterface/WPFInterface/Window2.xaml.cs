using System.Diagnostics;
using System.Windows;

namespace WPFInterface
{
	/// <summary>
	/// Логика взаимодействия для Window2.xaml
	/// </summary>
	public partial class Window2 : Window
	{
		public Window2()
		{
			InitializeComponent();
			tx6.Text += "Молния";
			tx7.Text += "v2.1";
			tx8.Text += "FoxRed";
			txt1.Text = "Программа для работы с ZIP архивами".ToUpper();
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(link1.NavigateUri.ToString());
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}
	}
}
