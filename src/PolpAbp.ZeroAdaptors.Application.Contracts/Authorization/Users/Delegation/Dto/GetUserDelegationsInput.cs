﻿using PolpAbp.ZeroAdaptors.Validation;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.ZeroAdaptors.Authorization.Users.Delegation.Dto
{
    public class GetUserDelegationsInput : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }

        public string Sorting { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting) || Sorting == "userName ASC")
            {
                Sorting = "Username";
            }
            else if (Sorting == "userName DESC")
            {
                Sorting = "UserName DESC";
            }
        }
    }
}