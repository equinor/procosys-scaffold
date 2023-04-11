﻿namespace Equinor.ProCoSys.PCS5.WebApi.IntegrationTests.Foos;

public class FooDetailsDto
{
    public int Id { get; set; }
    public string ProjectName { get; set; }
    public string Title { get; set; }
    public string Text { get; set;  }
    public PersonDto CreatedBy { get; set; }
    public bool IsVoided { get; set; }
    public string RowVersion { get; set; }
}
