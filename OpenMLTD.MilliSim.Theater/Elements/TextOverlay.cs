using System.IO;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Rendering;
using OpenMLTD.MilliSim.Rendering.Drawing;
using OpenMLTD.MilliSim.Rendering.Extensions;
using FontStyle = OpenMLTD.MilliSim.Rendering.FontStyle;

namespace OpenMLTD.MilliSim.Theater.Elements {
    /// <summary>
    /// A basic text overlay.
    /// Suggested for dynamic texts or long texts.
    /// </summary>
    public class TextOverlay : TextOverlayBase {

        public TextOverlay(GameBase game)
            : base(game) {
        }

        public override string Name { get; set; } = "Text Overlay";

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var text = Text;
            if (!string.IsNullOrWhiteSpace(text)) {
                var textSize = DirectWriteHelper.MeasureText(context.DirectWriteFactory, text, _font);
                var lineHeight = DirectWriteHelper.PointToDip(FontSize);
                OnBeforeTextRendering(context, textSize, lineHeight);

                var location = Location;
                context.Begin2D();
                context.DrawText(text, _textBrush, _font, location.X, location.Y);
                context.End2D();
            }
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);
            var fontPath = Path.GetFullPath(Program.Settings.Window.Fonts.UI);
            var familyName = DirectWriteHelper.GetFontFamilyName(fontPath);
            _font = new D2DFont(context.DirectWriteFactory, familyName, FontSize, FontStyle.Regular, 10);

            _textBrush = new D2DSolidBrush(context, FillColor);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            _textBrush.Dispose();
            _font.Dispose();
        }

        private ID2DBrush _textBrush;
        private D2DFont _font;

    }
}