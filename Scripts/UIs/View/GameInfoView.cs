using System.Threading;
using Chocolate.Audio;
using Chocolate.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Chocolate.UIs.View
{
    public class GameInfoView : MonoBehaviour
    {
        // 開始メッセージ
        [SerializeField] private TextMeshProUGUI startMessageText;
        public void SetActiveStartMessageText(bool isActive)
        {
            startMessageText.gameObject.SetActive(isActive);
        }
        public void SetTextStartMessage(string text)
        {
            startMessageText.text = text;
        }

        public async void ShowStartMessageAsync(UnityAction<GameAudioType> onPlayOneShot, UnityAction onNextCallback, CancellationToken token)
        {
            // 開始メッセージを点滅
            startMessageText.text = "";
            await UniTask.Delay(500, cancellationToken: token);
            for (var i = 0; i < 2; i++)
            {
                startMessageText.text = "チョコを入手せよ！";
                await UniTask.Delay(500, cancellationToken: token);
                startMessageText.text = "";
                await UniTask.Delay(500, cancellationToken: token);
            }

            // カウントダウン
            var fromScale = startMessageText.transform.localScale;
            var toScale = 1.8f;
            startMessageText.text = "3";
            startMessageText.transform.DOScale(toScale, 0.3f)
                .SetEase(Ease.OutQuart);
            startMessageText.transform.DOScale(fromScale, 0.3f)
                .SetEase(Ease.OutQuart);
            onPlayOneShot(GameAudioType.SeCount);
            await UniTask.Delay(1000, cancellationToken: token);
            startMessageText.text = "2";
            startMessageText.transform.DOScale(toScale, 0.3f)
                .SetEase(Ease.OutQuart);
            startMessageText.transform.DOScale(fromScale, 0.3f)
                .SetEase(Ease.OutQuart);
            onPlayOneShot(GameAudioType.SeCount);
            await UniTask.Delay(1000, cancellationToken: token);
            startMessageText.text = "1";
            startMessageText.transform.DOScale(toScale, 0.3f)
                .SetEase(Ease.OutQuart);
            startMessageText.transform.DOScale(fromScale, 0.3f)
                .SetEase(Ease.OutQuart);
            onPlayOneShot(GameAudioType.SeCount);
            await UniTask.Delay(1000, cancellationToken: token);
            startMessageText.text = "スタート！";
            startMessageText.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 360.0f), 0.35f, RotateMode.FastBeyond360);
            startMessageText.transform.DOScale(toScale, 0.3f)
                .SetEase(Ease.OutQuart);
            startMessageText.transform.DOScale(fromScale, 0.3f)
                .SetEase(Ease.OutQuart);
            onPlayOneShot(GameAudioType.SeWhistle);
            await UniTask.Delay(1200, cancellationToken: token);
            onNextCallback();
        }

        // チョコスコア
        [SerializeField] private GameObject chocoScoreArea;
        [SerializeField] private TextMeshProUGUI chocoScoreText;
        [SerializeField] private ShakeEffectBehaviour chocoScoreShakeEffect;
        public void SetActiveChocoScoreArea(bool isActive)
        {
            chocoScoreArea.gameObject.SetActive(isActive);
        }
        public void SetTextChocoScore(int score)
        {
            chocoScoreText.SetText(score.ToString());
        }
        public async void SetTextChocoScoreAsync(int score, UnityAction countAction, CancellationToken token)
        {
            var currentScore = int.Parse(chocoScoreText.text);
            var diffScore = score - currentScore;
            for (var i = 0; i < diffScore; i++)
            {
                currentScore++;
                chocoScoreText.SetText(currentScore.ToString());
                chocoScoreShakeEffect.StartShake(0.5f, 30.0f, 50.0f);
                countAction?.Invoke();
                await UniTask.Delay(100, cancellationToken: token);
            }
        }

        // 残り時間
        [SerializeField] private GameObject remainTimeArea;
        [SerializeField] private TextMeshProUGUI remainTimeText;
        public void SetActiveRemainTimeArea(bool isActive)
        {
            remainTimeArea.gameObject.SetActive(isActive);
        }
        public void SetTextRemainTime(float time, bool isAnim)
        {
            var prevTime = Mathf.CeilToInt(float.Parse(remainTimeText.text));
            var nextTime = Mathf.CeilToInt(time); // 切り上げ
            remainTimeText.SetText(nextTime.ToString());
            if (isAnim && prevTime != nextTime)
            {
                var fromScale = 1.0f;
                var toScale = 1.8f;
                remainTimeText.transform.localScale = fromScale * Vector3.one;
                remainTimeText.transform.DOScale(toScale, 0.3f)
                    .SetEase(Ease.OutQuart);
                remainTimeText.transform.DOScale(fromScale, 0.3f)
                    .SetEase(Ease.OutQuart);
            }
        }
    }
}
