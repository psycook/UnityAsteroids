using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneManagerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject AsteroidsWave;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private AsteroidWaveState CurrentWaveState;


    private Coroutine FlashingTextCoroutine;
    private int Score = 0;

    // #####################
    // # Lifecycle Methods #
    // #####################

    void Start()
    {
        // register for asteroid wave events
        Debug.Log("GameSceneManagerBehaviour:Start, registering for asteroid wave events");
        AsteroidsWave.GetComponent<AsteroidsWaveBehaviour>().OnWaveStateChange += OnAsteroidWaveStateChanged;
    }

    // ##################
    // # Custom Methods #
    // ##################

    private void OnAsteroidWaveStateChanged(AsteroidWaveState newState)
    {
        Debug.Log($"GameSceneManagerBehaviour:OnAsteroidWaveStateChanged, event is {newState}");
        CurrentWaveState = newState;
        switch(CurrentWaveState)
        {
            case AsteroidWaveState.Initialising:
                Score = 0;
                DisplayScore();
                DisplayInfo("INCOMING ASTEROIDS", true);
                break;
            case AsteroidWaveState.InProgress:
                HideInfo();
                break;
            case AsteroidWaveState.Finished:
                InfoText.text = "ASTEROIDS DESTROYED";
                break;
        }
    }

    public void AddScore(int score)
    {
        Score += score;
        ScoreText.text = "SCORE " + Score.ToString("000000");
    }

    private void DisplayScore()
    {
        ScoreText.text = "SCORE " + Score.ToString("000000");
    }

    private void DisplayInfo(string text, bool flashing)
    {
        InfoText.text = text;
        InfoText.enabled = true;
        if (flashing)
        {
            FlashingTextCoroutine = StartCoroutine(FlashText(InfoText));
        }
    }

    private void HideInfo()
    {
        if (FlashingTextCoroutine != null)
        {
            StopCoroutine(FlashingTextCoroutine);
        }
        InfoText.enabled = false;
    }

    // ##############
    // # Coroutines #
    // ##############

    private IEnumerator FlashText(TextMeshProUGUI text)
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(AsteroidContants.FlashignTextSpeed);
        }
    }
}