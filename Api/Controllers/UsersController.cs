using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Api.Dtos.Users;
using MyMVCProject.Api.Services;

namespace MyMVCProject.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(UsersService usersService) : ControllerBase
{
}