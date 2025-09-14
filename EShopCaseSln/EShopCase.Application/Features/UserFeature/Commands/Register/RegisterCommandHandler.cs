using EShopCase.Application.Bases;
using EShopCase.Application.Helpers;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.UserFeature.Commands.Register;

public class RegisterCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<RegisterCommandRequest, ResponseDto<RegisterCommandResponse>>
{
    public async Task<ResponseDto<RegisterCommandResponse>> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<Users, RegisterCommandRequest>(request);

        user.RoleId = 1;
        
        await unitOfWork.GetReadRepository<Users>().GetAsync(y => y.Email == request.Email&&!y.IsDeleted);

        user.Password = PasswordHash.HashPassword(request.Password);

       await unitOfWork.OpenTransactionAsync(cancellationToken);

        await unitOfWork.GetWriteRepository<Users>().AddAsync(user,cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return new ResponseDto<RegisterCommandResponse>().Success();
    }
}