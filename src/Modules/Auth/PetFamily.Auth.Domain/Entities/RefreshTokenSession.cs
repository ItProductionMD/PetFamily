﻿using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Auth.Domain.Entities;
public class RefreshTokenSession : IEntity<Guid> 
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
    public Guid Jti {  get; set; }
    public string FingerPrint {  get; set; }
    public string Token { get; set; }      
    public DateTime ExpiresAt { get; set; } 
    public DateTime CreatedAt { get; set; } 
    public DateTime? RevokedAt { get; set; } 
    private RefreshTokenSession()  { /* for EFCore */ }

    public RefreshTokenSession(
        Guid id,
        Guid userId,
        string token,
        DateTime expiresAt,
        string fingerPrint,
        Guid jti)
    {

        Id = id;
        UserId = userId;
        Token = token;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        FingerPrint = fingerPrint;
        Jti = jti;
    }

    public UnitResult ValidateFingerPrint(string fingerPrint)
    {
        if (FingerPrint == fingerPrint)
            return UnitResult.Ok();

        return UnitResult.Fail(Error.Authentication("FingerPrint validation error"));
    }

    public bool IsActive => 
        RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    public void Revoke()
    {
        if (!IsActive)
            return;
        RevokedAt = DateTime.UtcNow;
    }
}
