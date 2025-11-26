using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Client.Envir;
using Client.Rendering;
using Client.Rendering.SharpDXD3D11;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Color = System.Drawing.Color;
using Device = SharpDX.Direct3D11.Device;
using NumericsVector2 = System.Numerics.Vector2;
using NumericsVector4 = System.Numerics.Vector4;

namespace Client.Rendering.PixelShaders
{
    public sealed class OutlinePixelShader : IPixelShader
    {
        private readonly int _thickness;
        private readonly Color _outlineColour;
        private readonly Dictionary<object, RenderTargetResource> _cache = new();

        private D3D11OutlineResources _d3D11Resources;

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

            if (_cache.TryGetValue(texture.NativeHandle!, out RenderTargetResource cachedTarget) && cachedTarget.IsValid)
            {
                return new PixelShaderResult(cachedTarget.Texture, GetOutputSize(sourceSize), GetOffset());
            }

            if (CanUseD3D11(texture))
                return ApplyWithD3D11Shader(texture, sourceSize);

            return ApplyWithFallbackDraw(texture, sourceSize);
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

        private PixelShaderResult ApplyWithFallbackDraw(RenderTexture texture, Size sourceSize)
        {
            Size outputSize = GetOutputSize(sourceSize);
            RenderTargetResource renderTarget = RenderingPipelineManager.CreateRenderTarget(outputSize);

            RenderSurface previousSurface = RenderingPipelineManager.GetCurrentSurface();
            bool oldBlend = RenderingPipelineManager.IsBlending();
            float oldRate = RenderingPipelineManager.GetBlendRate();
            BlendMode oldMode = RenderingPipelineManager.GetBlendMode();

            RenderingPipelineManager.SetSurface(renderTarget.Surface);
            RenderingPipelineManager.Clear(RenderClearFlags.Target, Color.Transparent, 1f, 0);
            RenderingPipelineManager.SetBlend(true, 1f, BlendMode.NORMAL);

            Rectangle sourceRectangle = new Rectangle(Point.Empty, sourceSize);
            RectangleF destination = new RectangleF(_thickness, _thickness, sourceSize.Width, sourceSize.Height);

            for (int dx = -_thickness; dx <= _thickness; dx += _thickness)
            {
                for (int dy = -_thickness; dy <= _thickness; dy += _thickness)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    RectangleF offsetDestination = new RectangleF(destination.X + dx, destination.Y + dy, destination.Width, destination.Height);
                    RenderingPipelineManager.DrawTexture(texture, sourceRectangle, offsetDestination, _outlineColour);
                }
            }

            RenderingPipelineManager.DrawTexture(texture, sourceRectangle, destination, Color.White);

            RenderingPipelineManager.SetBlend(oldBlend, oldRate, oldMode);
            RenderingPipelineManager.SetSurface(previousSurface);

            _cache[texture.NativeHandle!] = renderTarget;

            return new PixelShaderResult(renderTarget.Texture, outputSize, GetOffset());
        }

        private PixelShaderResult ApplyWithD3D11Shader(RenderTexture texture, Size sourceSize)
        {
            if (texture.NativeHandle is not SharpD3D11TextureResource nativeTexture)
                throw new ArgumentException("Native texture must be a Direct3D11 texture when using the D3D11 pipeline.", nameof(texture));

            EnsureD3D11Resources();

            Size outputSize = GetOutputSize(sourceSize);
            RenderTargetResource renderTarget = RenderingPipelineManager.CreateRenderTarget(outputSize);

            RenderSurface previousSurface = RenderingPipelineManager.GetCurrentSurface();
            RenderingPipelineManager.SetSurface(renderTarget.Surface);

            DrawOutline(nativeTexture, sourceSize, outputSize);

            RenderingPipelineManager.SetSurface(previousSurface);

            _cache[texture.NativeHandle!] = renderTarget;

            return new PixelShaderResult(renderTarget.Texture, outputSize, GetOffset());
        }

        private void DrawOutline(SharpD3D11TextureResource texture, Size sourceSize, Size outputSize)
        {
            Device device = SharpDXD3D11Manager.Device;
            DeviceContext context = SharpDXD3D11Manager.Context;

            if (device == null || context == null)
                throw new InvalidOperationException("Direct3D11 device is not available.");

            ViewportF[] originalViewports = context.Rasterizer.GetViewports<ViewportF>();
            BlendState previousBlendState = context.OutputMerger.GetBlendState(out RawColor4 previousBlendFactor, out int previousSampleMask);

            context.Rasterizer.SetViewport(new ViewportF(0, 0, outputSize.Width, outputSize.Height, 0, 1));

            Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(0, outputSize.Width, outputSize.Height, 0, 0f, 1f);
            context.UpdateSubresource(ref projection, _d3D11Resources.MatrixBuffer);

            float texelWidth = 1f / sourceSize.Width;
            float texelHeight = 1f / sourceSize.Height;

            OutlineBuffer outlineBuffer = new OutlineBuffer
            {
                TexelSize = new NumericsVector2(texelWidth, texelHeight),
                Thickness = _thickness,
                Padding = 0f,
                OutlineColor = new NumericsVector4(_outlineColour.R / 255f, _outlineColour.G / 255f, _outlineColour.B / 255f, _outlineColour.A / 255f)
            };

            context.UpdateSubresource(ref outlineBuffer, _d3D11Resources.OutlineBuffer);

            Vertex[] vertices =
            {
                new Vertex(new NumericsVector2(0, 0), new NumericsVector2(-_thickness * texelWidth, -_thickness * texelHeight)),
                new Vertex(new NumericsVector2(outputSize.Width, 0), new NumericsVector2(1 + _thickness * texelWidth, -_thickness * texelHeight)),
                new Vertex(new NumericsVector2(0, outputSize.Height), new NumericsVector2(-_thickness * texelWidth, 1 + _thickness * texelHeight)),
                new Vertex(new NumericsVector2(outputSize.Width, outputSize.Height), new NumericsVector2(1 + _thickness * texelWidth, 1 + _thickness * texelHeight))
            };

            using Buffer vertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices);
            context.InputAssembler.InputLayout = _d3D11Resources.InputLayout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Marshal.SizeOf<Vertex>(), 0));

            context.VertexShader.Set(_d3D11Resources.VertexShader);
            context.VertexShader.SetConstantBuffer(0, _d3D11Resources.MatrixBuffer);

            context.PixelShader.Set(_d3D11Resources.PixelShader);
            context.PixelShader.SetConstantBuffer(0, _d3D11Resources.OutlineBuffer);
            context.PixelShader.SetSampler(0, _d3D11Resources.SamplerState);
            context.PixelShader.SetShaderResource(0, GetShaderResourceView(texture));

            context.OutputMerger.SetBlendState(_d3D11Resources.BlendState, new RawColor4(0, 0, 0, 0), -1);

            context.Draw(4, 0);

            context.PixelShader.SetShaderResource(0, null);
            context.OutputMerger.SetBlendState(previousBlendState, previousBlendFactor, previousSampleMask);
            context.Rasterizer.SetViewports(originalViewports);
        }

        private ShaderResourceView GetShaderResourceView(SharpD3D11TextureResource texture)
        {
            if (!_d3D11Resources.ShaderResourceViews.TryGetValue(texture.Texture, out ShaderResourceView view) || view.IsDisposed)
            {
                view = new ShaderResourceView(SharpDXD3D11Manager.Device, texture.Texture);
                _d3D11Resources.ShaderResourceViews[texture.Texture] = view;
            }

            return view;
        }

        private void EnsureD3D11Resources()
        {
            if (_d3D11Resources != null)
                return;

            Device device = SharpDXD3D11Manager.Device;

            if (device == null)
                throw new InvalidOperationException("Direct3D11 device is not available.");

            _d3D11Resources = new D3D11OutlineResources(device);
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

        private sealed class D3D11OutlineResources : IDisposable
        {
            public readonly VertexShader VertexShader;
            public readonly PixelShader PixelShader;
            public readonly InputLayout InputLayout;
            public readonly Buffer MatrixBuffer;
            public readonly Buffer OutlineBuffer;
            public readonly SamplerState SamplerState;
            public readonly BlendState BlendState;
            public readonly Dictionary<Texture2D, ShaderResourceView> ShaderResourceViews = new();

            public D3D11OutlineResources(Device device)
            {
                if (device == null)
                    throw new ArgumentNullException(nameof(device));

                using CompilationResult vsByteCode = ShaderBytecode.Compile(ShaderSource, "VS", "vs_5_0");
                using CompilationResult psByteCode = ShaderBytecode.Compile(ShaderSource, "PS", "ps_5_0");

                VertexShader = new VertexShader(device, vsByteCode);
                PixelShader = new PixelShader(device, psByteCode);

                InputLayout = new InputLayout(device, vsByteCode, new[]
                {
                    new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32_Float, 0, 0),
                    new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 8, 0)
                });

                MatrixBuffer = new Buffer(device, Marshal.SizeOf<Matrix4x4>(),
                    ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

                OutlineBuffer = new Buffer(device, Marshal.SizeOf<OutlineBuffer>(),
                    ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

                SamplerState = new SamplerState(device, new SamplerStateDescription
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp,
                    AddressW = TextureAddressMode.Clamp,
                    ComparisonFunction = Comparison.Never,
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                });

                BlendState = new BlendState(device, new BlendStateDescription
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
            }

            public void Dispose()
            {
                foreach (ShaderResourceView view in ShaderResourceViews.Values)
                    view?.Dispose();

                SamplerState?.Dispose();
                BlendState?.Dispose();
                MatrixBuffer?.Dispose();
                OutlineBuffer?.Dispose();
                InputLayout?.Dispose();
                PixelShader?.Dispose();
                VertexShader?.Dispose();
            }

            private const string ShaderSource = @"
cbuffer MatrixBuffer : register(b0)
{
    matrix Projection;
};

cbuffer OutlineBuffer : register(b0)
{
    float2 TexelSize;
    float Thickness;
    float Padding;
    float4 OutlineColor;
};

struct VS_INPUT
{
    float2 Pos : POSITION;
    float2 Tex : TEXCOORD;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Projection);
    output.Tex = input.Tex;
    return output;
}

Texture2D shaderTexture : register(t0);
SamplerState sampleState : register(s0);

float4 PS(PS_INPUT input) : SV_Target
{
    float4 baseColor = shaderTexture.Sample(sampleState, input.Tex);

    if (baseColor.a > 0.0001)
        return baseColor;

    float maxAlpha = 0.0;

    [unroll]
    for (int x = -4; x <= 4; ++x)
    {
        [unroll]
        for (int y = -4; y <= 4; ++y)
        {
            if (abs(x) == 0 && abs(y) == 0)
                continue;

            if (abs(x) > Thickness || abs(y) > Thickness)
                continue;

            float2 offset = input.Tex + float2(x * TexelSize.x, y * TexelSize.y);
            float alpha = shaderTexture.Sample(sampleState, offset).a;
            maxAlpha = max(maxAlpha, alpha);
        }
    }

    if (maxAlpha > 0.0001)
        return float4(OutlineColor.rgb, OutlineColor.a * maxAlpha);

    return float4(0, 0, 0, 0);
}
";
        }
    }
}
