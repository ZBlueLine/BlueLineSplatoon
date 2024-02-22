using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SplatterSystem
{
    public class PaintableObject : MonoBehaviour
    {
        public float MoveSpeed = 2.0f; // 移动速度
        public float ChangeDirectionInterval = 2.0f; // 改变方向的时间间隔
        public float PaintInterval = 0.3f;

        private float mTimeInterval = 3.0f;
        private float mCurrentTime = 0;
        private float mPaintCurrentTime = 0;
        private bool mNeverMove = true;
        private Vector3 mRandomDirection = Vector3.zero;
        const int PaintingRTWidth = 512;
        const int PaintingRTHeight = 512;
        private float Radius = 0.01f;

        private void Awake()
        {
        }

        private void Update()
        {
            UpdatePosition();
            mPaintCurrentTime += Time.deltaTime;
            if (mPaintCurrentTime > PaintInterval)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, Vector3.down, out hit, 400);
                if (hit.collider == null || hit.lightmapCoord == null) return;
                hit.transform.gameObject.GetComponent<Renderer>().material.EnableKeyword("_PAINTED");
                mPaintCurrentTime = 0;
            }
        }

        private void UpdatePosition()
        {
            mCurrentTime += Time.deltaTime;
            if (mCurrentTime > mTimeInterval || mNeverMove || PositionCheck())
            {
                mNeverMove = false;
                mRandomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                mCurrentTime = 0;
            }
            transform.Translate(mRandomDirection * MoveSpeed * Time.deltaTime);
        }

        private bool PositionCheck()
        {
            if (transform.position.x > 130 || transform.position.x < -130 ||
                transform.position.z > 130 || transform.position.z < -130)
            {
                return true;
            }
            return false;
        }
    }
}