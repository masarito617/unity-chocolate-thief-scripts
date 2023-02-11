using Chocolate.Audio;

namespace Chocolate.Services
{
    public interface IAudioService
    {
        /// <summary>
        /// オーディオ情報
        /// </summary>
        public class AudioInfo
        {
            public readonly string Name;
            public readonly float Volume;
            public AudioInfo(string name, float volume)
            {
                Name = name;
                Volume = volume;
            }
        }

        /// <summary>
        /// 効果音再生
        /// </summary>
        public void PlayOneShot(GameAudioType gameAudioType);

        /// <summary>
        /// BGM再生
        /// </summary>
        public void PlayBGM(GameAudioType gameAudioType);

        /// <summary>
        /// BGM停止
        /// </summary>
        public void StopBGM();

        /// <summary>
        /// SEピッチ変更
        /// </summary>
        /// <param name="pitch">ピッチ値</param>
        public void ChangeSePitch(float pitch);

        /// <summary>
        /// BGMボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeBgmVolume(float volume);

        /// <summary>
        /// SEボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeSeVolume(float volume);

        /// <summary>
        /// ボリュームOnOffフラグ設定
        /// </summary>
        public bool ChangeBgmVolumeOnOff();
        public bool GetIsBgmVolumeOff();
        public bool ChangeSeVolumeOnOff();
        public bool GetIsSeVolumeOff();
    }
}