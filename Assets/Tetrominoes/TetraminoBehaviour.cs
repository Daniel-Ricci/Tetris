using System;
using System.Collections;
using Score;
using TreeEditor;
using UnityEngine;

namespace Tetrominoes
{
    public class TetraminoBehaviour : MonoBehaviour
    {
        [SerializeField] private float intervalBetweenMovements = 0.1f;
        [SerializeField] private Transform pivot;
        [SerializeField] private GameObject ghostPrefab;

        private float _lastMovementTime;
        private float _lastFallTime;

        private const int GridHeight = 20;
        private const int GridWidth = 10;
        private static readonly Transform[,] Grid = new Transform[GridWidth, GridHeight];

        private TetraminoSpawner _spawner;
        private ScoreController _scoreController;
        private GameObject _ghost;

        private static float _fallTime = 0.5f;
        private static bool _gameOver;

        private void Start()
        {
            _spawner = FindObjectOfType<TetraminoSpawner>();
            _scoreController = FindObjectOfType<ScoreController>();
        }

        private void Update()
        {
            if (!_gameOver)
            {
                UpdateGhost();

                if (Time.time - _lastMovementTime > intervalBetweenMovements)
                {
                    if(Input.GetKey(KeyCode.LeftArrow))
                    {
                        _lastMovementTime = Time.time;
                        transform.position += Vector3.left;
                        if (!ValidMove())
                        {
                            transform.position -= Vector3.left;
                        }
                    }

                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        _lastMovementTime = Time.time;
                        transform.position += Vector3.right;
                        if (!ValidMove())
                        {
                            transform.position -= Vector3.right;
                        }
                    }
                }

                if (Time.time - _lastFallTime > (Input.GetKey(KeyCode.DownArrow) ? _fallTime / 10 : _fallTime))
                {
                    _lastFallTime = Time.time;
                    transform.position += Vector3.down;
                    if (!ValidMove())
                    {
                        OnTetraminoLanded();
                    }
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _lastFallTime = Time.time;
                    while (ValidMove())
                    {
                        transform.position += Vector3.down;
                    }
                    OnTetraminoLanded();
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    transform.RotateAround(pivot.position, Vector3.forward, 90f);
                    if (!ValidMove())
                    {
                        transform.RotateAround(pivot.position, Vector3.forward, -90f);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    transform.RotateAround(pivot.position, Vector3.forward, -90f);
                    if (!ValidMove())
                    {
                        transform.RotateAround(pivot.position, Vector3.forward, 90f);
                    }
                }
            }
        }

        private void OnTetraminoLanded()
        {
            transform.position -= Vector3.down;
            AddToGrid();
            StartCoroutine(CheckLines());
            _spawner.SpawnNextTetramino();
            enabled = false;
            Destroy(_ghost);
        }

        private void AddToGrid()
        {
            foreach (Transform child in transform)
            {
                if (!child.gameObject.CompareTag("Pivot"))
                {
                    var pos = child.transform.position;
                    var roundedX = Mathf.RoundToInt(pos.x);
                    var roundedY = Mathf.RoundToInt(pos.y);

                    if (roundedY >= GridHeight)
                    {
                        GameOver();
                        break;
                    }
                    else
                    {
                        Grid[roundedX, roundedY] = child;
                    }
                }
            }
        }

        private IEnumerator CheckLines()
        {
            var linesDestroyed = 0;
            for (var line = 0; line < GridHeight; line++)
            {
                var shouldDestroyLine = true;
                for (var row = 0; row < GridWidth; row++)
                {
                    if (Grid[row, line] == null)
                    {
                        shouldDestroyLine = false;
                    }
                }
                if (shouldDestroyLine)
                {
                    linesDestroyed++;
                    yield return StartCoroutine(DestroyLine(line));
                    line--;
                }
            }

            switch (linesDestroyed)
            {
                case 1:
                    _scoreController.AddToScore(10);
                    break;
                case 2:
                    _scoreController.AddToScore(50);
                    break;
                case 3:
                    _scoreController.AddToScore(200);
                    break;
                case 4:
                    _scoreController.AddToScore(500);
                    break;
            }

            var score = _scoreController.GetCurrentScore();
            var currentLevel = 1;
            if (score >= 1000 && score < 2000)
            {
                _fallTime = 0.45f;
                currentLevel = 2;
            }
            else if (score >= 2000 && score < 3000)
            {
                _fallTime = 0.4f;
                currentLevel = 3;
            }
            else if (score >= 3000 && score < 4000)
            {
                _fallTime = 0.35f;
                currentLevel = 4;
            }
            else if (score >= 4000 && score < 5000)
            {
                _fallTime = 0.3f;
                currentLevel = 5;
            }
            else if (score >= 5000 && score < 6000)
            {
                _fallTime = 0.25f;
                currentLevel = 6;
            }
            else if (score >= 6000 && score < 7000)
            {
                _fallTime = 0.2f;
                currentLevel = 7;
            }
            else if (score >= 7000 && score < 8000)
            {
                _fallTime = 0.15f;
                currentLevel = 8;
            }
            else if (score >= 8000 && score < 9000)
            {
                _fallTime = 0.125f;
                currentLevel = 9;
            }
            else if (score >= 9000)
            {
                _fallTime = 0.1f;
                currentLevel = 10;
            }
            _scoreController.UpdateLevel(currentLevel);

        }

        private IEnumerator DestroyLine(int line)
        {
            for (var row = 0; row < GridWidth; row++)
            {
                StartCoroutine(DestroyBlock(Grid[row, line]));
                Grid[row, line] = null;
                yield return new WaitForSeconds(0.015f);
            }
            LineDown(line);
        }

        private IEnumerator DestroyBlock(Transform block)
        {
            var elapsedTime = 0.0f;
            while (elapsedTime < 1.0f)
            {
                block.localScale = Vector3.Lerp(block.localScale, Vector3.zero, elapsedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(block.gameObject);
        }

        private void LineDown(int lineDestroyed)
        {
            for (var line = lineDestroyed + 1; line < GridHeight; line++)
            {
                for (var row = 0; row < GridWidth; row++)
                {
                    if (Grid[row, line] != null)
                    {
                        Grid[row, line].transform.position += Vector3.down;
                        Grid[row, line - 1] = Grid[row, line];
                        Grid[row, line] = null;
                    }
                }
            }
        }

        public void InstantiateGhost()
        {
            _ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        }

        private void UpdateGhost()
        {
            _ghost.transform.position = transform.position;
            _ghost.transform.rotation = transform.rotation;
            while (ValidMove(_ghost.transform))
            {
                _ghost.transform.position += Vector3.down;
            }
            _ghost.transform.position -= Vector3.down;
        }

        private bool ValidMove()
        {
            return ValidMove(transform);
        }

        private bool ValidMove(Transform t)
        {
            foreach (Transform child in t)
            {
                if (!child.gameObject.CompareTag("Pivot"))
                {
                    var pos = child.transform.position;
                    var roundedX = Mathf.RoundToInt(pos.x);
                    var roundedY = Mathf.RoundToInt(pos.y);

                    if (roundedX < 0 || roundedX >= GridWidth || roundedY < 0 ||
                        Grid[roundedX, Math.Min(roundedY, GridHeight-1)] != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void GameOver()
        {
            Destroy(_spawner.gameObject);
            _gameOver = true;
        }
    }
}
