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
using System.IO;

namespace Treasure
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly object _dummyNode = null;
		private string _rootLocation = "";

		public MainWindow()
		{
			InitializeComponent();
		}

		private void TreasureMainWindow_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void PopulateDirectoryList(string targetDirectory)
		{
			DirectoryList.Items.Clear();

			TreeViewItem root = new TreeViewItem();
			root.Header = targetDirectory;
			root.Tag = targetDirectory;
			root.FontWeight = FontWeights.Bold;

			root.Items.Add(_dummyNode);
			root.Expanded += new RoutedEventHandler(folder_Expanded);
			root.ItemContainerStyle = DirectoryList.Resources["CheckableTreeViewItemStyle"] as Style;
			DirectoryList.Items.Add(root);
			root.IsExpanded = true;
		}

		private void folder_Expanded(object sender, RoutedEventArgs e)
		{
			TreeViewItem item = (TreeViewItem) sender;
			if (item.Items.Count == 1 && item.Items[0] == _dummyNode)
			{
				item.Items.Clear();
				try
				{
					// Add all directories first.
					foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
					{
						TreeViewItem subitem = new TreeViewItem();
						subitem.ItemContainerStyle = DirectoryList.Resources["CheckableTreeViewItemStyle"] as Style;
						subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
						subitem.Tag = s;
						subitem.FontWeight = FontWeights.Normal;
						
						// Is this item hidden?
						if (subitem.Header.ToString().StartsWith(".", false, System.Globalization.CultureInfo.CurrentCulture))
						{
							subitem.FontStyle = FontStyles.Italic;
							subitem.Foreground = new SolidColorBrush(Color.FromRgb(164, 195, 255));
						}

						subitem.Items.Add(_dummyNode);
						subitem.Expanded += new RoutedEventHandler(folder_Expanded);
						item.Items.Add(subitem);
					}

					// Add all files second.
					foreach (string s in Directory.GetFiles(item.Tag.ToString()))
					{
						TreeViewItem subitem = new TreeViewItem();
						subitem.ItemContainerStyle = DirectoryList.Resources["CheckableTreeViewItemStyle"] as Style;
						subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
						subitem.Tag = s;
						subitem.FontWeight = FontWeights.Normal;
						
						// Is this item hidden?
						if (subitem.Header.ToString().StartsWith(".", false, System.Globalization.CultureInfo.CurrentCulture))
						{
							subitem.FontStyle = FontStyles.Italic;
							subitem.Foreground = new SolidColorBrush(Color.FromRgb(164, 195, 255));
						}

						item.Items.Add(subitem);
					}
				}
				catch (Exception) { }
			}
		}

		private void DropTarget_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Link;
				Label target = (Label)sender;
				target.Background = new SolidColorBrush(Color.FromRgb(164, 195, 255));
			}
			else { e.Effects = DragDropEffects.None; }
		}

		private void DropTarget_DragLeave(object sender, DragEventArgs e)
		{
			Label target = (Label)sender;
			target.Background = new SolidColorBrush(Color.FromArgb(127, 164, 195, 255));
		}

		private void DropTarget_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] dropData = (string[])e.Data.GetData(DataFormats.FileDrop);

				if (dropData.Length == 1) {
					_rootLocation = dropData[0];
					PopulateDirectoryList(_rootLocation);
					Label target = (Label)sender;
					target.Visibility = Visibility.Collapsed;
					target.Background = new SolidColorBrush(Color.FromArgb(127, 164, 195, 255));
				}
				else
				{
					// TODO: Handle invalid drop.
				}
			}
				

			
		}

		private void TreasureMainWindow_DragLeave(object sender, DragEventArgs e)
		{
			if (_rootLocation != "") { DropTarget.Visibility = Visibility.Collapsed; }
		}

		private void TreasureMainWindow_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) { DropTarget.Visibility = Visibility.Visible; }
		}

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void DirectoryList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{

		}
	}
}
