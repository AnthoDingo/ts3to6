using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ts3to6.Data.Models.Ts3;

[Table("group_server_to_client")]
public partial class GroupServerToClient
{
    public int GroupId { get; set; }

    public int ServerId { get; set; }

    public int Id1 { get; set; }

    public int Id2 { get; set; }
}
