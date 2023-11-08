using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class PaintWeapon : MonoBehaviour
{
    public Texture2D BrushTexture;
    public Shader PaintShader = null;
    public PaintColor PaintColor = PaintColor.FirstColor;

    protected PaintHelper PaintHelperInstance;
    protected CommandBuffer Cmd;
    protected Material PaintMaterial = null;

    protected RenderTexture PaintRT = null;
    protected RenderTexture PaintRenderRT = null;

    protected int BrushTexProp = Shader.PropertyToID("_BrushTexture");
    protected int PaintColorProp = Shader.PropertyToID("_PaintColor");
    protected int PaintColorRTProp = Shader.PropertyToID("_PaintRT");
    protected virtual void Start()
    {
        Init();
    }
    protected virtual void OnValidate()
    {
        if (PaintMaterial != null)
        {
            UpdateBrush();
        }
    }

    protected virtual void Init()
    {
        PaintHelperInstance = PaintHelper.Instance;
        PaintRT = PaintHelperInstance.PaintRT;
        PaintRenderRT = PaintHelperInstance.PaintRenderRT;
        if (PaintShader != null)
        {
            InitBrush();
        }
        else
        {
            Debug.LogError("Paint Gun脚本没有设置PaintShader！");
            return;
        }
        SetCommandBuffer();
    }

    protected virtual void InitBrush()
    {
        PaintMaterial = new Material(PaintShader);
        PaintMaterial.SetTexture(BrushTexProp, BrushTexture == null ? Texture2D.whiteTexture : BrushTexture);
        PaintMaterial.SetColor(PaintColorProp, PaintHelper.GetBrushColor(PaintColor));
    }
    protected virtual void UpdateBrush()
    {
        PaintMaterial.SetTexture(BrushTexProp, BrushTexture == null ? Texture2D.whiteTexture : BrushTexture);
        PaintMaterial.SetColor(PaintColorProp, PaintHelper.GetBrushColor(PaintColor));
    }
    public abstract void Paint();

    private void SetCommandBuffer()
    {
        Cmd = new CommandBuffer();
        Cmd.name = "BrushPaint";
        Cmd.Blit(null, PaintRT, PaintMaterial);
        Cmd.CopyTexture(PaintRT, PaintRenderRT);
    }
}
