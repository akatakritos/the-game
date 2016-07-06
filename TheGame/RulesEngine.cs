using System;
using System.Collections.Generic;
using System.Linq;

using TheGame;

public class RulesEngine
{
    private readonly GameState _state;

    public RulesEngine(GameState state)
    {
        _state = state;
    }

    public bool CanUseItem()
    {
        return DateTime.UtcNow > NextItemTime;
    }

    public DateTime NextItemTime => _state.LastItemUse + TimeSpan.FromMinutes(1);

    public bool CanAttack(string name)
    {
        return !GameState.UserWhitelist.Contains(name);
    }

    public const int PointsRateLimitMS = 1000;
}