using FluentValidation;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands
{
    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(1, 100).WithMessage("Title must be between 1 and 100 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .Length(1, 100).WithMessage("Location must be between 1 and 100 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.EventDate)
                .GreaterThan(DateTime.Now).WithMessage("Event date must be in the future.");

            RuleFor(x => x.NrOfTickets)
                .GreaterThan(0).WithMessage("Number of tickets must be greater than 0.");
        }
    }
}
