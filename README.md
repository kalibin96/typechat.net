# TypeChat.NET

TypeChat.NET is an **experimental project** from the [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel) team. TypeChat.NET brings the ideas of [TypeChat](https://github.com/microsoft/TypeChat) to .NET. 

TypeChat.NET provides **cross platform** libraries that help you build natural language interfaces with language models using strong types, type validation and simple type safe programs (plans). Strong typing may help make software that uses language models more deterministic and reliable.    
```
// Translates user intent into strongly typed Calendar Actions
var model = new LanguageModel(Config.LoadOpenAI());
var translator = new JsonTranslator<CalendarActions>(model);

// Translate natural language request 
CalendarActions actions = await translator.TranslateAsync(requestText);
```

TypeChat.NET is in **active development** with frequent updates. The framework will evolve as the team explores the space and incorporates feedback. Supported scenarios are shown in the included [Examples](./examples). Documentation will also continue to improve. When in doubt, please look at the code.  

# Assemblies
TypeChat.NET consists of the following assemblies:

* **Microsoft.TypeChat**: Classes that translate user intent into strongly typed and validated objects. 

* **Microsoft.TypeChat.Program**: Classes to synthesize, validate and run  ***JSON programs***. 

* **Microsoft.TypeChat.SemanticKernel**: Integration with Microsoft Semantic Kernel for language models, plugins and embeddings.

* **Microsoft.TypeChat.Dialog**: Classes for working with interactive Agents that have history. 

## Microsoft.TypeChat ##
TypeChat uses language models to translate user intent into JSON that conforms to a schema. This JSON is then validated and deserialized into a typed object. Additional constraint checking is applied as needed. Validation errors are sent back to the language model, which uses them to **repair** the Json it originally returned. 

TypeChat provides:
* Json translation, validation, repair and deserialization.
* Extensibility: interfaces for customizing schemas, validators and prompts.
* Schema export: classes to generate schema for the .NET Type you want to translate to. Exported schema includes dependencies and base classes. The exported schema is specified using **Typescript**, which can concisely express schema for JSON objects. 
  * Support for common scenarios shown in TypeChat examples. When you encounter limitations, you can supply schema text, such as Typescript authored by hand. 
  * Helper attributes for Vocabularies and Comments. Vocabularies are string tables that constrain the values that can be assigned to string properties. Dynamic loading of vocabularies enables scenarios where they vary at runtime.
  * **Note**: Like TypeChat, TypeChat.NET has only been tested with schema specified in Typescript. 
```
[Comment("Milks currently in stock")]
public class Milks
{
    [JsonVocab("whole milk | two percent milk | nonfat milk | soy milk | almond milk")]
    public string Name { get; set; }
}
``` 

## Microsoft.TypeChat.Program ##
TypeChat.Program translates natural language requests into simple programs (***Plans***), represented as JSON. 

JSON programs can be thought of as a [DSL](https://en.wikipedia.org/wiki/Domain-specific_language) or [Plan](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/planners/?tabs=Csharp), expressed in JSON, with an associated [**grammar**](src/typechat.program/ProgramSchema.ts) that is enforced. JSON programs can be type checked against the APIs they target. They can be then be run using an interpreter, or compiled into .NET code. Both mechanisms enforce type safety.

TypeChat.Program includes:
* Program Translator: translates user intent into programs that follow the [Program Grammar](src/typechat.program/ProgramSchema.ts)
* Program Interpreter: runs programs generated by ProgramTranslator using an interpreter.
* Program Compiler: uses the dynamic language runtime (DLR) to compile programs/plans with type checking. Compilation diagnostics are used to repair programs. 

```
// Translates user intent into typed Programs that call methods on a Math API
var model = new LanguageModel(Config.LoadOpenAI());
var api = new MathAPI();
var translator = new ProgramTranslator<IMathAPI>(model, api);

// Translate natural language request
Program program = await translator.TranslateAsync(requestText);
// Run the program
program.Run(api);
```

## Microsoft.TypeChat.SemanticKernel ##
TypeChat.SemanticKernel provides default bindings for language models, plugins and embeddings to Typechat.NET and TypeChat.NET examples.

TypeChat.SemanticKernel include classes for:
* **Json Programs for Plugins**: turn registered plugins into **APIs** that Json programs can target. See the [Plugins Example](examples/Plugins/Program.cs).
* Language model and embeddings access: all TypeChat examples use the Semantic Kernel to call models and generate embeddings. 
 
## Microsoft.TypeChat.Dialog
TypeChat.Dialog demonstrates how TypeChat.NET may be used for strongly typed interactions with message passing Agents or Bots. These agents can include features such as built in interaction history. 

```
// Create an agent with history
agent = new AgentWithHistory<HealthDataResponse>(new LanguageModel(Config.LoadOpenAI()));
agent.Instructions.Append("Help me enter my health data step by step");

// Run a chat loop
while (true)
{
   ...
    var userInput = Console.ReadLine();
    HealthDataResponse response = await agent.GetResponseAsync(userInput);
}
```

# Getting Started 

## Prerequisite: Open AI
* **Open AI Language Models**: TypeChat.NET and its examples currently require familiarity with and access to language models from Open AI. 
* TypeChat.NET has been tested with and supports the following models: 
    * gpt-35-turbo
    * gpt-4
    * ada-002
* Some examples and scenarios will work best with gpt-4
* Since TypeChat.NET uses the Semantic Kernel, models from other providers ***may*** be used for experimentation.

## Building

* Visual Studio 2022. 
  * Load **typechat.sln** from the root directory. 
  * Restore packages
  * Build
* dotnet build
  * Launch a command prompt / terminal
  * Go to the root directory of the project
  * dotnet build Typechat.sln

## Nuget Packages

* Microsoft.Typechat
```
dotnet add package Microsoft.TypeChat --prerelease
dotnet add package Microsoft.TypeChat.SemanticKernel --prerelease
```
* Microsoft.TypeChat.Program
```
dotnet add package Microsoft.TypeChat.Program --prerelease
```
* Microsoft.TypeChat.Dialog
```
dotnet add package Microsoft.TypeChat.Dialog --prerelease
```

## Examples

To see TypeChat in action, explore the [TypeChat example projects](./examples). The list below describes which examples will best introduce which concept. Some examples or scenarios may work best with gpt-4.

* Hello World: The [Sentiment](./examples/Sentiment/Program.cs) example is TypeChat's Hello World and a minimal introduction to JsonTranslator. 

* JsonTranslator, Schemas and Vocabularies: 
  
  * CoffeeShop: Natural language ordering at a coffee shop. Demonstrates a complex schema with polymorphic deserialzation.
  * Calendar: Transform language into calendar actions
  * Restaurant: Order processing at a pizza restaurant

* Hierarchical schemas and routing:
  * MultiSchema: dynamically route user intent to other 'sub-apps'
  * SchemaHierarchy: A Json Translator than uses multiple child JsonTranslators

* Json Programs and TypeChat.Program:
  * Math: Turn user requests into calculator programs
  * Plugins: Generate programs that use Semantic Kernel Plugins

* Typed Interactive Agents (bots):
  * Healthdata
  
Each example includes an **input.txt** with sample input. Pass the input file as an argument to run the example in **batch mode**. 

## Api Key and Configuration
To use TypeChat.net or run the examples, you need an **API key** for an Open AI service. Azure Open AI and the Open AI service are both supported.

### Configure Api Key for examples
* Go to the **[examples](./examples)** folder in the solution
* Make a copy of the [appSettings.json](./examples/appSettings.json) file and name it **appSettings.Development.json**. Ensure it is in the same folder as appSettings.json
* appSettings.Development.json is a local development only override of the settings in appSettings.json and is **never** checked in.
* Add your Api Key to **appSettings.Development.json**. 

A typical appSettings.Development.json will look like this:
```
// For Azure Open AI service
{
  "OpenAI": {
    "Azure": true,
    "ApiKey": "YOUR API KEY",
    "Endpoint": "https://YOUR_RESOURCE_NAME.openai.azure.com",
    "Model": "gpt-35-turbo"  // Name of Azure deployment
  }
}

// For Open AI Service:
{
  "OpenAI": {
    "Azure": false,
    "ApiKey": "YOUR API KEY",
    "Endpoint": "https://api.openai.com/v1/chat/completions",
    "Model": "gpt-3.5-turbo"  // Name of Open AI model
  }
}
```

### OpenAIConfig
TypeChat examples accesses language models using the [LanguageModel](./src/typechat.sk/LanguageModel.cs) class. The OpenAIConfig class supplies configuration for LanguageModel. You initialize OpenAIConfig from your application's configuration, from a Json file or from environment variables. 

See [OpenAIConfig.cs](./src/typechat.sk/OpenAIConfig.cs) for a list of :
  * Configurable properties
  * Supported environment variables.
```
// Your configuration 
OpenAIConfig config = Config.LoadOpenAI();
// Or Json file
OpenAIConfig config = OpenAIConfig.LoadFromJsonFile(...);
// Or from config
config = OpenAIConfig.FromEnvironment();

var model = new LanguageModel(config);
```

### Using Semantic Kernel directly
You can also initialize LanguageModel using an IKernel object you created using a KernelBuilder.
```
const string modelName = "gpt-35-turbo";
new LanguageModel(_kernel.GetService<IChatCompletion>(modelName), modelName);
```

### Using your own client
TypeChat accesses language models using the [ILanguageModel](src/typechat/ILanguageModel.cs) interface. [LanguageModel](src/typechat.sk/LanguageModel.cs) implements ILanguageModel. 

You can use your own model client by implementing ILanguageModel.

## Code of Conduct

This project has adopted the
[Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the
[Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com)
with any additional questions or comments.

## License

Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE) license.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
