using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CSharpFunctionalExtensions;


namespace PetFamily.Domain.Validation
{
    public static class ValidationExtensions
    {
        private const string _validationColorPattern = @"^[A-Za-z\s]+$";
        private const string _validationEmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private const string _validationNamePattern = @"^[A-Za-z-]$";
        private const string _validationBreedPattern = @"^[A-Za-z\s]+$";
        private const string _validationPhonePattern = @"^\+?[1-9]\d{0,2}[\s\-]?\(?\d{1,4}\)?[\s\-]?\d{1,4}[\s\-]?\d{1,4}$";
        public static Result<string> ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<string>("Name is required");
            if (name.Length < 3)
                return Result.Failure<string>("Name is too short");
            if (name.Length > 50)
                return Result.Failure<string>("Name is too long");
            if (!Regex.IsMatch(name, _validationNamePattern))
                return Result.Failure<string>("Name format is invalid(only A-Z,a-z,-)");
            return Result.Success(name);
        }
        public static Result ValidateBreed(string breed)
        {
            if(!Regex.IsMatch(breed,_validationBreedPattern))
                return Result.Failure("Breed format is invalid");
            return Result.Success();
        }
        public static Result ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure("Email is required");
            if (!Regex.IsMatch(email, _validationEmailPattern))
                return Result.Failure("Email format is invalid");
            return Result.Success();
        }
        public static Result ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return Result.Failure("Phone is required");
            if (!Regex.IsMatch(phone, _validationPhonePattern))
                return Result.Failure("Phone format is invalid");
            return Result.Success();
        }
        public static Result ValidateWeight(double weight)
        {
            if (weight < 0)
                return Result.Failure("Weight value cannot be nagative");
            return Result.Success();
        }
        public static Result ValidateHeight(double height)
        {
            if (height < 0)
                return Result.Failure("Height value cannot be nagative");
            return Result.Success();
        }
        public static Result ValidateColor(string color)
        {
            if(Regex.IsMatch(color,_validationColorPattern))
                return Result.Failure("Color format is invalid");
            return Result.Success();
        }
        public static Result ValidateDescription(string? description)
        {
            if (description == null)
                return Result.Success();
            if (description.Length > 1000)
                return Result.Failure("Description cannot be more than 1000 characters");
            return Result.Success();
        }
    }
}
