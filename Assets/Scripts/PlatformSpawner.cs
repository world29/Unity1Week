using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    // 足場やアイテムの配置を制御する
    public class PlatformSpawner : MonoBehaviour
    {
        [SerializeField]
        private Rect spawnRect;

        [SerializeField]
        private float nextOffset = 10;

        [SerializeField]
        private Transform platform;

        [SerializeField]
        private PlatformManager platformManager;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // 前方に足場を生成する
                SpawnPlatform();

                // 次の位置に移動する
                MoveNextPosition(nextOffset);
            }
        }

        private void SpawnPlatform()
        {
            var x = Random.Range(spawnRect.xMin, spawnRect.xMax);
            var y = Random.Range(spawnRect.yMin, spawnRect.yMax);

            platform.position = new Vector3(transform.position.x + x, transform.position.y + y, platform.position.z);

            // プラットフォームの状態をリセット
            var platformNormal = platform.GetComponentInChildren<PlatformNormal>();
            if (platformNormal)
            {
                platformNormal.ResetState();

                float size;
                bool isMovingPlatform;
                platformManager.GetPlatformSpawnParams(out size, out isMovingPlatform);

                platformNormal.ChangeSize(size);
                Debug.Log($"Platform spawned. size={size.ToString("F1")}");

                // プラットフォーム生成通知
                platformManager.NotifyPlatformSpawned(platformNormal);
            }
        }

        private void MoveNextPosition(float offset)
        {
            transform.Translate(offset, 0, 0);
        }

        private static float NormDist01()
        {
            return Mathf.Clamp01((NormalDistribution() + 3f) / 6f);
        }

        private static float NormalDistribution()
        {
            var ret = 0f;
            for (int i = 0; i < 12; i++)
            {
                ret += Random.value;
            }
            return ret - 6f;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var gizmoColor = Gizmos.color;

            Gizmos.color = new Color(0, 1, 0, .2f);
            Gizmos.DrawCube(transform.position + (Vector3)spawnRect.center, spawnRect.size);

            Gizmos.color = gizmoColor;
        }
#endif
    }
}
