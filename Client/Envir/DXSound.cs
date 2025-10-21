using NAudio.Wave;
using SharpDX;
using SharpDX.DirectSound;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.IO;

namespace Client.Envir
{
    public sealed class DXSound
    {
        public string FileName { get; set; }

        public List<SecondarySoundBuffer> BufferList = new List<SecondarySoundBuffer>();

        private WaveFormat Format;
        private byte[] RawData;


        public DateTime ExpireTime { get; set; }
        public bool Loop { get; set; }

        public SoundType SoundType { get; set; }

        public int Volume { get; set; }

        public DXSound(string fileName, SoundType type)
        {
            FileName = fileName;
            SoundType = type;

            Volume = DXSoundManager.GetVolume(SoundType);
        }
        public void Play()
        {
            if (RawData == null)
            {
                if (!File.Exists(FileName))
                {
                    return;
                }

                if (string.Equals(Path.GetExtension(FileName), ".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    using (var mp3 = new Mp3FileReader(FileName))
                    {
                        Format = ConvertWaveFormat(mp3.WaveFormat); 

                        RawData = ReadAllBytes(mp3);
                    }
                }
                else
                {
                    using (var waveReader = new WaveFileReader(FileName))
                    {
                        Format = ConvertWaveFormat(waveReader.WaveFormat);
                        RawData = ReadAllBytes(waveReader);
                    }
                }
                DXManager.SoundList.Add(this);
            }


            if (BufferList.Count == 0)
            {
                CreateBuffer();
            }

            if (Loop)
            {
                if ((BufferList[0].Status & BufferStatus.Playing) != BufferStatus.Playing)
                {
                    BufferList[0].Play(0, PlayFlags.Looping);
                }

                ExpireTime = DateTime.MaxValue;
                return;
            }
            ExpireTime = CEnvir.Now + Config.CacheDuration;

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }

                if (BufferList[i].Status == BufferStatus.Playing)
                {
                    continue;
                }

                BufferList[i].Play(0, PlayFlags.None);
                return;
            }

            if (BufferList.Count >= Config.SoundOverLap)
            {
                return;
            }

            SecondarySoundBuffer buff = CreateBuffer();
            buff.Play(0, PlayFlags.None);
        }
        public void Stop()
        {
            if (BufferList == null)
            {
                return;
            }

            if (Loop)
            {
                ExpireTime = CEnvir.Now + Config.CacheDuration;
            }

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }
                BufferList[i].CurrentPlayPosition = 0;
                BufferList[i].Stop();
            }
        }

        private SecondarySoundBuffer CreateBuffer()
        {
            SecondarySoundBuffer buff;
            BufferFlags flags = BufferFlags.ControlVolume;

            if (Config.SoundInBackground)
            {
                flags |= BufferFlags.GlobalFocus;
            }

            var description = new SoundBufferDescription
            {
                Format = Format,
                BufferBytes = RawData.Length,
                Flags = flags
            };

            BufferList.Add(buff = new SecondarySoundBuffer(DXSoundManager.Device, description)
            {
                Volume = Volume
            });

            using (var dataStream = DataStream.Create(RawData, true, false))
            {
                buff.Write(dataStream, RawData.Length, LockFlags.EntireBuffer);
            }

            return buff;
        }
        public void DisposeSoundBuffer()
        {
            RawData = null;

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (!BufferList[i].IsDisposed)
                {
                    BufferList[i].Dispose();
                }

                BufferList.RemoveAt(i);
            }

            DXManager.SoundList.Remove(this);
            ExpireTime = DateTime.MinValue;
        }

        public void SetVolume()
        {
            Volume = DXSoundManager.GetVolume(SoundType);

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (BufferList[i].IsDisposed)
                {
                    BufferList.RemoveAt(i);
                    continue;
                }

                BufferList[i].Volume = Volume;
            }
        }

        public void UpdateFlags()
        {
            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                SecondarySoundBuffer buffer = CreateBuffer();

                buffer.CurrentPlayPosition = BufferList[0].CurrentPlayPosition;

                if ((BufferList[0].Status & BufferStatus.Playing) == BufferStatus.Playing)
                {
                    buffer.Play(0, Loop ? PlayFlags.Looping : PlayFlags.None);
                }

                if (!BufferList[0].IsDisposed)
                {
                    BufferList[0].Dispose();
                }

                BufferList.RemoveAt(0);
            }

        }

        private static WaveFormat ConvertWaveFormat(global::NAudio.Wave.WaveFormat sourceFormat)
        {
            if (!Enum.TryParse(sourceFormat.Encoding.ToString(), out WaveFormatEncoding encoding))
                encoding = WaveFormatEncoding.Pcm;

            return WaveFormat.CreateCustomFormat(
                encoding,
                sourceFormat.SampleRate,
                sourceFormat.Channels,
                sourceFormat.AverageBytesPerSecond,
                sourceFormat.BlockAlign,
                sourceFormat.BitsPerSample);
        }

        private static byte[] ReadAllBytes(WaveStream stream)
        {
            stream.Position = 0;

            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }
    }
}
