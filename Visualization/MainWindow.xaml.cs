using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visualization.ViewModels;

namespace Visualization
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly CrossroadsViewModel _VM;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = _VM = new CrossroadsViewModel(new WpfContext(Dispatcher));
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_VM.Dispose();
		}
	}
}
