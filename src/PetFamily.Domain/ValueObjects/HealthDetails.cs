using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects
{
    public class HealthDetails
    {
        private bool _isVaccinated;
        private bool _isNeutered;
        private HealthStatus _healthStatus;
        private HealthDetails(bool? isVaccinated,bool? isNeutered,HealthStatus? healthStatus)
        {
            _isVaccinated = isVaccinated??false;
            _isNeutered = isNeutered??false;
            _healthStatus = healthStatus??HealthStatus.Absent;
        }
        public static Result<HealthDetails> Create(bool? isVaccinated, bool? isNeutered, HealthStatus? healthStatus)
        {
            return Result.Success(new HealthDetails(isVaccinated,isNeutered,healthStatus));
        }
    }
    public enum HealthStatus
    {
        Healthy,
        Sick,
        Injured,
        Absent
    }
}
