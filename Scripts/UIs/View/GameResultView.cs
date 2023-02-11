using System.Threading;
using Chocolate.Audio;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Chocolate.UIs.View
{
    public class GameResultView : MonoBehaviour
    {
        private CancellationTokenSource _cancellationTokenSource;
        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            resultArea.SetActive(false);
            resultBestScore.SetActive(false);
            resultChocoArea.SetActive(false);
            resultExpArea.SetActive(false);
        }

        /// <summary>
        /// 結果表示
        /// </summary>
        public void ShowResult(int chocoScore, int exp, UnityAction displayAction, UnityAction okButtonAction, UnityAction tweetButtonAction, UnityAction<GameAudioType> onPlayOneShot, bool isBestScore)
        {
            ShowResultAsync(chocoScore, exp, displayAction, okButtonAction, tweetButtonAction, onPlayOneShot, isBestScore, _cancellationTokenSource.Token);
        }
        private async void ShowResultAsync(int chocoScore, int exp, UnityAction displayAction, UnityAction okButtonAction, UnityAction tweetButtonAction, UnityAction<GameAudioType> onPlayOneShot, bool isBestScore, CancellationToken token)
        {
            resultArea.transform.localScale = Vector3.zero;
            resultBestScore.SetActive(false);
            resultChocoArea.SetActive(false);
            resultExpArea.SetActive(false);
            resultOkButton.gameObject.SetActive(false);
            tweetButton.gameObject.SetActive(false);
            SetListenerResultOkButton(okButtonAction);
            SetListenerTweetButton(tweetButtonAction);

            // 終了メッセージの表示
            SetActiveEndMessageText(true);
            await UniTask.Delay(2000, cancellationToken: token);
            SetActiveEndMessageText(false);
            await UniTask.Delay(1000, cancellationToken: token);
            displayAction?.Invoke();

            // 結果ウィンドウの表示
            var sequence = DOTween.Sequence();
            sequence.Append(resultArea.transform.DOScale(1f, 0.85f).SetEase(Ease.OutBounce));
            sequence.AppendInterval(0.8f);
            // チョコスコア
            sequence.AppendCallback(() =>
            {
                SetTextResultChoco(0);
                resultChocoArea.SetActive(true);
            });
            sequence.Append(IncrementText(resultChocoText, 0, chocoScore, 0.3f, (val) => onPlayOneShot(GameAudioType.SeResultCount)));
            sequence.AppendInterval(0.8f);
            // EXP
            sequence.AppendCallback(() =>
            {
                SetTextResultExp(0);
                resultExpArea.SetActive(true);
            });
            sequence.Append(IncrementText(resultExpText, 0, exp, 0.3f, (val) => onPlayOneShot(GameAudioType.SeResultCount)));
            sequence.AppendInterval(0.8f);

            sequence.AppendCallback(() =>
            {
                // ベストスコアの場合
                if (isBestScore)
                {
                    onPlayOneShot(GameAudioType.SeOkBonus);
                    resultBestScore.SetActive(true);
                    resultBestScore.transform.DOLocalMoveY(20f, 0.4f)
                        .SetRelative(true)
                        .SetEase(Ease.OutQuad)
                        .SetLoops(-1, LoopType.Yoyo);
                }
                // OKボタン、ツイートボタン
                resultOkButton.gameObject.SetActive(true);
                tweetButton.gameObject.SetActive(true);
            });
            resultArea.SetActive(true);
        }

        /// <summary>
        /// テキストを加算する
        /// </summary>
        private Tween IncrementText(TextMeshProUGUI text, int current, int target, float duration = 1.0f, UnityAction<int> setAction = null)
        {
            return DOTween.To( (val) =>
            {
                var intValue = Mathf.RoundToInt(val);
                if (int.Parse(text.text) != intValue)
                {
                    text.text = intValue.ToString();
                    if (setAction != null)
                    {
                        setAction(intValue);
                    }
                }
            }, current, target, duration);
        }

        [SerializeField] private TextMeshProUGUI endMessageText;
        public void SetActiveEndMessageText(bool isActive)
        {
            endMessageText.gameObject.SetActive(isActive);
        }

        [SerializeField] private GameObject resultArea;
        [SerializeField] private GameObject resultChocoArea;
        [SerializeField] private TextMeshProUGUI resultChocoText;
        [SerializeField] private GameObject resultExpArea;
        [SerializeField] private TextMeshProUGUI resultExpText;
        [SerializeField] private GameObject resultBestScore;
        [SerializeField] private Button resultOkButton;
        public void SetTextResultChoco(int score)
        {
            resultChocoText.SetText(score.ToString());
        }
        public void SetTextResultExp(int exp)
        {
            resultExpText.SetText(exp.ToString());
        }
        public void SetListenerResultOkButton(UnityAction action)
        {
            resultOkButton.onClick.RemoveAllListeners();
            resultOkButton.onClick.AddListener(action);
        }

        /// <summary>
        /// ツイートボタン
        /// </summary>
        [SerializeField] private Image tweetImage;
        [SerializeField] private Button tweetButton;
        public void SetActiveTweetButton(bool isActive)
        {
            tweetButton.gameObject.SetActive(isActive);
        }
        public void SetListenerTweetButton(UnityAction action)
        {
            tweetButton.onClick.RemoveAllListeners();
            tweetButton.onClick.AddListener(action);
        }
    }
}
