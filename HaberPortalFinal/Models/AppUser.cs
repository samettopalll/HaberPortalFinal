using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace HaberPortalFinal.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string City { get; set; }
    }
}
