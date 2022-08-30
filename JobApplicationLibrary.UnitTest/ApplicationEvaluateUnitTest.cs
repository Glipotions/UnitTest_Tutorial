using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using Moq;

namespace JobApplicationLibrary.UnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            // Aranged
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                },
            };
            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert

            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);
        }


        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            // Aranged

            /// sahte veri oluþturmak
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(x => x.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 32, IdentityNumber = "" },
                TechStackList = new List<string> { "" },
            };
            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert

            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithTechStackOver75p_TransferredToAutoAccepted()
        {
            // Aranged
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(x => x.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 32 },
                TechStackList = new List<string> { "RabbitMQ", "C#", "Microservice", "Visual Studio" },
                YearsOfExperience = 16
            };
            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert

            Assert.AreEqual(appResult, ApplicationResult.AutoAccepted);
        }

        [Test]
        public void Application_WithInvalidIdentityNumber_TransferredToHR()
        {
            // Aranged
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Strict);
            mockValidator.DefaultValue = DefaultValue.Mock;
            
            mockValidator.Setup(x => x.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 32 },
            };
            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert

            Assert.AreEqual(appResult, ApplicationResult.TransferedToHR);
        }

        /// <ÖZET>
        /// Mock ile Property kontrolü
        /// </summary>
        [Test]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            // Aranged
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Strict);
            mockValidator.Setup(x => x.CountryDataProvider.CountryData.Country).Returns("SPAIN");


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 32 },
            };
            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert

            Assert.AreEqual(appResult, ApplicationResult.TransferedToCTO);
        }

        /// <ÖZET>
        /// SetupAllProperties ile tüm proplar kayýt edilir. 
        /// Eðer baþka setup yapýlacaksa bu komutun altýnda yapýlmalýdýr!!1
        /// </summary>
        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            // Aranged
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Strict);

            mockValidator.SetupAllProperties();

            mockValidator.Setup(x => x.CountryDataProvider.CountryData.Country).Returns("SPAIN");


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 51 },
            };
            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert

            Assert.AreEqual(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }
    }
}