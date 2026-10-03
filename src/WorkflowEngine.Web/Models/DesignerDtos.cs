namespace WorkflowEngine.Web.Models;

public class DesignerGraphDto
{
    public List<DesignerNodeDto> Nodes { get; set; } = new();
    public List<DesignerEdgeDto> Edges { get; set; } = new();
}

public class DesignerNodeDto
{
    public Guid? Id { get; set; }
    public string NodeKey { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string NodeType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ConfigurationJson { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class DesignerEdgeDto
{
    public string SourceNodeKey { get; set; } = string.Empty;
    public string TargetNodeKey { get; set; } = string.Empty;
    public string? SourcePort { get; set; }
}
