using System.Collections.Generic;
using Interaction;
using Player;
using Scriptable_Objects;
using UI;
using UnityEngine;

namespace Officer
{
    /// <summary>
    /// Checks the passport in the player's hands, then the outfit, then a birth date question.
    /// A mismatch stops before the question. The panel shows the verdict and the console logs why.
    /// </summary>
    public class PassportOfficer : MonoBehaviour, IInteractable
    {
        [SerializeField] private string displayName = "Passport Officer"; // shown in the prompt
        [SerializeField] private OfficerDialogue dialogue;

        private readonly System.Random _rng = new System.Random();
        private PlayerMovement _movement;
        private Interactor _interactor;
        private bool _locked;

        /// <summary>
        /// Text the Interactor shows, for example "Talk to Passport Officer".
        /// </summary>
        public string Prompt => $"Talk to {displayName}";

        /// <summary>
        /// True when a dialogue is assigned and no check is already on screen.
        /// </summary>
        public bool CanInteract(Interactor interactor)
        {
            return dialogue != null && !dialogue.IsOpen;
        }

        /// <summary>
        /// Freezes the player and runs the passport check. Continue gives control back.
        /// </summary>
        public void Interact(Interactor interactor)
        {
            if (dialogue == null)
            {
                Debug.LogError("PassportOfficer has no dialogue.", this);
                return;
            }

            _movement = interactor.GetComponentInParent<PlayerMovement>();
            _interactor = interactor.GetComponentInParent<Interactor>();
            SetPlayerControl(false);

            PlayerHands hands = interactor.GetComponentInParent<PlayerHands>();
            Pickup held = hands != null ? hands.Held : null;
            PassportObject passport = held != null ? held.GetComponent<PassportObject>() : null;
            if (passport == null || passport.Passport == null)
            {
                Debug.Log("ID, please.", this);
                dialogue.Tell("ID, please.", Close);
                return;
            }

            PlayerOutfit outfit = interactor.GetComponentInParent<PlayerOutfit>();
            if (outfit == null)
            {
                Debug.LogError("Passport check needs a PlayerOutfit on the player.", this);
                Close();
                return;
            }

            List<ItemDefinition> missing = passport.Passport.GetMissing(outfit.Worn);
            if (missing.Count > 0)
            {
                Judge(false, $"You don't match your passport. Missing: {Describe(missing)}");
                return;
            }

            BirthDateQuestion question = BirthDateQuestion.Create(passport.Passport.BirthDate, _rng); // Passport date is created here?
            PassportDate birthDate = passport.Passport.BirthDate;
            dialogue.Ask(question.Text, Labels(question), index =>
            {
                if (question.IsCorrect(index))
                    Judge(true, $"Birth date matches {birthDate}.");
                else
                    Judge(false, $"Birth date does not match {birthDate}.");
            });
        }

        private void Judge(bool passed, string reason)
        {
            string verdict = passed ? "Welcome through" : "Detained";
            Debug.Log($"{verdict}. {reason}", this);
            dialogue.Tell(passed ? "Welcome through." : "Detained.", Close);
        }

        private void Close()
        {
            SetPlayerControl(true);
        }

        private void OnDisable()
        {
            if (_locked)
                SetPlayerControl(true);
        }

        private void SetPlayerControl(bool on)
        {
            _locked = !on;

            if (_movement != null)
                _movement.enabled = on;
            if (_interactor != null)
                _interactor.enabled = on;

            Cursor.lockState = on ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !on;
        }

        private static string Describe(List<ItemDefinition> items)
        {
            var parts = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
                parts[i] = $"{items[i].color} {items[i].type}";
            return string.Join(", ", parts);
        }

        private static List<string> Labels(BirthDateQuestion question)
        {
            var labels = new List<string>(question.Choices.Count);
            for (int i = 0; i < question.Choices.Count; i++)
                labels.Add(question.Choices[i].ToString());
            return labels;
        }
    }
}
