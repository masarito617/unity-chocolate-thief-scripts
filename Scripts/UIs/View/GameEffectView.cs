using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Chocolate.UIs.View
{
    public class GameEffectView : MonoBehaviour
    {
        [SerializeField] private RectTransform comicLineEffectTransform;

        private void Awake()
        {
            comicLineEffectTransform.gameObject.SetActive(true); // 一度activeにしておけばキャッシュされるようなので、固まるタイミングが早くなる(アクション実行時よりはマシ)
        }

        private void Start()
        {
            comicLineEffectTransform.gameObject.SetActive(false);
        }

        public async void ShowComicLineEffect(CancellationToken token)
        {
            comicLineEffectTransform.gameObject.SetActive(true);
            var hideScale = Vector3.one * 3;
            var showScale = Vector3.one * 1.2f;
            comicLineEffectTransform.localScale = hideScale;
            comicLineEffectTransform.DOScale(showScale, 0.2f);
            await UniTask.Delay(500, cancellationToken: token);
            comicLineEffectTransform.DOScale(hideScale, 0.5f);
            comicLineEffectTransform.gameObject.SetActive(false);
        }
    }
}
