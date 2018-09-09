using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShop.Model.Models
{
    [Table("Functions")]
    public class Function
    {
        [Key]
        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string ID { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MaxLength(256)]
        [Required]
        public string URL { get; set; }

        public int DisplayOrder { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar")]
        public string ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Function Parent { get; set; }

        public bool Status { set; get; }

        public string IconCss { get; set; }
    }
}