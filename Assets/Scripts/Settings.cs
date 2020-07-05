using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    [CreateAssetMenu(fileName = "Setting")]
    public class Settings : ScriptableObject
    {
        [Space]
        [Header("General")]
        public SceneEntity playerPrefab;
        public SceneEntity enemyPrefab;
        public SceneEntity projectilePrefab;
        public float minDuration;
        public float maxDuration;
        public float winScore;

        [Space]
        [Header("Enemy")]
        public int enemyMaxCount;
        public float enemyMaxSpeed;
        public AnimationCurve enemySpeed;
        public AnimationCurve enemyCount;
        public AnimationCurve enemyOscillation;
        public float enemyOscillationAmplitude;
        public float enemyOscillationFrequency;
        public float enemyMinRadius;
        public float enemyMaxRadius;

        [Space]
        [Header("Projectile")]
        public float projectileSpeed;
    }
}