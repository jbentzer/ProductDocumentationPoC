using Microsoft.SemanticKernel;

using Data;

namespace Steps;

public sealed class PublishDocumentationStep : KernelProcessStep
{
    [KernelFunction]
    public DocumentInfo PublishDocumentation(DocumentInfo document)
    {
        // For example purposes we just write the generated docs to the console
        Console.WriteLine($"[{nameof(PublishDocumentationStep)}]:\tPublishing product documentation approved by user: \n{document.Title}\n{document.Content}");
        return document;
    }
}
