using Chocolate.Audio;
using Chocolate.Common;
using Chocolate.Const;
using Chocolate.Services;
using Chocolate.Settings;
using Chocolate.UIs.View;
using UnityEngine;
using VContainer;

namespace Chocolate.UIs.Presenter
{
    public class RewardPresenter : MonoBehaviour
    {
        [SerializeField] private RewardView rewardView;

        private IAudioService _audioService;
        private ITransitionService _transitionService;
        private GameSettings _gameSettings;

        [Inject]
        public void Construct(IAudioService audioService, ITransitionService transitionService, GameSettings gameSettings)
        {
            _audioService = audioService;
            _transitionService =  transitionService;
            _gameSettings = gameSettings;

            // BGMは止めておく
            _audioService.StopBGM();
        }

        /// <summary>
        /// タイムライン側から呼ばれる想定
        /// </summary>
        private bool _isCanTitleBack;
        public void CanTitleBack()
        {
            _isCanTitleBack = true;
        }
        public void EndTimeline()
        {
            Debug.Log("End Reward Timeline.");
            _audioService.PlayBGM(GameAudioType.BgmGameResult);
        }
        public void StartRewardImageAnimation()
        {
            rewardView.StartRewardImageAnimation();
        }

        private void Awake()
        {
            _isCanTitleBack = false;
        }

        private void Update()
        {
            if (!_isCanTitleBack) return;

            // タップでタイトル画面へ遷移
            if (TouchUtil.IsScreenTouch(true))
            {
                _gameSettings.IsTransitionTuneUp = false; // タイトルアニメーションを再度表示させる
                _audioService.PlayOneShot(GameAudioType.SeDecide);
                _transitionService.LoadScene(GameConst.SceneNameTitle);
            }
        }
    }
}
