using System; // 需要這個來使用 Action

public static class GameEvents
{
    public static Action OnPlayerDied; // 當玩家死亡時觸發的事件
    public static Action OnGameStart;  // <<<<<< 新增：當遊戲開始時觸發的事件 >>>>>>
}