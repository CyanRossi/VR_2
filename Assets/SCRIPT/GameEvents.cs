// GameEvents.cs  (UNCHANGED)  :contentReference[oaicite:4]{index=4}
using System;
using UnityEngine;

public enum DartRing
{
    Miss, InnerBull, OuterBull, Single, Double, Triple
}

public static class GameEvents
{
    public static Action<Dart> OnDartThrown;
    public static Action<int, DartRing, int, Vector3> OnDartScored;
    public static Action OnRackRefilled;

    // UI helpers
    public static Action<int> OnTotalScoreChanged;
    public static Action<int> OnDartsRemainingChanged;
    public static Action<string> OnLastHitText;

    public static Action<bool> OnPauseToggled;
}
