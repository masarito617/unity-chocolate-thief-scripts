namespace Chocolate.Players
{
    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    public enum PlayerState
    {
        None,
        Wait,     // 待機
        Move,     // 移動
        Approach, // アプローチ
        Please,   // 土下座
        Thief,    // 強奪
        Dead,     // 死亡
    }
}
