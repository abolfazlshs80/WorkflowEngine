using Microsoft.Extensions.DependencyInjection;
using WorkflowEngine.Application.Abstractions;
using WorkflowEngine.Application.Execution;
using WorkflowEngine.Application.Nodes;
using WorkflowEngine.Application.Parsing;
using WorkflowEngine.Application.Registry;
using WorkflowEngine.Application.Validation;
using WorkflowEngine.Domain.Abstractions;

namespace WorkflowEngine.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHttpClient("workflow");

        services.AddSingleton<INodeRegistry>(sp =>
        {
            var registry = new NodeRegistry();
            registry.Register(new ManualTriggerNode());
            registry.Register(new ApiTriggerNode());
            registry.Register(new SchedulerTriggerNode());
            registry.Register(new LogActionNode());
            registry.Register(new HttpRequestNode(sp.GetRequiredService<IHttpClientFactory>()));
            registry.Register(new DelayNode());
            registry.Register(new ConditionNode());
            registry.Register(new EndNode());
            return registry;
        });

        services.AddSingleton<IGraphParser, GraphParser>();
        services.AddSingleton<IWorkflowValidator, WorkflowValidator>();
        services.AddSingleton<RetryPolicy>();
        services.AddScoped<IWorkflowExecutor, WorkflowExecutor>();
        return services;
    }
}
