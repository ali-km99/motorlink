namespace CarDealer.API.Services;

public interface ITenantFeatureService
{
    Task<bool> HasFeatureAsync(string featureCode);
}