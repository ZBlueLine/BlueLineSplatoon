using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class PaintRoller : PaintWeapon
{
    public Transform RollerLeftPos;
    public Transform RollerRightPos;
    [Range(0f, 1f)]
    public float FiringRange = 0.5f;
    [Range(1, 10f)]
    public float PaintWidth;
    [Range(0, 1f)]
    public float PaintHeight;
    [Range(0, 0.5f)]
    public float PaintSpacing = 0.2f;


    private Vector2 mPaintSize;
    private Vector3 mPreviousPos = Vector3.zero;
    private Quaternion mPreviousRotation = Quaternion.identity;

    private Vector3 mRayCastDir = Vector3.down;
    private int mLeftHitPosProp = Shader.PropertyToID("_FirstHitPos");
    private int mRightHitPosProp = Shader.PropertyToID("_SecondHitPos");
    private int mPaintSizeProp = Shader.PropertyToID("_PaintSize");

    public void Update()
    {
        if(Vector3.Distance(mPreviousPos, transform.position) > PaintSpacing || Quaternion.Angle(mPreviousRotation, transform.rotation) > 0.4f)
        {
            Paint();
            mPreviousPos = transform.position;
            mPreviousRotation = transform.rotation;
        }
    }


    protected override void InitBrush()
    {
        base.InitBrush();
        mPaintSize = new Vector2(PaintWidth, PaintHeight);  
        PaintMaterial.SetVector(mPaintSizeProp, mPaintSize);
    }
    protected override void UpdateBrush()
    {
        base.UpdateBrush();
        mPaintSize.x = PaintWidth;
        mPaintSize.y = PaintHeight;
        PaintMaterial.SetVector(mPaintSizeProp, mPaintSize);
    }

    public override void Paint()
    {
        RaycastHit hitInfoLeft;
        RaycastHit hitInfoRight;
        int layer = 3; 
        int layerMask = 1 << layer;
        Physics.Raycast(RollerLeftPos.position, mRayCastDir, out hitInfoLeft, FiringRange, layerMask);
        Physics.Raycast(RollerRightPos.position, mRayCastDir, out hitInfoRight, FiringRange, layerMask);
        Debug.DrawRay(RollerLeftPos.position, mRayCastDir * FiringRange, Color.red, 0.1f);
        Debug.DrawRay(RollerRightPos.position, mRayCastDir * FiringRange, Color.red, 0.1f);
        if (!PaintHelperInstance.CheckPaintObject(hitInfoRight) || !PaintHelperInstance.CheckPaintObject(hitInfoLeft))
        {
            return;
        }
        PaintMaterial.SetVector(mLeftHitPosProp, hitInfoLeft.lightmapCoord);
        PaintMaterial.SetVector(mRightHitPosProp, hitInfoRight.lightmapCoord);
        Debug.Log(string.Format("Right:{0}, Left:{1}", hitInfoRight.lightmapCoord, hitInfoLeft.lightmapCoord));
        Graphics.ExecuteCommandBuffer(Cmd);
    }
}
