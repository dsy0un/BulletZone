using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int,int> { }
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class Gun : MonoBehaviour
{
    public GameObject bullet;

    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent onMagazineEvent = new MagazineEvent();

    [Header("Fire Effect")]
    [SerializeField]
    private GameObject muzzleFlashEffect;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint;
    [SerializeField]
    private Transform bulletSpawnPoint;
    private ImpactMemoryPool impactMemoryPool;

    private Camera mainCamera;

    private CasingMemoryPool casingMemoryPool;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutGun;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip audioClipFire;
    [SerializeField]
    private AudioClip audioClipReload;
    [SerializeField]
    private AudioClip audioClipCheckIn;
    [SerializeField]
    private AudioClip audioClipCheckOut;
    [SerializeField]
    private AudioClip audioClipCheckArming;
    [SerializeField]
    private AudioClip audioClipCheckCocked;

    [Header("Weapon Setting")]
    [SerializeField]
    public WeaponSetting weaponSetting;

    private float lastAttackTime = 0;
    public bool isReload = false;
    public bool isAttack;
    public bool autoReload;

    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int Maxmagazine => weaponSetting.maxMagazine;

    private PlayerAnimatorController animator;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        weaponSetting.currentMagazine = weaponSetting.maxMagazine;

        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutGun);
        muzzleFlashEffect.SetActive(false);

        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

    }

    public void StartWeaponAction(int type = 0)
    {
        if (isReload == true) return;
        if (type == 0)
        {
            if (weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            else
            {
                OnAttack();
            }
        }
    }
    public void StopWeaponAction(int type = 0)
    {
        isAttack = false;
        if (type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
        else
        {
            OnAttack();
        }
    }

    public void StartReload()
    {
        if (isReload == true) return;
        StopWeaponAction();

        if (weaponSetting.currentAmmo <= 0) StartCoroutine("OnReload");
        // else if (weaponSetting.currentAmmo < weaponSetting.maxAmmo) StartCoroutine("OnHalfReload");
        else if (weaponSetting.currentAmmo < weaponSetting.maxAmmo) StartCoroutine("OnHalfReload");
    }

    public void StartCheck()
    {
        if (isReload == true) return;
        StopWeaponAction();

        StartCoroutine("OnCheck");
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            if (animator.MoveSpeed > 0.5f) return;
            if (weaponSetting.currentAmmo <= 0)
            {
                autoReload = true;
                isAttack = false;
                return;
            }

            isAttack = true;

            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            lastAttackTime = Time.time;
            animator.Play("Fire", -1, 0);
            StartCoroutine("OnMuzzleFlashEffect");
            PlaySound(audioClipFire);

            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            TwoStepRaycast();
        }
    }

    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload()
    {
        isReload = true;
        autoReload = false;

        if (weaponSetting.currentAmmo <= 0)
        {
            animator.OnReload();
            yield return new WaitForSeconds(0.38f);
            PlaySound(audioClipReload);
        }

        while (true)
        {
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                //if (weaponSetting.currentAmmo < weaponSetting.maxAmmo)
                //{
                //    weaponSetting.currentMagazine--;
                //    onMagazineEvent.Invoke(weaponSetting.currentMagazine);
                //}

                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator OnHalfReload()
    {
        isReload = true;
        animator.OnHalfReload();
        yield return new WaitForSeconds(0.38f);
        PlaySound(audioClipReload);

        while (true)
        {
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                //if (weaponSetting.currentAmmo < weaponSetting.maxAmmo)
                //{
                //    weaponSetting.currentMagazine--;
                //    onMagazineEvent.Invoke(weaponSetting.currentMagazine);
                //}

                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator OnCheck()
    {
        isReload = true;
        // if (weaponSetting.currentAmmo < weaponSetting.maxAmmo) yield break;

        animator.OnCheck();
        yield return new WaitForSeconds(0.38f);
        PlaySound(audioClipCheckOut);
        yield return new WaitForSeconds(0.6f);
        PlaySound(audioClipCheckCocked);
        yield return new WaitForSeconds(1);
        PlaySound(audioClipCheckCocked);
        yield return new WaitForSeconds(1.3f);
        PlaySound(audioClipCheckIn);

        while (true)
        {
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;
                yield break;
            }
            yield return null;
        }
    }

    void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
