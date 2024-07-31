using FluentValidation;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands
{
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid category.");

            RuleFor(x => x.EventDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Event date must be in the future.");

            RuleFor(x => x.NrOfTickets)
                .GreaterThan(0).WithMessage("Number of tickets must be greater than zero.");
        }
    }
}
