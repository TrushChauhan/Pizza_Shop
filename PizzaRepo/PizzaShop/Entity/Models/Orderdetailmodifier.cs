using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entity.Models;

[Table("orderdetailmodifier")]
public partial class Orderdetailmodifier
{
    [Key]
    [Column("ordermodifierid")]
    public int Ordermodifierid { get; set; }

    [Column("orderdetailid")]
    public int Orderdetailid { get; set; }

    [Column("createdby")]
    public int? Createdby { get; set; }

    [Column("modifiedby")]
    public int? Modifiedby { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [Column("modifierid")]
    public int? Modifierid { get; set; }

    [ForeignKey("Modifierid")]
    [InverseProperty("Orderdetailmodifiers")]
    public virtual Modifier? Modifier { get; set; }

    [ForeignKey("Orderdetailid")]
    [InverseProperty("Orderdetailmodifiers")]
    public virtual Orderdetail Orderdetail { get; set; } = null!;
}
