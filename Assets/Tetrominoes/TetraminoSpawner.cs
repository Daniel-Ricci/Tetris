using UnityEngine;

namespace Tetrominoes
{
    public class TetraminoSpawner : MonoBehaviour
    {
        [SerializeField] private TetraminoBehaviour[] tetraminoesPrefabs;
        [SerializeField] private Transform nextTetraminoSpawnPoint;
        [SerializeField] private Transform heldTetraminoSpawnPoint;

        private TetraminoBehaviour _nextTetramino;
        private TetraminoBehaviour _heldTetramino;

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

        private void SpawnHeldTetramino()
        {
            _heldTetramino.transform.position = transform.position;
            _heldTetramino.enabled = true;
            _heldTetramino.InstantiateGhost();
        }

        public void HoldTetramino(TetraminoBehaviour tetramino)
        {
            if (_heldTetramino == null)
            {
                _heldTetramino = tetramino;
                _heldTetramino.transform.position = heldTetraminoSpawnPoint.position;
                _heldTetramino.enabled = false;
                _heldTetramino.DestroyGhost();
                SpawnNextTetramino();
            }
            else
            {
                SpawnHeldTetramino();
                _heldTetramino = tetramino;
                _heldTetramino.transform.position = heldTetraminoSpawnPoint.position;
                _heldTetramino.enabled = false;
                _heldTetramino.DestroyGhost();
            }
        }
    }
}
