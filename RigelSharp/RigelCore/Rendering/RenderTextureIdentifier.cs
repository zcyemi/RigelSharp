using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;

namespace RigelCore.Rendering
{
    public enum InternalRenderTextureIdentifier
    {
        Default,
        DepthStencilView,
        RenderTargetView,
    }

    public class RenderTextureIdentifier
    {
        private Texture2D m_tex;

        public InternalRenderTextureIdentifier m_identifier = InternalRenderTextureIdentifier.Default;

        public RenderTextureIdentifier(Texture2D texture)
        {
            m_tex = texture;
        }

        public RenderTextureIdentifier(InternalRenderTextureIdentifier identifier)
        {
            m_identifier = identifier;
            m_tex = null;
        }

        public T GetRawRenderTexture<T>(GraphicsContext context) where T: ResourceView
        {
            switch (m_identifier)
            {
                case InternalRenderTextureIdentifier.RenderTargetView:
                    return context.DefaultRenderTargetView as T;
                case InternalRenderTextureIdentifier.DepthStencilView:
                    return context.DefaultDepthStencilView as T;
            }

            return null;
        }

        public static readonly RenderTextureIdentifier DefaultRenderTargetView = new RenderTextureIdentifier(InternalRenderTextureIdentifier.RenderTargetView);
        public static readonly RenderTextureIdentifier DefaultDepthStencilView = new RenderTextureIdentifier(InternalRenderTextureIdentifier.DepthStencilView);
    }
}
