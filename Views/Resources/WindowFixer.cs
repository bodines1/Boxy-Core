using System.Windows;

namespace Boxy_Core.Views.Resources
{
    /// <summary>
    /// Has methods for fixing the layout of a window if it was, for example, closed off screen.
    /// </summary>
    public static class WindowFixer
    {
        /// <summary>
        /// Sizes a window to fit the current desktop if it is too large.
        /// </summary>
        /// <param name="window">Window to manipulate.</param>
        public static void SizeToFit(Window window)
        {
            if (window.Height > SystemParameters.VirtualScreenHeight)
            {
                window.Height = SystemParameters.VirtualScreenHeight;
            }

            if (window.Width > SystemParameters.VirtualScreenWidth)
            {
                window.Width = SystemParameters.VirtualScreenWidth;
            }
        }

        /// <summary>
        /// Moves a window back onto the desktop if it is too far out of bounds. This is not perfect, and has some issues with monitors/resolutions of different sizes being combined. However it should work in most real world cases.
        /// </summary>
        /// <param name="window">Window to manipulate.</param>
        public static void MoveIntoView(Window window)
        {
            // Fix if the window is under the screen
            if (window.Top + window.Height > SystemParameters.VirtualScreenHeight)
            {
                window.Top = SystemParameters.VirtualScreenHeight - window.Height;
            }

            // Fix if the window is to the right of the screen
            if (window.Left + window.Width > SystemParameters.VirtualScreenWidth)
            {
                window.Left = SystemParameters.VirtualScreenWidth - window.Width;
            }
            
            // Fix if the window is above the screen
            if (window.Top < SystemParameters.VirtualScreenTop)
            {
                window.Top = SystemParameters.VirtualScreenTop;
            }
            
            // Fix if the window is to the left of the screen
            if (window.Left < SystemParameters.VirtualScreenLeft)
            {
                window.Left = SystemParameters.VirtualScreenLeft;
            }
        }
    }
}
