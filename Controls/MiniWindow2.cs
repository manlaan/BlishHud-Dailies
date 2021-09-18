using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Manlaan.Dailies.Models;

namespace Manlaan.Dailies.Controls
{
    /// <summary>
    /// The StandardWindow is a control meant to replicate the standard Guild Wars 2 windows.
    /// </summary>
    public class MiniWindow2 : WindowBase2
    {

        public MiniWindow2(Texture2D background, Rectangle windowRegion, Rectangle contentRegion) {
            this.ConstructWindow(background, windowRegion, contentRegion);
        }

        public IView _view;

        /// <summary>
        /// Shows the window with the provided view.
        /// </summary>
        public void Show(IView view) {
            _view = view;
            this.ShowView(view);
            base.Show();
        }

        /// <summary>
        /// Shows the window with the provided view if it is hidden.
        /// Hides the window if it is currently showing.
        /// </summary>
        public void ToggleWindow(IView view) {
            _view = view;
            if (this.Visible) {
                Hide();
            }
            else {
                Show(view);
            }
        }

    }
}
