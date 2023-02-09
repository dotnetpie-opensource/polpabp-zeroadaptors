using Volo.Abp.Auditing;
using PolpAbp.ZeroAdaptors.Configuration.Dto;

namespace PolpAbp.ZeroAdaptors.Configuration.Tenants.Dto
{
    public class TenantEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}