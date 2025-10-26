using FluentValidation;
using Models.Dto.V1.Requests;

namespace WebApi.Validators;

public class V1CreateAuditLogOrderRequestValidator : AbstractValidator<V1CreateAuditLogRequest>
{
    public V1CreateAuditLogOrderRequestValidator()
    {
        RuleForEach(x => x.Orders).NotNull();
        
        RuleForEach(x => x.Orders).ChildRules(order =>
        {
            order.RuleFor(o => o.OrderId).GreaterThan(0);
            
            order.RuleFor(o => o.CustomerId).GreaterThan(0);
            
            order.RuleFor(o => o.OrderStatus).NotEmpty();
        });
    }
}