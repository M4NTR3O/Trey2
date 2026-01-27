using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trey2.Controllers
{
    public class TrainingController : Controller
    {
        [HttpPost("upload-training")]
        public async Task<IActionResult> UploadToPython(IFormFile file)
        {
            using var client = new HttpClient();

            using var content = new MultipartFormDataContent();
            await using var fs = file.OpenReadStream();

            content.Add(
                new StreamContent(fs),
                "file",
                file.FileName
            );

            var response = await client.PostAsync(
                "http://localhost:8001/convert",
                content
            );

            var json = await response.Content.ReadAsStringAsync();
            return Ok(json);
        }
    }
}
