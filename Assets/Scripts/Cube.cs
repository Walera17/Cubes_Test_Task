using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class Cube : MonoBehaviour
    {
        public static event UnityAction<Cube> OnRelease;

        [SerializeField] private Rigidbody rb;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private TMP_Text[] numbers;

        private int number;
        private Vector2 maxSpawnPosition;
        private Vector3 position;
        private bool isMove, isBig;
        private Cube target;

        public Rigidbody Rb => rb;

        public void SetupNumber(int i, Vector2 maxSquareSpawn)
        {
            maxSpawnPosition = maxSquareSpawn;
            meshRenderer.material.color = new Color(Random.value, Random.value, Random.value);
            number = i;
            foreach (TMP_Text text in numbers)
            {
                text.text = i.ToString();
            }
        }

        public void StartMove()
        {
            position = new Vector3(Random.Range(maxSpawnPosition.x, maxSpawnPosition.y), transform.position.y,
                Random.Range(maxSpawnPosition.x, maxSpawnPosition.y));
            isMove = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isMove && !isBig)
            {
                isMove = false;
                StartMove();
            }

            if (isBig && collision.transform.TryGetComponent(out Cube cube))
            {
                SpawnerController.CubeList.Remove(cube);
                OnRelease?.Invoke(cube);

                if (SpawnerController.CubeList.Count > 0)
                    SetTarget();
                else
                    StopMove();
            }
        }


        void FixedUpdate()
        {
            if (!isMove) return;

            Vector3 direction = position - transform.position;
            if (direction.sqrMagnitude > 0.5f)
                Rb.velocity = direction.normalized * 5 + Vector3.down * 0.5f;
            else
            {
                isMove = false;
                if (isBig)
                    SetTarget();
                else
                    StartMove();
            }
        }

        public void StopMove()
        {
            isMove = false;
            Rb.velocity = Vector3.zero;
        }

        public void SetBigCube()
        {
            isBig = true;
            Rb.isKinematic = true;
            transform.localScale = Vector3.one * 2f;
            transform.position = transform.position + Vector3.up * transform.position.y * 2;
            Rb.isKinematic = false;
            SpawnerController.CubeList.Remove(this);

            SetTarget();
        }

        private void SetTarget()
        {
            SpawnerController.CubeList.Sort((x,y)=> (x.transform.position - transform.position).sqrMagnitude.CompareTo((y.transform.position - transform.position).sqrMagnitude));
            target = SpawnerController.CubeList[0];
            isMove = true;
        }

        void Update()
        {
            if (isBig && target != null) 
                position = target.transform.position;
        }
    }
}