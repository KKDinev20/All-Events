namespace AllEvents.TicketManagement.Application.Records
{
    public record PdfSettings(
        int Margin,
        int LogoWidth,
        int LogoHeight,
        int YOffsetStep,
        int FooterHeight
    );
}
