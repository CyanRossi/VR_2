// DartsManager.cs  (UPDATED for XRI; no GrabbableToggle)  :contentReference[oaicite:3]{index=3}
using UnityEngine;
using System.Collections;

public class DartsManager : MonoBehaviour
{
    [Header("Prefab & Holders")]
    public GameObject dartPrefab;     // NOTE: must include: Rigidbody, CapsuleCollider, XRThrowableOnActivate, Dart, DartStickOnHit (+optional AimAssist)
    public Transform leftHandHolder;
    public Transform[] leftHandSlots; // size 3
    public Transform boardRoot;       // parent for stuck darts on the board (optional)

    int dartsRemaining;
    int sticksThisRack;
    int totalScore;

    void OnEnable()
    {
        GameEvents.OnDartThrown += OnThrown;
        GameEvents.OnDartScored += OnScored;
    }
    void OnDisable()
    {
        GameEvents.OnDartThrown -= OnThrown;
        GameEvents.OnDartScored -= OnScored;
    }

    void Start()
    {
        totalScore = 0;
        SpawnThree();
        GameEvents.OnTotalScoreChanged?.Invoke(totalScore);
        GameEvents.OnDartsRemainingChanged?.Invoke(dartsRemaining);
    }

    void SpawnThree()
    {
        // clear left rack
        for (int i = leftHandHolder.childCount - 1; i >= 0; i--)
            Destroy(leftHandHolder.GetChild(i).gameObject);

        dartsRemaining = 3;
        sticksThisRack = 0;

        for (int i = 0; i < 3 && i < leftHandSlots.Length; i++)
        {
            var go = Instantiate(dartPrefab, leftHandSlots[i].position, leftHandSlots[i].rotation, leftHandHolder);

            // optional: set stickParentAtBoard if your prefab has DartStickOnHit
            var stick = go.GetComponent<DartStickOnHit>();
            if (stick && boardRoot) stick.stickParentAtBoard = boardRoot;

            // keep it kinematic in rack
            var rb = go.GetComponent<Rigidbody>();
            if (rb) { rb.isKinematic = true; rb.useGravity = false; }
        }

        GameEvents.OnDartsRemainingChanged?.Invoke(dartsRemaining);
        GameEvents.OnRackRefilled?.Invoke();
    }

    void OnThrown(Dart _)
    {
        dartsRemaining = Mathf.Max(0, dartsRemaining - 1);
        GameEvents.OnDartsRemainingChanged?.Invoke(dartsRemaining);
        // start refill only after all three have stuck
        if (dartsRemaining == 0) StartCoroutine(WaitAndRefill());
    }

    void OnScored(int score, DartRing ring, int sector, Vector3 _)
    {
        sticksThisRack++;
        totalScore += score;
        GameEvents.OnTotalScoreChanged?.Invoke(totalScore);
    }

    IEnumerator WaitAndRefill()
    {
        // wait until all 3 are stuck
        while (sticksThisRack < 3) yield return null;
        // wait 3 seconds, then clear board darts and respawn rack
        yield return new WaitForSeconds(3f);

        if (boardRoot)
        {
            for (int i = boardRoot.childCount - 1; i >= 0; i--)
                Destroy(boardRoot.GetChild(i).gameObject);
        }
        SpawnThree();
    }
}
