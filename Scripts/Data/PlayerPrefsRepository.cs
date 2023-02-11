using Chocolate.Players;
using Chocolate.Services;
using UnityEngine;
using VContainer;

namespace Chocolate.Data
{
    /// <summary>
    /// PlayerPrefsリポジトリ
    /// </summary>
    public class PlayerPrefsRepository
    {
        private readonly IPlayerPrefsService _playerPrefsService;

        [Inject]
        public PlayerPrefsRepository(IPlayerPrefsService playerPrefsService)
        {
            _playerPrefsService = playerPrefsService;
        }

        /// <summary>
        /// エフェクト描画するか？
        /// </summary>
        private static readonly string SaveKeyIsDrawEffect = "SaveKeyIsDrawEffect";
        public bool GetIsDrawEffect()
        {
            return _playerPrefsService.GetBool(SaveKeyIsDrawEffect);
        }
        public void SaveIsDrawEffect(bool isOn)
        {
            _playerPrefsService.SetBool(SaveKeyIsDrawEffect, isOn);
        }

        /// <summary>
        /// ステータスMAXでクリア済か？
        /// </summary>
        private static readonly string SaveKeyIsClearStatusMax = "SaveKeyIsStatusMaxClear";
        public bool GetIsClearStatusMax()
        {
            return _playerPrefsService.GetBool(SaveKeyIsClearStatusMax);
        }
        public void SaveIsClearStatusMax(bool isOn)
        {
            _playerPrefsService.SetBool(SaveKeyIsClearStatusMax, isOn);
        }

        /// <summary>
        /// ヘルプウィンドウを一度でも表示したか？
        /// </summary>
        private static readonly string SaveKeyIsEvenOnceHelpWindow = "SaveKeyIsEvenOnceHelpWindow";
        public bool GetIsEvenOnceHelpWindow()
        {
            return _playerPrefsService.GetBool(SaveKeyIsEvenOnceHelpWindow);
        }
        public void SaveIsEvenOnceHelpWindow(bool isOn)
        {
            _playerPrefsService.SetBool(SaveKeyIsEvenOnceHelpWindow, isOn);
        }

        /// <summary>
        /// ストアレビューを一度でも表示したか？
        /// </summary>
        private static readonly string SaveKeyIsEvenOnceStoreReview = "SaveKeyIsEvenOnceStoreReview";
        public bool GetIsEvenOnceStoreReview()
        {
            return _playerPrefsService.GetBool(SaveKeyIsEvenOnceStoreReview);
        }
        public void SaveIsEvenOnceStoreReview(bool isOn)
        {
            _playerPrefsService.SetBool(SaveKeyIsEvenOnceStoreReview, isOn);
        }

        // ---------- スコア関連 ----------
        private static readonly string SaveKeyTotalChocoScore = "SaveKeyTotalChocoScore";
        public int GetTotalChocoScore()
        {
            return _playerPrefsService.GetInt(SaveKeyTotalChocoScore);
        }
        public void SaveTotalChocoScore(int value)
        {
            value = Mathf.Min(value, 9999);
            _playerPrefsService.SetInt(SaveKeyTotalChocoScore, value);
        }
        private static readonly string SaveKeyBestChocoScore = "SaveKeyBestChocoScore";
        public int GetBestChocoScore()
        {
            return _playerPrefsService.GetInt(SaveKeyBestChocoScore);
        }
        public void SaveBestChocoScore(int value)
        {
            value = Mathf.Min(value, 9999);
            _playerPrefsService.SetInt(SaveKeyBestChocoScore, value);
        }

        // ---------- ステータス関連 ----------
        /// <summary>
        /// EXP
        /// </summary>
        private static readonly string SaveKeyPlayerExp = "SaveKeyPlayerExp";
        public int GetPlayerExp()
        {
            return _playerPrefsService.GetInt(SaveKeyPlayerExp);
        }
        public void SavePlayerExp(int value)
        {
            value = Mathf.Min(value, 9999);
            _playerPrefsService.SetInt(SaveKeyPlayerExp, value);
        }

        /// <summary>
        /// ステータス
        /// </summary>
        private static readonly string SaveKeyPlayerStatusCharm = "SaveKeyPlayerStatusCharm";
        private static readonly string SaveKeyPlayerStatusTech = "SaveKeyPlayerStatusTech";
        private static readonly string SaveKeyPlayerStatusSpeed = "SaveKeyPlayerStatusSpeed";
        public PlayerStatusModel.PlayerStatus GetPlayerStatus()
        {
            return new PlayerStatusModel.PlayerStatus(
                    _playerPrefsService.GetInt(SaveKeyPlayerStatusCharm),
                    _playerPrefsService.GetInt(SaveKeyPlayerStatusTech),
            _playerPrefsService.GetInt(SaveKeyPlayerStatusSpeed)
                );
        }
        public void SavePlayerStatus(int charmParameter, int techParameter, int speedParameter)
        {
            _playerPrefsService.SetInt(SaveKeyPlayerStatusCharm, charmParameter);
            _playerPrefsService.SetInt(SaveKeyPlayerStatusTech, techParameter);
            _playerPrefsService.SetInt(SaveKeyPlayerStatusSpeed, speedParameter);
        }

        // ---------- オーディオ関連 ----------
        /// <summary>
        /// SEボリュームオフフラグ
        /// </summary>
        private static readonly string SaveKeyIsSeVolumeOff = "SaveKeyIsSeVolumeOff";
        public bool GetIsSeVolumeOff()
        {
            return _playerPrefsService.GetBool(SaveKeyIsSeVolumeOff);
        }
        public void SaveIsSeVolumeOff(bool isVolumeOff)
        {
            _playerPrefsService.SetBool(SaveKeyIsSeVolumeOff, isVolumeOff);
        }

        /// <summary>
        /// BGMボリュームオフフラグ
        /// </summary>
        private static readonly string SaveKeyIsBgmVolumeOff = "SaveKeyIsBgmVolumeOff";
        public bool GetIsBgmVolumeOff()
        {
            return _playerPrefsService.GetBool(SaveKeyIsBgmVolumeOff);
        }
        public void SaveIsBgmVolumeOff(bool isVolumeOff)
        {
            _playerPrefsService.SetBool(SaveKeyIsBgmVolumeOff, isVolumeOff);
        }
    }
}
