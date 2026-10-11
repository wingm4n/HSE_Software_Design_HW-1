using VyshKat.Domain.Entities.Transport;
using VyshKat.Domain.Interfaces;

namespace VyshKat.Application.Services;

public sealed class ServiceCenter : IServiceCenter
{
    public bool Inspect(Transport transport)
    {
        if (transport is null)
            return false;

        return !string.IsNullOrWhiteSpace(transport.InventoryNumber);
    }
}