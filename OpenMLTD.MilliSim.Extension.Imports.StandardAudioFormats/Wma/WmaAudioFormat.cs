using System;
using System.Composition;
using JetBrains.Annotations;
using NAudio.Wave;
using NAudio.WindowsMediaFormat;
using OpenMLTD.MilliSim.Audio.Extending;

namespace OpenMLTD.MilliSim.Extension.Imports.StandardAudioFormats.Wma {
    [Export(typeof(IAudioFormat))]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class WmaAudioFormat : IAudioFormat {

        public string PluginID => "plugin.audio.wma";

        public string PluginName => "WMA Audio Format";

        public string PluginDescription => "WMA audio format reader and factory.";

        public string PluginAuthor => "OpenMLTD";

        public Version PluginVersion => MyVersion;

        public WaveStream Read(string fileName) {
            return new WMAFileReader(fileName);
        }

        public bool SupportsFileType(string fileName) {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".wma");
        }

        public string FormatDescription => "Windows Media Audio";

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
