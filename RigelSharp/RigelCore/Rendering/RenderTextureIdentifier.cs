using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;

namespace Rigel.Rendering
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


        public static readonly RenderTextureIdentifier RenderTargetView = new RenderTextureIdentifier(InternalRenderTextureIdentifier.RenderTargetView);
        public static readonly RenderTextureIdentifier DepthStencilView = new RenderTextureIdentifier(InternalRenderTextureIdentifier.DepthStencilView);

        public RenderTextureIdentifier(Texture2D texture)
        {
            m_tex = texture;
        }

        private RenderTextureIdentifier(InternalRenderTextureIdentifier identifier)
        {
            m_identifier = identifier;
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

        public override bool Equals(object obj)
        {
            RenderTextureIdentifier other = obj as RenderTextureIdentifier;
            if (other == null) return false;
            if (other == this) return true;

            if (m_tex == other.m_tex) return true;
            return false;
        }

        public static readonly RenderTextureIdentifier DefaultRenderTarget = new RenderTextureIdentifier(InternalRenderTextureIdentifier.RenderTargetView);
        public static readonly RenderTextureIdentifier DefaultDepthStencilView = new RenderTextureIdentifier(InternalRenderTextureIdentifier.DepthStencilView);
    }
}
