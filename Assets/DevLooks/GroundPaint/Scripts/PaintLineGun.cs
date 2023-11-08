using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class PaintLineGun : PaintWeapon
{
    [Range(0, 2)]
    public float PaintWidth;
    [Range(0f, 200f)]
    public float FiringRange = 100f;
    [Range(0, 0.5f)]
    public float PaintSpacing = 0.2f;


    private Vector2 mPaintSize;

    private Vector3 mRayCastDir;
    private int mNearHitPosProp = Shader.PropertyToID("_FirstHitPos");
    private int mFarHitPosProp = Shader.PropertyToID("_SecondHitPos");
    private int mPaintSizeProp = Shader.PropertyToID("_PaintSize");

    private float mIntervalTime = 1;
    private float mCurrentTime = 0;

    const float rayCastRange = 20;
    public void Update()
    {
        mCurrentTime += Time.fixedDeltaTime;
        if (mCurrentTime > mIntervalTime)
        {
            Paint();
            mCurrentTime = 0;
        }
    }


    protected override void InitBrush()
    {
        base.InitBrush();
        mPaintSize = new Vector2(1, PaintWidth);  
        PaintMaterial.SetVector(mPaintSizeProp, mPaintSize);
    }
    protected override void UpdateBrush()
    {
        base.UpdateBrush();
        mPaintSize.x = 1;
        mPaintSize.y = PaintWidth;
        PaintMaterial.SetVector(mPaintSizeProp, mPaintSize);
    }

    public override void Paint()
    {
        mRayCastDir = transform.TransformDirection(Vector3.forward);

        RaycastHit hitInfoNear;
        RaycastHit hitInfoFar;
        Vector3 nearPos = transform.position;
        Vector3 farPos = transform.position + mRayCastDir * FiringRange;
        Physics.Raycast(nearPos, Vector3.down, out hitInfoNear, rayCastRange);
        Physics.Raycast(farPos, Vector3.down, out hitInfoFar, rayCastRange);
        Debug.DrawRay(transform.position, mRayCastDir * FiringRange, Color.red, 0.1f);
        Debug.DrawRay(nearPos, Vector3.down * rayCastRange, Color.red, 0.1f);
        Debug.DrawRay(farPos, Vector3.down * rayCastRange, Color.red, 0.1f);
        if (!PaintHelperInstance.CheckPaintObject(hitInfoNear) || !PaintHelperInstance.CheckPaintObject(hitInfoFar))
        {
            return;
        }
        PaintMaterial.SetVector(mNearHitPosProp, hitInfoNear.lightmapCoord);
        PaintMaterial.SetVector(mFarHitPosProp, hitInfoFar.lightmapCoord);
        Graphics.ExecuteCommandBuffer(Cmd);
    }
}
