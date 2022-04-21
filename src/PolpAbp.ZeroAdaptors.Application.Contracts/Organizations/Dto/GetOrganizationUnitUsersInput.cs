using PolpAbp.ZeroAdaptors.Validation;
using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Organizations.Dto
{
    public class GetOrganizationUnitUsersInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        // todo ??? Fix this
        // [Range(1, long.MaxValue)]
        public Guid Id { get; set; }

        public string Filter { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "user.Name, user.Surname";
            }
            else if (Sorting.Contains("userName"))
            {
                Sorting = Sorting.Replace("userName", "user.userName");
            }
            else if (Sorting.Contains("addedTime"))
            {
                Sorting = Sorting.Replace("addedTime", "uou.creationTime");
            }
        }
    }
}