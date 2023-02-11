using Chocolate.Players.Action;
using UnityEngine;

namespace Chocolate.Players
{
    public class PlayerStatusModel
    {
        private readonly int MaxParameterValue = 1000;
        public enum PlayerStatusType
        {
            Charm,
            Tech,
            Speed,
        }
        public class PlayerStatus
        {
            public int CharmParameter; // 魅力
            public int TechParameter;  // テクニック
            public int SpeedParameter; // スピード
            public PlayerStatus(int charmParameter, int techParameter, int speedParameter)
            {
                CharmParameter = charmParameter;
                TechParameter = techParameter;
                SpeedParameter = speedParameter;
            }
        }
        private PlayerStatus _playerStatus; // 基本パラメータ
        private PlayerStatus _addPlayerStatus; // 追加パラメータ
        private int _exp; // 経験値
        private int _lostExp;

        /// <summary>
        /// プレイヤーステータスの初期化
        /// </summary>
        public void InitializePlayerStatus(int charm, int tech, int speed, int exp)
        {
            _playerStatus = new PlayerStatus(charm, tech, speed);
            _addPlayerStatus = new PlayerStatus(0, 0, 0);
            _exp = exp;
            _lostExp = 0;
        }

        /// <summary>
        /// プレイヤーステータスを加算できるか？
        /// </summary>
        public bool IsCanAddPlayerStatus(int value, PlayerStatusType playerStatusType)
        {
            if (value > _exp - _lostExp) value = _exp - _lostExp; // 丸める
            if (value <= 0) return false;
            switch (playerStatusType)
            {
                case PlayerStatusType.Charm:
                    return _playerStatus.CharmParameter + _addPlayerStatus.CharmParameter < MaxParameterValue;
                case PlayerStatusType.Tech:
                    return _playerStatus.TechParameter + _addPlayerStatus.TechParameter < MaxParameterValue;
                case PlayerStatusType.Speed:
                    return _playerStatus.SpeedParameter + _addPlayerStatus.SpeedParameter < MaxParameterValue;
            }
            return false;
        }

        public bool IsMaxParameter(int value)
        {
            return value >= MaxParameterValue;
        }

        public bool IsAllMaxParameter()
        {
            return _playerStatus.CharmParameter >= MaxParameterValue
                   && _playerStatus.TechParameter >= MaxParameterValue
                   && _playerStatus.SpeedParameter >= MaxParameterValue;
        }

        /// <summary>
        /// プレイヤーステータスの加算
        /// </summary>
        public void AddPlayerStatus(int value, PlayerStatusType playerStatusType)
        {
            if (value > _exp - _lostExp) value = _exp - _lostExp; // 丸める
            switch (playerStatusType)
            {
                case PlayerStatusType.Charm:
                    var tmpAddCharParameter = _addPlayerStatus.CharmParameter;
                    _addPlayerStatus.CharmParameter += value;
                    var dispCharmStatus = Mathf.Min(_playerStatus.CharmParameter + _addPlayerStatus.CharmParameter, MaxParameterValue);
                    _lostExp += dispCharmStatus - (_playerStatus.CharmParameter + tmpAddCharParameter);
                    break;
                case PlayerStatusType.Tech:
                    var tmpAddTechParameter = _addPlayerStatus.TechParameter;
                    _addPlayerStatus.TechParameter += value;
                    var dispTechStatus = Mathf.Min(_playerStatus.TechParameter + _addPlayerStatus.TechParameter, MaxParameterValue);
                    _lostExp += dispTechStatus - (_playerStatus.TechParameter + tmpAddTechParameter);
                    break;
                case PlayerStatusType.Speed:
                    var tmpAddSpeedParameter = _addPlayerStatus.SpeedParameter;
                    _addPlayerStatus.SpeedParameter += value;
                    var dispSpeedStatus = Mathf.Min(_playerStatus.SpeedParameter + _addPlayerStatus.SpeedParameter, MaxParameterValue);
                    _lostExp += dispSpeedStatus - (_playerStatus.SpeedParameter + tmpAddSpeedParameter);
                    break;
            }
        }

        /// <summary>
        /// 表示するプレイヤーステータスの取得
        /// </summary>
        /// <returns></returns>
        public PlayerStatus GetDisplayPlayerStatus()
        {
            return new PlayerStatus(
                Mathf.Min(_playerStatus.CharmParameter + _addPlayerStatus.CharmParameter, MaxParameterValue),
                Mathf.Min(_playerStatus.TechParameter + _addPlayerStatus.TechParameter, MaxParameterValue),
                    Mathf.Min(_playerStatus.SpeedParameter + _addPlayerStatus.SpeedParameter, MaxParameterValue)
                );
        }

        public int GetRemainExp()
        {
            return _exp - _lostExp;
        }

        /// <summary>
        /// 成功確率の計算
        /// </summary>
        public int CalculateActionSuccessPercent(int minPercent, int maxPercent, PlayerActionType actionType)
        {
            var parameter = 0;
            switch (actionType)
            {
                case PlayerActionType.Approach:
                    parameter = _playerStatus.CharmParameter + _addPlayerStatus.CharmParameter;
                    break;
                case PlayerActionType.Please:
                    parameter = (_playerStatus.CharmParameter + _addPlayerStatus.CharmParameter
                                                              + _playerStatus.TechParameter +
                                                              _addPlayerStatus.TechParameter) / 2;
                    break;
                case PlayerActionType.Thief:
                    parameter = _playerStatus.TechParameter + _addPlayerStatus.TechParameter;
                    break;
            }
            var ratio = (float) parameter / MaxParameterValue;
            ratio = EaseOutStatus(ratio);
            // 最終的な確率の計算
            // パラメータが最大でない場合は、ステータスも-1しておく
            var successPercent = minPercent + Mathf.CeilToInt((maxPercent - minPercent) * ratio);
            if (successPercent == maxPercent && parameter < MaxParameterValue)
            {
                successPercent -= 1;
            }
            return successPercent;
        }

        /// <summary>
        /// プレイヤー速度の計算
        /// </summary>
        public float CalculatePlayerSpeed(float minValue, float maxValue)
        {
            var parameter = (_playerStatus.SpeedParameter + _addPlayerStatus.SpeedParameter);
            var ratio = (float) parameter / MaxParameterValue;
            ratio = EaseOutStatus(ratio);
            var successPercent = minValue + (maxValue - minValue) * ratio;
            if (Mathf.RoundToInt(successPercent*10) == Mathf.RoundToInt(maxValue*10) && parameter < MaxParameterValue)
            {
                successPercent -= 0.1f;
            }
            return successPercent;
        }

        // 成長曲線を意識して初めは成長が早い
        private float EaseOutStatus(float x) {
            return Mathf.Sqrt(1 - Mathf.Pow(1 - x, 2)); // EaseOutCirc
        }
    }
}
