using System.Collections.Generic;

namespace Officer
{
    /// <summary>
    /// A multiple-choice birth date question built from a passport date.
    /// One choice is the real date. The rest are nearby decoys.
    /// </summary>
    public class BirthDateQuestion
    {
        /// <summary>
        /// The line the officer asks.
        /// </summary>
        public string Text { get; } = "So....when were you born?";

        /// <summary>
        /// The shuffled dates shown as answers.
        /// </summary>
        public IReadOnlyList<PassportDate> Choices { get; }

        /// <summary>
        /// Index of the passport's real birth date inside Choices.
        /// </summary>
        public int CorrectIndex { get; }

        private BirthDateQuestion(List<PassportDate> choices, int correctIndex)
        {
            Choices = choices;
            CorrectIndex = correctIndex;
        }

        /// <summary>
        /// Builds a question whose correct choice is answer. Decoys fall within five years of it.
        /// The same rng sequence always gives the same choices.
        /// </summary>
        public static BirthDateQuestion Create(PassportDate answer, System.Random rng, int choiceCount = 3)
        {
            if (choiceCount < 1)
                choiceCount = 1;

            var choices = new List<PassportDate>(choiceCount) { answer };

            // A five-year window can still repeat a date, so give up rather than spin.
            int attempts = 0;
            int attemptLimit = choiceCount * 20;
            while (choices.Count < choiceCount && attempts < attemptLimit)
            {
                attempts++;
                PassportDate decoy = PassportDate.CreateRandom(rng, answer.Year - 5, answer.Year + 5);
                if (!choices.Contains(decoy))
                    choices.Add(decoy);
            }

            for (int i = choices.Count - 1; i > 0; i--)
            {
                int swap = rng.Next(i + 1);
                (choices[i], choices[swap]) = (choices[swap], choices[i]);
            }

            return new BirthDateQuestion(choices, choices.IndexOf(answer));
        }

        /// <summary>
        /// True when index is the real birth date.
        /// </summary>
        public bool IsCorrect(int index) => index == CorrectIndex;
    }
}
