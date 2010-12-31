
namespace GameOfLife
{
	/// <summary>
	/// A pair of x and y coordinates representing a cell within a LifeGrid instance.
	/// </summary>
	public struct LifeCell
	{
		/// <summary>
		/// Gets the x-coordinate of the cell.
		/// </summary>
		public int X
		{
			get { return m_x; }
		}

		/// <summary>
		/// Gets the y-coordinate of the cell.
		/// </summary>
		public int Y
		{
			get { return m_y; }
		}

		// Author's note: I made the constructor internal because I'm too lazy to do parameter validation. I didn't really want to document it, either.
		internal LifeCell(int x, int y)
		{
			m_x = x;
			m_y = y;
		}

		readonly int m_x;
		readonly int m_y;
	}
}
