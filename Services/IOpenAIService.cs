namespace WebApiChatGpt.Services;

public interface IOpenAIService
{   
    Task<List<string>>ConsultaDeCartilla(string consult);
    /*Task<string> CompleteSentence(string text);
    Task<string> CompleteSentenceAdvance(string text);*/
    
}