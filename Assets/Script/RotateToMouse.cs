using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    private MovementCharacterController movement;

    [SerializeField]
    private float rotCamXAxisSpeed = 5;
    [SerializeField]
    private float rotCamYAxisSpeed = 3;

    private float limitMinX = -80;
    private float limitMaxX = 50;
    private float eulerAngleX;
    private float eulerAngleY;

    private void Awake()
    {
        movement = GetComponent<MovementCharacterController>();
    }

    public void UpdateRotate(float mouseX, float mouseY)
    {
        if (GameManager.isPause) return;
        eulerAngleY += mouseX * rotCamYAxisSpeed;
        eulerAngleX -= mouseY * rotCamXAxisSpeed;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    public void RotateRecoil(float mouseX)
    {
        eulerAngleY += mouseX * rotCamYAxisSpeed;
        eulerAngleX -= rotCamXAxisSpeed;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        if (movement.isCrouch) transform.rotation = Quaternion.Euler(eulerAngleX += 4.98f, eulerAngleY += Random.Range(-0.2f, 0.2f), 0);
        else transform.rotation = Quaternion.Euler(eulerAngleX += 4.95f, eulerAngleY += Random.Range(-0.4f, 0.4f), 0);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
