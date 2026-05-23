using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestionTurnos.Domain.Entities
{
    public class Schedule : BaseEntity
    {
            public Guid Id { get; set; }
            public Guid BranchId { get; set; } // FK a Branch
            public DayOfWeek DayOfWeek { get; set; } // Enum de C# (0 = Domingo, 1 = Lunes...)
            public TimeSpan StartTime { get; set; } // Ej: 08:00:00
            public TimeSpan EndTime { get; set; }   // Ej: 18:00:00
            public int SlotDurationMinutes { get; set; } // Ej: 30
                                                         // ... campos de auditoría (IsDeleted, etc.)
    }
}
