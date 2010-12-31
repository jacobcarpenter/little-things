
using System;
using System.Windows;
using System.Windows.Media;
using GameOfLife;

namespace WpfGameOfLife
{
	sealed class LifeGridControl : FrameworkElement
	{
		public LifeGridControl(LifeGrid initialGrid)
		{
			m_grid = initialGrid;
			m_visual = new DrawingVisual();
			AddVisualChild(m_visual);

			// Author's note: by attaching the event handler here, we actually miss seeing the first couple of generations as the control
			//   is rendering. But the code is so much simpler this way, that I'm tentatively willing to make that compromise.
			CompositionTarget.Rendering += Rendering;
		}

		protected override int VisualChildrenCount
		{
			get { return 1; }
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index != 0)
				throw new ArgumentOutOfRangeException();

			return m_visual;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			return GetConstrainedSize(constraint);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			return GetConstrainedSize(arrangeBounds);
		}

		private void Rendering(object sender, EventArgs e)
		{
			m_grid = m_grid.Step();
			UpdateVisual();
		}

		private void UpdateVisual()
		{
			using (DrawingContext dc = m_visual.RenderOpen())
			{
				dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));

				foreach (LifeCell liveCell in m_grid.GetLiveCells())
					dc.DrawRectangle(Brushes.Black, null, new Rect(liveCell.X * c_cellSpriteSize, liveCell.Y * c_cellSpriteSize, c_cellSpriteSize, c_cellSpriteSize));
			}
		}

		private Size GetConstrainedSize(Size constraint)
		{
			Size desired = new Size(
				Math.Min(m_grid.Width * c_cellSpriteSize, constraint.Width),
				Math.Min(m_grid.Height * c_cellSpriteSize, constraint.Height));
			return desired;
		}

		const int c_cellSpriteSize = 2;

		readonly DrawingVisual m_visual;
		LifeGrid m_grid;
	}
}
