using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class ViewController
    {
        private readonly Events _events;
        private readonly View _view;

        public ViewController(View prefab, Events events)
        {
            _events = events;
            _view = GameObject.Instantiate(prefab);

            _view.playButton.gameObject.SetActive(true);

            _view.playButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            _events.GameStarted();
            ShowGameOverPanel(false);
            ShowWinPanel(false);
        }

        public void SetTimer(float time)
        {
            _view.timer.text = $"Time: {time}";
        }

        public void SetScore(int score)
        {
            _view.score.text = $"Score: {score}";
        }

        public void ShowGameOverPanel(bool show)
        {
            _view.gameOverPanel.gameObject.SetActive(show);
            _view.playButton.gameObject.SetActive(show);
        }

        public void ShowWinPanel(bool show)
        {
            _view.winPanel.gameObject.SetActive(show);
            _view.playButton.gameObject.SetActive(show);
        }
    }
}
