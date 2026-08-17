namespace EtykietyIT.Printing;

public sealed record LabelPrintabilityIssue(
    LabelPrintabilitySeverity Severity,
    string Code,
    string Message);
