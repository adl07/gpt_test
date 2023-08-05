using Microsoft.Extensions.Options;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using WebApiChatGpt.Configurations;
using OpenAI_API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace WebApiChatGpt.Services;


public class OpenService : IOpenAIService
{   
    private readonly OpenAIConfig _openAiConfig;

    private readonly JObject _prestaciones;

    public OpenService(
        IOptionsMonitor<OpenAIConfig> optionsMonitor
    )
    {
        _openAiConfig = optionsMonitor.CurrentValue;

        string jsonFilePath = Path.Combine("DataBase", "Prestadores.json");
        
        string jsonContent = File.ReadAllText(jsonFilePath);
        _prestaciones = JObject.Parse(jsonContent);
    }

    
    public async Task<List<string>>ConsultaDeCartilla(string consult)
    
    {   
        //SE AGREGA UN TRY&CATCH PARA CAPTURAR ALGUN ERROR QUE PUEDA DARNOS LA API DE OPEN AI AL EJECUTARSE
        try
        {
                //PARAMETROS DE LA API
                var api = new OpenAI_API.OpenAIAPI(_openAiConfig.Key);

                var chat = api.Chat.CreateConversation(new OpenAI_API.Chat.ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.1,
                    MaxTokens = 200,
                });

                chat.AppendUserInput(consult);

                //DECLARO EL INSTRUCTIVO DE LA PRIMERA INSTANCIA AL BOT
                var prompt1 = "Te solicito que identifiques solamente tres paramentros: la(s) especialidad(es) médica(s) en su denominación técnica, el nombre de la ciudad y el nombre del prestador médico o especialista. Por ejemplo, si se trata de un dentista, me gustaría que me proporciones la especialidad de odontología en lugar de usar el término 'dentista'. Si falta el parametro especilidad/es medicas en alguna de las combinaciones agregarle siempre el valor booleano null. Si falta el parametro nombre del 'prestador' médico o médico especialista en alguna de las combinaciones agregarle siempre el valor booleano null. No autocompletes con otros datos. Responde en formato JSON mediante la creación de una lista que se llame 'Prestaciones' y contenga los parametros con sus valores. Por favor siempre identificarlos y diferenciar correctamente a que busqueda pertenece cada parametro, por ejemplo si se trata de una consulta: busco un dentista en boedo y un en lomas de zamora, debes saber diferenciar que son dos busquedas dentro de una misma consulta ya que en una si hay la(s) especialidad(es) médica(s) y en la otra no, por lo cual debes responder el siguiente formato: {Prestaciones: [{Especialidad: odontología,Ciudad: Boedo Prestador: null,Codigo: 11.01},{Especialidad: null,Ciudad: Lomas de Zamora,Prestador: null,Codigo: null}]}, es de importancia que las diferencies correctamente. Limitate solamente a responder el JSON, no agregar informacion.";

                chat.AppendSystemMessage(prompt1);

                //RECIBIMOS LA PRIMERA RESPUESTA
                var response1 = await chat.GetResponseFromChatbotAsync();

                //Console.WriteLine("Respuesta del primer prompt: " + response1);

                //DECLARAMOS EL INSTRUCTIVO DE LA SEGUNDA INSTANCIA AL BOT
                var prompt2= "Verifica si el valor del parametro 'Especialidad' dentro del objeto"+response1+"se encuentra en el JSON de prestaciones llamado "+_prestaciones+". Si las especialidades se encuentran en el JSON, agrega como parametro 'Codigo' y el valor que solamente tiene en 'CodEspecialidad' y no 'id', sino agregarle el valor booleano null por defecto al parametro 'Codigo'. Manten el mismo formato de respuesta. ";

                //EN ESTA INSTANCIA LA APLICA SOBRE LA PRIMERA RESPUESTA
                chat.AppendSystemMessage(prompt2);


                //OBTENEMOS EL RESULTADO FINAL
                var response2 = await chat.GetResponseFromChatbotAsync();

                Console.WriteLine("Respuesta del segundo prompt: " + response2);
            
                var resultadoDelResponse = respuestaDeConsulta(response2);

                return resultadoDelResponse;
                
                //return response2;
        }
        catch (Exception ex)
        {
            //Console.WriteLine("Error en la consulta: " + ex.Message);
            
            //return "Error en la consulta: " + ex.Message;

            return new List<string>();
        }
            
    
    }
        

    public List<string>respuestaDeConsulta(string json)
    {
        JObject jsonObject = JObject.Parse(json);
        JArray prestaciones = (JArray)jsonObject["Prestaciones"];

        List<string> listadoDeResultados = new List<string>();

        string especialidad = "";
        string ubicacion = "";
        string nombre="";
        string resultado= "";
        string codigo="";

        foreach (JObject item in prestaciones)
        {
            especialidad = item["Especialidad"].ToString();
            if (string.IsNullOrEmpty(especialidad))
            {
                listadoDeResultados.Add("Verificar si los datos ingresados son correctos");
            }
            else{
                ubicacion = item["Ciudad"].ToString();
                nombre= item["Prestador"].ToString();
                codigo = item["Codigo"].ToString();

                resultado = textoDeConsulta(especialidad, ubicacion, nombre);
                listadoDeResultados.Add(resultado);
            }
            
        
        }
        if (listadoDeResultados.Contains("Verificar si los datos ingresados son correctos"))
        {
            List<string> respuesta = new List<string>();
            respuesta.Add("Verificar si los datos ingresados son correctos");
            return respuesta;
        }

        //DatosDelListado(listadoDeResultados);

        return listadoDeResultados;
    }


    public string textoDeConsulta(string? especialidad, string? ubicacion, string? nombre)
    {   
        
        string respuesta = "-"+especialidad+"-"+ubicacion+"-"+nombre;
        
        return respuesta;
    }
    
    public void DatosDelListado(List<string> resultados)
    {   
        string fecha = DateTime.Now.ToString();
        
        foreach (string resultado in resultados)
        {
            // Mostar el historial del array con cada resultado
            Console.WriteLine("Historial:"+"\n" +fecha+"\n"+resultado);
        }
    }


}
