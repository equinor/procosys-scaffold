﻿using System;

namespace Equinor.ProCoSys.PCS5.WebApi.IntegrationTests.Invitations
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public PersonDto CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string RowVersion { get; set; }
    }
}
