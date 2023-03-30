﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.ProCoSys.PCS5.WebApi.IntegrationTests;

public static class TestsHelper
{
    public static async Task AssertResponseAsync(
        HttpResponseMessage response, 
        HttpStatusCode expectedStatusCode,
        string expectedMessagePartOnBadRequest)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Bad request details: {jsonString}");
                
            if (!string.IsNullOrEmpty(expectedMessagePartOnBadRequest))
            {
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(jsonString);
                Assert.IsTrue(
                    // ReSharper disable once PossibleNullReferenceException
                    problemDetails.Errors.SelectMany(e => e.Value)
                        .Any(e => e.Contains(expectedMessagePartOnBadRequest)), 
                    $"Expected to find message part '{expectedMessagePartOnBadRequest}'");
            }
        }

        Assert.AreEqual(expectedStatusCode, response.StatusCode);
    }
}