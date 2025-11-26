using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Client.Envir;
using Client.Rendering;
using Client.Rendering.SharpDXD3D11;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using Color = System.Drawing.Color;
using Device = SharpDX.Direct3D11.Device;
using NumericsVector2 = System.Numerics.Vector2;
using NumericsVector4 = System.Numerics.Vector4;

namespace Client.Rendering.PixelShaders
{
    public sealed class OutlinePixelShader : IPixelShader
    {
        private const string ShaderRelativePath = "Shaders/Outline.hlsl";

        private static VertexShader _vertexShader;
        private static PixelShader _pixelShader;
        private static InputLayout _inputLayout;
        private static Buffer _matrixBuffer;
        private static Buffer _outlineBuffer;
        private static SamplerState _samplerState;
        private static BlendState _blendState;
        private static bool _resourcesReady;

        private readonly int _thickness;
        private readonly Color _outlineColour;

        public OutlinePixelShader(int thickness, Color outlineColour)
        {
            if (thickness <= 0)
                throw new ArgumentOutOfRangeException(nameof(thickness));

            _thickness = thickness;
            _outlineColour = outlineColour;
        }

        public PixelShaderResult Apply(RenderTexture texture, Size sourceSize)
        {
            if (!texture.IsValid)
                throw new ArgumentException("A valid texture handle is required.", nameof(texture));

            if (!CanUseD3D11(texture))
                return PixelShaderResult.FromTexture(texture, sourceSize);

            return ApplyWithD3D11Shader(texture, sourceSize);
        }

        private Size GetOutputSize(Size sourceSize)
        {
            return new Size(sourceSize.Width + _thickness * 2, sourceSize.Height + _thickness * 2);
        }

        private PointF GetOffset()
        {
            return new PointF(-_thickness, -_thickness);
        }

        private bool CanUseD3D11(RenderTexture texture)
        {
            return RenderingPipelineManager.ActivePipelineId == RenderingPipelineIds.SharpDXD3D11 &&
                   texture.NativeHandle is SharpD3D11TextureResource;
        }

        private PixelShaderResult ApplyWithD3D11Shader(RenderTexture texture, Size sourceSize)
        {
            if (texture.NativeHandle is not SharpD3D11TextureResource nativeTexture)
                throw new ArgumentException("Native texture must be a Direct3D11 texture when using the D3D11 pipeline.", nameof(texture));

            EnsureResources();

            Size outputSize = GetOutputSize(sourceSize);
            RenderTargetResource renderTarget = RenderingPipelineManager.CreateRenderTarget(outputSize);

            RenderSurface previousSurface = RenderingPipelineManager.GetCurrentSurface();
            RenderingPipelineManager.SetSurface(renderTarget.Surface);
            RenderingPipelineManager.Clear(RenderClearFlags.Target, Color.Transparent, 1f, 0);

            DrawOutline(nativeTexture, sourceSize, outputSize);

            RenderingPipelineManager.SetSurface(previousSurface);

            return new PixelShaderResult(renderTarget.Texture, outputSize, GetOffset());
        }

        private void DrawOutline(SharpD3D11TextureResource texture, Size sourceSize, Size outputSize)
        {
            Device device = SharpDXD3D11Manager.Device;
            DeviceContext context = SharpDXD3D11Manager.Context;

            if (device == null || context == null)
                throw new InvalidOperationException("Direct3D11 device is not available.");

            RawViewportF[] originalViewports = context.Rasterizer.GetViewports<RawViewportF>();
            BlendState previousBlendState = context.OutputMerger.GetBlendState(out RawColor4 previousBlendFactor, out int previousSampleMask);

            RawViewportF viewport = new RawViewportF
            {
                X = 0,
                Y = 0,
                Width = outputSize.Width,
                Height = outputSize.Height,
                MinDepth = 0,
                MaxDepth = 1
            };

            context.Rasterizer.SetViewports(viewport);

            Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(0, outputSize.Width, outputSize.Height, 0, 0f, 1f);
            context.UpdateSubresource(ref projection, _matrixBuffer);

            float texelWidth = 1f / sourceSize.Width;
            float texelHeight = 1f / sourceSize.Height;

            OutlineBuffer outlineBuffer = new OutlineBuffer
            {
                TexelSize = new NumericsVector2(texelWidth, texelHeight),
                Thickness = _thickness,
                Padding = 0f,
                OutlineColor = new NumericsVector4(_outlineColour.R / 255f, _outlineColour.G / 255f, _outlineColour.B / 255f, _outlineColour.A / 255f)
            };

            context.UpdateSubresource(ref outlineBuffer, _outlineBuffer);

            Vertex[] vertices =
            {
                new Vertex(new NumericsVector2(0, 0), new NumericsVector2(-_thickness * texelWidth, -_thickness * texelHeight)),
                new Vertex(new NumericsVector2(outputSize.Width, 0), new NumericsVector2(1 + _thickness * texelWidth, -_thickness * texelHeight)),
                new Vertex(new NumericsVector2(0, outputSize.Height), new NumericsVector2(-_thickness * texelWidth, 1 + _thickness * texelHeight)),
                new Vertex(new NumericsVector2(outputSize.Width, outputSize.Height), new NumericsVector2(1 + _thickness * texelWidth, 1 + _thickness * texelHeight))
            };

            using Buffer vertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices);
            context.InputAssembler.InputLayout = _inputLayout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Marshal.SizeOf<Vertex>(), 0));

            using ShaderResourceView shaderResource = new ShaderResourceView(device, texture.Texture);

            context.VertexShader.Set(_vertexShader);
            context.VertexShader.SetConstantBuffer(0, _matrixBuffer);

            context.PixelShader.Set(_pixelShader);
            context.PixelShader.SetConstantBuffer(1, _outlineBuffer);
            context.PixelShader.SetSampler(0, _samplerState);
            context.PixelShader.SetShaderResource(0, shaderResource);

            context.OutputMerger.SetBlendState(_blendState, new RawColor4(0, 0, 0, 0), -1);

            context.Draw(4, 0);

            context.PixelShader.SetShaderResource(0, null);
            context.OutputMerger.SetBlendState(previousBlendState, previousBlendFactor, previousSampleMask);
            context.Rasterizer.SetViewports(originalViewports);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Vertex
        {
            public NumericsVector2 Position;
            public NumericsVector2 TexCoord;

            public Vertex(NumericsVector2 position, NumericsVector2 texCoord)
            {
                Position = position;
                TexCoord = texCoord;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct OutlineBuffer
        {
            public NumericsVector2 TexelSize;
            public float Thickness;
            public float Padding;
            public NumericsVector4 OutlineColor;
        }

        private void EnsureResources()
        {
            if (_resourcesReady)
                return;

            Device device = SharpDXD3D11Manager.Device ?? throw new InvalidOperationException("Direct3D11 device is not available.");

            string shaderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ShaderRelativePath);

            using CompilationResult vsByteCode = ShaderBytecode.CompileFromFile(shaderPath, "VS", "vs_5_0");
            using CompilationResult psByteCode = ShaderBytecode.CompileFromFile(shaderPath, "PS", "ps_5_0");

            _vertexShader = new VertexShader(device, vsByteCode);
            _pixelShader = new PixelShader(device, psByteCode);

            _inputLayout = new InputLayout(device, vsByteCode, new[]
            {
                new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32_Float, 0, 0),
                new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 8, 0)
            });

            _matrixBuffer = new Buffer(device, Marshal.SizeOf<Matrix4x4>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _outlineBuffer = new Buffer(device, Marshal.SizeOf<OutlineBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            _samplerState = new SamplerState(device, new SamplerStateDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunction = Comparison.Never,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            });

            _blendState = new BlendState(device, new BlendStateDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false,
                RenderTarget =
                {
                    [0] = new RenderTargetBlendDescription
                    {
                        IsBlendEnabled = true,
                        SourceBlend = BlendOption.SourceAlpha,
                        DestinationBlend = BlendOption.InverseSourceAlpha,
                        BlendOperation = BlendOperation.Add,
                        SourceAlphaBlend = BlendOption.One,
                        DestinationAlphaBlend = BlendOption.InverseSourceAlpha,
                        AlphaBlendOperation = BlendOperation.Add,
                        RenderTargetWriteMask = ColorWriteMaskFlags.All
                    }
                }
            });

            _resourcesReady = true;
        }
    }
}
