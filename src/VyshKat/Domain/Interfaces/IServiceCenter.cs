using VyshKat.Domain.Entities.Transports;

namespace VyshKat.Domain.Interfaces;

public interface IServiceCenter
{
    bool Inspect(Transport transport);
}