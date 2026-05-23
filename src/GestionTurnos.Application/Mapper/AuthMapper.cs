using GestionTurnos.Application.Request;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Mapper
{
    public static class AuthMapper
    {

        public static Staff ToRegisterNewBusinessAndStaff(this  SignUpRequest authRequest, Business newBusiness, Branch newBranch)
        {
            return new Staff
            {
                Id = Guid.NewGuid(),
                Name = authRequest.Name,
                Email = authRequest.Email,
                Password = authRequest.Password,
                Phone = authRequest.AdminPhone,
                Rol = Rol.Admin,
                LinkPhoto = authRequest.LinkPhoto,
                BusinessId = newBusiness.Id,
                Business = newBusiness,
                BranchId = newBranch.Id,
                Branch = newBranch
            };
        }
    }
}
