using VyshKat.Domain.Entities.Transport;

namespace VyshKat.Domain.Interfaces;

public interface IServiceCenter
{
    bool Inspect(Transport transport);
}