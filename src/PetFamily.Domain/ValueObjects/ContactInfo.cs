using CSharpFunctionalExtensions;
using PetFamily.Domain.Validation;

namespace PetFamily.Domain.ValueObjects
{
    public class ContactInfo
    {
        public string? Adress { get; private set; }
        public string? OwnerPhone { get; private set; }
        private ContactInfo(string? adress, string? ownerPhone)
        {
            Adress = adress;
            OwnerPhone = ownerPhone;
        }
        public static Result<ContactInfo> Create(string? adress, string? ownerPhone)
        {
            if(!string.IsNullOrWhiteSpace(ownerPhone))
            {
                var validatePhone = ValidationExtensions.ValidatePhone(ownerPhone);
                if (validatePhone.IsFailure)
                    return Result.Failure<ContactInfo>(validatePhone.Error);
            }
            return Result.Success(new ContactInfo(adress, ownerPhone));
        }
    }
}
