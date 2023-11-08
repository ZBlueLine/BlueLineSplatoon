using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

public enum PaintColor
{
    FirstColor,
    SecondColor
}

public class PaintHelper
{
    public List<int> mColliderInstanceIDList = new List<int>();

    public RenderTexture PaintRenderRT = null;
    public RenderTexture PaintRT = null;
    private RenderTextureDescriptor mPaintRTDescriptor;

    private int mFirstPaintColorProp = Shader.PropertyToID("_FirstPaintColor");
    private int mSecondPaintColorProp = Shader.PropertyToID("_SecondPaintColor");
    private int mPaintColorRTProp = Shader.PropertyToID("_PaintRT");
    private int mPaintRTResolutionProp = Shader.PropertyToID("_PaintRTResolution");

    public static int PaintRTRes = 512;

    private static PaintHelper instance;
    public static PaintHelper Instance
    {
        get
        { 
            if (instance == null)
            {
                instance = new PaintHelper();
            }
            return instance;
        }
    }

    private PaintHelper()
    {
    }
    public void Init()
    {
        mPaintRTDescriptor = new RenderTextureDescriptor(PaintRTRes, PaintRTRes, RenderTextureFormat.RGB111110Float, 0, 0);
        PaintRT = new RenderTexture(mPaintRTDescriptor);
        PaintRT.filterMode = FilterMode.Bilinear;
        PaintRT.name = "PaintRTfront";
        PaintRT.useMipMap = true;

        PaintRenderRT = new RenderTexture(mPaintRTDescriptor);
        PaintRenderRT.filterMode = FilterMode.Bilinear;
        PaintRenderRT.name = "PaintRenderRT";
        PaintRenderRT.useMipMap = true;

        Shader.SetGlobalTexture(mPaintColorRTProp, PaintRenderRT);
        Shader.SetGlobalColor(mFirstPaintColorProp, Color.red);
        Shader.SetGlobalColor(mSecondPaintColorProp, Color.green);
        Shader.SetGlobalVector(mPaintRTResolutionProp, new Vector2(PaintRTRes, PaintRTRes));
        SetPaintColor(Color.red, Color.green);
    }
    public void SetPaintColor(Color firstPaintColor, Color secondPaintColor)
    {
        Shader.SetGlobalColor(mFirstPaintColorProp, firstPaintColor);
        Shader.SetGlobalColor(mSecondPaintColorProp, secondPaintColor);
    }

    public bool CheckPaintObject(in RaycastHit hitInfo)
    {
        if (hitInfo.collider == null) 
            return false;
        if (!mColliderInstanceIDList.Contains(hitInfo.colliderInstanceID))
        {
            mColliderInstanceIDList.Add(hitInfo.colliderInstanceID);
            hitInfo.transform.GetComponent<Renderer>().material.EnableKeyword("_PAINTED");
        }
        return true;
    }
    static public Color GetBrushColor(PaintColor color)
    {
        return color == PaintColor.FirstColor ? Color.red : Color.green;
    }
}