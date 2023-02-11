using System;
using UnityEngine;

namespace Chocolate.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Chocolate/GameSettings")]
    public class GameSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// モバイルプラットフォームか？
        /// </summary>
        [NonSerialized] public bool IsMobilePlatform;

        /// <summary>
        /// チューンアップ画面に直接遷移するか？
        /// </summary>
        [SerializeField] private bool initIsTransitionTuneUp;
        [NonSerialized] public bool IsTransitionTuneUp;

        /// <summary>
        /// エフェクト描画するか？
        /// </summary>
        [SerializeField] private bool initIsDrawEffect;
        [NonSerialized] public bool IsDrawEffect;

        /// <summary>
        /// EXPブーストオプション
        /// </summary>
        [NonSerialized] public bool IsExpBoostOption;

        /// <summary>
        /// 最後に広告再生してからのプレイ回数
        /// </summary>
        [NonSerialized] public int LastAdmobPlayCount;

        /// <summary>
        /// 各アクションの情報
        /// </summary>
        [SerializeField] private PlayerActionInfo initApproachActionInfo;
        [NonSerialized] public PlayerActionInfo ApproachActionInfo;
        [SerializeField] private PlayerActionInfo initPleaseActionInfo;
        [NonSerialized] public PlayerActionInfo PleaseActionInfo;
        [SerializeField] private PlayerActionInfo initThiefActionInfo;
        [NonSerialized] public PlayerActionInfo ThiefActionInfo;

        /// <summary>
        /// 移動速度
        /// </summary>
        [SerializeField] private float initPlayerSpeed;
        [NonSerialized] public float PlayerSpeed;
        [SerializeField] private float minPlayerSpeed;
        public float MinPlayerSpeed => minPlayerSpeed;
        [SerializeField] private float maxPlayerSpeed;
        public float MaxPlayerSpeed => maxPlayerSpeed;

        /// <summary>
        /// プレイヤーステータス
        /// </summary>
        [SerializeField] private int initAddPlayerStatus;
        [NonSerialized] public int AddPlayerStatus;
        [SerializeField] private int initPlayerExp;
        [NonSerialized] public int PlayerExp;
        [SerializeField] private int initPlayerStatusCharm;
        [NonSerialized] public int PlayerStatusCharm;
        [SerializeField] private int initPlayerStatusTech;
        [NonSerialized] public int PlayerStatusTech;
        [SerializeField] private int initPlayerStatusSpeed;
        [NonSerialized] public int PlayerStatusSpeed;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            // ランタイムでの書き込み用にコピー
            IsTransitionTuneUp = initIsTransitionTuneUp;
            IsDrawEffect = initIsDrawEffect;
            IsExpBoostOption = false; // 最初はfalse
            LastAdmobPlayCount = 100; // 最初は適当に大きな値を入れておく
            ApproachActionInfo = initApproachActionInfo.Clone();
            PleaseActionInfo = initPleaseActionInfo.Clone();
            ThiefActionInfo = initThiefActionInfo.Clone();
            PlayerSpeed = initPlayerSpeed;

            AddPlayerStatus = initAddPlayerStatus;
            PlayerExp = initPlayerExp;
            PlayerStatusCharm = initPlayerStatusCharm;
            PlayerStatusTech = initPlayerStatusTech;
            PlayerStatusSpeed = initPlayerStatusSpeed;
        }
    }

    [Serializable]
    public class PlayerActionInfo
    {
        // 成功確率
        public int minSuccessPercent;
        public int maxSuccessPercent;
        public int successPercent; // タイトル画面で計算して設定する

        public PlayerActionInfo Clone()
        {
            var copyActionInfo = new PlayerActionInfo();
            copyActionInfo.minSuccessPercent = minSuccessPercent;
            copyActionInfo.maxSuccessPercent = maxSuccessPercent;
            copyActionInfo.successPercent = successPercent;
            return copyActionInfo;
        }
    }
}
