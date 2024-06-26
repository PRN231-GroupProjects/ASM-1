using API.Annotations;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models;
using Service.Models.Dtos;
using Service.Models.Payload.Requests.Member;
using Service.Models.Payload.Responses;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _service;

    public MembersController(IMemberService memberService)
    {
        _service = memberService;
    }
    
    [HttpGet]
    public async Task<ActionResult<Result<PaginatedList<MemberDto>>>> GetMembers([FromQuery]GetMembersRequest request)
    {
        var products = await _service.GetMembers(request);
        return Ok(Result<PaginatedList<MemberDto>>.Succeed(products));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Result<MemberDto>>> GetMemberById([FromRoute]int id)
    {
        var product = await _service.GetMemberById(id);
        return Ok(Result<MemberDto>.Succeed(product));
    }

    [HttpPost]
    public async Task<ActionResult<Result<MemberDto>>> CreateMember([FromBody]CreateMemberRequest request)
    {
        var product = await _service.CreateMember(request);
        return Ok(Result<MemberDto>.Succeed(product));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Result<MemberDto>>> UpdateMember([FromBody] UpdateMemberRequest request, [FromRoute] int id)
    {
        var product = await _service.UpdateMember(id, request);
        return Ok(Result<MemberDto>.Succeed(product));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Result<MemberDto>>> DeleteMember([FromRoute] int id)
    {
        var product = await _service.DeleteMember(id);
        return Ok(Result<MemberDto>.Succeed(product));
    }

    [HttpPost("/login")]
    public async Task<ActionResult<Result<MemberDto>>> LoginMember([FromBody] LoginRequest request)
    {
        var member = await _service.Login(request);
        
        HttpContext.Session.SetInt32("role", member.Role);
        
        return Ok(Result<MemberDto>.Succeed(member));
    }
    
    [HttpPost("/logout")]
        public async Task<ActionResult<Result<Boolean>>> LogoutMember()
        {
            
            HttpContext.Session.Clear();
            
            return Ok(Result<Boolean>.Succeed(true));
        }
}