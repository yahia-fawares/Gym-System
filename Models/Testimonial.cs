using System;
using System.Collections.Generic;

namespace GymSystem2.Models;

public partial class Testimonial
{
    public decimal Testimonialid { get; set; }

    public string Content { get; set; } = null!;

    public decimal? Userid { get; set; }

    public virtual User? User { get; set; }
    public bool IsApprove { get; set; } = false;
}
