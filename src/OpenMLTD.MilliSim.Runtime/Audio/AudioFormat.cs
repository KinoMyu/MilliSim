using System;
using NAudio.Wave;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Audio {
    public abstract class AudioFormat : IAudioFormat {

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public string PluginCategory => "Audio Format";

        public abstract WaveStream Read(string fileName);

        public abstract bool SupportsFileType(string fileName);

        public abstract string FormatDescription { get; }

        public int ApiVersion => 1;

    }
}