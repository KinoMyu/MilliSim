﻿using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Rendering {
    public interface IDrawable {

        void Draw(GameTime gameTime, RenderContext context);

        bool Visible { get; set; }
        
        void OnGotContext(RenderContext context);

        void OnLostContext(RenderContext context);

    }
}
