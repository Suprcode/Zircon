using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = System.Drawing.Color;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Matrix3x2 = System.Numerics.Matrix3x2;
using Matrix4x4 = System.Numerics.Matrix4x4;
using RectangleF = System.Drawing.RectangleF;
using Vector2 = System.Numerics.Vector2;

namespace Client.Rendering.SharpDXD3D11
{
    public sealed class SharpDXD3D11SpriteRenderer : IDisposable
    {
        private readonly Device _device;
        private readonly DeviceContext _context;
        private VertexShader _vertexShader;
        private PixelShader _pixelShader;
        private InputLayout _inputLayout;
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private Buffer _matrixBuffer;
        private SamplerState _samplerState;

        private RawViewportF _viewport;
        private bool _pipelineConfigured;
        private ShaderResourceView _currentTextureView;
        private BlendMode _currentBlendMode = (BlendMode)(-1);
        private RawColor4 _currentBlendFactor;
        private VertexBufferBinding _vertexBufferBinding;
        private readonly int _vertexSize = Utilities.SizeOf<VertexType>();
        private const int MaxBatchSize = 1024;
        private readonly VertexType[] _vertexData = new VertexType[MaxBatchSize * 4];
        private int _queuedSprites;

        private readonly Dictionary<BlendMode, BlendState> _blendStates = new Dictionary<BlendMode, BlendState>();
        private readonly Dictionary<Texture2D, SpriteResources> _textureResources = new Dictionary<Texture2D, SpriteResources>();

        private readonly struct SpriteResources
        {
            public ShaderResourceView ShaderResourceView { get; }
            public int Width { get; }
            public int Height { get; }

            public SpriteResources(ShaderResourceView shaderResourceView, int width, int height)
            {
                ShaderResourceView = shaderResourceView;
                Width = width;
                Height = height;
            }
        }

        private static readonly string ShaderSource = @"
            cbuffer MatrixBuffer : register(b0)
            {
                matrix Projection;
            };

            struct VS_INPUT
            {
                float2 Pos : POSITION;
                float2 Tex : TEXCOORD;
                float4 Col : COLOR;
            };

            struct PS_INPUT
            {
                float4 Pos : SV_POSITION;
                float2 Tex : TEXCOORD;
                float4 Col : COLOR;
            };

            PS_INPUT VS(VS_INPUT input)
            {
                PS_INPUT output;
                // Apply projection to transformed screen-space position
                output.Pos = mul(float4(input.Pos, 0.0, 1.0), Projection);
                output.Tex = input.Tex;
                output.Col = input.Col;
                return output;
            }

            Texture2D shaderTexture : register(t0);
            SamplerState sampleState : register(s0);

            float4 PS(PS_INPUT input) : SV_Target
            {
                float4 texColor = shaderTexture.Sample(sampleState, input.Tex);
                return texColor * input.Col;
            }
            ";

        [StructLayout(LayoutKind.Sequential)]
        private struct VertexType
        {
            public Vector2 position;
            public Vector2 texture;
            public RawColor4 color;

            public VertexType(Vector2 pos, Vector2 tex, RawColor4 col)
            {
                position = pos;
                texture = tex;
                color = col;
            }
        }

        public SharpDXD3D11SpriteRenderer(Device device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _context = _device.ImmediateContext;

            InitializeShaders();
            InitializeBuffers();
            InitializeBlendStates();
            InitializeSampler();
        }

        public void SetViewport(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            _viewport = new RawViewportF
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                MinDepth = 0,
                MaxDepth = 1
            };

            _context.Rasterizer.SetViewport(_viewport);

            UpdateProjectionMatrix();
        }

        private void InitializeShaders()
        {
            // Compile Vertex Shader
            using (var vertexShaderByteCode = ShaderBytecode.Compile(ShaderSource, "VS", "vs_5_0"))
            {
                _vertexShader = new VertexShader(_device, vertexShaderByteCode);
                _inputLayout = new InputLayout(_device, vertexShaderByteCode, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 8, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                });
            }

            // Compile Pixel Shader
            using (var pixelShaderByteCode = ShaderBytecode.Compile(ShaderSource, "PS", "ps_5_0"))
            {
                _pixelShader = new PixelShader(_device, pixelShaderByteCode);
            }
        }

        private void InitializeBuffers()
        {
            // Dynamic vertex buffer for quads (4 vertices)
            _vertexBuffer = new Buffer(_device, new BufferDescription
            {
                Usage = ResourceUsage.Dynamic,
                SizeInBytes = Utilities.SizeOf<VertexType>() * MaxBatchSize * 4,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            });

            var indices = new int[MaxBatchSize * 6];
            int baseVertex = 0;
            for (int i = 0; i < MaxBatchSize; i++)
            {
                int indexOffset = i * 6;
                indices[indexOffset] = baseVertex;
                indices[indexOffset + 1] = baseVertex + 1;
                indices[indexOffset + 2] = baseVertex + 2;
                indices[indexOffset + 3] = baseVertex + 2;
                indices[indexOffset + 4] = baseVertex + 1;
                indices[indexOffset + 5] = baseVertex + 3;

                baseVertex += 4;
            }

            _indexBuffer = Buffer.Create(_device, BindFlags.IndexBuffer, indices);

            // Matrix constant buffer
            _matrixBuffer = new Buffer(_device, new BufferDescription
            {
                Usage = ResourceUsage.Dynamic,
                SizeInBytes = Utilities.SizeOf<Matrix4x4>(), // Must be multiple of 16
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            });

            _vertexBufferBinding = new VertexBufferBinding(_vertexBuffer, _vertexSize, 0);
        }

        private void InitializeSampler()
        {
            _samplerState = new SamplerState(_device, new SamplerStateDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunction = Comparison.Never,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            });
        }

        private void InitializeBlendStates()
        {
            // Replicate DX9 default behavior (Screen Blend for both Color and Alpha) for falling-through modes
            // DX9: SourceBlend = InverseDestinationColor, DestinationBlend = One
            // Applied to both Color and Alpha channels.

            CreateBlendState(BlendMode.NORMAL, BlendOption.InverseDestinationColor, BlendOption.One, BlendOption.InverseDestinationAlpha, BlendOption.One);
            CreateBlendState(BlendMode.LIGHT, BlendOption.InverseDestinationColor, BlendOption.One, BlendOption.InverseDestinationAlpha, BlendOption.One);
            CreateBlendState(BlendMode.LIGHTINV, BlendOption.InverseDestinationColor, BlendOption.One, BlendOption.InverseDestinationAlpha, BlendOption.One);
            CreateBlendState(BlendMode.INVNORMAL, BlendOption.InverseDestinationColor, BlendOption.One, BlendOption.InverseDestinationAlpha, BlendOption.One);
            CreateBlendState(BlendMode.INVLIGHTINV, BlendOption.InverseDestinationColor, BlendOption.One, BlendOption.InverseDestinationAlpha, BlendOption.One);
            CreateBlendState(BlendMode.INVCOLOR, BlendOption.InverseDestinationColor, BlendOption.One, BlendOption.InverseDestinationAlpha, BlendOption.One);
            CreateBlendState(BlendMode.INVBACKGROUND, BlendOption.InverseDestinationColor, BlendOption.One, BlendOption.InverseDestinationAlpha, BlendOption.One);

            // Explicit mappings matching DX9
            // INVLIGHT: Source = BlendFactor, Destination = InverseSourceColor
            // Alpha: BlendFactor, InverseSourceAlpha
            CreateBlendState(BlendMode.INVLIGHT, BlendOption.BlendFactor, BlendOption.InverseSourceColor, BlendOption.BlendFactor, BlendOption.InverseSourceAlpha);

            // COLORFY: Source = SourceAlpha, Destination = One
            // Alpha: SourceAlpha, One
            CreateBlendState(BlendMode.COLORFY, BlendOption.SourceAlpha, BlendOption.One, BlendOption.SourceAlpha, BlendOption.One);

            // MASK: Source = Zero, Destination = InverseSourceAlpha
            // Alpha: Zero, InverseSourceAlpha
            CreateBlendState(BlendMode.MASK, BlendOption.Zero, BlendOption.InverseSourceAlpha, BlendOption.Zero, BlendOption.InverseSourceAlpha);

            // EFFECTMASK: Source = DestinationAlpha, Destination = One
            // Alpha: DestinationAlpha, One
            CreateBlendState(BlendMode.EFFECTMASK, BlendOption.DestinationAlpha, BlendOption.One, BlendOption.DestinationAlpha, BlendOption.One);

            // HIGHLIGHT: Source = BlendFactor, Destination = One
            // Alpha: BlendFactor, One
            CreateBlendState(BlendMode.HIGHLIGHT, BlendOption.BlendFactor, BlendOption.One, BlendOption.BlendFactor, BlendOption.One);

            // LIGHTMAP: Source = Zero, Destination = SourceColor
            // Alpha: Zero, SourceAlpha (SourceColor is invalid for Alpha, maps to SourceAlpha)
            CreateBlendState(BlendMode.LIGHTMAP, BlendOption.Zero, BlendOption.SourceColor, BlendOption.Zero, BlendOption.SourceAlpha);

            // NONE: Typically standard alpha or opaque.
            // In DX9, SetBlend(..., NONE) would fall through to default (Screen Blend) if called with Blending=true.
            // However, if Blending=false, it's standard AlphaBlend.
            // D3D11SpriteRenderer is only called when Blending=true.
            // To strictly replicate DX9 fall-through bug/feature, NONE should be Screen Blend.
            // But since NONE is filtered out in RenderingPipeline (handled by D2D), this might be unused.
            // We'll set it to Standard Alpha as a safe fallback or Screen if desired.
            // Let's stick to Standard Alpha for NONE to mean "Standard Blending" in this context.
            CreateBlendState(BlendMode.NONE, BlendOption.SourceAlpha, BlendOption.InverseSourceAlpha, BlendOption.SourceAlpha, BlendOption.InverseSourceAlpha);
        }

        private void EnsurePipelineConfigured()
        {
            if (_pipelineConfigured)
                return;

            _context.InputAssembler.InputLayout = _inputLayout;
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.InputAssembler.SetVertexBuffers(0, _vertexBufferBinding);
            _context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);

            _context.VertexShader.Set(_vertexShader);
            _context.PixelShader.Set(_pixelShader);
            _context.PixelShader.SetSampler(0, _samplerState);
            _context.VertexShader.SetConstantBuffer(0, _matrixBuffer);

            _pipelineConfigured = true;
        }

        private void CreateBlendState(BlendMode mode, BlendOption src, BlendOption dest, BlendOption srcAlpha, BlendOption destAlpha)
        {
            var desc = new BlendStateDescription();
            desc.RenderTarget[0].IsBlendEnabled = true;

            // Color Blending
            desc.RenderTarget[0].SourceBlend = src;
            desc.RenderTarget[0].DestinationBlend = dest;
            desc.RenderTarget[0].BlendOperation = BlendOperation.Add;

            // Alpha Blending
            // Use valid Alpha options passed in
            desc.RenderTarget[0].SourceAlphaBlend = srcAlpha;
            desc.RenderTarget[0].DestinationAlphaBlend = destAlpha;
            desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;

            desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            _blendStates[mode] = new BlendState(_device, desc);
        }

        public void Draw(Texture2D texture, RectangleF destination, RectangleF? source, Color color, Matrix3x2 transform, BlendMode blendMode, float opacity, float blendRate)
        {
            if (texture == null) return;

            if (_viewport.Width <= 0 || _viewport.Height <= 0)
                return;

            EnsurePipelineConfigured();

            // Get or create SRV and cached texture dimensions
            if (!_textureResources.TryGetValue(texture, out var resources))
            {
                var desc = texture.Description;
                var srv = new ShaderResourceView(_device, texture);
                resources = new SpriteResources(srv, desc.Width, desc.Height);
                _textureResources[texture] = resources;
            }

            if (_queuedSprites >= MaxBatchSize)
                Flush();

            // Flush when state changes to keep batches contiguous
            RawColor4 factor = new RawColor4(blendRate, blendRate, blendRate, blendRate);
            bool textureChanged = _currentTextureView != null && _currentTextureView != resources.ShaderResourceView;
            bool blendChanged = _currentBlendMode != blendMode || !_currentBlendFactor.Equals(factor);
            if ((_queuedSprites > 0) && (textureChanged || blendChanged))
                Flush();

            if (_currentTextureView != resources.ShaderResourceView)
            {
                _context.PixelShader.SetShaderResource(0, resources.ShaderResourceView);
                _currentTextureView = resources.ShaderResourceView;
            }

            if (_currentBlendMode != blendMode || !_currentBlendFactor.Equals(factor))
            {
                if (!_blendStates.TryGetValue(blendMode, out var blendState))
                    blendState = _blendStates[BlendMode.NORMAL];

                _context.OutputMerger.SetBlendState(blendState, factor, -1);
                _currentBlendMode = blendMode;
                _currentBlendFactor = factor;
            }

            // Queue sprite vertices transformed on the CPU to keep GPU state stable during batching
            QueueSprite(destination, source, resources.Width, resources.Height, color, opacity, transform);
        }

        private void QueueSprite(RectangleF dest, RectangleF? source, int texWidth, int texHeight, Color color, float opacity, Matrix3x2 transform)
        {
            float left = dest.Left;
            float right = dest.Right;
            float top = dest.Top;
            float bottom = dest.Bottom;

            float u1 = 0, v1 = 0, u2 = 1, v2 = 1;
            if (source.HasValue)
            {
                u1 = source.Value.Left / (float)texWidth;
                v1 = source.Value.Top / (float)texHeight;
                u2 = source.Value.Right / (float)texWidth;
                v2 = source.Value.Bottom / (float)texHeight;
            }

            var col = new RawColor4(color.R / 255f, color.G / 255f, color.B / 255f, (color.A / 255f) * opacity);

            // Transform positions on CPU so the GPU only applies projection during batching
            var topLeft = Vector2.Transform(new Vector2(left, top), transform);
            var topRight = Vector2.Transform(new Vector2(right, top), transform);
            var bottomLeft = Vector2.Transform(new Vector2(left, bottom), transform);
            var bottomRight = Vector2.Transform(new Vector2(right, bottom), transform);

            int baseVertex = _queuedSprites * 4;
            _vertexData[baseVertex] = new VertexType(topLeft, new Vector2(u1, v1), col);
            _vertexData[baseVertex + 1] = new VertexType(topRight, new Vector2(u2, v1), col);
            _vertexData[baseVertex + 2] = new VertexType(bottomLeft, new Vector2(u1, v2), col);
            _vertexData[baseVertex + 3] = new VertexType(bottomRight, new Vector2(u2, v2), col);

            _queuedSprites++;
        }

        public void Flush()
        {
            if (_queuedSprites == 0)
                return;

            DataBox box = _context.MapSubresource(_vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(box.DataPointer, _vertexData, 0, _queuedSprites * 4);
            _context.UnmapSubresource(_vertexBuffer, 0);

            _context.DrawIndexed(_queuedSprites * 6, 0, 0);
            _queuedSprites = 0;
        }

        private void UpdateProjectionMatrix()
        {
            float width = _viewport.Width;
            float height = _viewport.Height;

            // Standard 2D Ortho: Top-Left (0,0) to Bottom-Right (w,h)
            // Map 0..W to -1..1, 0..H to 1..-1

            // Direct3D NDC: -1 to 1.
            // X: (x / W) * 2 - 1
            // Y: -((y / H) * 2 - 1) = 1 - (y / H) * 2

            Matrix4x4 projection = Matrix4x4.Identity;
            projection.M11 = 2.0f / width;
            projection.M22 = -2.0f / height;
            projection.M41 = -1.0f;
            projection.M42 = 1.0f;

            Matrix4x4 final = Matrix4x4.Transpose(projection);

            DataBox box = _context.MapSubresource(_matrixBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(box.DataPointer, ref final);
            _context.UnmapSubresource(_matrixBuffer, 0);
        }

        public void Dispose()
        {
            foreach (var resources in _textureResources.Values)
                resources.ShaderResourceView.Dispose();

            _textureResources.Clear();

            foreach (var bs in _blendStates.Values) bs.Dispose();
            _blendStates.Clear();

            _samplerState?.Dispose();
            _indexBuffer?.Dispose();
            _matrixBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _inputLayout?.Dispose();
            _pixelShader?.Dispose();
            _vertexShader?.Dispose();
        }
    }
}
