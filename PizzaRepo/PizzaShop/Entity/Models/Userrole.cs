﻿using System;
using System.Collections.Generic;

namespace Entity.Models;

public partial class Userrole
{
    public int Roleid { get; set; }

    public string? Rolename { get; set; }

    public DateTime Createddate { get; set; }

    public DateTime? Modifieddate { get; set; }

    public int? Createdby { get; set; }

    public int? Modifiedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual Userlogin? CreatedbyNavigation { get; set; }

    public virtual Userlogin? ModifiedbyNavigation { get; set; }

    public virtual ICollection<Roleandpermission> Roleandpermissions { get; } = new List<Roleandpermission>();

    public virtual ICollection<Userdetail> Userdetails { get; } = new List<Userdetail>();

    public virtual ICollection<Userlogin> Userlogins { get; } = new List<Userlogin>();
}
