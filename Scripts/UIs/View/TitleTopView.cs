using Chocolate.Audio;
using Chocolate.Common;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Chocolate.UIs.View
{
    public class TitleTopView : MonoBehaviour
    {
        private Vector3 _titleImageInitPosition;
        private Vector3 _titleImageInitScale;
        private void Awake()
        {
            _titleImageInitPosition = titleImage.transform.localPosition;
            _titleImageInitScale = titleImage.transform.localScale;
        }

        // タイトル画像
        [SerializeField] private Image titleImage;
        [SerializeField] private ShakeEffectBehaviour titleImageShakeEffect;
        [SerializeField] private TextMeshProUGUI specialThanksText;
        [SerializeField] private Image whiteImage;
        [SerializeField] private GameObject blackMaskRtoL;
        [SerializeField] private GameObject blackMaskLtoR;
        public void StartTitleImageAnimation(UnityAction<GameAudioType> onPlayOneShot, UnityAction callback)
        {
            // タイムライン再生により変わっている可能性があるため
            titleImage.transform.localPosition = _titleImageInitPosition;
            titleImage.transform.localScale = _titleImageInitScale;
            titleImage.gameObject.SetActive(true);

            // アニメーション開始
            var toScale = titleImage.transform.localScale;
            var fromScale = 1.8f * Vector3.one;
            titleImage.transform.localScale = fromScale;
            var tmp = whiteImage.color;
            tmp.a = 1.0f;
            whiteImage.color = tmp;
            whiteImage.gameObject.SetActive(true);
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.2f);
            sequence.Append(FeedImageAlpha(whiteImage, 1.0f, 0.0f, 0.35f));
            sequence.Join(titleImage.transform.DOScale(toScale, 0.5f).SetEase(Ease.InExpo));
            sequence.AppendCallback(() =>
            {
                titleImageShakeEffect.StartShake(0.2f, 35.0f, 15.0f);
                onPlayOneShot(GameAudioType.SeBomb);
                whiteImage.gameObject.SetActive(false);
            });
            sequence.AppendInterval(0.5f);
            sequence.Append(FeedImageAlpha(specialThanksText, 0.0f, 1.0f, 0.5f));
            sequence.AppendCallback(() =>
            {
                callback?.Invoke();
            });
        }

        public void SetActiveTitleAnimationObjects(bool isActive)
        {
            whiteImage.gameObject.SetActive(isActive);
            blackMaskRtoL.gameObject.SetActive(isActive);
            blackMaskLtoR.gameObject.SetActive(isActive);
        }

        public void SetActiveSpecialThanksText(bool isActive)
        {
            var tmp = specialThanksText.color;
            tmp.a = 1.0f;
            specialThanksText.color = tmp;
        }

        /// <summary>
        /// Alpha値をフェードする
        /// </summary>
        private Tween FeedImageAlpha(Image image, float current, float target, float duration = 1.0f)
        {
            var tmp = image.color;
            tmp.a = current;
            image.color = tmp;
            return DOTween.ToAlpha(
                () => image.color,
                color => image.color = color,
                target,
                duration);
        }
        private Tween FeedImageAlpha(TextMeshProUGUI text, float current, float target, float duration = 1.0f)
        {
            var tmp = text.color;
            tmp.a = current;
            text.color = tmp;
            return DOTween.ToAlpha(
                () => text.color,
                color => text.color = color,
                target,
                duration);
        }

        [SerializeField] private GameObject topLeftButtonArea;
        [SerializeField] private GameObject topRightButtonArea;
        public void SetActiveAllButtonArea(bool isActive)
        {
            topLeftButtonArea.SetActive(isActive);
            topRightButtonArea.SetActive(isActive);
            startButton.gameObject.SetActive(isActive);
        }

        // STARTボタン
        [SerializeField] private Button startButton;
        public void SetListenerStartButton(UnityAction action)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(action);
        }

        // MORE APP ボタン
        [SerializeField] private Button moreAppButton;
        public void SetListenerMoreAppButton(UnityAction action)
        {
            moreAppButton.onClick.RemoveAllListeners();
            moreAppButton.onClick.AddListener(action);
        }

        // SEボタン
        [SerializeField] private Button seButton;
        [SerializeField] private Image seButtonImage;
        [SerializeField] private Sprite seOnSprite;
        [SerializeField] private Sprite seOffSprite;
        public void ChangeSeOnOffSprite(bool isOff)
        {
            seButtonImage.sprite = isOff ? seOffSprite : seOnSprite;
        }
        public void SetListenerSeButton(UnityAction action)
        {
            seButton.onClick.RemoveAllListeners();
            seButton.onClick.AddListener(action);
        }

        // BGMボタン
        [SerializeField] private Button bgmButton;
        [SerializeField] private Image bgmButtonImage;
        [SerializeField] private Sprite bgmOnSprite;
        [SerializeField] private Sprite bgmOffSprite;
        public void ChangeBgmOnOffSprite(bool isVolumeOff)
        {
            bgmButtonImage.sprite = isVolumeOff ? bgmOffSprite : bgmOnSprite;
        }
        public void SetListenerBgmButton(UnityAction action)
        {
            bgmButton.onClick.RemoveAllListeners();
            bgmButton.onClick.AddListener(action);
        }

        // ハートボタン（クリアの証）
        [SerializeField] private Button heartButton;
        public void SetActiveHeartButton(bool isActive)
        {
            heartButton.gameObject.SetActive(isActive);
        }
        public void SetListenerHeartButton(UnityAction action)
        {
            heartButton.onClick.RemoveAllListeners();
            heartButton.onClick.AddListener(action);
        }
    }
}
