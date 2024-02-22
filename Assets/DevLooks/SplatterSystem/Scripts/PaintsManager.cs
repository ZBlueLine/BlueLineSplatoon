//using UnityEngine;
//namespace SplatterSystem
//{
//    public class PaintsManager : MonoBehaviour
//    {


//        public Color FirstPaintColor = Color.red;
//        public Color SecondPaintColor = Color.blue;
//        public int PaintRTReslution = 512;

//        private PaintHelper mPaint;

//        private void Awake()
//        {
//            PaintHelper.PaintRTRes = PaintRTReslution;
//            mPaint = PaintHelper.Instance;
//            mPaint.Init();
//            mPaint.SetPaintColor(FirstPaintColor, SecondPaintColor);
//        }
//        private void Start()
//        {
//            Application.targetFrameRate = 60;
//        }
//        private void OnValidate()
//        {
//            if (mPaint == null)
//            {
//                mPaint = PaintHelper.Instance;
//            }
//            mPaint.SetPaintColor(FirstPaintColor, SecondPaintColor);
//        }
//    }
//}