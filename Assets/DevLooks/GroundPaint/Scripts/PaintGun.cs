using System;
using UnityEngine;

public class PaintGun : PaintWeapon
{
    public Vector3 RayCastDir = Vector3.forward;
    [Range(0, 10)]
    public int FireRate = 2;
    [Range(0f, 1f)]
    public float BrushRadius = 0.5f;
    [Range(0f, 200f)]
    public float FiringRange = 100f;

    protected int PaintRadiusProp = Shader.PropertyToID("_PaintRadius");
    protected int HitCenter = Shader.PropertyToID("_HitCenter");

    private Vector3 mRayCastDir = Vector3.zero;
    private float mIntervalTime = 0;
    private float mCurrentTime = 0;

    protected override void Start()
    {
        mIntervalTime = 1f / FireRate;
        base.Start();
    }

    private void Update()
    {
        mCurrentTime += Time.fixedDeltaTime;
        if(mCurrentTime > mIntervalTime)
        {
            Paint();
            mCurrentTime = 0;
        }
    }
    protected override void Init()
    {
        base.Init();
    }
    protected override void InitBrush()
    {
        base.InitBrush();
        PaintMaterial.SetFloat(PaintRadiusProp, BrushRadius);
    }

    protected override void UpdateBrush()
    {
        base.UpdateBrush();
        PaintMaterial.SetFloat(PaintRadiusProp, BrushRadius);
    }
    public override void Paint()
    {
        mRayCastDir = transform.TransformDirection(RayCastDir);
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, mRayCastDir, out hitInfo, FiringRange);
        Debug.DrawRay(transform.position, mRayCastDir * FiringRange, Color.red, 0.1f);
        var t = PaintHelper.Instance;
        if (!PaintHelperInstance.CheckPaintObject(hitInfo))
        {
            return;
        }
        PaintMaterial.SetVector(HitCenter, hitInfo.lightmapCoord);
        Graphics.ExecuteCommandBuffer(Cmd);
    }
}
