namespace CarDealer.API.Shared.Services.Interfaces;

public interface ITenantFeatureService
{
    Task<bool> HasFeatureAsync(string featureCode);
}