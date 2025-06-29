﻿using Microsoft.EntityFrameworkCore;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Auth.Infrastructure.Repository;

public class RoleWriteRepository(
     AuthWriteDbContext context) : IRoleWriteRepository
{
    private readonly AuthWriteDbContext _context = context;

    public async Task<Result<Guid>> AddRole(Role role, CancellationToken ct = default)
    {
        await _context.Roles.AddAsync(role, ct);
        return Result.Ok(role.Id.Value);
    }

    public async Task AddRoles(IEnumerable<Role> roles, CancellationToken ct = default)
    {
        await _context.Roles.AddRangeAsync(roles, ct);
    }

    public Task<UnitResult> DeleteRole(RoleId roleId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Role>> GetByIdAsync(RoleId id, CancellationToken ct = default)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(p => p.Id == id);
        if (role == null)
            return Result.Fail(Error.NotFound($"role with id: {id}"));
        
        return Result.Ok(role);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
