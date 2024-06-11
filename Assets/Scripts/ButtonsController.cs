using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class ButtonsController : MonoBehaviour
    {
        [SerializeField] private Button spawnButton;
        [SerializeField] private Button moveButton;
        [SerializeField] private Button shootButton;
        [SerializeField] private Button bigButton;
        [SerializeField] private SpawnerController spawnerController;
        [SerializeField] private Rifleman rifleman;
        private bool isSleeping;

        void Start()
        {
            spawnButton.onClick.AddListener(SpawnCubes);
            moveButton.onClick.AddListener(MoveCubes);
            shootButton.onClick.AddListener(StartShoot);
            bigButton.onClick.AddListener(StartBig);
        }

        private void StartBig()
        {
            rifleman.IsStopShooting = false;
            spawnerController.BigCubeTask();
            bigButton.interactable = false;
        }

        private void StartShoot()
        {
            rifleman.StartShoot();
            shootButton.interactable = false;
        }

        private void MoveCubes()
        {
            spawnerController.StartMoveCubes();
            moveButton.interactable = false;
            shootButton.interactable = true;
        }

        void Update()
        {
            if (!isSleeping && !spawnButton.interactable && spawnerController.IsSleeping())
            {
                isSleeping = true;
                moveButton.interactable = true;
            }
            if(rifleman.IsStopShooting)
                bigButton.interactable = true;
        }

        private void SpawnCubes()
        {
            spawnerController.SpawnCubes();
            spawnButton.interactable = false;
        }
    }
}