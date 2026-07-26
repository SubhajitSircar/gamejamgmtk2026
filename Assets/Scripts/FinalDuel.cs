using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalDuel : MonoBehaviour
{
    [Header("P1 UI")]
    public GameObject p1Panel;
    public TextMeshProUGUI[] p1KeyTexts;

    [Header("P2 UI")]
    public GameObject p2Panel;
    public TextMeshProUGUI[] p2KeyTexts;

    [Header("Controllers to Lock")]
    public ArmController p1Arm;
    public ArmController p2Arm;

    // Specifically grabbing the Tri-Lasers for the final round
    public TriLaserShoot p1Gun;
    public TriLaserShoot p2Gun;

    [Header("Arm Lock Settings")]
    public Transform p1ArmTransform;
    public Transform p2ArmTransform;

    public float p1LockedAngle = -90f;
    public float p2LockedAngle = 90f;

    [Header("Settings")]
    public int keysPerSet = 5;
    public int totalSets = 3;

    private readonly KeyCode[] p1Keys =
    {
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D
    };

    private readonly KeyCode[] p2Keys =
    {
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow
    };

    private List<KeyCode> p1Sequence;
    private List<KeyCode> p2Sequence;

    private int p1Index;
    private int p2Index;

    private int p1CurrentSet;
    private int p2CurrentSet;

    private bool challengeStarted;
    private bool p1Finished;
    private bool p2Finished;

    void Start()
    {
        p1Panel.SetActive(false);
        p2Panel.SetActive(false);

        p1Arm.enabled = false;
        p2Arm.enabled = false;

        // Disable the Tri-Lasers entirely so they don't fire or draw sweeping lasers early
        if (p1Gun != null) p1Gun.enabled = false;
        if (p2Gun != null) p2Gun.enabled = false;

        LockP1Arm();
        LockP2Arm();
    }

    void Update()
    {
        if (!challengeStarted && GameManager.instance != null && GameManager.instance.isDuelActive)
        {
            StartChallenge();
        }

        if (!challengeStarted) return;

        if (!p1Finished) CheckP1Input();
        if (!p2Finished) CheckP2Input();
    }

    void LateUpdate()
    {
        if (!p1Finished) LockP1Arm();
        if (!p2Finished) LockP2Arm();
    }

    void LockP1Arm()
    {
        if (p1ArmTransform != null)
        {
            p1ArmTransform.localRotation = Quaternion.Euler(0f, 0f, p1LockedAngle);
        }
    }

    void LockP2Arm()
    {
        if (p2ArmTransform != null)
        {
            p2ArmTransform.localRotation = Quaternion.Euler(0f, 0f, p2LockedAngle);
        }
    }

    void StartChallenge()
    {
        challengeStarted = true;
        p1Panel.SetActive(true);
        p2Panel.SetActive(true);

        GenerateP1Set();
        GenerateP2Set();
    }

    // =========================
    // P1 Logic
    // =========================

    void GenerateP1Set()
    {
        p1Sequence = GenerateSequence(p1Keys);
        p1Index = 0;
        DisplaySequence(p1Sequence, p1KeyTexts);
    }

    void CheckP1Input()
    {
        KeyCode pressedKey = KeyCode.None;

        if (Input.GetKeyDown(KeyCode.W)) pressedKey = KeyCode.W;
        else if (Input.GetKeyDown(KeyCode.A)) pressedKey = KeyCode.A;
        else if (Input.GetKeyDown(KeyCode.S)) pressedKey = KeyCode.S;
        else if (Input.GetKeyDown(KeyCode.D)) pressedKey = KeyCode.D;

        if (pressedKey == KeyCode.None) return;

        if (pressedKey == p1Sequence[p1Index])
        {
            p1Index++;
            UpdateP1Visuals();

            if (p1Index >= keysPerSet)
            {
                p1CurrentSet++;
                if (p1CurrentSet >= totalSets) CompleteP1();
                else GenerateP1Set();
            }
        }
    }

    // =========================
    // P2 Logic
    // =========================

    void GenerateP2Set()
    {
        p2Sequence = GenerateSequence(p2Keys);
        p2Index = 0;
        DisplaySequence(p2Sequence, p2KeyTexts);
    }

    void CheckP2Input()
    {
        KeyCode pressedKey = KeyCode.None;

        if (Input.GetKeyDown(KeyCode.UpArrow)) pressedKey = KeyCode.UpArrow;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) pressedKey = KeyCode.DownArrow;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) pressedKey = KeyCode.LeftArrow;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) pressedKey = KeyCode.RightArrow;

        if (pressedKey == KeyCode.None) return;

        if (pressedKey == p2Sequence[p2Index])
        {
            p2Index++;
            UpdateP2Visuals();

            if (p2Index >= keysPerSet)
            {
                p2CurrentSet++;
                if (p2CurrentSet >= totalSets) CompleteP2();
                else GenerateP2Set();
            }
        }
    }

    // =========================
    // Sequence Generation & UI
    // =========================

    List<KeyCode> GenerateSequence(KeyCode[] availableKeys)
    {
        List<KeyCode> sequence = new List<KeyCode>();
        for (int i = 0; i < keysPerSet; i++)
        {
            sequence.Add(availableKeys[Random.Range(0, availableKeys.Length)]);
        }
        return sequence;
    }

    void DisplaySequence(List<KeyCode> sequence, TextMeshProUGUI[] texts)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = GetKeyName(sequence[i]);
            texts[i].color = Color.white;
        }
    }

    void UpdateP1Visuals()
    {
        for (int i = 0; i < p1Index; i++) p1KeyTexts[i].color = Color.green;
    }

    void UpdateP2Visuals()
    {
        for (int i = 0; i < p2Index; i++) p2KeyTexts[i].color = Color.green;
    }

    string GetKeyName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.UpArrow: return "↑";
            case KeyCode.DownArrow: return "↓";
            case KeyCode.LeftArrow: return "←";
            case KeyCode.RightArrow: return "→";
            default: return key.ToString();
        }
    }

    // =========================
    // Completion
    // =========================

    void CompleteP1()
    {
        p1Finished = true;
        p1Panel.SetActive(false);

        // Unlock the arm and the Tri-Laser gun!
        p1Arm.enabled = true;
        if (p1Gun != null) p1Gun.enabled = true;
    }

    void CompleteP2()
    {
        p2Finished = true;
        p2Panel.SetActive(false);

        // Unlock the arm and the Tri-Laser gun!
        p2Arm.enabled = true;
        if (p2Gun != null) p2Gun.enabled = true;
    }
}