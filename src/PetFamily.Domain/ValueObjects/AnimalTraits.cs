using CSharpFunctionalExtensions;
using PetFamily.Domain.Validation;

namespace PetFamily.Domain.ValueObjects
{
    public class AnimalTraits
    {
        private double _weight;
        private double _height;
        private string _color;
        private AnimalTraits(double weight,double height, string? color)
        {
            _weight = weight;
            _height = height;
            _color = color??string.Empty;
        }
        public static Result<AnimalTraits> Create(double weight, double height, string? color)
        {
            var validateWeight = ValidationExtensions.ValidateWeight(weight);
            if (validateWeight.IsFailure)
                return Result.Failure<AnimalTraits>(validateWeight.Error); 
            var validateHeight = ValidationExtensions.ValidateHeight(height);   
            if(validateHeight.IsFailure)
                return Result.Failure<AnimalTraits>(validateHeight.Error);
            if (!string.IsNullOrWhiteSpace(color))
            {
                var validateColor = ValidationExtensions.ValidateBreed(color);           
                if (validateColor.IsFailure)
                    return Result.Failure<AnimalTraits>(validateColor.Error);
            }
            return Result.Success(new AnimalTraits(weight, height, color));
        }
    }
    
}
