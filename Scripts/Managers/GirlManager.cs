using System.Collections.Generic;
using Chocolate.Girls;
using Chocolate.Roads;
using UnityEngine;
using VContainer;

namespace Chocolate.Managers
{
    public class GirlManager
    {
        private readonly GirlFactory _girlFactory;
        private readonly RoadPassesBehaviour _roadPassesBehaviour;

        /// <summary>
        /// 女の子のPool用
        /// </summary>
        private static readonly int GirlPoolCount = 10;
        private List<Girl> _girlPoolList;

        /// <summary>
        /// 生成間隔
        /// </summary>
        private static readonly float GenerateIntervalTime = 1.0f;
        private float _intervalTotalTime = 0.0f;

        [Inject]
        public GirlManager(GirlFactory girlFactory, RoadPassesBehaviour roadPassesBehaviour)
        {
            _girlFactory = girlFactory;
            _roadPassesBehaviour = roadPassesBehaviour;
        }

        public void CreateGirls()
        {
            _intervalTotalTime = 0.0f;

            // 女の子を一定数生成しておく
            _girlPoolList = new List<Girl>();
            for (var i = 0; i < GirlPoolCount; i++)
            {
                var girl = _girlFactory.CreateGirl();
                girl.SetGirlId(i+1); // 一意となるようIDを振っておく
                _girlPoolList.Add(girl);
            }
        }

        public void UpdateGirls(int maxActiveGirlCount)
        {
            _intervalTotalTime += Time.deltaTime;

            var waitGirlList = new List<Girl>();
            var angryGirlList = new List<Girl>();
            for (var i = 0; i < _girlPoolList.Count; i++)
            {
                // 待機中、激怒中の女の子を調べる
                var girl = _girlPoolList[i];
                if (girl.IsCurrentState(GirlState.Wait))
                {
                    waitGirlList.Add(girl);
                }
                if (girl.IsAngryMove())
                {
                    angryGirlList.Add(girl);
                }

                // 状態の更新
                girl.OnUpdate();
            }

            // アクティブな女の子が最大数より少ない場合、新たに出現させる
            var activeGirlCount = _girlPoolList.Count - waitGirlList.Count - angryGirlList.Count; // 怒っている女の子は数えない
            if (activeGirlCount < maxActiveGirlCount && _intervalTotalTime >= GenerateIntervalTime)
            {
                // 若干の差異を持たせるため9割はスキップ
                if (Random.Range(0, 100) < 90) return;

                // 女の子出現！
                waitGirlList[0].StartWalk(_roadPassesBehaviour.GetRandomRoadPass());
                _intervalTotalTime = 0.0f;
            }
        }
    }
}
