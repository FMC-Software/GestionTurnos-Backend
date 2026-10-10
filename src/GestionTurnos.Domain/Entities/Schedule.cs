using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestionTurnos.Domain.Entities
{
    public class Schedule : BaseEntity
    {
            public Guid BranchId { get; set; } 
            public DayOfWeek DayOfWeek { get; set; } // Enum de C# (0 = Domingo, 1 = Lunes...)
            public TimeSpan StartTime { get; set; } 
            public TimeSpan EndTime { get; set; }   
            public int SlotDurationMinutes { get; set; }
            public bool IsActive { get; set; } = true; // false = la sucursal no atiende ese dia
                                                         
    }
}
