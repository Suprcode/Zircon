using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;
using D3DCompiler = SharpDX.D3DCompiler.ShaderBytecode;
using D3DCompilerShaderFlags = SharpDX.D3DCompiler.ShaderFlags;
using EffectFlags = SharpDX.D3DCompiler.EffectFlags;
using DxVector2 = SharpDX.Vector2;
using NumericsMatrix3x2 = System.Numerics.Matrix3x2;

namespace Client.Rendering.SharpDXD3D9
{
    public sealed class SharpDXD3D9SpriteRenderer : IDisposable
    {
        private readonly Device _device;

        private VertexShader _vertexShader;
        private PixelShader _outlinePixelShader;
        private VertexBuffer _vertexBuffer;
        private VertexDeclaration _vertexDeclaration;

        private const string OutlineShaderFileName = "OutlineSpriteD3D9.hlsl";

        [StructLayout(LayoutKind.Sequential)]
        private struct VertexType
        {
            public DxVector2 Position;
            public DxVector2 TexCoord;
            public RawColorBGRA Color;
        }

        public bool SupportsOutlineShader => _outlinePixelShader != null && _vertexShader != null;

        public SharpDXD3D9SpriteRenderer(Device device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            InitializeShaders();
            InitializeBuffers();
        }

        private void InitializeShaders()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
            string[] candidates = new[]
            {
                Path.Combine(baseDirectory, "Rendering", "Shaders", OutlineShaderFileName),
                Path.Combine(baseDirectory, "Shaders", OutlineShaderFileName),
                Path.Combine(baseDirectory, OutlineShaderFileName)
            };

            string shaderPath = null;

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    shaderPath = candidate;
                    break;
                }
            }

            if (shaderPath == null)
                return;

            using (var vertexByteCode = D3DCompiler.CompileFromFile(shaderPath, "VS", "vs_3_0", D3DCompilerShaderFlags.OptimizationLevel3, EffectFlags.None))
            using (var pixelByteCode = D3DCompiler.CompileFromFile(shaderPath, "PS_OUTLINE", "ps_3_0", D3DCompilerShaderFlags.OptimizationLevel3, EffectFlags.None))
            {
                _vertexShader = new VertexShader(_device, vertexByteCode);
                _outlinePixelShader = new PixelShader(_device, pixelByteCode);
            }
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

            if (outlineThickness > 0)
            {
                left -= outlineThickness;
                right += outlineThickness;
                top -= outlineThickness;
                bottom += outlineThickness;

                float uPad = outlineThickness / desc.Width;
                float vPad = outlineThickness / desc.Height;
                u1 -= uPad;
                v1 -= vPad;
                u2 += uPad;
                v2 += vPad;
            }

            var vertexColor = new RawColorBGRA(color.R, color.G, color.B, color.A);

            var viewport = _device.Viewport;

            UpdateMatrix(transform, viewport.Width, viewport.Height);
            UpdateOutlineConstants(desc.Width, desc.Height, outlineColor, outlineThickness, u1, v1, u2, v2);

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
            _device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
            _device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
            _device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);

            _device.VertexDeclaration = _vertexDeclaration;
            _device.SetStreamSource(0, _vertexBuffer, 0, Marshal.SizeOf<VertexType>());
            _device.VertexShader = _vertexShader;
            _device.PixelShader = _outlinePixelShader;

            _device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            stateBlock.Apply();
        }

        private void UpdateMatrix(NumericsMatrix3x2 transform, int backBufferWidth, int backBufferHeight)
        {
            Matrix projection = Matrix.Identity;
            projection.M11 = 2f / backBufferWidth;
            projection.M22 = -2f / backBufferHeight;
            projection.M41 = -1f;
            projection.M42 = 1f;

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

        public void Dispose()
        {
            _vertexBuffer?.Dispose();
            _vertexDeclaration?.Dispose();
            _vertexShader?.Dispose();
            _outlinePixelShader?.Dispose();
        }
    }
}
