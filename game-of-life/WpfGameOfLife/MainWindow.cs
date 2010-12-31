
using System;
using System.Windows;
using GameOfLife;

namespace WpfGameOfLife
{
	sealed class MainWindow : Window
	{
		[STAThread]
		static void Main()
		{
			Application app = new Application();
			app.Run(new MainWindow());
		}

		public MainWindow()
		{
			Title = "Game of Life";
			SizeToContent = SizeToContent.WidthAndHeight;
			ResizeMode = ResizeMode.CanMinimize;

			LifeGrid grid = new LifeGrid(768, 368)
				// initialize the acorn methuselah: http://www.conwaylife.com/wiki/index.php?title=Acorn
				.SetLiveCell(254, 203)
				.SetLiveCell(256, 204)
				.SetLiveCell(253, 205)
				.SetLiveCell(254, 205)
				.SetLiveCell(257, 205)
				.SetLiveCell(258, 205)
				.SetLiveCell(259, 205);

			Content = new LifeGridControl(grid);
		}
	}
}
