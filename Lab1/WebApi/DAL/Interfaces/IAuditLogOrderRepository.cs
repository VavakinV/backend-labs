using WebApi.DAL.Models;

namespace WebApi.DAL.Interfaces;

public interface IAuditLogOrderRepository
{
    Task<V1AuditLogOrderDal[]> BulkInsert(V1AuditLogOrderDal[] logs, CancellationToken token);
}