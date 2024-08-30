using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Records;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Reflection;

namespace AllEvents.TicketManagement.Application.Reports.Commands
{
    public class GenerateUserOrdersReportCommandHandler : IRequestHandler<GenerateUserOrdersReportCommand, byte[]>
    {
        private readonly IExternalUserRepository _externalUserRepository;
        private readonly IOrderRepository _orderRepository;

        public GenerateUserOrdersReportCommandHandler(
            IExternalUserRepository externalUserRepository,
            IOrderRepository orderRepository)
        {
            _externalUserRepository = externalUserRepository;
            _orderRepository = orderRepository;
        }

        public async Task<byte[]> Handle(GenerateUserOrdersReportCommand request, CancellationToken cancellationToken)
        {
            var user = await _externalUserRepository.GetUserByEmailAsync(request.ExternalUserEmail);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var orders = await _orderRepository.GetOrdersByUserAndDateRangeAsync(user.Id, request.FromDate, request.ToDate);
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found for the specified user and date range");
            }

            return GeneratePdfReport(orders);
        }

        private byte[] GeneratePdfReport(List<Order> orders)
        {
            using (var stream = new MemoryStream())
            {
                var document = new PdfDocument();

                var headerFont = new XFont("Lucida Console", 18, XFontStyle.Bold);
                var eventFont = new XFont("Lucida Console", 16, XFontStyle.Bold);
                var font = new XFont("Lucida Console", 12, XFontStyle.Regular);

                var backgroundBrushPrim = new XSolidBrush(XColor.FromKnownColor(XKnownColor.LightGray));
                var backgroundBrushAlt = new XSolidBrush(XColor.FromKnownColor(XKnownColor.WhiteSmoke));
                var fontBrush = XBrushes.Black;

                var settings = new PdfSettings(
                    Margin: 40,
                    LogoWidth: 100,
                    LogoHeight: 80,
                    YOffsetStep: 120,
                    FooterHeight: 30
                );

                XImage logo = LoadLogo(); // Load the embedded logo

                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                try
                {
                    int yOffset = DrawHeader(gfx, page, logo, headerFont, fontBrush, settings);
                    bool useAltColor = false;

                    foreach (var order in orders)
                    {
                        if (IsNewPageNeeded(yOffset, settings, page.Height))
                        {
                            gfx.Dispose();

                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            yOffset = DrawHeader(gfx, page, logo, headerFont, fontBrush, settings);
                        }

                        var backgroundBrush = useAltColor ? backgroundBrushPrim : backgroundBrushAlt;
                        yOffset = DrawOrder(gfx, order, backgroundBrush, eventFont, font, fontBrush, page, settings, yOffset);
                        useAltColor = !useAltColor;
                    }
                }
                finally
                {
                    gfx?.Dispose();
                }

                DrawFooters(document, font, fontBrush, settings.FooterHeight);

                document.Save(stream, false);
                return stream.ToArray();
            }
        }

        private int DrawOrder(XGraphics gfx, Order order, XBrush backgroundBrush, XFont eventFont, XFont font, XBrush fontBrush, PdfPage page, PdfSettings settings, int yOffset)
        {
            gfx.DrawRectangle(backgroundBrush, settings.Margin, yOffset, page.Width - 2 * settings.Margin, settings.YOffsetStep - 10);

            gfx.DrawString($"Event: {order.Event.Title} - {order.Event.Location}", eventFont, fontBrush, new XRect(settings.Margin + 10, yOffset + 20, page.Width - 2 * settings.Margin, settings.YOffsetStep), XStringFormats.TopLeft);
            gfx.DrawString($"Date: {order.CreatedOn.ToShortDateString()}", font, fontBrush, new XRect(settings.Margin + 10, yOffset + 50, page.Width - 2 * settings.Margin, settings.YOffsetStep), XStringFormats.TopLeft);
            gfx.DrawString($"Tickets: {string.Join(", ", order.TicketNames)}", font, fontBrush, new XRect(settings.Margin + 10, yOffset + 70, page.Width - 2 * settings.Margin, settings.YOffsetStep), XStringFormats.TopLeft);
            gfx.DrawString($"Status: {order.Status}", font, fontBrush, new XRect(settings.Margin + 10, yOffset + 90, page.Width - 2 * settings.Margin, settings.YOffsetStep), XStringFormats.TopLeft);

            return yOffset + settings.YOffsetStep;
        }

        private XImage LoadLogo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AllEvents.TicketManagement.Application.Assets.Accedia-Logo.png";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Logo file not found");
                }

                return XImage.FromStream(() => stream);
            }
        }


        private int DrawHeader(XGraphics gfx, PdfPage page, XImage logo, XFont headerFont, XBrush fontBrush, PdfSettings settings)
        {
            int yOffset = settings.Margin + 10;
            gfx.DrawImage(logo, settings.Margin, yOffset, settings.LogoWidth, settings.LogoHeight);
            gfx.DrawString("User Orders Report", headerFont, fontBrush,
                new XRect(settings.Margin, yOffset + settings.LogoHeight + 10, page.Width - 2 * settings.Margin, settings.LogoHeight), XStringFormats.TopCenter);

            return yOffset + settings.LogoHeight + 60;
        }

        private bool IsNewPageNeeded(int yOffset, PdfSettings settings, double pageHeight)
        {
            return yOffset + settings.YOffsetStep > pageHeight - settings.Margin - settings.FooterHeight;
        }

        private void DrawFooters(PdfDocument document, XFont font, XBrush fontBrush, int footerHeight)
        {
            int pageCount = document.PageCount;
            for (int i = 0; i < pageCount; i++)
            {
                using (var pageGfx = XGraphics.FromPdfPage(document.Pages[i]))
                {
                    pageGfx.DrawString($"Page {i + 1} of {pageCount}", font, fontBrush, new XRect(0, document.Pages[i].Height - footerHeight, document.Pages[i].Width, footerHeight), XStringFormats.Center);
                }
            }
        }
    }
}
