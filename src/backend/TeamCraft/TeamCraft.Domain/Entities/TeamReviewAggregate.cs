using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Domain.Entities
{
    public class TeamReviewAggregate
    {
        public Guid Id { get; private set; }
        public int Score { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public Guid TeamId { get; }

        public  TeamReviewAggregate(int score, string description, Guid teamId)
        {
            Id = Guid.NewGuid();
            Score = CheckScoreValidity(score);
            Description = CheckNullOrEmptyAndLengthDescription(description);
            TeamId = teamId;
        }

        public void UpdateScore(int newScore) 
        {
            Score = CheckScoreValidity(newScore);
        }

        public void UpdateDescription(string newDescription)
        {
            Description = CheckNullOrEmptyAndLengthDescription(newDescription);
        }

        private static int CheckScoreValidity(int score) 
        {
            if (score < 1 || score > 5)
            {
                throw new DomainRuleViolationException("Inserire uno score valido compreso tra 1 e 5.");
            }
            return score;
        }

        private static string CheckNullOrEmptyAndLengthDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new DomainRuleViolationException("La descrizione non può essere vuota");
            }
            if (description.Length < 10)
            {
                throw new DomainRuleViolationException($"La descrizione deve contenere almeno 10 caratteri");
            }
            return description;
        }
    }
}
