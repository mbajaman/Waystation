using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Screen-space panel for one line of officer text, then either answer buttons or Continue.
    /// It knows nothing about passports. The caller supplies the text and what happens next.
    /// </summary>
    public class OfficerDialogue : MonoBehaviour
    {
        [SerializeField] private TMP_Text line;
        [SerializeField] private GameObject visual; // the panel, hidden until Ask or Tell
        [SerializeField] private Button[] choiceButtons;
        [SerializeField] private Button continueButton;

        private Action<int> _onChoice;
        private Action _onClose;
        private bool _open;

        /// <summary>
        /// True while a question or a line is on screen.
        /// </summary>
        public bool IsOpen => _open;

        private void Awake()
        {
            if (choiceButtons != null)
            {
                for (int i = 0; i < choiceButtons.Length; i++)
                {
                    int index = i;
                    if (choiceButtons[i] != null)
                        choiceButtons[i].onClick.AddListener(() => Choose(index));
                }
            }

            if (continueButton != null)
                continueButton.onClick.AddListener(Continue);

            Hide();
        }

        /// <summary>
        /// Shows the line and one button per choice. Continue stays hidden.
        /// </summary>
        public void Ask(string text, IReadOnlyList<string> choices, Action<int> onChoice)
        {
            if (line == null)
            {
                Debug.LogError("OfficerDialogue has no line.", this);
                return;
            }

            _onClose = null;
            _onChoice = onChoice;
            line.text = text;
            ShowChoices(choices);
            Open();
            Select(FirstActive(choiceButtons));
        }

        /// <summary>
        /// Shows the line and Continue. Choice buttons stay hidden.
        /// </summary>
        public void Tell(string text, Action onClose = null)
        {
            if (line == null)
            {
                Debug.LogError("OfficerDialogue has no line.", this);
                return;
            }

            _onChoice = null;
            _onClose = onClose;
            line.text = text;
            ShowContinue();
            Open();
            Select(continueButton != null ? continueButton.gameObject : null);
        }

        private void Choose(int index)
        {
            // Clear first so the callback can open the next line without being wiped.
            Action<int> chosen = _onChoice;
            _onChoice = null;
            _onClose = null;
            chosen?.Invoke(index);
        }

        private void Continue()
        {
            Action close = _onClose;
            _onChoice = null;
            _onClose = null;
            Hide();
            close?.Invoke();
        }

        private void ShowChoices(IReadOnlyList<string> choices)
        {
            if (continueButton != null)
                continueButton.gameObject.SetActive(false);

            int count = choices != null ? choices.Count : 0;
            if (choiceButtons == null)
                return;

            if (count > choiceButtons.Length)
                Debug.LogWarning($"OfficerDialogue has {choiceButtons.Length} choice buttons for {count} choices.", this);

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                Button button = choiceButtons[i];
                if (button == null)
                    continue;

                bool show = i < count;
                button.gameObject.SetActive(show);
                if (!show)
                    continue;

                TMP_Text label = button.GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = choices[i];
            }
        }

        private void ShowContinue()
        {
            if (choiceButtons != null)
            {
                for (int i = 0; i < choiceButtons.Length; i++)
                {
                    if (choiceButtons[i] != null)
                        choiceButtons[i].gameObject.SetActive(false);
                }
            }

            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }

        private void Open()
        {
            _open = true;
            if (visual != null)
                visual.SetActive(true);
        }

        private void Hide()
        {
            _open = false;
            if (visual != null)
                visual.SetActive(false);
        }

        private static GameObject FirstActive(Button[] buttons)
        {
            if (buttons == null)
                return null;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null && buttons[i].gameObject.activeInHierarchy)
                    return buttons[i].gameObject;
            }

            return null;
        }

        private static void Select(GameObject target)
        {
            if (target == null || EventSystem.current == null)
                return;

            EventSystem.current.SetSelectedGameObject(target);
        }
    }
}
