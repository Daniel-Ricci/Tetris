using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class HighscoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highscoreText;

    private DatabaseReference _dbReference;
    private bool _dbStatus;
    private int _currentHighscore;

    // Start is called before the first frame update
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                _dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                _dbStatus = true;
                StartCoroutine(GetHighscoreFromDB());
            } else {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                _dbStatus = false;
            }
        });
    }

    public void UpdateHighscore(int score)
    {
        if (_dbStatus)
        {
            StartCoroutine(UpdateHighscoreInDB(score));
        }
    }

    public int GetCurrentHighscore()
    {
        return _currentHighscore;
    }

    private IEnumerator UpdateHighscoreInDB(int score)
    {
        var dbTask = _dbReference.Child("Highscore").SetValueAsync(score);

        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError("Failed to update db. Exception: " + dbTask.Exception);
        }
        else
        {
            _currentHighscore = score;
            UpdateHighscoreUI();
        }
    }

    private IEnumerator GetHighscoreFromDB()
    {
        var dbTask = _dbReference.Child("Highscore").GetValueAsync();

        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError("Failed to get data from db. Exception: " + dbTask.Exception);
        }
        else
        {
            _currentHighscore = int.Parse(dbTask.Result.Value.ToString());
            UpdateHighscoreUI();
        }
    }

    private void UpdateHighscoreUI()
    {
        highscoreText.text = "Highscore: " + _currentHighscore;
    }
}
