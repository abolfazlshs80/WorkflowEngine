using WorkflowEngine.Domain.Entities;

namespace WorkflowEngine.Domain.Abstractions;

public interface IWorkflowValidator
{
    ValidationResult Validate(Workflow workflow);
}

public class ValidationResult
{
    public List<string> Errors { get; } = new();
    public bool IsValid => Errors.Count == 0;
}
