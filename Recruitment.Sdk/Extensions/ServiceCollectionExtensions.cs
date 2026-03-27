using Microsoft.Extensions.DependencyInjection;
using Recruitment.Sdk.Clients;

namespace Recruitment.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Recruitment SDK typed HTTP clients using IHttpClientFactory.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="apiBaseAddress">The base address of the Recruitment API.</param>
        public static IServiceCollection AddRecruitmentSdk(this IServiceCollection services, Uri apiBaseAddress)
        {
            services.AddHttpClient<ICandidateClient, CandidateClient>(client =>
            {
                client.BaseAddress = apiBaseAddress;
            });

            services.AddHttpClient<IJobClient, JobClient>(client =>
            {
                client.BaseAddress = apiBaseAddress;
            });

            services.AddHttpClient<IRecruiterClient, RecruiterClient>(client =>
            {
                client.BaseAddress = apiBaseAddress;
            });

            services.AddHttpClient<ISkillClient, SkillClient>(client =>
            {
                client.BaseAddress = apiBaseAddress;
            });

            return services;
        }
    }
}
