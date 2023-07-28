using Microsoft.AspNetCore.Mvc;
using WebApiChatGpt.Services;

namespace WebApiChatGpt.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenAiController : ControllerBase
{
    

    private readonly ILogger<OpenAiController> _logger;
    private readonly IOpenAIService _openAiService;

    public OpenAiController(
        ILogger<OpenAiController> logger,
        IOpenAIService openAIService)
    {
        _logger = logger;
        _openAiService= openAIService;
    }

    [HttpPost()]
    [Route("AskQuestion")]
    public async Task<IActionResult> AskQuestion(string consult)
    {
        var result = await _openAiService.QuestionMedic(consult);
        return Ok(result);
    }
    /*[HttpPost()]
    [Route("CompleteSentence")]
    public async Task<IActionResult> CompleteSentence(string text)
    {
        var result = await _openAiService.CompleteSentence(text);
        return Ok(result);
    }*/

    
}