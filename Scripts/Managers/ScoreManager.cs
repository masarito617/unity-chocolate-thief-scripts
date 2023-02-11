using System;
using Chocolate.Data;
using Chocolate.Data.Event;
using MessagePipe;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chocolate.Managers
{
    public class ScoreManager : IDisposable {
        /// <summary>
        /// スコア
        /// </summary>
        private ReactiveProperty<int> _chocoScore = new(0);
        public IReadOnlyReactiveProperty<int> ChocoScore => _chocoScore;

        /// <summary>
        /// スコア通知
        /// </summary>
        private readonly ISubscriber<ScoreData> _onReceiveScoreEvent;
        private IDisposable _disposable;

        [Inject]
        public ScoreManager(ISubscriber<ScoreData> onReceiveScoreEvent)
        {
            _onReceiveScoreEvent = onReceiveScoreEvent;
        }

        public void OnInitialize()
        {
            var d = DisposableBag.CreateBuilder();
            _onReceiveScoreEvent.Subscribe(scoreData =>
            {
                _chocoScore.Value += scoreData.GetChocoCount;
                Debug.Log("ChocoScore: " + _chocoScore);
            });
            _disposable = d.Build();
        }

        public void ResetScore()
        {
            _chocoScore.Value = 0;
        }

        public int GetScore()
        {
            return _chocoScore.Value;
        }

        public int GetAddExp()
        {
            // 2倍にして5の倍数にしたものを経験値にする
            var score = _chocoScore.Value * 2;
            return score - score % 5;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _chocoScore.Dispose();
        }
    }
}
