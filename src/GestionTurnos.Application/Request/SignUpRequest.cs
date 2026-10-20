using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GestionTurnos.Application.Request
{
    public class SignUpRequest
    {
        public required string Name { get; set; } = string.Empty;

        [EmailAddress]
        public required string Email { get; set; } = string.Empty;
        [MinLength(3, ErrorMessage = "La contraseña debe ser mayor a 3 dígitos")]
        public required string Password { get; set; } = string.Empty;

        // Sin regex fijo a un pais: el frontend ya valida formato con libphonenumber-js
        // (selector de pais + numero), asi que cualquier pais es valido aca.
        [Phone]
        public required string AdminPhone { get; set; } = string.Empty;
        public string? LinkPhoto { get; set; } = string.Empty;

        public required string BusinessCategory { get; set; } = string.Empty;

        public required string Address { get; set; } = string.Empty;
        public required string City { get; set; } = string.Empty;

        [Phone]
        public required string BranchPhone { get; set; }

        public Plan? Plan { get; set; } = null;
    }
}
