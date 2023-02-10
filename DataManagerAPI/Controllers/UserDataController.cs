﻿using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataManagerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserDataController : ControllerBase
{
    private readonly IUserDataService _service;

    public UserDataController(IUserDataService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("Add")]
    public async Task<ActionResult<UserDataDto>> AddUserData([FromBody] AddUserDataDto data)
    {
        ResultWrapper<UserDataDto> result = await _service.AddUserData(data);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpPut]
    [Route("Update")]
    public async Task<ActionResult<UserDataDto>> UpdateUserData([FromBody] UserDataDto data)
    {
        ResultWrapper<UserDataDto> result = await _service.UpdateUserData(data);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpDelete]
    [Route("{UserId}")]
    public async Task<ActionResult<UserDataDto>> UpdateUserData(int UserId)
    {
        ResultWrapper<UserDataDto> result = await _service.DeleteUserData(UserId);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet]
    [Route("{userDataId}")]
    public async Task<ActionResult<UserDataDto>> GetUser(int userDataId)
    {
        ResultWrapper<UserDataDto> result = await _service.GetUserData(userDataId);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet]
    [Route("all/{userId}")]
    public async Task<ActionResult<List<UserDataDto>>> GetUserDataByUserId(int userId)
    {
        ResultWrapper<UserDataDto[]> result = await _service.GetUserDataByUserId(userId);
        return StatusCode(result.StatusCode, result.Data);
    }

    /*
        [HttpPost]
        [Route("Upload")]
        [AllowAnonymous]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile()
        {
            string? boundary = HeaderUtilities.RemoveQuotes(
                MediaTypeHeaderValue.Parse(Request.ContentType).Boundary
            ).Value;

            var reader = new MultipartReader(boundary!, Request.Body);

            var section = await reader.ReadNextSectionAsync();

            await _service.UploadFile(reader, section);

            return Ok();
        }
    */
}
