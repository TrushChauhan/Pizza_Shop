using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entity.Models;

[Table("itemandmodifiergroup")]
public partial class Itemandmodifiergroup
{
    [Key]
    [Column("itemandmodifiergroupid")]
    public int Itemandmodifiergroupid { get; set; }

    [Column("itemid")]
    public int? Itemid { get; set; }

    [Column("modifiergroupid")]
    public int? Modifiergroupid { get; set; }

    [Column("minselect")]
    public int? Minselect { get; set; }

    [Column("maxselect")]
    public int? Maxselect { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("createdby")]
    public int? Createdby { get; set; }

    [Column("modifiedby")]
    public int? Modifiedby { get; set; }

    [Column("isdeleted")]
    public bool Isdeleted { get; set; }

    [ForeignKey("Createdby")]
    [InverseProperty("ItemandmodifiergroupCreatedbyNavigations")]
    public virtual Userlogin? CreatedbyNavigation { get; set; }

    [ForeignKey("Itemid")]
    [InverseProperty("Itemandmodifiergroups")]
    public virtual Menuitem? Item { get; set; }

    [ForeignKey("Modifiedby")]
    [InverseProperty("ItemandmodifiergroupModifiedbyNavigations")]
    public virtual Userlogin? ModifiedbyNavigation { get; set; }

    [ForeignKey("Modifiergroupid")]
    [InverseProperty("Itemandmodifiergroups")]
    public virtual Modifiergroup? Modifiergroup { get; set; }
}
