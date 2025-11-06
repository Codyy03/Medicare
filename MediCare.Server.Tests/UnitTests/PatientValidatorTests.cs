using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using MediCare.Server.Controllers;

namespace MediCare.Server.Tests.UnitTests
{
    public class PatientValidatorTests
    {
        private readonly PatientsController controller = new PatientsController(null!, null!);
        // ValidatePassword
        [Fact]
        public void ValidatePassword_ReturnsError_WhenTooShort()
        {
            var errors = controller.ValidatePassword("abc");
            Assert.Contains("Password must be at least 8 characters long.", errors);
        }

        [Fact]
        public void ValidatePassword_ReturnsError_WhenNoUppercase()
        {
            var errors = controller.ValidatePassword("test1234!");
            Assert.Contains("Password must contain at least one uppercase letter.", errors);
        }

        [Fact]
        public void ValidatePassword_ReturnsEmpty_WhenValid()
        {
            var errors = controller.ValidatePassword("Test1234!");
            Assert.Empty(errors);
        }

        // ValidateName
        [Fact]
        public void ValidateName_ReturnsError_WhenEmpty()
        {
            var errors = controller.ValidateName("");
            Assert.Contains("Name is required.", errors);
        }

        [Fact]
        public void ValidateName_ReturnsError_WhenContainsDigits()
        {
            var errors = controller.ValidateName("John1");
            Assert.Contains("Name must contain only letters.", errors);
        }

        [Fact]
        public void ValidateName_ReturnsEmpty_WhenValid()
        {
            var errors = controller.ValidateName("John");
            Assert.Empty(errors);
        }

        // ValidateSurname
        [Fact]
        public void ValidateSurname_ReturnsError_WhenEmpty()
        {
            var errors = controller.ValidateSurname("");
            Assert.Contains("Surname is required.", errors);
        }

        [Fact]
        public void ValidateSurname_ReturnsError_WhenContainsDigits()
        {
            var errors = controller.ValidateSurname("Smith2");
            Assert.Contains("Surname must contain only letters.", errors);
        }

        [Fact]
        public void ValidateSurname_ReturnsEmpty_WhenValid()
        {
            var errors = controller.ValidateSurname("Smith");
            Assert.Empty(errors);
        }

        // ValidatePESEL
        [Fact]
        public void ValidatePESEL_ReturnsError_WhenEmpty()
        {
            var errors = controller.ValidatePESEL("");
            Assert.Contains("PESEL is required.", errors);
        }

        [Fact]
        public void ValidatePESEL_ReturnsError_WhenTooShort()
        {
            var errors = controller.ValidatePESEL("12345");
            Assert.Contains("PESEL must be exactly 11 digits.", errors);
        }

        [Fact]
        public void ValidatePESEL_ReturnsEmpty_WhenValid()
        {
            var errors = controller.ValidatePESEL("12345678901");
            Assert.Empty(errors);
        }

        // ValidateBirthday
        [Fact]
        public void ValidateBirthday_ReturnsError_WhenInFuture()
        {
            var futureDate = DateTime.Now.AddDays(1);
            var errors = controller.ValidateBirthday(futureDate);
            Assert.Contains("Birthday cannot be in the future.", errors);
        }

        [Fact]
        public void ValidateBirthday_ReturnsError_WhenTooOld()
        {
            var oldDate = DateTime.Now.AddYears(-130);
            var errors = controller.ValidateBirthday(oldDate);
            Assert.Contains("Birthday is too far in the past", errors);
        }

        [Fact]
        public void ValidateBirthday_ReturnsEmpty_WhenValid()
        {
            var validDate = DateTime.Now.AddYears(-30);
            var errors = controller.ValidateBirthday(validDate);
            Assert.Empty(errors);
        }
    }
}
