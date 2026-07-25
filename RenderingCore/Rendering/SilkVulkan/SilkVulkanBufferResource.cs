using Silk.NET.Vulkan;
using System;
using System.Drawing;
using VkImage = Silk.NET.Vulkan.Image;
using VkImageLayout = Silk.NET.Vulkan.ImageLayout;

namespace Shared.Rendering.SilkVulkan
{
    internal sealed unsafe class SilkVulkanBufferResource : IDisposable
    {
        private readonly Vk _vk;
        private readonly Device _device;
        private readonly bool _mapped;
        private bool _disposed;

        public SilkVulkanBufferResource(Vk vk, Device device, Silk.NET.Vulkan.Buffer buffer, DeviceMemory memory, ulong size, IntPtr mappedPointer)
        {
            _vk = vk ?? throw new ArgumentNullException(nameof(vk));
            _device = device;
            Buffer = buffer;
            Memory = memory;
            Size = size;
            MappedPointer = mappedPointer;
            _mapped = mappedPointer != IntPtr.Zero;
        }

        public Silk.NET.Vulkan.Buffer Buffer { get; private set; }
        public DeviceMemory Memory { get; private set; }
        public ulong Size { get; }
        public IntPtr MappedPointer { get; }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_mapped && Memory.Handle != 0)
                _vk.UnmapMemory(_device, Memory);

            if (Buffer.Handle != 0)
            {
                _vk.DestroyBuffer(_device, Buffer, null);
                Buffer = default;
            }

            if (Memory.Handle != 0)
            {
                _vk.FreeMemory(_device, Memory, null);
                Memory = default;
            }
        }
    }

    internal sealed unsafe class SilkVulkanTextureResource : IDisposable
    {
        private readonly Vk _vk;
        private readonly Device _device;
        private readonly bool _ownsImage;
        private readonly bool _keepCpuData;
        private byte[] _data;
        private bool _disposed;

        public SilkVulkanTextureResource(
            Vk vk,
            Device device,
            Size size,
            RenderTextureFormat format,
            VkImage image,
            DeviceMemory memory,
            ImageView imageView,
            byte[] data,
            VkImageLayout layout,
            bool ownsImage,
            bool keepCpuData,
            bool flipVerticallyWhenSampling = false)
        {
            _vk = vk ?? throw new ArgumentNullException(nameof(vk));
            _device = device;
            Size = size;
            Format = format;
            Image = image;
            Memory = memory;
            ImageView = imageView;
            _data = data;
            Layout = layout;
            _ownsImage = ownsImage;
            _keepCpuData = keepCpuData;
            FlipVerticallyWhenSampling = flipVerticallyWhenSampling;
        }

        public Size Size { get; }
        public RenderTextureFormat Format { get; }
        public VkImage Image { get; private set; }
        public DeviceMemory Memory { get; private set; }
        public ImageView ImageView { get; private set; }
        public DescriptorSet DescriptorSet { get; set; }
        public DescriptorPool DescriptorPool { get; set; }
        public byte[] Data => _data ??= new byte[Size.Width * Size.Height * 4];
        public VkImageLayout Layout { get; set; }
        public bool FlipVerticallyWhenSampling { get; }
        public TextureFilterMode AppliedFilter { get; set; }

        public void ReleaseCpuDataIfUnused()
        {
            if (!_keepCpuData)
                _data = null;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (DescriptorSet.Handle != 0 && DescriptorPool.Handle != 0)
            {
                DescriptorSet descriptorSet = DescriptorSet;
                _vk.FreeDescriptorSets(_device, DescriptorPool, 1, in descriptorSet);
                DescriptorSet = default;
                DescriptorPool = default;
            }

            if (ImageView.Handle != 0)
            {
                _vk.DestroyImageView(_device, ImageView, null);
                ImageView = default;
            }

            if (_ownsImage && Image.Handle != 0)
            {
                _vk.DestroyImage(_device, Image, null);
                Image = default;
            }

            if (_ownsImage && Memory.Handle != 0)
            {
                _vk.FreeMemory(_device, Memory, null);
                Memory = default;
            }
        }
    }

    internal sealed unsafe class SilkVulkanRenderTarget : IDisposable
    {
        private readonly Vk _vk;
        private readonly Device _device;
        private readonly bool _ownsImageView;
        private bool _disposed;

        public SilkVulkanRenderTarget(
            Vk vk,
            Device device,
            Size size,
            VkImage image,
            ImageView imageView,
            Framebuffer framebuffer,
            SilkVulkanTextureResource texture,
            bool isBackBuffer,
            uint swapchainImageIndex,
            bool ownsImageView)
        {
            _vk = vk ?? throw new ArgumentNullException(nameof(vk));
            _device = device;
            Size = size;
            Image = image;
            ImageView = imageView;
            Framebuffer = framebuffer;
            Texture = texture;
            IsBackBuffer = isBackBuffer;
            SwapchainImageIndex = swapchainImageIndex;
            _ownsImageView = ownsImageView;
        }

        public Size Size { get; private set; }
        public VkImage Image { get; }
        public ImageView ImageView { get; private set; }
        public Framebuffer Framebuffer { get; private set; }
        public SilkVulkanTextureResource Texture { get; }
        public bool IsBackBuffer { get; }
        public uint SwapchainImageIndex { get; }
        public VkImageLayout Layout { get; set; } = VkImageLayout.Undefined;

        public void ResizeBackBuffer(Size size)
        {
            if (!IsBackBuffer)
                throw new InvalidOperationException("Only the back buffer render target can be resized.");

            Size = size;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (Framebuffer.Handle != 0)
            {
                _vk.DestroyFramebuffer(_device, Framebuffer, null);
                Framebuffer = default;
            }

            if (_ownsImageView && ImageView.Handle != 0)
            {
                _vk.DestroyImageView(_device, ImageView, null);
                ImageView = default;
            }

            if (!IsBackBuffer)
                Texture?.Dispose();
        }
    }
}