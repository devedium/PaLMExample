using Google.Ai.Generativelanguage.V1Beta2;
using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;

namespace PaLMExample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //api key
            var apiKey = "Your API KEY";
            var callSettings = CallSettings.FromHeader("x-goog-api-key", apiKey);


            //list models
            var modelServiceClientBuilder = new ModelServiceClientBuilder()
            {
                GoogleCredential = GoogleCredential.FromAccessToken(null),
                Settings = new ModelServiceSettings()
                {
                     CallSettings = callSettings
                }               
            };            

            var modelServiceClient = modelServiceClientBuilder.Build();

            var models = modelServiceClient.ListModels();
            foreach (var model in models)
            {
                Console.WriteLine(model.ModelName);
            }


            //text service
            var textServiceClientBuilder = new TextServiceClientBuilder()
            {
                GoogleCredential = GoogleCredential.FromAccessToken(null),
                Settings = new TextServiceSettings()
                {
                    CallSettings = callSettings,                    
                }
            };                      

            var textServiceClient = textServiceClientBuilder.Build();

            var textPrompt = new TextPrompt();
            textPrompt.Text = "once upon a time, ";            
            
            var textRequest = new GenerateTextRequest
            {
                ModelAsModelName = ModelName.FromModel("text-bison-001"),
                Prompt = textPrompt,
                Temperature = 0.5F,
                CandidateCount = 1,               
            };            
            
            GenerateTextResponse textResponse = await textServiceClient.GenerateTextAsync(textRequest);
            Console.WriteLine(textResponse.Candidates[0].Output);

            //discuss service
            var discussServiceClientBuilder = new DiscussServiceClientBuilder()
            {
                GoogleCredential = GoogleCredential.FromAccessToken(null),
                Settings = new DiscussServiceSettings()
                {
                    CallSettings = callSettings
                }
            };

            var discussServiceClient = discussServiceClientBuilder.Build();

            var messagePrompt = new MessagePrompt();
            messagePrompt.Context = "Imagine you are explaining this concept to a 5th grader.";
            messagePrompt.Examples.Add(new Example()
            {
                Input = new Message() { Content = "how our solar system works?" },
                Output = new Message() { Content = " It's like a big family of planets and other objects that live together in space. Picture the Sun as the big parent, and all the planets, moons, and asteroids as its children and their friends. Each one has a unique personality and role in this cosmic family." },
            });

            messagePrompt.Messages.Add(new Message()
            {
                Content = "how comptuer works?"
            });

            GenerateMessageRequest messageRequest = new GenerateMessageRequest
            {
                 ModelAsModelName = ModelName.FromModel("chat-bison-001"),
                 Prompt = messagePrompt,
                Temperature = 0.5F,
                CandidateCount = 1,
            };

            var messageResponse = await discussServiceClient.GenerateMessageAsync(messageRequest);
            Console.WriteLine(messageResponse.Candidates[0].Content);
            

            //embedding service
            var embedText = "Large Language Models (LLMs) are a powerful, versatile type of machine learning model that enables computers to comprehend and generate natural language better than ever. ";
            var embedTextRequest = new EmbedTextRequest()
            {
                ModelAsModelName = ModelName.FromModel("embedding-gecko-001"),
                Text = embedText,
            };

            var embedTextResponse = await textServiceClient.EmbedTextAsync(embedTextRequest);
            foreach(var value in embedTextResponse.Embedding.Value.ToList())
            {
                Console.Write(value);
                Console.Write(" ");
            }
        }
    }
}