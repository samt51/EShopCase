using EShopCase.Application.Interfaces.Mapper;

namespace EShopCase.Infrastructure.Concrete.Mapping;

public sealed class Mapper : IMapper
{
    private readonly   AutoMapper.IMapper _mapper;
    public Mapper(AutoMapper.IMapper mapper) => _mapper = mapper;

    public TDest Map<TDest, TSrc>(TSrc src)
        => _mapper.Map<TSrc, TDest>(src);

    public IList<TDest> Map<TDest, TSrc>(IEnumerable<TSrc> src)
        => _mapper.Map<List<TDest>>(src);

    public TDest Map<TDest>(object src)
        => _mapper.Map<TDest>(src);
}