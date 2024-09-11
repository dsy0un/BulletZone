using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    [Header("Interaction Object")]
    [SerializeField]
    protected int maxObjectHP = 100;
    protected int currentObjectHP;

    private void Awake()
    {
        currentObjectHP = maxObjectHP;
    }

    public abstract void TakeDamage(int damage);
}
