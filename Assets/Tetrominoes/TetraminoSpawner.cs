using UnityEngine;

namespace Tetrominoes
{
    public class TetraminoSpawner : MonoBehaviour
    {
        [SerializeField] private TetraminoBehaviour[] tetraminoesPrefabs;
        [SerializeField] private Transform nextTetraminoSpawnPoint;

        private TetraminoBehaviour _nextTetramino;

        private void Start()
        {
            SpawnRandomNextTetramino();
            SpawnNextTetramino();
        }

        private void SpawnRandomNextTetramino()
        {
            var i = Random.Range(0, tetraminoesPrefabs.Length);
            _nextTetramino = Instantiate(tetraminoesPrefabs[i], nextTetraminoSpawnPoint.position, Quaternion.identity);
            _nextTetramino.enabled = false;
        }

        public void SpawnNextTetramino()
        {
            _nextTetramino.transform.position = transform.position;
            _nextTetramino.enabled = true;
            _nextTetramino.InstantiateGhost();
            SpawnRandomNextTetramino();
        }
    }
}
