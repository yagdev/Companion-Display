using CoreAudio;

namespace CompanionDisplayWinUI.ClassImplementations
{
    class AudioManager
    {
        public static MMDeviceEnumerator DevEnum = new();
        public static MMDevice[] mmDevices =
        [
            DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia)
        ];
    }
}
