using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using Data;

namespace Steps;

public sealed class GenerateDocumentationStep : KernelProcessStep<GeneratedDocumentationState>
{
    private GeneratedDocumentationState _state = new();

    private const string SystemPrompt =
            """
            Your job is to write high quality and engaging customer facing documentation for a new product from Contoso. You will be provide with information
            about the product in the form of internal documentation, specs, and troubleshooting guides and you must use this information and
            nothing else to generate the documentation. If suggestions are provided on the documentation you create, take the suggestions into account and
            rewrite the documentation. Make sure the product sounds amazing.
            """;

    public override ValueTask ActivateAsync(KernelProcessStepState<GeneratedDocumentationState> state)
    {
        this._state = state.State!;
        this._state.ChatHistory ??= new ChatHistory(SystemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction]
    public async Task GenerateDocumentationAsync(Kernel kernel, KernelProcessStepContext context, string productInfo)
    {
        Console.WriteLine($"[{nameof(GenerateDocumentationStep)}]:\tGenerating documentation for provided productInfo...");

        // Add the new product info to the chat history
        this._state.ChatHistory!.AddUserMessage($"Product Info:\n\n{productInfo}");

        // Get a response from the LLM
        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse = await chatCompletionService.GetChatMessageContentAsync(this._state.ChatHistory!);

        DocumentInfo generatedContent = new()
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Generated document - {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            Content = generatedDocumentationResponse.Content!,
        };

        this._state!.LastGeneratedDocument = generatedContent;

        await context.EmitEventAsync("DocumentationGenerated", generatedContent);
    }
}

public class GeneratedDocumentationState
{
    public DocumentInfo LastGeneratedDocument { get; set; } = new();
    public ChatHistory? ChatHistory { get; set; }
}
