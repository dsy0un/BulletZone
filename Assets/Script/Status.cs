using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int> { }

[System.Serializable]
public class ScoreEvent : UnityEngine.Events.UnityEvent<int> { }

public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [HideInInspector]
    public ScoreEvent onScoreEvent = new ScoreEvent();

    [Header("Walk, Run, Crouch Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;

    public static int maxPlayerHP = 100;
    public static int currentPlayerHP;

    public static int maxEnemyHP = 100;
    public static int currentEnemyHP;

    [Header("Score")]
    [SerializeField]
    public int score = 0;
    public static int currentScore;

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float CrouchSpeed => crouchSpeed;

    public int CurrentPlayerHP => currentPlayerHP;
    public int MaxPlayerHP => maxPlayerHP;

    public int CurrentEnemyHP => currentEnemyHP;
    public int MaxEnemyHP => maxEnemyHP;

    public int Score => score;
    public int CurrentScore => currentScore;

    private void Awake()
    {
        currentPlayerHP = maxPlayerHP;
        currentEnemyHP = maxEnemyHP;
        currentScore = score;

        onScoreEvent.Invoke(score);
    }

    private void Update()
    {
        currentScore = CurrentScore;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (EnemyFSM.isEnemyDie == true)
        {
            currentScore += Random.Range(10, 100);
            onScoreEvent.Invoke(currentScore);
            EnemyFSM.isEnemyDie = false;
        }
    }

    public bool DecreasePlayerHP(int damage)
    {
        currentPlayerHP = currentPlayerHP - damage > 0 ? currentPlayerHP - damage : 0;

        onHPEvent.Invoke(currentPlayerHP);

        if (currentPlayerHP == 0)
        {
            return true;
        }

        return false;
    }

    public bool DecreaseEnemyHP(int damage)
    {
        currentEnemyHP = currentEnemyHP - damage > 0 ? currentEnemyHP - damage : 0;

        if (currentEnemyHP == 0)
        {
            return true;
        }

        return false;
    }
}
