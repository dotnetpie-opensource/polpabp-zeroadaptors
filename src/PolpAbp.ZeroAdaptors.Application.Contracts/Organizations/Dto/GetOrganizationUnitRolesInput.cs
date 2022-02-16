using PolpAbp.ZeroAdaptors.Validation;
using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class GetOrganizationUnitRolesInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        //[Range(1, long.MaxValue)]
        public Guid Id { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "role.DisplayName, role.Name";
            }
            else if (Sorting.Contains("displayName"))
            {
                Sorting = Sorting.Replace("displayName", "role.displayName");
            }
            else if (Sorting.Contains("addedTime"))
            {
                Sorting = Sorting.Replace("addedTime", "uou.creationTime");
            }
        }
    }
}