namespace EShopCase.Application.Interfaces.Mapper;

public interface IMapper
{
    TDest Map<TDest, TSrc>(TSrc src);
    IList<TDest> Map<TDest, TSrc>(IEnumerable<TSrc> src);
    TDest Map<TDest>(object src);
}