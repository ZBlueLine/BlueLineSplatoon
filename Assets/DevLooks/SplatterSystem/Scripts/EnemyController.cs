using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;
using SplatterSystem;

public class EnemyController : MonoBehaviour
{
    //public PaintsManager mPaintManager = null;
    public int EnemyNum = 1;
    public Transform RootNode;
    public GameObject EnemyPrefab;
    public GameObject GroundObject;
    public float MoveSpeed = 2.0f; // 移动速度
    public float PaintInterval = 0.3f;
    public ParticleSystem ExplodeParticle;

    private List<Particle> mParticle;
    private EmitParams mEmitParams = new EmitParams();
    private Vector3 mBoundHalfSize;
    private Vector3 mBoundCenter;

    List<Enemy> mEnemyList = new List<Enemy>();

    private void Awake()
    {
        var bounds = GroundObject.GetComponent<Renderer>().bounds;
        mBoundHalfSize = bounds.size/2.0f - bounds.size * 0.4f;
        mBoundCenter = bounds.center;
        GenerateSpheres();
    }

    private void Start()
    {
        Debug.unityLogger.logEnabled = false;
    }

    private void Update()
    {
        UpdatePosition();
    }
    void GenerateSpheres()
    {
        if (RootNode == null)
        {
            Debug.LogError("未设置根节点");
        }
        for (int i = 0; i < EnemyNum; i++)
        {
            float x = Random.Range(-mBoundHalfSize.x, mBoundHalfSize.x);
            float y = Random.Range(-mBoundHalfSize.z, mBoundHalfSize.z);
            Vector3 randomPosition = new Vector3(x, 1, y) + mBoundCenter;
            GameObject enemyGameObject = Instantiate(EnemyPrefab, randomPosition, Quaternion.identity, RootNode);
            float directionChangeTime = Random.Range(1, 4);
            Vector3 moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            Enemy newEnemy = new Enemy(enemyGameObject.transform, directionChangeTime, moveDirection);
            mEnemyList.Add(newEnemy);
        }
    }
    private void EnemyDeath(Enemy enemy)
    {
        RaycastHit hit;
        Physics.Raycast(enemy.Transform.position, Vector3.down, out hit, 5);
        if (hit.collider == null) return;
        //if(mPaintManager == null)
        //{
        //    Debug.LogError("没有挂载PaintManager");
        //}
        //mPaintManager.Paint(hit, enemy.Radius, enemy.PaintColor);
        mEmitParams.position = enemy.Transform.position;
        ExplodeParticle.Emit(mEmitParams, 1);
        GameObject.Destroy(enemy.Transform.gameObject);
        mEnemyList.Remove(enemy);
    }
    private void UpdatePosition()
    {
        for(int i = mEnemyList.Count - 1; i >= 0; --i)
        {
            var enemy = mEnemyList[i];
            if(enemy == null) continue;
            enemy.CurrentTime += Time.deltaTime;
            enemy.TotalTime += Time.deltaTime;
            if(enemy.TotalTime > enemy.LifeTime)
            {
                
                EnemyDeath(enemy);
                continue;
            }
            if (enemy.CurrentTime > enemy.DirectionChangeTime)
            {
                enemy.DirectionChangeTime = Random.Range(0.5f, 6f);
                enemy.MoveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                enemy.CurrentTime = 0;
            }
            mEnemyList[i].Transform.Translate(mEnemyList[i].MoveDirection * MoveSpeed * Time.deltaTime);
            mEnemyList[i] = enemy;
        }
        if (mEnemyList.Count == 0)
        {
            GenerateSpheres();
        }
    }
}
class Enemy
{
    public Transform Transform;
    public float DirectionChangeTime;
    public Vector3 MoveDirection;
    public PaintColorType PaintColor;
    public float Radius;
    public float LifeTime;
    public float CurrentTime = 0;
    public float TotalTime = 0;
    public Enemy() { }
    public Enemy(Transform transform, float directionChangeTime, Vector3 moveDirection)
    {
        this.Transform = transform;
        this.DirectionChangeTime = directionChangeTime;
        this.MoveDirection = moveDirection;
        this.LifeTime = Random.Range(1, 10);
        this.PaintColor = Random.value > 0.5f ? PaintColorType.SecondColor : PaintColorType.FirstColor;
        this.Radius = Random.Range(0.1f, 0.2f);
    }
    public Enemy(Transform transform, float directionChangeTime, Vector3 moveDirection, float lifeTime)
    {
        this.Transform = transform;
        this.DirectionChangeTime = directionChangeTime;
        this.MoveDirection = moveDirection;
        this.LifeTime = lifeTime;
        this.PaintColor = Random.value > 0.5f ? PaintColorType.SecondColor : PaintColorType.FirstColor;
        this.Radius = Random.Range(0.1f, 0.2f);
    }
}