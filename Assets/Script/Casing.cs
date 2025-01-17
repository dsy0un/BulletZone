using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float deactivateTime = 5.0f;
    [SerializeField]
    private float casingSpin = 1.0f;
    [SerializeField]
    private AudioClip[] audioClips;

    private Rigidbody rigidbody3D;
    private AudioSource audioSource;
    private MemoryPool memoryPool;

    public void Setup(MemoryPool pool, Vector3 direction)
    {
        rigidbody3D = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        memoryPool = pool;

        rigidbody3D.velocity = new Vector3(direction.x * 2.0f, 3.0f, direction.z * 2.0f);
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin), 
                                                  Random.Range(-casingSpin, casingSpin));

        StartCoroutine("DeactivateAfterTime");
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime);
        memoryPool.DeactivatePoolItem(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        int indax = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[indax];
        audioSource.Play();
    }
}
