
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GameOfLife
{
	public sealed class LifeGrid
	{
		/// <summary>
		/// Initializes a new LifeGrid (used to simulate Conway's Game of Life: http://en.wikipedia.org/wiki/Conway's_Game_of_Life).
		/// </summary>
		/// <param name="width">The grid's width in cells. The value must be a multiple of 8.</param>
		/// <param name="height">The grid's height in cells.</param>
		/// <remarks>The current implementation creates a toroidal grid (the edges wrap around like Pac-Man). LifeGrid instances are immutable.</remarks>
		public LifeGrid(int width, int height)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException("width");
			if (height <= 0)
				throw new ArgumentOutOfRangeException("height");

			if (width % 8 != 0)
				throw new ArgumentException("width");

			m_width = width;
			m_bytesPerRow = m_width / 8;
			m_height = height;
			m_state = Array.AsReadOnly(new byte[m_bytesPerRow * m_height]);
		}

		/// <summary>
		/// Gets the width of the grid.
		/// </summary>
		public int Width
		{
			get { return m_width; }
		}

		/// <summary>
		/// Gets the height of the grid.
		/// </summary>
		public int Height
		{
			get { return m_height; }
		}

		/// <summary>
		/// Returns a new <see cref="LifeGrid"/> instance with the specified cell set as "live".
		/// </summary>
		/// <param name="x">The x-coordinate of the cell to enable.</param>
		/// <param name="y">The y-coordinate of the cell to enable.</param>
		public LifeGrid SetLiveCell(int x, int y)
		{
			if (x < 0 || x >= Width)
				throw new ArgumentOutOfRangeException("x");
			if (y < 0 || y >= Height)
				throw new ArgumentOutOfRangeException("y");

			byte[] newState = new byte[m_state.Count];
			m_state.CopyTo(newState, 0);

			int blockIndex = y * m_bytesPerRow + x / 8;
			newState[blockIndex] |= (byte) (0x80 >> (x % 8));

			return new LifeGrid(this, newState);
		}

		/// <summary>
		/// Returns a sequence of "live" cells for the grid's current generation.
		/// </summary>
		/// <remarks>This method is implemented with deferred execution.</remarks>
		public IEnumerable<LifeCell> GetLiveCells()
		{
			for (int blockIndex = 0; blockIndex < m_state.Count; blockIndex++)
			{
				byte block = m_state[blockIndex];

				// if there are no live cells in this block, skip it
				if (block == 0)
					continue;

				int y = blockIndex / m_bytesPerRow;
				int baseX = (blockIndex % m_bytesPerRow) * 8;

				for (int cellIndex = 0; cellIndex < 8; cellIndex++)
				{
					if ((block & (0x80 >> cellIndex)) != 0)
						yield return new LifeCell(baseX + cellIndex, y);
				}
			}
		}

		/// <summary>
		/// Returns a new <see cref="LifeGrid"/> instance representing the next generation of the automaton.
		/// </summary>
		/// <returns></returns>
		public LifeGrid Step()
		{
			byte[] newState = new byte[m_state.Count];

			for (int blockIndex = 0; blockIndex < m_state.Count; blockIndex++)
			{
				int y = blockIndex / m_bytesPerRow;
				int blockColumn = blockIndex % m_bytesPerRow;

				int indexBlockAbove = ((y + m_height - 1) % m_height) * m_bytesPerRow + blockColumn;
				int indexBlockBelow = ((y + 1) % m_height) * m_bytesPerRow + blockColumn;

				int leftBlockOffset = -1;
				if (blockColumn == 0)
					leftBlockOffset += m_bytesPerRow;

				int rightBlockOffset = 1;
				if (blockColumn == m_bytesPerRow - 1)
					rightBlockOffset -= m_bytesPerRow;

				// if the current and surrounding blocks don't contain any live cells ...
				if ((m_state[indexBlockAbove + leftBlockOffset] + m_state[indexBlockAbove] + m_state[indexBlockAbove + rightBlockOffset] +
					m_state[blockIndex + leftBlockOffset] + m_state[blockIndex] + m_state[blockIndex + rightBlockOffset] +
					m_state[indexBlockBelow + leftBlockOffset] + m_state[indexBlockBelow] + m_state[indexBlockBelow + rightBlockOffset]) == 0)
				{
					// ... skip it.
					continue;
				}

				for (int cellIndex = 0; cellIndex < 8; cellIndex++)
				{
					byte centerMask = (byte) (0x80 >> cellIndex);
					byte leftMask = (byte) ((centerMask << 1) | (centerMask >> 7));
					byte rightMask = (byte) ((centerMask >> 1) | (centerMask << 7));

					int cellLeftBlockOffset = cellIndex == 0 ? leftBlockOffset : 0;
					int cellRightBlockOffset = cellIndex == 7 ? rightBlockOffset : 0;

					int aliveNeighbors = 0;

					// top row
					if ((m_state[indexBlockAbove + cellLeftBlockOffset] & leftMask) != 0)
						aliveNeighbors++;
					if ((m_state[indexBlockAbove] & centerMask) != 0)
						aliveNeighbors++;
					if ((m_state[indexBlockAbove + cellRightBlockOffset] & rightMask) != 0)
						aliveNeighbors++;

					// center row
					if ((m_state[blockIndex + cellLeftBlockOffset] & leftMask) != 0)
						aliveNeighbors++;
					if ((m_state[blockIndex + cellRightBlockOffset] & rightMask) != 0)
						aliveNeighbors++;

					// bottom row
					if ((m_state[indexBlockBelow + cellLeftBlockOffset] & leftMask) != 0)
						aliveNeighbors++;
					if ((m_state[indexBlockBelow] & centerMask) != 0)
						aliveNeighbors++;
					if ((m_state[indexBlockBelow + cellRightBlockOffset] & rightMask) != 0)
						aliveNeighbors++;

					bool isCurrentCellAlive = (m_state[blockIndex] & centerMask) != 0;
					if (aliveNeighbors == 3 || (isCurrentCellAlive && aliveNeighbors == 2))
						newState[blockIndex] |= centerMask;
				}
			}

			return new LifeGrid(this, newState);
		}

		private LifeGrid(LifeGrid source, byte[] newState)
		{
			m_width = source.m_width;
			m_bytesPerRow = source.m_bytesPerRow;
			m_height = source.m_height;
			m_state = Array.AsReadOnly(newState);
		}

		readonly int m_width;
		readonly int m_bytesPerRow;
		readonly int m_height;
		readonly ReadOnlyCollection<byte> m_state;
	}
}
