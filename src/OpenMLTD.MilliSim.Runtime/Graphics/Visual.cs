using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    public abstract class Visual : BaseGameComponent, IDrawable {

        protected Visual([NotNull] BaseGame game, [CanBeNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public void Draw(GameTime gameTime) {
            if (Visible) {
                OnDraw(gameTime);
            }
        }

        public int DrawOrder {
            get => _drawOrder;
            set {
                var b = _drawOrder != value;
                _drawOrder = value;

                if (b) {
                    DrawOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool Visible {
            get => _visible;
            set {
                var b = _visible != value;
                _visible = value;

                if (b) {
                    VisibleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;

        public event EventHandler<EventArgs> VisibleChanged;

        protected virtual void OnDraw([NotNull] GameTime gameTime) {
        }

        private int _drawOrder;
        private bool _visible = true;

    }
}
