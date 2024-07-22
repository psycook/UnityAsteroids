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
    [SerializeField] private TextMeshProUGUI WaveText;
    [SerializeField] private AsteroidWaveState CurrentWaveState;
    private WaveBehaviour AsteroidsWaveBehaviour;
    private Coroutine FlashingTextCoroutine;
    private int WaveNumber = 0;
    private int Score = 0;
    private int Lives = 3;

    // #####################
    // # Lifecycle Methods #
    // #####################

    void Start()
    {
        // register for asteroid wave events
        AsteroidsWaveBehaviour = AsteroidsWave.GetComponent<WaveBehaviour>();
        AsteroidsWaveBehaviour.OnWaveStateChange += OnAsteroidWaveStateChanged;
        AsteroidsWaveBehaviour.OnAsteroidHitEvent += AddScore;
        AsteroidsWaveBehaviour.OnEnemyShipHitEvent += AddScore;
        Invoke("NextWave", 1.0f);
    }

    // ##################
    // # Custom Methods #
    // ##################

    private void OnAsteroidWaveStateChanged(AsteroidWaveState newState)
    {
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
        ScoreText.text = "SCORE " + Score.ToString("00000000");
    }

    private void DisplayScore()
    {
        ScoreText.text = "SCORE " + Score.ToString("00000000");
    }

    private void DisplayWave()
    {
        ScoreText.text = "WAVE " + WaveNumber.ToString("00");
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

    void NextWave() 
    {
        WaveNumber++;
        DisplayWave();
        AsteroidsWaveBehaviour.InitialiseWave();
    }

    // ##############
    // # Coroutines #
    // ##############

    private IEnumerator FlashText(TextMeshProUGUI text)
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(Constants.TextFlashSpeed);
        }
    }
}