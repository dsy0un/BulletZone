using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform movement;
    private float projectileDistance = 200;
    private int damage = 10;

    private Vector3 move;

    public void Setup(Vector3 position)
    {
        movement = GetComponent<MovementTransform>();

        StartCoroutine("OnMove", position);
    }

    private IEnumerator OnMove(Vector3 targetPosition)
    {
        move = (targetPosition - transform.position).normalized;

        Vector3 start = transform.position;

        movement.MoveTo(move);

        while (true)
        {
            if (Vector3.Distance(transform.position, start) >= projectileDistance)
            {
                Destroy(gameObject);

                yield break;
            }

            yield return null;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        other.GetComponent<PlayerController>().TakeDamage(damage);

    //        Destroy(gameObject);
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerController>().TakeDamage(damage);

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("ImpactNormal") || collision.gameObject.CompareTag("ImpactObstacle") ||
            collision.gameObject.CompareTag("ImpactEnemy") || collision.gameObject.CompareTag("ImpactBox") ||
            collision.gameObject.CompareTag("ImpactGold") || collision.gameObject.CompareTag("ImpactRed"))
        {
            Vector3 newMove = Vector3.Reflect(move, collision.contacts[0].normal);
            movement.MoveTo(newMove);
        }
    }
}
