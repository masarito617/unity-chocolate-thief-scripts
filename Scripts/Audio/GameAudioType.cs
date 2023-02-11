using System.Collections.Generic;
using AudioInfo = Chocolate.Services.IAudioService.AudioInfo;

namespace Chocolate.Audio
{
    /// <summary>
    /// オーディオタイプ
    /// </summary>
    public enum GameAudioType
    {
        None,
        BgmTitleTop,
        BgmGamePlay,
        BgmGameResult,
        SeClick,
        SeDecide,
        SeOk,
        SeOkBonus,
        SeNg,
        SeCount,
        SeCountGo,
        SeWhistle,
        SeActionApproach,
        SeActionPlease,
        SeActionThief,
        SeAngry,
        SeDead,
        SeResultCount,
        SeBomb,
    }

    public static class GameAudioUtil
    {
        /// <summary>
        /// オーディオ情報を返却
        /// </summary>
        public static Dictionary<GameAudioType, AudioInfo> GetAudioInfos()
        {
            var audioInfos = new Dictionary<GameAudioType, AudioInfo>();
            audioInfos.Add(GameAudioType.BgmTitleTop, new AudioInfo("BGM-Main", 0.8f));
            audioInfos.Add(GameAudioType.BgmGamePlay, new AudioInfo("BGM-Playing", 0.8f));
            audioInfos.Add(GameAudioType.BgmGameResult, new AudioInfo("BGM-Result", 0.8f));
            audioInfos.Add(GameAudioType.SeClick, new AudioInfo("SE-Button", 1.0f));
            audioInfos.Add(GameAudioType.SeDecide, new AudioInfo("se_decide", 0.35f));
            audioInfos.Add(GameAudioType.SeOk, new AudioInfo("SE-OK", 0.6f));
            audioInfos.Add(GameAudioType.SeOkBonus, new AudioInfo("SE-OKAAAAY", 0.5f));
            audioInfos.Add(GameAudioType.SeNg, new AudioInfo("SE-NG", 0.4f));
            audioInfos.Add(GameAudioType.SeCount, new AudioInfo("se_count", 1.0f));
            audioInfos.Add(GameAudioType.SeCountGo, new AudioInfo("se_count_go", 1.0f));
            audioInfos.Add(GameAudioType.SeWhistle, new AudioInfo("se_whistle", 0.8f));
            audioInfos.Add(GameAudioType.SeActionApproach, new AudioInfo("SE-Approach", 0.6f));
            audioInfos.Add(GameAudioType.SeActionPlease, new AudioInfo("SE-Dogeza", 0.6f));
            audioInfos.Add(GameAudioType.SeActionThief, new AudioInfo("SE-Robbery", 0.5f));
            audioInfos.Add(GameAudioType.SeAngry, new AudioInfo("SE-Anger", 0.6f));
            audioInfos.Add(GameAudioType.SeDead, new AudioInfo("SE-Miss", 0.6f));
            audioInfos.Add(GameAudioType.SeResultCount, new AudioInfo("se_result_count", 0.8f));
            audioInfos.Add(GameAudioType.SeBomb, new AudioInfo("se_bomb", 0.8f));
            return audioInfos;
        }
    }
}