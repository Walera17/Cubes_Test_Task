using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class SpawnerController : MonoBehaviour
    {
        [SerializeField] private PoolingController poolingController;
        [SerializeField] private Cube cubePrefab;
        [SerializeField] private int countCubes;
        [SerializeField] private float heightSpawn;
        [SerializeField] private Vector2 maxSquareSpawn;

        static List<Cube> cubeList;

        public static List<Cube> CubeList => cubeList;

        public void SpawnCubes()
        {
            cubeList = new List<Cube>();
            for (int i = 0; i < countCubes; i++)
            {
                Cube newCube = poolingController.Get<Cube>();
                newCube.transform.position = new Vector3(Random.Range(maxSquareSpawn.x, maxSquareSpawn.y), heightSpawn, Random.Range(maxSquareSpawn.x, maxSquareSpawn.y));
                newCube.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                newCube.SetupNumber(i, maxSquareSpawn);
                cubeList.Add(newCube);
            }
        }

        public void StartMoveCubes()
        {
            foreach (Cube cube in cubeList)
            {
                cube.StartMove();
            }
        }

        public void BigCubeTask()
        {
            foreach (Cube cube in cubeList)
            {
                cube.StartMove();
            }

            Cube bigCube = cubeList[Random.Range(0, cubeList.Count)];
            bigCube.SetBigCube();
        }

        public bool IsSleeping()
        {
            return cubeList.TrueForAll(c => c.Rb.IsSleeping());
        }
    }
}
