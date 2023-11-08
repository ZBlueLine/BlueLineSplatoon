using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PaintClick : PaintWeapon
{
    public new Camera camera;
    [Range(0f, 1f)]
    public float BrushRadius = 0.5f;
    public float RayCastInterval = 0.1f;
    [Range(0f, 200f)]
    public float FiringRange = 100f;

    private float mMousePaintCurrentTime = 0;
    bool mMouseDownState = false;

    protected int PaintRadiusProp = Shader.PropertyToID("_PaintRadius");
    protected int HitCenter = Shader.PropertyToID("_HitCenter");
    void Update()
    {
        DoMouseRayCast();
    }
    public void DoMouseRayCast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mMouseDownState = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            mMouseDownState = false;
        }
        if (mMousePaintCurrentTime > RayCastInterval)
        {
            if (mMouseDownState)
            {
                mMousePaintCurrentTime = 0;
                Paint();
            }
        }
        mMousePaintCurrentTime += Time.deltaTime;
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
        RaycastHit hitInfo;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hitInfo, FiringRange);
        if (!PaintHelperInstance.CheckPaintObject(hitInfo))
        {
            return;
        }
        PaintMaterial.SetVector(HitCenter, hitInfo.lightmapCoord);
        Graphics.ExecuteCommandBuffer(Cmd);
    }
}
