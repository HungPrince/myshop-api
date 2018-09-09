using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShop.Model.Models
{
    [Table("AppRole")]
    public class AppRole : IdentityRole
    {
        public AppRole() : base()
        {
        }

        public AppRole(string name, string description)
        {
            this.Description = description;
        }

        public virtual string Description { get; set; }
    }
}