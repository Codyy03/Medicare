using MediCare.Server.Controllers;

namespace MediCare.Server.Tests.UnitTests
{
    public class DoctorValidatorTests
    {
        private readonly DoctorsController controller = new DoctorsController(null!, null!);

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

        // ValidatePhoneNumber
        [Fact]
        public void ValidatePhoneNumber_ReturnsError_WhenEmpty()
        {
            var errors = controller.ValidatePhoneNumber("");
            Assert.Contains("Phone number is required.", errors);
        }

        [Fact]
        public void ValidatePhoneNumber_ReturnsError_WhenStartsWithZero()
        {
            var errors = controller.ValidatePhoneNumber("012345678");
            Assert.Contains("Phone number cannot start with zero.", errors);
        }

        [Fact]
        public void ValidatePhoneNumber_ReturnsEmpty_WhenValid()
        {
            var errors = controller.ValidatePhoneNumber("123456789");
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

        //  ValidateWorkHours
        [Fact]
        public void ValidateWorkHours_ReturnsError_WhenNull()
        {
            var errors = controller.ValidateWorkHours(null, null);
            Assert.Contains("Start hour and end hour are required.", errors);
        }

        [Fact]
        public void ValidateWorkHours_ReturnsError_WhenStartLaterThanEnd()
        {
            var errors = controller.ValidateWorkHours(new TimeOnly(16, 0), new TimeOnly(8, 0));
            Assert.Contains("Start hour must be earlier than end hour.", errors);
        }

        [Fact]
        public void ValidateWorkHours_ReturnsEmpty_WhenValid()
        {
            var errors = controller.ValidateWorkHours(new TimeOnly(8, 0), new TimeOnly(16, 0));
            Assert.Empty(errors);
        }
    }
}
