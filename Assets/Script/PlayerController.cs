using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private RotateToMouse rotateToMouse;
    private MovementCharacterController movement;
    private PlayerAnimatorController animator;
    private WeaponSetting weaponSetting;

    private Gun gun;

    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keycodeRun = KeyCode.LeftShift;
    private Status status;
    [SerializeField]
    private KeyCode keycodeJump = KeyCode.Space;
    [SerializeField]
    private KeyCode keycodeReload = KeyCode.R;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;
    [SerializeField]
    private AudioClip audioClipRun;
    private AudioSource audioSource;

    [SerializeField]
    private Transform player;

    private float run = 0.5f;
    private CharacterController characterController;
    public static bool isDie;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementCharacterController>();
        status = GetComponent<Status>();
        animator = GetComponent<PlayerAnimatorController>();
        audioSource = GetComponent<AudioSource>();
        gun = GetComponentInChildren<Gun>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (GameManager.isPause || GameManager.isGameOver || GameManager.isClear)
        {
            audioSource.Stop();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            UpdateRotate();
            RotateRecoil();
            UpdateMove();
            UpdateJump();
            UpdateCrouch();
            UpdateWeaponAction();
            StartCoroutine("ChangeMove");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        //UpdateRotate();
        //RotateRecoil();
        //UpdateMove();
        //UpdateJump();
        //UpdateCrouch();
        //UpdateWeaponAction();
        //StartCoroutine("ChangeMove");
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    private void RotateRecoil()
    {
        float mouseX = Input.GetAxis("Mouse X");

        if (gun.isAttack == false) return;
        rotateToMouse.RotateRecoil(mouseX);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (x != 0 || z != 0)
        {
            bool isRun = false;
            if (z > 0) isRun = Input.GetKey(keycodeRun);
            if (gun.isReload) isRun = false;
            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            if (!isRun)
            {
                run = 0.5f;
                StopCoroutine("ChangeMove");
            }
            if (movement.isCrouch)
            {
                movement.MoveSpeed = status.CrouchSpeed;
                animator.MoveSpeed = 0.5f;
            }
            animator.MoveSpeed = isRun == true ? run : 0.5f;
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            movement.MoveSpeed = 0;
            animator.MoveSpeed = 0;

            if(audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }
        movement.MoveTo(new Vector3(x, 0, z));
    }

    private void UpdateCrouch()
    {
        movement.Crouch();
    }

    private void UpdateJump()
    {
        if (!characterController.isGrounded) audioSource.Stop();
        if (Input.GetKeyDown(keycodeJump))
        {
            movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gun.StartWeaponAction();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            gun.StopWeaponAction();
        }

        if (Input.GetKeyDown(keycodeReload))
        {
            gun.StartReload();
        }
        if (gun.autoReload)
        {
            gun.StartReload();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            gun.StartCheck();
        }
    }

    private IEnumerator ChangeMove()
    {
        while (run <= 1 && Input.GetKey(keycodeRun))
        {
            run += 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
        while (run <= 0.5 && Input.GetKey(keycodeRun))
        {
            run -= 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void TakeDamage(int damage)
    {
        isDie = status.DecreasePlayerHP(damage);
    }
}
