using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Service_Template.Settings;

namespace Service_Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        private readonly GitSecrets _gitSecrets;

        public LoginController(IHttpClientFactory httpClientFactory, IOptions<GitSecrets> gitSecret)
        {
            _httpClientFactory = httpClientFactory;
            
            _gitSecrets = gitSecret.Value;
        }
        
        // GET: api/<LoginController>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] string code)
        {
            string url = "https://github.com/login/oauth/access_token";
            string settings = "?client_id=" + _gitSecrets.Client + "&client_secret=" + _gitSecrets.Secret + "&code=" + code;
            
            System.Console.WriteLine(url);
            System.Console.WriteLine(settings);
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("accepts", "application/json"),
            });
            
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsync(url + settings, content);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                var responseData = await response.Content.ReadAsStringAsync();
                
                return Ok(responseData);
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Error fetching data: {e.Message}");
            }
        }
    }
}
