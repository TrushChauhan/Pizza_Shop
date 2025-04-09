using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entity.Models;

[Table("ordertable")]
public partial class Ordertable
{
    [Key]
    [Column("ordertableid")]
    public int Ordertableid { get; set; }

    [Column("orderid")]
    public int? Orderid { get; set; }

    [Column("tableid")]
    public int? Tableid { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("createdby")]
    public int? Createdby { get; set; }

    [Column("modifiedby")]
    public int? Modifiedby { get; set; }

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [ForeignKey("Orderid")]
    [InverseProperty("Ordertables")]
    public virtual Customerorder? Order { get; set; }

    [ForeignKey("Tableid")]
    [InverseProperty("Ordertables")]
    public virtual Dinetable? Table { get; set; }
}
