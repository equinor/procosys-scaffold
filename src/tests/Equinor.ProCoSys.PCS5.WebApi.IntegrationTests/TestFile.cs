﻿using System;
using System.Net.Http;
using System.Text;
using Equinor.ProCoSys.PCS5.WebApi.Controllers;

namespace Equinor.ProCoSys.PCS5.WebApi.IntegrationTests;


public class TestFile
{
    private readonly byte[] _bytes;
    private readonly string _fileName;

    public TestFile(string content, string fileName)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        _fileName = fileName;
        _bytes = Encoding.UTF8.GetBytes(content);
    }

    public MultipartFormDataContent CreateHttpContent()
    {
        var bytes = new ByteArrayContent(_bytes);
        var multipartContent = new MultipartFormDataContent();
        var parameterName = nameof(UploadBaseDto.File);
        multipartContent.Add(bytes, parameterName, _fileName);
        return multipartContent;
    }
}
