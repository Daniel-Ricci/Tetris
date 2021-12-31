using System.Collections;
using System.Linq;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;

public class HighscoreManager : MonoBehaviour
{
    [SerializeField] private Text highscoreText;
    [SerializeField] private string username;

    private DatabaseReference _dbReference;
    private bool _dbStatus;
    private long _currentHighscore;

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

    public long GetCurrentHighscore()
    {
        return _currentHighscore;
    }

    private IEnumerator UpdateHighscoreInDB(int score)
    {
        var dbTask = _dbReference.Child("Highscore").Child(username).SetValueAsync(score);

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
        var dbTask = _dbReference.Child("Highscore").OrderByValue().LimitToLast(1).GetValueAsync();

        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError("Failed to get data from db. Exception: " + dbTask.Exception);
        }
        else
        {
            _currentHighscore = long.Parse(dbTask.Result.Children.First().Value.ToString());
            UpdateHighscoreUI();
        }
    }

    private void UpdateHighscoreUI()
    {
        highscoreText.text = "Highscore: " + _currentHighscore;
    }
}
