using System.Numerics;

namespace _02_RayTracing.Rendering;

public static class RenderingUtils
{
		/// <summary>
		/// Maps the Pixels of screen space to screen projection space (from -1 to 1)
		/// </summary>
		public static Vector2 PixelToScreenProjectionSpace(int x, int y, int width, int height)
		{
				var xNormed = 2 * ((float)x - ((float)(width - 1) / 2f)) / (float)width; // Between -1 and 1
				var yNormed = 2 * ((float)(height - 1 - y) - ((float)(height - 1)) / 2f) / (float)height; // Between -1 and 1

				return new Vector2(xNormed, yNormed);
		}

		public static Vector2 ScreenProjectionSpaceToPixel(Vector2 screenProjectionPixel, int width, int height)
		{
				return new Vector2((int)((screenProjectionPixel.X + 1) / 2 * width),
					(int)(height - ((screenProjectionPixel.Y + 1) / 2 * height)));
		}
}
