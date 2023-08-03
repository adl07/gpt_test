namespace WebApiChatGpt.Services;

public interface IOpenAIService
{   
    Task<List<string>>QuestionMedic(string consult);
    /*Task<string> CompleteSentence(string text);
    Task<string> CompleteSentenceAdvance(string text);*/
    
}