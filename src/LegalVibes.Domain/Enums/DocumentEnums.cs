namespace LegalVibes.Domain.Enums;

public enum DocumentStatus
{
    Draft,
    Generated,
    UnderReview,
    Approved,
    Rejected,
    Final,
    Archived
}

public enum DocumentType
{
    TrademarkApplication,
    SupportingDocument,
    ClientInstructions,
    LegalAnalysis,
    AIGeneratedDraft,
    OfficialResponse,
    Other
} 