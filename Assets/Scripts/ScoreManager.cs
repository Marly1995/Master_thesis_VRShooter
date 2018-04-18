using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    Text score;

    string text = "Score: ";

	void Start ()
    {
        score = GetComponent<Text>();	
	}
	
	void Update ()
    {
        score.text = text + WorldState.Score.ToString();
	}
}
