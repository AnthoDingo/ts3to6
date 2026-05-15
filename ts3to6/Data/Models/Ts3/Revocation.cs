using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("revocations")]
public partial class Revocation
{
    public string RevocationKey { get; set; } = null!;

    public int RevocationType { get; set; }

    public int RevocationExpiration { get; set; }
}
