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
        private Buffer _matrixBuffer;
        private SamplerState _samplerState;

        private RawViewportF _viewport;
        private bool _pipelineConfigured;
        private ShaderResourceView _currentTextureView;
        private BlendMode _currentBlendMode = (BlendMode)(-1);
        private RawColor4 _currentBlendFactor;
        private VertexBufferBinding _vertexBufferBinding;
        private readonly int _vertexSize = Utilities.SizeOf<VertexType>();

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
                matrix Matrix;
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
                // Transform position by matrix (Projection * World)
                output.Pos = mul(float4(input.Pos, 0.0, 1.0), Matrix);
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
                SizeInBytes = Utilities.SizeOf<VertexType>() * 4,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            });

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
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            _context.InputAssembler.SetVertexBuffers(0, _vertexBufferBinding);

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

            // 2. Update Vertex Buffer (Quad)
            UpdateVertexBuffer(destination, source, resources.Width, resources.Height, color, opacity);

            // 3. Update Constants (Matrix)
            UpdateMatrixBuffer(transform);

            // 4. Setup Shaders
            if (_currentTextureView != resources.ShaderResourceView)
            {
                _context.PixelShader.SetShaderResource(0, resources.ShaderResourceView);
                _currentTextureView = resources.ShaderResourceView;
            }

            // 5. Setup Blend State
            RawColor4 factor = new RawColor4(blendRate, blendRate, blendRate, blendRate);
            if (_currentBlendMode != blendMode || !_currentBlendFactor.Equals(factor))
            {
                if (!_blendStates.TryGetValue(blendMode, out var blendState))
                    blendState = _blendStates[BlendMode.NORMAL];

                _context.OutputMerger.SetBlendState(blendState, factor, -1);
                _currentBlendMode = blendMode;
                _currentBlendFactor = factor;
            }

            // 6. Draw
            _context.Draw(4, 0);
        }

        private void UpdateVertexBuffer(RectangleF dest, RectangleF? source, int texWidth, int texHeight, Color color, float opacity)
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

            // Triangle Strip: TopLeft, TopRight, BottomLeft, BottomRight
            var v0 = new VertexType(new Vector2(left, top), new Vector2(u1, v1), col);
            var v1_ = new VertexType(new Vector2(right, top), new Vector2(u2, v1), col); // v1 is reserved
            var v2_ = new VertexType(new Vector2(left, bottom), new Vector2(u1, v2), col);
            var v3 = new VertexType(new Vector2(right, bottom), new Vector2(u2, v2), col);

            DataBox box = _context.MapSubresource(_vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            var dataPtr = box.DataPointer;

            Utilities.Write(dataPtr, ref v0);
            Utilities.Write(IntPtr.Add(dataPtr, _vertexSize), ref v1_);
            Utilities.Write(IntPtr.Add(dataPtr, _vertexSize * 2), ref v2_);
            Utilities.Write(IntPtr.Add(dataPtr, _vertexSize * 3), ref v3);
            _context.UnmapSubresource(_vertexBuffer, 0);
        }

        private void UpdateMatrixBuffer(Matrix3x2 transform)
        {
            // Create Orthographic Projection Matrix based on BackBuffer size
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

            // Apply the object transform (Matrix3x2)
            // 3x2 = [ M11 M12 ]
            //       [ M21 M22 ]
            //       [ M31 M32 ]
            // Extend to 4x4
            Matrix4x4 world = Matrix4x4.Identity;
            world.M11 = transform.M11;
            world.M12 = transform.M12;
            world.M21 = transform.M21;
            world.M22 = transform.M22;
            world.M41 = transform.M31; // Translation X
            world.M42 = transform.M32; // Translation Y

            // Final Matrix = World * Projection
            // Transpose because HLSL defaults to column-major in mul(v, M) or row-major?
            // SharpDX matrices are Row-Major. HLSL mul(vector, matrix) expects Row-Major matrix if vector is row.
            // mul(float4, matrix) -> Row Vector * Matrix.
            // So we send it as is.

            Matrix4x4 final = world * projection;

            // However, D3D default constant buffer layout expects Column-Major logic if we don't transpose,
            // OR we construct it carefully.
            // SharpDX Matrix.Transpose() is usually needed if the shader uses `matrix` type and `mul(pos, mat)`.
            final = Matrix4x4.Transpose(final);

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
            _matrixBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _inputLayout?.Dispose();
            _pixelShader?.Dispose();
            _vertexShader?.Dispose();
        }
    }
}
