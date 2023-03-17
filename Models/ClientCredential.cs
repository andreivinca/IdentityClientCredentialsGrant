using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public class ClientCredential
    {
        public ClientCredential()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string ClientSecret { get; set; }
        public DateTimeOffset ExpireDate { get; set; }
    }
}
