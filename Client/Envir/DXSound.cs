using SlimDX.DirectSound;
using SlimDX.Multimedia;
using NAudio;
using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using WaveFormat = SlimDX.Multimedia.WaveFormat;
using WaveStream = SlimDX.Multimedia.WaveStream;

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

                if (Path.GetExtension(FileName) == ".mp3")
                {
                    using (var mp3 = new Mp3FileReader(FileName))
                    {
                        var format = mp3.WaveFormat.AsStandardWaveFormat();
                        Format = new WaveFormat()
                        {
                            AverageBytesPerSecond = format.AverageBytesPerSecond,
                            BitsPerSample = (short)format.BitsPerSample,
                            BlockAlignment = (short)format.BlockAlign,
                            Channels = (short)format.Channels,
                            SamplesPerSecond = format.SampleRate,
                            FormatTag = (WaveFormatTag)Enum.Parse(typeof(WaveFormatTag), format.Encoding.ToString())
                        };
                        RawData = new byte[mp3.Length];
                        mp3.Read(RawData, 0, RawData.Length);
                    }
                }
                else
                {
                    using (WaveStream wStream = new WaveStream(FileName))
                    {
                        Format = wStream.Format;
                        RawData = new byte[wStream.Length];

                        wStream.Position = 44;
                        wStream.Read(RawData, 0, RawData.Length);
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
                if (BufferList[i].Disposed)
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
                if (BufferList[i].Disposed)
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

            BufferList.Add(buff = new SecondarySoundBuffer(DXSoundManager.Device, new SoundBufferDescription { Format = Format, SizeInBytes = RawData.Length, Flags = flags })
            {
                Volume = Volume
            });


            buff.Write(RawData, 0, LockFlags.EntireBuffer);

            return buff;
        }
        public void DisposeSoundBuffer()
        {
            RawData = null;

            for (int i = BufferList.Count - 1; i >= 0; i--)
            {
                if (!BufferList[i].Disposed)
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
                if (BufferList[i].Disposed)
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

                if (!BufferList[0].Disposed)
                {
                    BufferList[0].Dispose();
                }

                BufferList.RemoveAt(0);
            }

        }
    }
}
