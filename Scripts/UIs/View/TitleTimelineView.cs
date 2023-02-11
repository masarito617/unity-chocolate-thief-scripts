using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Chocolate.UIs.View
{
    public class TitleTimelineView : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector;
        private UnityAction _endAnimationCallback;

        private bool _isPlayAnimation;
        public bool IsPlayAnimation()
        {
            return _isPlayAnimation;
        }

        public void StartTitleTimeline(UnityAction endCallback)
        {
            _isPlayAnimation = true;
            playableDirector.Play();
            _endAnimationCallback = endCallback;
        }

        public void SkipAnimation()
        {
            _isPlayAnimation = false;
            playableDirector.Stop();
            _endAnimationCallback?.Invoke();
        }

        // Signalで呼ばれる想定
        public void EndAnimation()
        {
            Debug.Log("EndAnimation");
            _isPlayAnimation = false;
            playableDirector.Stop();
            _endAnimationCallback?.Invoke();
        }
    }
}
