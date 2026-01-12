using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using PaperlessREST.Test;
using System.Net;
using System.Net.Http.Headers;

namespace PaperlessREST.IntegrationTests
{
    [TestFixture]
    public class DocumentUploadIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new TestWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        private MultipartFormDataContent CreateValidUpload()
        {
            var content = new MultipartFormDataContent();

            var fileBytes = new byte[] { 1, 2, 3 };
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

            content.Add(fileContent, "file", "test.pdf");

            content.Add(new StringContent(Guid.NewGuid().ToString()), "id");
            content.Add(new StringContent("Test Dokument"), "title");
            content.Add(new StringContent("Summary"), "summary");
            content.Add(new StringContent("application/pdf"), "fileType");
            content.Add(new StringContent(fileBytes.Length.ToString()), "fileSize");
            content.Add(new StringContent(DateTime.UtcNow.ToString("o")), "createdOn");
            content.Add(new StringContent(DateTime.UtcNow.ToString("o")), "modifiedLast");

            return content;
        }

        [Test]
        public async Task UploadDocument_ShouldReturnOk()
        {
            var content = CreateValidUpload();

            var response = await _client.PostAsync("/api/MetaData/upload", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task UploadDocument_ShouldReturnSuccessMessage()
        {
            var content = CreateValidUpload();

            var response = await _client.PostAsync("/api/MetaData/upload", content);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("Upload successful");
        }

        [Test]
        public async Task UploadDocument_MetadataShouldExist()
        {
            var content = CreateValidUpload();

            await _client.PostAsync("/api/MetaData/upload", content);

            var getResponse = await _client.GetAsync("/api/MetaData");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await getResponse.Content.ReadAsStringAsync();
            body.Should().Contain("Test Dokument");
        }
    }
}
