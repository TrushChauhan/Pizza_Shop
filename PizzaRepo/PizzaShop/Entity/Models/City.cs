using System;
using System.Collections.Generic;

namespace Entity.Models;

public partial class City
{
    public int Cityid { get; set; }

    public string Name { get; set; } = null!;

    public int? Stateid { get; set; }

    public DateTime Createddate { get; set; }

    public DateTime? Modifieddate { get; set; }

    public int? Createdby { get; set; }

    public int? Modifiedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual Userlogin? CreatedbyNavigation { get; set; }

    public virtual Userlogin? ModifiedbyNavigation { get; set; }

    public virtual State? State { get; set; }

    public virtual ICollection<Userdetail> Userdetails { get; } = new List<Userdetail>();
}
