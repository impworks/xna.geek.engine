using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using Microsoft.Win32;

namespace SpriteMaker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : INotifyPropertyChanged
	{
		#region Constructor

		public MainWindow()
		{
			InitializeComponent();
			Items = new ObservableCollection<string>();

			AddFilesCommand = new RoutedCommand(
				"AddFilesCommand",
				GetType(),
				new InputGestureCollection(
					new [] { new KeyGesture(Key.O, ModifierKeys.Control) }
				)
			);

			GenerateCommand = new RoutedCommand(
				"GenerateCommand",
				GetType(),
				new InputGestureCollection(
					new [] { new KeyGesture(Key.S, ModifierKeys.Control) }
				)
			);

			DeleteCommand = new RoutedCommand(
				"DeleteCommand",
				GetType(),
				new InputGestureCollection(
					new[] { new KeyGesture(Key.Delete) }
				)
			);

			CommandBindings.Add(new CommandBinding(AddFilesCommand, AddFilesCommand_Executed));
			CommandBindings.Add(new CommandBinding(GenerateCommand, GenerateCommand_Executed, GenerateCommand_CanExecute));
			CommandBindings.Add(new CommandBinding(DeleteCommand, DeleteCommand_Executed, DeleteCommand_CanExecute));

			DataContext = this;
		}

		#endregion

		#region Fields

		/// <summary>
		/// List of frames for the sprite.
		/// </summary>
		public ObservableCollection<string> Items { get; set; }

		#endregion

		#region Commands

		/// <summary>
		/// Add files to list.
		/// </summary>
		public ICommand AddFilesCommand { get; set; }

		/// <summary>
		/// Generate the resulting image.
		/// </summary>
		public ICommand GenerateCommand { get; set; }

		/// <summary>
		/// Delete the selected items.
		/// </summary>
		public ICommand DeleteCommand { get; set; }

		private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs args)
		{
			args.CanExecute = FilesList.SelectedItems.Count > 0;
		}

		private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs args)
		{
			var toRemove = Items.Where(item => FilesList.SelectedItems.Contains(item)).ToList();
			foreach (var curr in toRemove)
				Items.Remove(curr);
		}

		private void GenerateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs args)
		{
			args.CanExecute = Items.Count > 1;
		}

		private void GenerateCommand_Executed(object sender, ExecutedRoutedEventArgs args)
		{
			var dlg = new SaveFileDialog
			          	{
			          		AddExtension = true,
							Filter = "PNG images|*.png",
			          		CheckPathExists = true,
			          		DefaultExt = "png",
			          		Title = "Save file as..."
			          	};

			var result = dlg.ShowDialog();
			if (result.Value)
				generateSprite(dlg.FileName);
		}

		private void AddFilesCommand_Executed(object sender, ExecutedRoutedEventArgs args)
		{
			var dlg = new OpenFileDialog
			          	{
			          		CheckFileExists = true,
			          		Filter = "PNG images|*.png",
			          		Title = "Open files",
			          		Multiselect = true
			          	};

			var result = dlg.ShowDialog();

			if (result.Value)
			{
				foreach(var name in dlg.FileNames)
					Items.Add(name);
			}

		}

		#endregion

		#region Sprite generation

		/// <summary>
		/// Create a sprite out of the list.
		/// </summary>
		/// <param name="fileName"></param>
		private void generateSprite(string fileName)
		{
			try
			{
				var imageList = loadImages(Items);
				validateImages(imageList);
				var bmp = createCanvas(imageList);
				pasteFrames(bmp, imageList);
				bmp.Save(fileName, ImageFormat.Png);

				showSuccess(fileName);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
		}

		/// <summary>
		/// Load images into memory.
		/// </summary>
		/// <returns></returns>
		private static List<Bitmap> loadImages(IEnumerable<string> sources)
		{
			return sources.Select( item => (Bitmap)Bitmap.FromFile(item) ).ToList();
		}

		/// <summary>
		/// Make sure there are enough images and they're of equal size!
		/// </summary>
		/// <param name="images">Image list.</param>
		private static void validateImages(ICollection<Bitmap> images)
		{
			if(images.Count < 2)
				throw new ArgumentException("At least 2 images are required to create a sprite.");

			var first = images.First();

			var invalidSizes = images.Skip(1).Any(item => item.Width != first.Width || item.Height != first.Height);
			if(invalidSizes)
				throw new ArgumentException(string.Format("All images must be {0} x {1} pixels in size!", first.Width, first.Height));
		}

		/// <summary>
		/// Create a new canvas to put all images onto.
		/// </summary>
		/// <param name="images">Image list.</param>
		/// <returns></returns>
		private static Bitmap createCanvas(ICollection<Bitmap> images)
		{
			var first = images.First();
			return new Bitmap(first.Width*images.Count, first.Height);
		}

		/// <summary>
		/// Paste all frames onto a single image.
		/// </summary>
		/// <param name="target">Target image.</param>
		/// <param name="sources">List of source images.</param>
		private static void pasteFrames(Image target, IEnumerable<Bitmap> sources)
		{
			using(var graphics = Graphics.FromImage(target))
			{
				var offset = 0;
				foreach (var curr in sources)
				{
					graphics.DrawImageUnscaled(curr, offset, 0);
					offset += curr.Width;
				}
			}
		}

		private static void showSuccess(string fileName)
		{
			var result = MessageBox.Show(
				"The sprite has been created. Do you want to view it?",
				"Success",
				MessageBoxButton.YesNo,
				MessageBoxImage.Information
				);

			if (result == MessageBoxResult.Yes)
				new Process {StartInfo = new ProcessStartInfo(fileName)}.Start();
		}

		#endregion

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;
		protected void notifyPropertyChanged(string property)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(property));
		}

		#endregion
	}
}
