using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts
{
    public class Rifleman : MonoBehaviour
    {
        [SerializeField] private PoolingController poolingController;
        [SerializeField] private Transform core;
        [SerializeField] private Transform gun;
        [SerializeField] private Transform spawnBullet;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private TMP_Text numberTarget;
        [SerializeField, Range(5f, 30f)] private float speedRotation = 10f;
        Quaternion coreStartRotation, gunStartLocalRotation;

        private List<int> numbers;
        private Cube target;
        private bool isStopShooting;

        public bool IsStopShooting
        {
            get => isStopShooting;
            set => isStopShooting = value;
        }

        private void OnEnable()
        {
            Bullet.OnHitCube += Bullet_OnHitCube;
        }

        private void OnDisable()
        {
            Bullet.OnHitCube -= Bullet_OnHitCube;
        }

        public void StartShoot()
        {
            coreStartRotation = core.rotation;
            gunStartLocalRotation = gun.localRotation;
            numbers = new List<int>();
            SelectTarget();
            StartCoroutine(CreateBullet());
        }

        private IEnumerator CreateBullet()
        {
            while (numbers.Count != SpawnerController.CubeList.Count || target != null)
            {
                Bullet bullet = poolingController.Get<Bullet>();
                bullet.transform.position = spawnBullet.position;
                bullet.transform.rotation = spawnBullet.rotation;
                yield return new WaitForSeconds(0.5f);
            }

            isStopShooting = true;
        }

        private void SelectTarget()
        {
            if (numbers.Count == SpawnerController.CubeList.Count) return;

            int number = Random.Range(0, SpawnerController.CubeList.Count);
            while (numbers.Contains(number))
            {
                number = Random.Range(0, SpawnerController.CubeList.Count);
            }

            target = SpawnerController.CubeList[number];
            numberTarget.text = number.ToString();
            numbers.Add(number);
        }

        private void Bullet_OnHitCube(Cube cube)
        {
            if (cube == target)
            {
                target = null;
                numberTarget.text = String.Empty;
                cube.StopMove();
                SelectTarget();
            }
        }

        void Update()
        {
            if (target)
            {
                Vector3 targetPosition = target.transform.position;
                Vector3 corePosition = core.position;
                Vector3 ganPosition = gun.position;

                Vector3 aimAt = new Vector3(targetPosition.x, corePosition.y, targetPosition.z);

                float distToTarget = (aimAt - ganPosition).magnitude;
                Vector3 relativeTargetPosition = ganPosition + gun.forward * distToTarget;
                relativeTargetPosition.y = targetPosition.y;
                gun.rotation = Quaternion.Slerp(gun.rotation,
                    Quaternion.LookRotation(relativeTargetPosition - ganPosition), speedRotation * Time.deltaTime);

                core.rotation = Quaternion.Slerp(core.rotation, Quaternion.LookRotation(aimAt - corePosition),
                    speedRotation * Time.deltaTime);
            }
            else if (gun.rotation != gunStartLocalRotation || core.rotation != coreStartRotation)
            {
                gun.localRotation = Quaternion.Slerp(gun.localRotation, gunStartLocalRotation, Time.deltaTime);
                core.rotation = Quaternion.Slerp(core.rotation, coreStartRotation, Time.deltaTime);
            }
        }
    }
}