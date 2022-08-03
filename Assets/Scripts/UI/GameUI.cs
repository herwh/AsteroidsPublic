using Common;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameObject _menuBar;
        [SerializeField] private GameController _gameController;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _changeInputButton;
        [SerializeField] private TextMeshProUGUI _textInput;

        public void ShowMenuBar()
        {
            _gameController.SpaceshipController.SetInputState(false);
            var spaceship = _gameController.SpaceshipController.GetSpaceship();
            _menuBar.gameObject.SetActive(true);

            if (spaceship != null && spaceship.CurrentHealthCount > 0)
            {
                Time.timeScale = 0;
                _continueButton.gameObject.SetActive(true);
            }
            else
            {
                _continueButton.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _gameController.SpaceshipController.SpaceshipDestroyed += ShowMenuBar;
            _gameController.SpaceshipController.SpaceshipDestroyed += ClearScene;
            _startButton.onClick.AddListener(StartGame);
            _continueButton.onClick.AddListener(Continue);
            _exitButton.onClick.AddListener(Exit);
            _changeInputButton.onClick.AddListener(ChangeInput);
            UpdateInputText();
        }

        private void StartGame()
        {
            _gameController.SpaceshipController.SetInputState(true);
            ClearScene();
            Time.timeScale = 1;
            _gameController.StartNewGame();
            _menuBar.gameObject.SetActive(false);
        }

        private void ChangeInput()
        {
            _gameController.SpaceshipController.ChangeInput();
            UpdateInputText();
        }

        private void UpdateInputText()
        {
            _textInput.text = $"Input: {_gameController.SpaceshipController.GetInputName()}";
        }

        private void Exit()
        {
            Application.Quit();
        }

        private void Continue()
        {
            _gameController.SpaceshipController.SetInputState(true);
            _menuBar.gameObject.SetActive(false);
            Time.timeScale = 1;
        }

        private void ClearScene()
        {
            _gameController.RemoveAllObjects();
        }

        private void OnDisable()
        {
            _gameController.SpaceshipController.SpaceshipDestroyed -= ShowMenuBar;
            _gameController.SpaceshipController.SpaceshipDestroyed -= ClearScene;
            _startButton.onClick.RemoveListener(StartGame);
            _continueButton.onClick.RemoveListener(Continue);
            _exitButton.onClick.RemoveListener(Exit);
            _changeInputButton.onClick.RemoveListener(ChangeInput);
        }
    }
}
