using Silk.NET.Shaderc;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Rendering.SilkVulkan
{
    internal static unsafe class SilkVulkanShaderCompiler
    {
        public static byte[] Compile(string source, ShaderKind shaderKind, string name)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Shader source must be provided.", nameof(source));

            Shaderc shaderc = Shaderc.GetApi();
            Compiler* compiler = shaderc.CompilerInitialize();
            if (compiler == null)
                throw new InvalidOperationException("Unable to initialize shaderc.");

            CompileOptions* options = shaderc.CompileOptionsInitialize();
            if (options == null)
            {
                shaderc.CompilerRelease(compiler);
                throw new InvalidOperationException("Unable to initialize shaderc compile options.");
            }

            CompilationResult* result = null;
            try
            {
                shaderc.CompileOptionsSetTargetEnv(options, TargetEnv.Vulkan, 0);
                shaderc.CompileOptionsSetOptimizationLevel(options, OptimizationLevel.Performance);

                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                fixed (byte* sourcePtr = sourceBytes)
                {
                    result = shaderc.CompileIntoSpv(compiler, sourcePtr, (nuint)sourceBytes.Length, shaderKind, name, "main", options);
                }

                if (result == null)
                    throw new InvalidOperationException($"Shaderc returned no result for {name}.");

                CompilationStatus status = shaderc.ResultGetCompilationStatus(result);
                if (status != CompilationStatus.Success)
                    throw new InvalidOperationException($"{name} shader compile failed: {shaderc.ResultGetErrorMessageS(result)}");

                nuint length = shaderc.ResultGetLength(result);
                byte* bytes = shaderc.ResultGetBytes(result);
                byte[] spirv = new byte[(int)length];
                Marshal.Copy((IntPtr)bytes, spirv, 0, spirv.Length);
                return spirv;
            }
            finally
            {
                if (result != null)
                    shaderc.ResultRelease(result);

                shaderc.CompileOptionsRelease(options);
                shaderc.CompilerRelease(compiler);
                shaderc.Dispose();
            }
        }
    }
}