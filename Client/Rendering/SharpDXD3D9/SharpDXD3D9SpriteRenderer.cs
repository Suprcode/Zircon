using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;
using D3D9ShaderBytecode = SharpDX.Direct3D9.ShaderBytecode;
using D3DCompilerBytecode = SharpDX.D3DCompiler.ShaderBytecode;
using D3DCompilerShaderFlags = SharpDX.D3DCompiler.ShaderFlags;
using DxVector2 = SharpDX.Vector2;
using EffectFlags = SharpDX.D3DCompiler.EffectFlags;
using NumericsMatrix3x2 = System.Numerics.Matrix3x2;

namespace Client.Rendering.SharpDXD3D9
{
    public sealed class SharpDXD3D9SpriteRenderer : IDisposable
    {
        private readonly Device _device;

        private VertexShader _vertexShader;
        private VertexShader _shadowVertexShader;
        private PixelShader _outlinePixelShader;
        private PixelShader _grayscalePixelShader;
        private PixelShader _dropShadowPixelShader;
        private VertexBuffer _vertexBuffer;
        private VertexDeclaration _vertexDeclaration;

        private const string OutlineShaderFileName = "OutlineD3D9.hlsl";
        private const string GrayscaleShaderFileName = "GrayscaleD3D9.hlsl";
        private const string DropShadowShaderFileName = "DropShadowD3D9.hlsl";

        [StructLayout(LayoutKind.Sequential)]
        private struct VertexType
        {
            public DxVector2 Position;
            public DxVector2 TexCoord;
            public RawColorBGRA Color;
        }

        public bool SupportsOutlineShader => _outlinePixelShader != null && _vertexShader != null;
        public bool SupportsGrayscaleShader => _grayscalePixelShader != null && _vertexShader != null;
        public bool SupportsDropShadowShader => _dropShadowPixelShader != null && _shadowVertexShader != null;

        public SharpDXD3D9SpriteRenderer(Device device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            InitializeShaders();
            InitializeBuffers();
        }

        private void InitializeShaders()
        {
            InitializeOutlineShader();
            InitializeGrayscaleShader();
            InitializeDropShadowShader();
        }

        private void InitializeOutlineShader()
        {
            string shaderPath = FindShaderPath(OutlineShaderFileName);

            if (string.IsNullOrEmpty(shaderPath) || !File.Exists(shaderPath))
                return;

            using (var compiledVertex = D3DCompilerBytecode.CompileFromFile(shaderPath, "VS", "vs_3_0", D3DCompilerShaderFlags.OptimizationLevel3, EffectFlags.None))
            using (var compiledPixel = D3DCompilerBytecode.CompileFromFile(shaderPath, "PS_OUTLINE", "ps_3_0", D3DCompilerShaderFlags.OptimizationLevel3, EffectFlags.None))
            using (var vertexByteCode = new D3D9ShaderBytecode(compiledVertex))
            using (var pixelByteCode = new D3D9ShaderBytecode(compiledPixel))
            {
                _vertexShader = new VertexShader(_device, vertexByteCode);
                _outlinePixelShader = new PixelShader(_device, pixelByteCode);
            }
        }

        private void InitializeGrayscaleShader()
        {
            string shaderPath = FindShaderPath(GrayscaleShaderFileName);

            if (string.IsNullOrEmpty(shaderPath) || !File.Exists(shaderPath))
                return;

            using (var compiledPixel = D3DCompilerBytecode.CompileFromFile(shaderPath, "PS_GRAY", "ps_3_0", D3DCompilerShaderFlags.OptimizationLevel3, EffectFlags.None))
            using (var pixelByteCode = new D3D9ShaderBytecode(compiledPixel))
            {
                _grayscalePixelShader = new PixelShader(_device, pixelByteCode);
            }
        }

        private void InitializeDropShadowShader()
        {
            string shaderPath = FindShaderPath(DropShadowShaderFileName);

            if (string.IsNullOrEmpty(shaderPath) || !File.Exists(shaderPath))
                return;

            using (var compiledVertex = D3DCompilerBytecode.CompileFromFile(shaderPath, "VS", "vs_3_0", D3DCompilerShaderFlags.OptimizationLevel3, EffectFlags.None))
            using (var compiledPixel = D3DCompilerBytecode.CompileFromFile(shaderPath, "PS_SHADOW", "ps_3_0", D3DCompilerShaderFlags.OptimizationLevel3, EffectFlags.None))
            using (var vertexByteCode = new D3D9ShaderBytecode(compiledVertex))
            using (var pixelByteCode = new D3D9ShaderBytecode(compiledPixel))
            {
                _shadowVertexShader = new VertexShader(_device, vertexByteCode);
                _dropShadowPixelShader = new PixelShader(_device, pixelByteCode);
            }
        }

        private static string FindShaderPath(string filename)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;

            string[] candidates = new[]
            {
                Path.Combine(baseDirectory, "Rendering", "SharpDXD3D9", "Shaders", filename)
            };

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                    return candidate;
            }

            return null;
        }

        private void InitializeBuffers()
        {
            _vertexBuffer = new VertexBuffer(_device, Marshal.SizeOf<VertexType>() * 4, Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

            _vertexDeclaration = new VertexDeclaration(_device, new[]
            {
                new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 8, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            });
        }

        public void DrawOutlined(Texture texture, System.Drawing.RectangleF destination, System.Drawing.Rectangle? source, System.Drawing.Color color, NumericsMatrix3x2 transform, Color4 outlineColor, float outlineThickness)
        {
            if (!SupportsOutlineShader || texture == null || texture.IsDisposed)
                return;

            float effectiveThickness = outlineThickness > 0 ? 1.0f : 0.0f;

            using var stateBlock = new StateBlock(_device, StateBlockType.All);
            stateBlock.Capture();

            var desc = texture.GetLevelDescription(0);

            float left = destination.Left;
            float right = destination.Right;
            float top = destination.Top;
            float bottom = destination.Bottom;

            float u1 = 0, v1 = 0, u2 = 1, v2 = 1;
            if (source.HasValue)
            {
                u1 = source.Value.Left / (float)desc.Width;
                v1 = source.Value.Top / (float)desc.Height;
                u2 = source.Value.Right / (float)desc.Width;
                v2 = source.Value.Bottom / (float)desc.Height;
            }

            if (effectiveThickness > 0)
            {
                left -= effectiveThickness;
                right += effectiveThickness;
                top -= effectiveThickness;
                bottom += effectiveThickness;

                float uPad = effectiveThickness / desc.Width;
                float vPad = effectiveThickness / desc.Height;
                u1 -= uPad;
                v1 -= vPad;
                u2 += uPad;
                v2 += vPad;
            }

            var vertexColor = new RawColorBGRA(color.R, color.G, color.B, color.A);

            var viewport = _device.Viewport;

            UpdateMatrix(transform, viewport.Width, viewport.Height);
            UpdateOutlineConstants(desc.Width, desc.Height, outlineColor, effectiveThickness, u1, v1, u2, v2);

            DataStream stream = _vertexBuffer.Lock(0, 0, LockFlags.Discard);
            stream.Write(new VertexType { Position = new DxVector2(left, top), TexCoord = new DxVector2(u1, v1), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(right, top), TexCoord = new DxVector2(u2, v1), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(left, bottom), TexCoord = new DxVector2(u1, v2), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(right, bottom), TexCoord = new DxVector2(u2, v2), Color = vertexColor });
            _vertexBuffer.Unlock();

            _device.SetRenderState(RenderState.AlphaBlendEnable, true);
            _device.SetRenderState(RenderState.SourceBlend, SharpDX.Direct3D9.Blend.SourceAlpha);
            _device.SetRenderState(RenderState.DestinationBlend, SharpDX.Direct3D9.Blend.InverseSourceAlpha);
            _device.SetRenderState(RenderState.CullMode, Cull.None);

            _device.SetTexture(0, texture);
            _device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
            _device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);
            _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
            _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);

            _device.VertexDeclaration = _vertexDeclaration;
            _device.SetStreamSource(0, _vertexBuffer, 0, Marshal.SizeOf<VertexType>());
            _device.VertexShader = _vertexShader;
            _device.PixelShader = _outlinePixelShader;

            _device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            stateBlock.Apply();
        }

        public void DrawGrayscale(Texture texture, System.Drawing.RectangleF destination, System.Drawing.Rectangle? source, System.Drawing.Color color, NumericsMatrix3x2 transform)
        {
            if (!SupportsGrayscaleShader || texture == null || texture.IsDisposed)
                return;

            using var stateBlock = new StateBlock(_device, StateBlockType.All);
            stateBlock.Capture();

            var desc = texture.GetLevelDescription(0);

            float left = destination.Left;
            float right = destination.Right;
            float top = destination.Top;
            float bottom = destination.Bottom;

            float u1 = 0, v1 = 0, u2 = 1, v2 = 1;
            if (source.HasValue)
            {
                u1 = source.Value.Left / (float)desc.Width;
                v1 = source.Value.Top / (float)desc.Height;
                u2 = source.Value.Right / (float)desc.Width;
                v2 = source.Value.Bottom / (float)desc.Height;
            }

            var vertexColor = new RawColorBGRA(color.R, color.G, color.B, color.A);

            var viewport = _device.Viewport;

            UpdateMatrix(transform, viewport.Width, viewport.Height);

            DataStream stream = _vertexBuffer.Lock(0, 0, LockFlags.Discard);
            stream.Write(new VertexType { Position = new DxVector2(left, top), TexCoord = new DxVector2(u1, v1), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(right, top), TexCoord = new DxVector2(u2, v1), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(left, bottom), TexCoord = new DxVector2(u1, v2), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(right, bottom), TexCoord = new DxVector2(u2, v2), Color = vertexColor });
            _vertexBuffer.Unlock();

            _device.SetRenderState(RenderState.AlphaBlendEnable, SharpDXD3D9Manager.Blending);
            _device.SetRenderState(RenderState.SourceBlend, Blend.InverseDestinationColor);
            _device.SetRenderState(RenderState.DestinationBlend, Blend.One);
            _device.SetRenderState(RenderState.CullMode, Cull.None);

            _device.SetTexture(0, texture);
            _device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
            _device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);
            _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
            _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);

            _device.VertexDeclaration = _vertexDeclaration;
            _device.SetStreamSource(0, _vertexBuffer, 0, Marshal.SizeOf<VertexType>());
            _device.VertexShader = _vertexShader;
            _device.PixelShader = _grayscalePixelShader;

            _device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            stateBlock.Apply();
        }

        public void DrawDropShadow(Texture texture, System.Drawing.RectangleF destination, System.Drawing.RectangleF shadowBounds, System.Drawing.Rectangle? source, System.Drawing.Color color, NumericsMatrix3x2 transform, SharpDX.Mathematics.Interop.RawColor4 shadowColor, float shadowWidth, float shadowMaxOpacity, float shadowOpacityExponent)
        {
            if (!SupportsDropShadowShader || texture == null || texture.IsDisposed)
                return;

            using var stateBlock = new StateBlock(_device, StateBlockType.All);
            stateBlock.Capture();

            var desc = texture.GetLevelDescription(0);

            float imageLeft = shadowBounds.Left;
            float imageRight = shadowBounds.Right;
            float imageTop = shadowBounds.Top;
            float imageBottom = shadowBounds.Bottom;

            float left = destination.Left;
            float right = destination.Right;
            float top = destination.Top;
            float bottom = destination.Bottom;

            float u1 = 0, v1 = 0, u2 = 1, v2 = 1;
            if (source.HasValue)
            {
                u1 = source.Value.Left / (float)desc.Width;
                v1 = source.Value.Top / (float)desc.Height;
                u2 = source.Value.Right / (float)desc.Width;
                v2 = source.Value.Bottom / (float)desc.Height;
            }

            float effectiveWidth = Math.Max(0f, shadowWidth);

            if (effectiveWidth > 0)
            {
                left -= effectiveWidth;
                right += effectiveWidth;
                top -= effectiveWidth;
                bottom += effectiveWidth;

                float uPad = effectiveWidth / desc.Width;
                float vPad = effectiveWidth / desc.Height;
                u1 -= uPad;
                v1 -= vPad;
                u2 += uPad;
                v2 += vPad;
            }

            var vertexColor = new RawColorBGRA(color.R, color.G, color.B, color.A);

            var viewport = _device.Viewport;

            UpdateMatrix(transform, viewport.Width, viewport.Height);
            UpdateShadowConstants(imageLeft, imageTop, imageRight, imageBottom, effectiveWidth, shadowMaxOpacity);

            DataStream stream = _vertexBuffer.Lock(0, 0, LockFlags.Discard);
            stream.Write(new VertexType { Position = new DxVector2(left, top), TexCoord = new DxVector2(u1, v1), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(right, top), TexCoord = new DxVector2(u2, v1), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(left, bottom), TexCoord = new DxVector2(u1, v2), Color = vertexColor });
            stream.Write(new VertexType { Position = new DxVector2(right, bottom), TexCoord = new DxVector2(u2, v2), Color = vertexColor });
            _vertexBuffer.Unlock();

            _device.SetRenderState(RenderState.AlphaBlendEnable, true);
            _device.SetRenderState(RenderState.SourceBlend, SharpDX.Direct3D9.Blend.SourceAlpha);
            _device.SetRenderState(RenderState.DestinationBlend, SharpDX.Direct3D9.Blend.InverseSourceAlpha);
            _device.SetRenderState(RenderState.CullMode, Cull.None);

            _device.SetTexture(0, texture);
            _device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
            _device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);
            _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
            _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);

            _device.VertexDeclaration = _vertexDeclaration;
            _device.SetStreamSource(0, _vertexBuffer, 0, Marshal.SizeOf<VertexType>());
            _device.VertexShader = _shadowVertexShader;
            _device.PixelShader = _dropShadowPixelShader;

            _device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            stateBlock.Apply();
        }

        private void UpdateMatrix(NumericsMatrix3x2 transform, int backBufferWidth, int backBufferHeight)
        {
            Matrix projection = Matrix.Identity;
            projection.M11 = 2f / backBufferWidth;
            projection.M22 = -2f / backBufferHeight;

            // Apply a half-pixel offset to align texels to pixel centers when using point sampling in D3D9.
            float halfPixelX = 1f / backBufferWidth;
            float halfPixelY = 1f / backBufferHeight;
            projection.M41 = -1f - halfPixelX;
            projection.M42 = 1f + halfPixelY;

            Matrix world = Matrix.Identity;
            world.M11 = transform.M11;
            world.M12 = transform.M12;
            world.M21 = transform.M21;
            world.M22 = transform.M22;
            world.M41 = transform.M31;
            world.M42 = transform.M32;

            Matrix final = Matrix.Multiply(world, projection);
            final = Matrix.Transpose(final);

            _device.SetVertexShaderConstant(0, final.ToArray());
        }

        private void UpdateOutlineConstants(int texWidth, int texHeight, Color4 outlineColor, float outlineThickness, float u1, float v1, float u2, float v2)
        {
            _device.SetPixelShaderConstant(4, new[] { outlineColor.Red, outlineColor.Green, outlineColor.Blue, outlineColor.Alpha });
            _device.SetPixelShaderConstant(5, new[] { texWidth, texHeight, outlineThickness, 0f });
            _device.SetPixelShaderConstant(6, new[] { u1, v1, u2, v2 });
        }

        private void UpdateShadowConstants(float imageLeft, float imageTop, float imageRight, float imageBottom, float shadowWidth, float shadowMaxOpacity)
        {
            _device.SetPixelShaderConstant(4, new[] { imageLeft, imageTop, imageRight, imageBottom });
            _device.SetPixelShaderConstant(5, new[] { shadowWidth, shadowMaxOpacity, 0f, 0f });
        }

        public void Dispose()
        {
            _vertexBuffer?.Dispose();
            _vertexDeclaration?.Dispose();
            _vertexShader?.Dispose();
            _outlinePixelShader?.Dispose();
            _grayscalePixelShader?.Dispose();
            _dropShadowPixelShader?.Dispose();
            _shadowVertexShader?.Dispose();
        }
    }
}
