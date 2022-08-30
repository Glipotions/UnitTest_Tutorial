using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary
{
    public class ApplicationEvaluator
    {

        private const int minAge = 18;
        private List<string> techStackList = new() { "C#", "RabbitMQ", "Microservice", "Visual Studio" };
        private const int autoAcceptedYearOfExperience = 12;
        private readonly IIdentityValidator _identityValidator;

        public ApplicationEvaluator(IIdentityValidator identityValidator)
        {
            this._identityValidator = identityValidator;
        }

        public ApplicationResult Evaluate(JobApplication form)
        {
            if (form.Applicant.Age < minAge)
                return ApplicationResult.AutoRejected;

            _identityValidator.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Detailed : ValidationMode.Quick;

            if (_identityValidator.CountryDataProvider.CountryData.Country != "TURKEY")
                return ApplicationResult.TransferedToCTO;

            var validIdentity = _identityValidator.IsValid(form.Applicant.IdentityNumber);

            if (!validIdentity)
                return ApplicationResult.TransferedToHR;

            var sr = GetTechStackSimilarityRate(form.TechStackList);

            if (sr < 25)
                return ApplicationResult.AutoRejected;
            if (sr > 75 && form.YearsOfExperience > autoAcceptedYearOfExperience)
                return ApplicationResult.AutoAccepted;

            return ApplicationResult.AutoAccepted;
        }

        private int GetTechStackSimilarityRate(List<string> techStacks)
        {
            var matchedCount = techStacks.Where(i => techStackList.Contains(i, StringComparer.OrdinalIgnoreCase)).Count();

            return (int)((double)matchedCount / techStackList.Count) * 100;
        }
    }

    public enum ApplicationResult
    {
        AutoRejected,
        TransferedToHR,
        TransferedToLead,
        TransferedToCTO,
        AutoAccepted,

    }
}