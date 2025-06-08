using System;
using Microsoft.Extensions.Options;
namespace Application.Interfaces;

public interface IUserGameAuthorizationService
{
    Task<bool> IsUserOwnIt(string userId, Guid gameId);
}
