using System.Collections.Generic;
using Chocolate.Const;
using Chocolate.Girls;
using UnityEngine;

namespace Chocolate.Players.Action
{
    public class PlayerActionArea : MonoBehaviour
    {
        /// <summary>
        /// 衝突中の女の子
        /// </summary>
        private readonly List<GirlBehaviour> _triggerGirlList = new List<GirlBehaviour>();

        private void LateUpdate()
        {
            // Update終わりにリセットする
            foreach (var triggerGirl in _triggerGirlList)
            {
                triggerGirl.SetIsPlayerActionTrigger(false);
            }
            _triggerGirlList.Clear();
        }

        private void OnTriggerStay(Collider other)
        {
            // 女の子と衝突したら保持する
            if (other.CompareTag(GameConst.TagNameGirl))
            {
                var girlBehaviour = other.GetComponentInParent<GirlBehaviour>();
                if (girlBehaviour == null) Debug.Log("GirlBehaviour is null.");

                // 同一の女の子に複数回衝突することがあったのでIDチェックを入れる
                foreach (var triggerGirl in _triggerGirlList)
                {
                    if (triggerGirl.girlId == girlBehaviour.girlId) return;
                }
                _triggerGirlList.Add(girlBehaviour);
                girlBehaviour.SetIsPlayerActionTrigger(true);
            }
        }

        /// <summary>
        /// 衝突している女の子
        /// </summary>
        public List<GirlBehaviour> GetTriggerActiveGirls()
        {
            List<GirlBehaviour> triggerActiveGirlList = new List<GirlBehaviour>();
            foreach (var triggerGirl in _triggerGirlList)
            {
                if (!triggerGirl.IsEvenTakeAction()) triggerActiveGirlList.Add(triggerGirl);
            }
            return triggerActiveGirlList;
        }
        public bool NotifyActionTriggerGirls(PlayerActionType actionType, bool isSuccess)
        {
            foreach (var triggerGirl in _triggerGirlList)
            {
                triggerGirl.ReceivePlayerAction(actionType, isSuccess);
            }
            return isSuccess;
        }

        /// <summary>
        /// 衝突しているかつ怒っている女の子
        /// </summary>
        public int GetTriggerAngryGirlsCount()
        {
            var triggerAngryGirlCount = 0;
            foreach (var triggerGirl in _triggerGirlList)
            {
                if (triggerGirl.IsAngryMove()) triggerAngryGirlCount++;
            }
            return triggerAngryGirlCount;
        }
        public void NotifyActionTriggerAngryGirls()
        {
            foreach (var triggerGirl in _triggerGirlList)
            {
                if (triggerGirl.IsAngryMove()) triggerGirl.ReceiveDownAngry();
            }
        }
    }
}
